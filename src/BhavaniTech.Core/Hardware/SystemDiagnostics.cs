using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BhavaniTech.Core.Hardware
{
    public record SystemInfo(
        int LogicalCores,
        double TotalRamMB,
        double AvailableRamMB,
        bool IsLowEndHardware,
        string OperatingSystem,
        string CpuArchitecture
    );

    public static class SystemDiagnostics
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public static SystemInfo GetSystemCapabilities()
        {
            int cores = Environment.ProcessorCount;
            double totalRamMB = 2048; // Default fallback for low-spec assumption
            
            try
            {
                if (GetPhysicallyInstalledSystemMemory(out long memoryInKB))
                {
                    totalRamMB = memoryInKB / 1024.0;
                }
            }
            catch
            {
                // Fallback estimation
                totalRamMB = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024.0 * 1024.0);
            }

            // Determine if the machine qualifies as low-end (<= 2.5 GB RAM or <= 2 CPU cores)
            bool isLowEnd = cores <= 2 || totalRamMB <= 2560;

            return new SystemInfo(
                LogicalCores: cores,
                TotalRamMB: Math.Round(totalRamMB, 1),
                AvailableRamMB: Math.Round(GetAvailableRamMB(), 1),
                IsLowEndHardware: isLowEnd,
                OperatingSystem: RuntimeInformation.OSDescription,
                CpuArchitecture: RuntimeInformation.OSArchitecture.ToString()
            );
        }

        public static double GetCurrentMemoryUsageMB()
        {
            using var proc = Process.GetCurrentProcess();
            return proc.WorkingSet64 / (1024.0 * 1024.0);
        }

        public static double GetPrivateBytesMB()
        {
            using var proc = Process.GetCurrentProcess();
            return proc.PrivateMemorySize64 / (1024.0 * 1024.0);
        }

        public static double GetAvailableRamMB()
        {
            try
            {
                var gcInfo = GC.GetGCMemoryInfo();
                return gcInfo.TotalAvailableMemoryBytes / (1024.0 * 1024.0);
            }
            catch
            {
                return 1024.0;
            }
        }

        public static void OptimizeMemoryUsage()
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
        }
    }
}
