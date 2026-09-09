using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record LinuxNamespaceInfo(
        string NamespaceType,
        string ContainerView,
        string HostView,
        string IsolationDescription
    );

    public record CgroupLimits(
        int MemoryMaxMb,
        int MemoryCurrentMb,
        int CpuQuotaPercent,
        int CpuCurrentPercent,
        bool IsThrottled,
        bool IsOomKilled
    );

    public record OverlayFsLayer(
        string LayerType,
        string Path,
        string Description,
        bool IsReadOnly,
        List<string> Files
    );

    public class SimulatedContainer
    {
        public string Id { get; init; } = "";
        public string Name { get; set; } = "";
        public string Image { get; set; } = "alpine:latest";
        public string Status { get; set; } = "Running";
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string Hostname { get; set; } = "";
        public string IpAddress { get; set; } = "172.17.0.2";
        public int HostRootPid { get; set; } = 14205;
        public int MemoryLimitMb { get; set; } = 128;
        public int MemoryUsageMb { get; set; } = 24;
        public int CpuQuotaPercent { get; set; } = 50;
        public int CpuUsagePercent { get; set; } = 15;
        public bool IsOomKilled { get; set; } = false;
        public bool IsCpuThrottled { get; set; } = false;
        public Dictionary<string, string> Environment { get; } = new();
        public List<string> UpperDirChanges { get; } = new();

        public List<LinuxNamespaceInfo> GetNamespaces()
        {
            return new List<LinuxNamespaceInfo>
            {
                new("PID (Process ID)", "PID 1 (init / /bin/sh), PID 2 (app)", $"Host PID {HostRootPid}, {HostRootPid + 5}", "Isolates process tree. Inside container, process appears as PID 1."),
                new("NET (Networking)", $"eth0 ({IpAddress}/16)", "veth_bta0 <-> docker0 bridge (172.17.0.1)", "Private IP and loopback device; traffic routes via host virtual ethernet bridge."),
                new("MNT (Mount Points)", "/ (merged rootfs: Lower+Upper)", "/var/lib/docker/overlay2/<id>/merged", "Isolates filesystem mounts so container cannot see host filesystems."),
                new("UTS (Hostnames)", Hostname, Environment.GetValueOrDefault("HOSTNAME", "bhavani-host"), "Isolates system hostname and domain name."),
                new("IPC (Inter-Process)", "IPC Namespace 4026532840", "IPC Namespace 4026531839", "Isolates POSIX message queues and SysV shared memory segments."),
                new("USER (UID / GID)", "UID 0 (root inside container)", "UID 1000 (unprivileged user on host)", "User namespace remapping prevents container breakouts with host root rights.")
            };
        }

        public CgroupLimits GetCgroupStatus()
        {
            return new CgroupLimits(
                MemoryMaxMb: MemoryLimitMb,
                MemoryCurrentMb: MemoryUsageMb,
                CpuQuotaPercent: CpuQuotaPercent,
                CpuCurrentPercent: CpuUsagePercent,
                IsThrottled: IsCpuThrottled,
                IsOomKilled: IsOomKilled
            );
        }

        public List<OverlayFsLayer> GetOverlayFs()
        {
            return new List<OverlayFsLayer>
            {
                new("Lower (Read-Only)", "/var/lib/docker/overlay2/base/diff", "Base Alpine OS layer (bin, sbin, usr, etc)", true, new List<string> { "/bin/sh", "/bin/ls", "/etc/os-release", "/lib/libc.musl.so" }),
                new("Lower (Read-Only)", "/var/lib/docker/overlay2/runtime/diff", "Application binaries and libraries", true, new List<string> { "/app/server.js", "/app/package.json" }),
                new("Upper (Read-Write)", $"/var/lib/docker/overlay2/{Id[..8]}/diff", "Container modifications (Copy-On-Write layer)", false, UpperDirChanges.Count > 0 ? UpperDirChanges : new List<string> { "/tmp/.init.lock" }),
                new("Merged (Union View)", $"/var/lib/docker/overlay2/{Id[..8]}/merged", "Unified active filesystem presented to container processes", false, new List<string> { "/bin/sh", "/bin/ls", "/etc/os-release", "/app/server.js" }.Concat(UpperDirChanges).Distinct().ToList())
            };
        }
    }

    public class ContainerLabService
    {
        private readonly List<SimulatedContainer> _containers = new();
        private int _nextPid = 14205;
        private int _nextIpSuffix = 2;

        public ContainerLabService()
        {
            CreateContainer("bhavani-web", "nginx:alpine", 128, 50);
        }

        public SimulatedContainer CreateContainer(string name, string image, int memoryMb = 128, int cpuPercent = 50)
        {
            var id = Guid.NewGuid().ToString("N")[..12];
            var container = new SimulatedContainer
            {
                Id = id,
                Name = string.IsNullOrWhiteSpace(name) ? $"container-{id[..6]}" : name,
                Image = string.IsNullOrWhiteSpace(image) ? "alpine:latest" : image,
                Hostname = $"node-{id[..6]}",
                IpAddress = $"172.17.0.{_nextIpSuffix++}",
                HostRootPid = _nextPid,
                MemoryLimitMb = Math.Max(32, memoryMb),
                MemoryUsageMb = 28,
                CpuQuotaPercent = Math.Clamp(cpuPercent, 10, 100),
                CpuUsagePercent = 12,
                Status = "Running"
            };
            _nextPid += 10;
            _containers.Add(container);
            return container;
        }

        public IReadOnlyList<SimulatedContainer> GetAllContainers() => _containers.AsReadOnly();

        public SimulatedContainer? GetContainer(string idOrName)
        {
            return _containers.FirstOrDefault(c =>
                c.Id.StartsWith(idOrName, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Equals(idOrName, StringComparison.OrdinalIgnoreCase));
        }

        public bool StopContainer(string idOrName)
        {
            var c = GetContainer(idOrName);
            if (c == null) return false;
            c.Status = "Exited (0)";
            c.CpuUsagePercent = 0;
            return true;
        }

        public string ApplyResourceStress(string idOrName, int targetMemoryMb, int targetCpuPercent)
        {
            var c = GetContainer(idOrName);
            if (c == null) return "Error: Container not found.";
            if (c.Status.StartsWith("Exited")) return "Error: Cannot stress stopped container.";

            var sb = new StringBuilder();
            sb.AppendLine($"⚡ Applying resource load to container '{c.Name}' ({c.Id})...");

            c.CpuUsagePercent = targetCpuPercent;
            if (targetCpuPercent > c.CpuQuotaPercent)
            {
                c.IsCpuThrottled = true;
                sb.AppendLine($"⚠️ [cgroups cpu.max]: CPU target {targetCpuPercent}% exceeds quota {c.CpuQuotaPercent}%. Throttling active! Execution slowed down.");
            }
            else
            {
                c.IsCpuThrottled = false;
                sb.AppendLine($"✅ [cgroups cpu.max]: CPU usage {targetCpuPercent}% within quota {c.CpuQuotaPercent}%.");
            }

            c.MemoryUsageMb = targetMemoryMb;
            if (targetMemoryMb > c.MemoryLimitMb)
            {
                c.IsOomKilled = true;
                c.Status = "Exited (137) - OOMKilled";
                c.CpuUsagePercent = 0;
                sb.AppendLine($"🚨 [cgroups memory.max]: Memory target {targetMemoryMb} MB exceeds limit {c.MemoryLimitMb} MB!");
                sb.AppendLine("💥 Kernel Out-of-Memory (OOM) Killer invoked! Process terminated with Exit Code 137 (SIGKILL 128+9).");
            }
            else
            {
                c.IsOomKilled = false;
                sb.AppendLine($"✅ [cgroups memory.max]: Memory usage {targetMemoryMb} MB / {c.MemoryLimitMb} MB ({(int)((double)targetMemoryMb / c.MemoryLimitMb * 100)}% utilized).");
            }

            return sb.ToString();
        }

        public string WriteContainerFile(string idOrName, string filePath, string content)
        {
            var c = GetContainer(idOrName);
            if (c == null) return "Error: Container not found.";
            if (c.Status.StartsWith("Exited")) return "Error: Container is not running.";

            c.UpperDirChanges.Add($"{filePath} ({content.Length} bytes modified via CoW)");
            c.MemoryUsageMb = Math.Min(c.MemoryLimitMb - 1, c.MemoryUsageMb + 2);
            return $"✅ [OverlayFS CoW]: File '{filePath}' written to upperdir (read-write diff layer). Base read-only image remains untouched!";
        }

        public string ExecuteCli(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine)) return "docker: missing command. Type 'docker help'.";

            var tokens = commandLine.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) return "";

            var cmd = tokens[0].Equals("docker", StringComparison.OrdinalIgnoreCase) && tokens.Length > 1
                ? tokens[1].ToLowerInvariant()
                : tokens[0].ToLowerInvariant();

            return cmd switch
            {
                "ps" => FormatDockerPs(),
                "run" => HandleDockerRun(tokens),
                "stop" => HandleDockerStop(tokens),
                "inspect" => HandleDockerInspect(tokens),
                "top" => HandleDockerTop(tokens),
                "help" or "--help" => GetHelpText(),
                _ => $"docker: '{cmd}' is not a recognized docker command. Run 'docker help'."
            };
        }

        private string FormatDockerPs()
        {
            var sb = new StringBuilder();
            sb.AppendLine("CONTAINER ID   IMAGE           COMMAND       STATUS        PORTS           NAMES");
            foreach (var c in _containers)
            {
                sb.AppendLine($"{c.Id,-14} {c.Image,-15} \"/bin/sh\"      {c.Status,-12} {c.IpAddress}:80     {c.Name}");
            }
            return sb.ToString();
        }

        private string HandleDockerRun(string[] tokens)
        {
            string name = $"app-{Guid.NewGuid().ToString()[..4]}";
            string image = "alpine:latest";
            int mem = 128;
            int cpu = 50;

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "--name" && i + 1 < tokens.Length) name = tokens[++i];
                else if (tokens[i] == "-m" && i + 1 < tokens.Length && int.TryParse(tokens[++i].Replace("m", "").Replace("M", ""), out int mVal)) mem = mVal;
                else if (tokens[i] == "--cpus" && i + 1 < tokens.Length && double.TryParse(tokens[++i], out double cpuVal)) cpu = (int)(cpuVal * 100);
                else if (!tokens[i].StartsWith("-") && !tokens[i].Equals("docker", StringComparison.OrdinalIgnoreCase) && !tokens[i].Equals("run", StringComparison.OrdinalIgnoreCase))
                {
                    image = tokens[i];
                }
            }

            var c = CreateContainer(name, image, mem, cpu);
            return $"{c.Id}\nContainer '{c.Name}' started successfully with {c.MemoryLimitMb}MB limit and {c.CpuQuotaPercent}% CPU quota.\nIP: {c.IpAddress}, Host PID: {c.HostRootPid}";
        }

        private string HandleDockerStop(string[] tokens)
        {
            if (tokens.Length < 3 && !tokens[0].Equals("stop", StringComparison.OrdinalIgnoreCase))
                return "Usage: docker stop <container-id-or-name>";

            string target = tokens[^1];
            return StopContainer(target)
                ? $"Stopped container: {target}"
                : $"Error: No such container: {target}";
        }

        private string HandleDockerInspect(string[] tokens)
        {
            string target = tokens.Length > 2 ? tokens[2] : (tokens.Length > 1 ? tokens[1] : "");
            var c = GetContainer(target);
            if (c == null) return $"Error: No such container: {target}";

            var sb = new StringBuilder();
            sb.AppendLine("=== DOCKER CONTAINER INSPECT ===");
            sb.AppendLine($"ID: {c.Id}");
            sb.AppendLine($"Name: /{c.Name}");
            sb.AppendLine($"Image: {c.Image}");
            sb.AppendLine($"State: {c.Status}");
            sb.AppendLine($"Hostname: {c.Hostname}");
            sb.AppendLine($"NetworkSettings.IPAddress: {c.IpAddress}");
            sb.AppendLine($"HostConfig.Memory: {c.MemoryLimitMb} MB");
            sb.AppendLine($"HostConfig.CpuQuota: {c.CpuQuotaPercent}%");
            sb.AppendLine($"HostPID: {c.HostRootPid}");
            sb.AppendLine("Namespaces Isolated: PID, NET, MNT, UTS, IPC, USER");
            sb.AppendLine($"Storage Driver: overlay2");
            return sb.ToString();
        }

        private string HandleDockerTop(string[] tokens)
        {
            string target = tokens.Length > 2 ? tokens[2] : (tokens.Length > 1 ? tokens[1] : "");
            var c = GetContainer(target);
            if (c == null) return $"Error: No such container: {target}";

            var sb = new StringBuilder();
            sb.AppendLine($"PID (Container)   PID (Host)   CMD");
            sb.AppendLine($"1                 {c.HostRootPid}        /bin/sh");
            sb.AppendLine($"2                 {c.HostRootPid + 5}        node /app/server.js");
            return sb.ToString();
        }

        private string GetHelpText()
        {
            return @"Bhavani Container Engine (Docker & Linux Isolation Simulator)
Available Commands:
  docker ps                      List active containers and statuses
  docker run -d --name <name> -m <mb> <image>   Spawn new isolated container
  docker stop <name/id>          Gracefully terminate running container
  docker inspect <name/id>       Inspect JSON manifest, namespaces & cgroups
  docker top <name/id>           Inspect host vs container PID mappings";
        }
    }
}
