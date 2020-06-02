using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace WpfApp1
{
    public class MonitorNetStream
    {
        List<PerformanceCounter> pcs = new List<PerformanceCounter>();
        List<PerformanceCounter> pcs2 = new List<PerformanceCounter>();

        public MonitorNetStream()
        {
            string[] names = getAdapter();
            foreach (string name in names)
            {
                try
                {
                    PerformanceCounter pc = new PerformanceCounter();
                    pc.CategoryName = "Network Interface";
                    pc.CounterName = "Bytes Received/sec";
                    pc.InstanceName = name.Replace('(', '[').Replace(')', ']');
                    pc.MachineName = ".";

                    PerformanceCounter pc2 = new PerformanceCounter();
                    pc2.CategoryName = "Network Interface";
                    pc2.CounterName = "Bytes Sent/sec";
                    pc2.InstanceName = name.Replace('(', '[').Replace(')', ']');
                    pc2.MachineName = ".";

                    pc.NextValue();
                    pc2.NextValue();
                    pcs.Add(pc);
                    pcs2.Add(pc2);
                }
                catch
                {
                    continue;
                }
            }
        }

        public double upload()
        {
            double sent = 0;
            foreach (PerformanceCounter pc in pcs2)
            {
                sent += pc.NextValue();
            }
            return sent;
        }

        public double download()
        {
            double recv = 0;
            foreach (PerformanceCounter pc in pcs)
            {
                recv += pc.NextValue();
            }
            return recv;
        }

        public string[] getAdapter()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string[] name = new string[adapters.Length];
            int index = 0;
            foreach (NetworkInterface ni in adapters)
            {
                name[index] = ni.Description;
                index++;
            }
            return name;
        }
    }
}
