using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class GlobalModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string dateTime = "";
        public string DateTime
        {
            get
            {
                return this.dateTime;
            }
            set
            {
                dateTime = value;
                RaisePropertyChanged("DateTime");
            }
        }

        private string week = "";
        public string Week
        {
            get
            {
                return this.week;
            }
            set
            {
                week = value;
                RaisePropertyChanged("Week");
            }
        }

        private long totalMemory = 0;
        public long TotalMemory
        {
            get
            {
                return this.totalMemory;
            }
            set
            {
                totalMemory = value;
                RaisePropertyChanged("TotalMemory");
            }
        }

        private string usedMemory = "";
        public string UsedMemory
        {
            get
            {
                return this.usedMemory;
            }
            set
            {
                usedMemory = value;
                RaisePropertyChanged("UsedMemory");
            }
        }

        private string memoryLoad = "";
        public string MemoryLoad
        {
            get
            {
                return this.memoryLoad;
            }
            set
            {
                memoryLoad = value;
                RaisePropertyChanged("MemoryLoad");
            }
        }

        private string memoryInfo = "";
        public string MemoryInfo
        {
            get
            {
                return this.memoryInfo;
            }
            set
            {
                memoryInfo = value;
                RaisePropertyChanged("MemoryInfo");
            }
        }

        private string cpuLoad = "";
        public string CPU_Load
        {
            get
            {
                return this.cpuLoad;
            }
            set
            {
                cpuLoad = value;
                RaisePropertyChanged("CPU_Load");
            }
        }

        // CPUTemperature

        private string cpuTemperature = "";
        public string CPUTemperature
        {
            get
            {
                return this.cpuTemperature;
            }
            set
            {
                cpuTemperature = value;
                RaisePropertyChanged("CPUTemperature");
            }
        }

        private string gpuTemperature = "";
        public string GPUTemperature
        {
            get
            {
                return this.gpuTemperature;
            }
            set
            {
                gpuTemperature = value;
                RaisePropertyChanged("GPUTemperature");
            }
        }

        private string gpuLoad = "";
        public string GPU_Load
        {
            get
            {
                return this.gpuLoad;
            }
            set
            {
                gpuLoad = value;
                RaisePropertyChanged("GPU_Load");
            }
        }

        private string uploadSpeed = "";
        public string UploadSpeed
        {
            get
            {
                return this.uploadSpeed;
            }
            set
            {
                uploadSpeed = value;
                RaisePropertyChanged("UploadSpeed");
            }
        }

        private string downloadSpeed = "";
        public string DownloadSpeed
        {
            get
            {
                return this.downloadSpeed;
            }
            set
            {
                downloadSpeed = value;
                RaisePropertyChanged("DownloadSpeed");
            }
        }
    }
}
