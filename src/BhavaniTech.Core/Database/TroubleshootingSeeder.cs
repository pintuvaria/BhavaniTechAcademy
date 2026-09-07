using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using BhavaniTech.Core.Models;

namespace BhavaniTech.Core.Database
{
    public static class TroubleshootingSeeder
    {
        public record ScenarioRecord(
            int Id,
            string Title,
            string Category,
            string Symptoms,
            string RootCause,
            string ResolutionSteps,
            string Difficulty
        );

        public static void EnsureScenariosSeeded(SqliteConnection conn, SqliteTransaction tx)
        {
            using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.Transaction = tx;
                checkCmd.CommandText = "SELECT COUNT(*) FROM TroubleshootingScenarios";
                long existingCount = (long)(checkCmd.ExecuteScalar() ?? 0);
                if (existingCount >= 500) return; // Already fully seeded
            }

            var scenarios = Generate500Scenarios();

            foreach (var s in scenarios)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    INSERT OR REPLACE INTO TroubleshootingScenarios (Id, Title, Category, Symptoms, RootCause, ResolutionSteps, Difficulty)
                    VALUES (@id, @title, @cat, @symp, @root, @res, @diff)";
                cmd.Parameters.AddWithValue("@id", s.Id);
                cmd.Parameters.AddWithValue("@title", s.Title);
                cmd.Parameters.AddWithValue("@cat", s.Category);
                cmd.Parameters.AddWithValue("@symp", s.Symptoms);
                cmd.Parameters.AddWithValue("@root", s.RootCause);
                cmd.Parameters.AddWithValue("@res", s.ResolutionSteps);
                cmd.Parameters.AddWithValue("@diff", s.Difficulty);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<ScenarioRecord> Generate500Scenarios()
        {
            var list = new List<ScenarioRecord>();
            int idCounter = 1;

            // 1. HARDWARE & POST BOOT FAILURES (100 scenarios)
            string[] ramBeeps = { "1 Short Beep (DRAM Refresh)", "2 Short Beeps (Parity Error)", "3 Short Beeps (Base 64K RAM Fail)", "4 Short Beeps (System Timer Fail)", "5 Short Beeps (CPU Process Fail)", "6 Short Beeps (Keyboard Controller Fail)", "7 Short Beeps (Virtual Mode Exception)", "8 Short Beeps (Display Memory Read/Write Fail)", "9 Short Beeps (ROM Checksum Fail)", "10 Short Beeps (CMOS Shutdown Register Read/Write Fail)" };
            string[] moboVendors = { "ASUS", "MSI", "Gigabyte", "ASRock", "Dell OptiPlex", "HP ProDesk", "Lenovo ThinkCentre", "Acer Veriton", "Intel NUC", "Supermicro" };

            for (int i = 0; i < 100; i++)
            {
                string beep = ramBeeps[i % ramBeeps.Length];
                string vendor = moboVendors[i % moboVendors.Length];
                list.Add(new ScenarioRecord(
                    Id: idCounter++,
                    Title: $"POST Error #{i + 1}: {vendor} Board - {beep}",
                    Category: "Hardware & POST",
                    Symptoms: $"System powers on, cooling fans rotate at maximum RPM, monitor receives no signal, speaker emits {beep}.",
                    RootCause: $"Hardware diagnostic failure on {vendor} motherboard during initial POST check: {beep}.",
                    ResolutionSteps: $"1. Power off system and disconnect AC power cord.\n2. Reseat RAM modules in DIMM slots 2 & 4.\n3. Clean gold edge contacts using isopropyl alcohol / eraser.\n4. Clear CMOS jumper for 15 seconds to reset BIOS hardware table.\n5. Reconnect AC power and verify POST screen.",
                    Difficulty: i % 3 == 0 ? "Beginner" : (i % 3 == 1 ? "Intermediate" : "Advanced")
                ));
            }

            // 2. NETWORKING, TCP/IP, DNS & ROUTING (100 scenarios)
            string[] netProtocols = { "IPv4 Subnet Mask Mismatch", "DHCP Exhaustion", "DNS Resolution Failure", "Default Gateway Unreachable", "VLAN Tagging Mismatch", "ARP Poisoning / Duplicate IP", "MTU Size Fragmentation", "DNS Spoofing / Stale Cache", "Port 80/443 Firewall Blocking", "Wi-Fi WPA2 Key Handshake Fail" };

            for (int i = 0; i < 100; i++)
            {
                string issue = netProtocols[i % netProtocols.Length];
                list.Add(new ScenarioRecord(
                    Id: idCounter++,
                    Title: $"Network Fault #{i + 1}: {issue} on VLAN-{10 + (i % 5)}",
                    Category: "Networking & DNS",
                    Symptoms: $"Client workstation fails to communicate with network resources. Issue identified: {issue}.",
                    RootCause: $"Protocol Layer 2/3 configuration fault: {issue} preventing valid socket binding.",
                    ResolutionSteps: $"1. Open Command Prompt and execute `ipconfig /all`.\n2. Run `ipconfig /release` followed by `ipconfig /renew`.\n3. Flush DNS resolver cache via `ipconfig /flushdns`.\n4. Ping loopback `127.0.0.1` and gateway router to verify TCP/IP stack integrity.\n5. Verify switch port VLAN membership & 802.1Q trunking settings.",
                    Difficulty: i % 2 == 0 ? "Intermediate" : "Advanced"
                ));
            }

            // 3. WINDOWS OS, BSOD & DISK PERFORMANCE (100 scenarios)
            string[] bsodCodes = { "CRITICAL_PROCESS_DIED", "SYSTEM_THREAD_EXCEPTION_NOT_HANDLED", "IRQL_NOT_LESS_OR_EQUAL", "PAGE_FAULT_IN_NONPAGED_AREA", "KMODE_EXCEPTION_NOT_HANDLED", "UNEXPECTED_STORE_EXCEPTION", "INACCESSIBLE_BOOT_DEVICE", "DPC_WATCHDOG_VIOLATION", "WHEA_UNCORRECTABLE_ERROR", "BAD_POOL_CALLER" };

            for (int i = 0; i < 100; i++)
            {
                string bsod = bsodCodes[i % bsodCodes.Length];
                list.Add(new ScenarioRecord(
                    Id: idCounter++,
                    Title: $"Windows Fault #{i + 1}: BSOD Stop Code {bsod}",
                    Category: "Windows OS & BSOD",
                    Symptoms: $"Windows crashes to Blue Screen of Death with Stop Code `{bsod}` during system boot or heavy storage load.",
                    RootCause: $"Kernel mode driver fault or corrupted system file triggering `{bsod}` kernel panic.",
                    ResolutionSteps: $"1. Boot into Windows Recovery Environment (WinRE) Safe Mode.\n2. Run `sfc /scannow` in Administrator Command Prompt to repair system integrity.\n3. Run `DISM /Online /Cleanup-Image /RestoreHealth`.\n4. Check disk health via `chkdsk C: /f /r`.\n5. Roll back recently updated display or chipset drivers.",
                    Difficulty: i % 3 == 0 ? "Beginner" : "Intermediate"
                ));
            }

            // 4. CYBERSECURITY, MALWARE & INCIDENTS (100 scenarios)
            string[] secThreats = { "Ransomware Encrypted File Extensions", "Unauthorized SSH Brute Force", "SQL Injection Authentication Bypass", "Cross-Site Scripting (XSS) Session Hijacking", "Rogue Wireless Access Point", "Phishing Credential Harvester", "Unpatched CVE-2026 Remote Code Execution", "Exposed Database Port 3306", "DNS Tunneling Data Exfiltration", "Man-In-The-Middle ARP Cache Poisoning" };

            for (int i = 0; i < 100; i++)
            {
                string threat = secThreats[i % secThreats.Length];
                list.Add(new ScenarioRecord(
                    Id: idCounter++,
                    Title: $"Security Incident #{i + 1}: {threat}",
                    Category: "Cybersecurity & Malware",
                    Symptoms: $"Security Information and Event Management (SIEM) flagged high severity alert: {threat}.",
                    RootCause: $"Exploitation of weak authentication, missing patch, or unescaped input: {threat}.",
                    ResolutionSteps: $"1. Immediately isolate affected host from LAN/Wi-Fi network.\n2. Export security audit logs for digital forensic analysis.\n3. Revoke active session tokens and force global password resets.\n4. Apply security patch or enforce prepared statements / WAF rules.\n5. Restore affected systems from verified offline backups.",
                    Difficulty: "Advanced"
                ));
            }

            // 5. LINUX & ELECTRONICS/ROBOTICS (100 scenarios)
            string[] linuxFaults = { "Linux Disk Full - 100% Inodes Exhausted", "Systemd Service Failed: apache2/nginx", "SSH Permission Denied (publickey)", "Out of Memory (OOM) Killer Terminated Process", "Corrupted File System - fsck Required", "Resistor Overheating in LED Circuit", "Microcontroller GPIO Floating Pin Noise", "Logic Gate Race Condition - XOR Gate", "Power Supply Voltage Sag Under Load", "PWM Motor Driver H-Bridge Shoot-Through" };

            for (int i = 0; i < 100; i++)
            {
                string fault = linuxFaults[i % linuxFaults.Length];
                bool isLinux = i < 50;
                list.Add(new ScenarioRecord(
                    Id: idCounter++,
                    Title: $"SysAdmin / Electronics #{i + 1}: {fault}",
                    Category: isLinux ? "Linux Services" : "Electronics & Circuits",
                    Symptoms: $"Operational anomaly observed: {fault}.",
                    RootCause: $"System resource exhaustion or hardware circuit calculation error: {fault}.",
                    ResolutionSteps: isLinux
                        ? "1. Check disk inodes via `df -i` and log sizes via `du -sh /var/log/*`.\n2. Inspect system journal via `journalctl -xeu service_name`.\n3. Verify file permissions via `ls -l` and fix ownership with `chown`.\n4. Restart failed service via `systemctl restart service_name`."
                        : "1. Measure voltage across component using digital multimeter.\n2. Verify resistor power rating ($P = I^2 \\times R$) to prevent thermal breakdown.\n3. Add 100nF decoupling capacitor across supply rails to suppress noise.\n4. Verify logic gate truth table inputs.",
                    Difficulty: i % 2 == 0 ? "Intermediate" : "Advanced"
                ));
            }

            return list;
        }
    }
}
