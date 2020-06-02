using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp1.Model;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.init();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength;//长度
            public uint dwMemoryLoad;//内存使用率
            public uint dwTotalPhys;//总物理内存
            public uint dwAvailPhys;//可用物理内存
            public uint dwTotalPageFile;//交换文件总大小
            public uint dwAvailPageFile;//可用交换文件大小
            public uint dwTotalVirtual;//总虚拟内存
            public uint dwAvailVirtual;//可用虚拟内存大小
        }
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MEMORY_INFO meminfo);

        ///win32 api
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = (-20);
        private const int WS_EX_TOOLWINDOW = 0x80;

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        GlobalModel globalModel = new GlobalModel();

        string[] weekMap = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };

        MEMORY_INFO info = new MEMORY_INFO();

        Computer myComputer = new Computer();
        UpdateVisitor updateVisitor = new UpdateVisitor();
        IHardware cpu, gpu;
        ISensor cpuTemperatureSensor, cpuLoadSensor, gpuTemperatureSensor, gpuLoadSensor;
        MonitorNetStream monitorNet = new MonitorNetStream();

        private void init()
        {
            this.Top = 0;
            this.Left = (SystemParameters.FullPrimaryScreenWidth - this.Width) / 2;

            this.DataContext = globalModel;

            globalModel.TotalMemory = GetTotalMemory();

            myComputer.Open();
            myComputer.GPUEnabled = true;
            myComputer.CPUEnabled = true;
            myComputer.Accept(updateVisitor);

            foreach (var hardwareItem in myComputer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.CPU)
                {
                    cpu = hardwareItem;
                    foreach (var sensor in cpu.Sensors)
                    {
                        // CPU 负载
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
                        {
                            cpuLoadSensor = sensor;
                        }

                        // CPU 温度
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name == "CPU Package")
                        {
                            cpuTemperatureSensor = sensor;
                        }
                    }
                }
                if (hardwareItem.HardwareType == HardwareType.GpuAti || hardwareItem.HardwareType == HardwareType.GpuNvidia)
                {
                    gpu = hardwareItem;
                    foreach (var sensor in gpu.Sensors)
                    {
                        // GPU 负载
                        if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                        {
                            gpuLoadSensor = sensor;
                        }

                        // GPU 温度
                        if (sensor.SensorType == SensorType.Temperature && sensor.Name == "GPU Core")
                        {
                            gpuTemperatureSensor = sensor;
                        }
                    }
                }
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            // WS_EX_TRANSPARENT 鼠标穿透
            // WS_EX_TOOLWINDOW 不在任务切换视图中出现
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            globalModel.DateTime = now.ToString("F");
            globalModel.Week = weekMap[int.Parse(now.DayOfWeek.ToString("d"))];

            GlobalMemoryStatus(ref info);
            globalModel.MemoryLoad = info.dwMemoryLoad + "%";
            globalModel.UsedMemory = MemoryFormat(globalModel.TotalMemory - info.dwAvailPhys);
            globalModel.MemoryInfo = globalModel.UsedMemory + " / " + MemoryFormat(globalModel.TotalMemory);

            globalModel.CPU_Load = float.Parse(cpuLoadSensor.Value.ToString()).ToString("f2") + "%";
            globalModel.CPUTemperature = cpuTemperatureSensor.Value + "℃";

            globalModel.GPU_Load = float.Parse(gpuLoadSensor.Value.ToString()).ToString("f2") + "%";
            globalModel.GPUTemperature = gpuTemperatureSensor.Value + "℃";

            cpu.Update();
            gpu.Update();

            globalModel.UploadSpeed = "↑ " + MemoryFormat(monitorNet.upload()) + "/s";
            globalModel.DownloadSpeed = "↓ " + MemoryFormat(monitorNet.download()) + "/s";
        }

        private long GetTotalMemory()
        {
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            long memorySize = 0;
            foreach (ManagementObject mo in moc)
            {
                memorySize = long.Parse(mo["TotalPhysicalMemory"].ToString());
            }

            return memorySize;
        }

        private string MemoryFormat(double memorySize)
        {
            if (memorySize < 1024)
            {
                return memorySize.ToString("0.00") + "B"; // 这里是字节单位 byte
            }
            double m = memorySize / 1024D;
            if (m < 1024)
            {
                return m.ToString("0.00") + "KB";
            }
            m = memorySize / 1024 / 1024D;
            if (m < 1024)
            {
                return m.ToString("0.00") + "MB";
            }
            m = memorySize / 1024 / 1024 / 1024D;
            if (m < 1024)
            {
                return m.ToString("0.00") + "GB";
            }
            m = memorySize / 1024 / 1024 / 1024 / 1024D;
            if (m < 1024)
            {
                return m.ToString("0.00") + "TB";
            }
            m = memorySize / 1024 / 1024 / 1024 / 1024 / 1024D;
            if (m < 1024)
            {
                return m.ToString("0.00") + "PB";
            }

            return "";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
