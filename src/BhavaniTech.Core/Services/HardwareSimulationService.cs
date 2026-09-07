using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public record PcComponent(string Category, string Name, string SocketType, int Wattage, double PriceUSD, string Details);
    public record BuildValidationResult(bool IsCompatible, List<string> Warnings, List<string> Errors, int EstimatedPowerUsageWatts);

    public record MemoryPerformanceResult(double TransferRateMTS, double LatencyNanoseconds, double DualChannelBandwidthGBps);
    public record PciePerformanceResult(string PcieGen, int Lanes, double MaxBandwidthGBps);

    public static class HardwareSimulationService
    {
        public static BuildValidationResult ValidatePcBuild(
            PcComponent cpu,
            PcComponent motherboard,
            PcComponent ram,
            PcComponent gpu,
            PcComponent psu)
        {
            var warnings = new List<string>();
            var errors = new List<string>();

            // Socket Check
            if (!string.Equals(cpu.SocketType, motherboard.SocketType, System.StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"Socket Mismatch! CPU requires socket {cpu.SocketType}, but Motherboard has socket {motherboard.SocketType}.");
            }

            // Power Consumption Calculation
            int totalWattage = cpu.Wattage + gpu.Wattage + 60; // 60W baseline for RAM/HDD/Motherboard
            if (psu.Wattage < totalWattage)
            {
                errors.Add($"Power Supply Insufficient! Total Build needs ~{totalWattage}W, but PSU is only {psu.Wattage}W.");
            }
            else if (psu.Wattage < totalWattage + 100)
            {
                warnings.Add($"Power Supply headroom is tight. Total usage is {totalWattage}W vs PSU rating {psu.Wattage}W.");
            }

            return new BuildValidationResult(
                IsCompatible: errors.Count == 0,
                Warnings: warnings,
                Errors: errors,
                EstimatedPowerUsageWatts: totalWattage
            );
        }

        public static MemoryPerformanceResult CalculateMemoryPerformance(double speedMhz, int casLatencyCL, bool isDualChannel)
        {
            // True Latency (ns) = (CL * 2000) / Speed_MHz
            double trueLatencyNs = (casLatencyCL * 2000.0) / Math.Max(1000, speedMhz);

            // Theoretical Bandwidth = Speed (MT/s) * 8 bytes (64-bit bus) * channels
            double bytesPerSec = speedMhz * 1000000.0 * 8.0 * (isDualChannel ? 2.0 : 1.0);
            double bandwidthGBps = bytesPerSec / (1024.0 * 1024.0 * 1024.0);

            return new MemoryPerformanceResult(
                TransferRateMTS: speedMhz,
                LatencyNanoseconds: Math.Round(trueLatencyNs, 2),
                DualChannelBandwidthGBps: Math.Round(bandwidthGBps, 2)
            );
        }

        public static PciePerformanceResult CalculatePcieBandwidth(int genVersion, int lanesCount)
        {
            // Per lane throughput in GB/s: Gen1=0.25, Gen2=0.5, Gen3=0.985, Gen4=1.969, Gen5=3.938
            double perLaneGBps = genVersion switch
            {
                1 => 0.25,
                2 => 0.50,
                3 => 0.985,
                4 => 1.969,
                5 => 3.938,
                _ => 1.969
            };

            double maxBandwidth = perLaneGBps * lanesCount;
            return new PciePerformanceResult(
                PcieGen: $"PCIe Gen {genVersion}.0",
                Lanes: lanesCount,
                MaxBandwidthGBps: Math.Round(maxBandwidth, 2)
            );
        }
    }
}
