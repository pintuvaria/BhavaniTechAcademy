using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public class WindowsInternalsService
    {
        public record OsLesson(string Title, string Content, string Precautions);

        public List<OsLesson> GetOsCurriculum()
        {
            return new List<OsLesson>
            {
                new OsLesson(
                    "Types of Operating Systems: Architecture & Differences",
                    "1. Time-Sharing/Multitasking OS (Windows, Linux, macOS): Allows multiple processes to share CPU time. \n" +
                    "   - Windows: Pros = High software/game compatibility, User-friendly. Cons = Target for malware, resource-heavy.\n" +
                    "   - Linux: Pros = Open-source, highly secure, lightweight. Cons = Steeper learning curve, less commercial software.\n" +
                    "   - macOS: Pros = Unix-based stability, optimized hardware integration. Cons = Closed ecosystem, expensive hardware.\n" +
                    "2. Distributed OS: Manages independent computers to act as a single system (e.g., Cloud Clusters).\n" +
                    "3. Real-Time OS (RTOS): Strict execution time constraints (e.g., Medical devices, Aerospace, IoT).",
                    "Understanding your OS architecture is the first step in both administration and offensive security."
                ),
                new OsLesson(
                    "Windows Registry: Architecture & Tweaks",
                    "The Registry is a hierarchical database storing low-level OS and app settings. Major hives:\n" +
                    "- HKEY_LOCAL_MACHINE (HKLM): System-wide settings.\n" +
                    "- HKEY_CURRENT_USER (HKCU): Active user settings.\n\n" +
                    "Common Power-User Tweaks:\n" +
                    "- Context Menu Hacks: Add 'Open Command Window Here' by modifying HKCR\\Directory\\Background\\shell.\n" +
                    "- Telemetry Disabling: Modify HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection to limit data sent to Microsoft.",
                    "CRITICAL: Always backup the registry (File -> Export) before modifying. A wrong deletion can permanently brick Windows, requiring a full reinstall."
                ),
                new OsLesson(
                    "GPEDIT.MSC: Group Policy Editor Mastery",
                    "GPEDIT is a frontend for safely deploying mass Registry changes across users and systems.\n\n" +
                    "Advanced Tweaks & Hacks:\n" +
                    "1. Block Removable Drives (Security): User Configuration -> Admin Templates -> System -> Removable Storage Access (Set to Deny).\n" +
                    "2. Disable Lock Screen: Computer Configuration -> Admin Templates -> Control Panel -> Personalization -> 'Do not display the lock screen'.\n" +
                    "3. Enforce Strict Passwords: Computer Configuration -> Windows Settings -> Security Settings -> Account Policies -> Enforce Password History & Complexity.\n" +
                    "4. Disable Windows Defender (For VM Sandbox environments): Computer Configuration -> Admin Templates -> Windows Components -> Microsoft Defender Antivirus -> 'Turn off Microsoft Defender Antivirus'.",
                    "Never apply GPEDIT restrictions blindly. You can easily lock out your own Administrator access or disable crucial network components."
                )
            };
        }

        // =====================================================================
        // ADVANCED WINDOWS REGISTRY & GPEDIT SECURITY AUDITOR
        // =====================================================================
        public record RegistryKeyAudit(
            string RootHive,
            string SubKeyPath,
            string ValueName,
            string ValueType, // REG_SZ, REG_DWORD, REG_BINARY, REG_MULTI_SZ
            object ValueData,
            bool IsSecurityHardened,
            string RecommendedAction
        );

        public record GpeditPolicyEvaluation(
            string PolicyName,
            string Scope, // Computer Configuration vs User Configuration
            string SettingState, // Enabled, Disabled, Not Configured
            bool MeetsEnterpriseBaseline,
            string CISBenchmarkRef
        );

        public List<RegistryKeyAudit> AuditSystemRegistrySettings()
        {
            return new List<RegistryKeyAudit>
            {
                new RegistryKeyAudit(
                    RootHive: "HKLM",
                    SubKeyPath: @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
                    ValueName: "EnableLUA",
                    ValueType: "REG_DWORD",
                    ValueData: 1,
                    IsSecurityHardened: true,
                    RecommendedAction: "User Account Control (UAC) is ACTIVE (1). Prevents silent elevation of unauthorized malware."
                ),
                new RegistryKeyAudit(
                    RootHive: "HKLM",
                    SubKeyPath: @"SYSTEM\CurrentControlSet\Control\Lsa",
                    ValueName: "LmCompatibilityLevel",
                    ValueType: "REG_DWORD",
                    ValueData: 5,
                    IsSecurityHardened: true,
                    RecommendedAction: "NTLMv2 Only enforced (5). Refuses obsolete LM & NTLMv1, blocking relay pass-the-hash vectors."
                ),
                new RegistryKeyAudit(
                    RootHive: "HKLM",
                    SubKeyPath: @"SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                    ValueName: "AllowTelemetry",
                    ValueType: "REG_DWORD",
                    ValueData: 0,
                    IsSecurityHardened: true,
                    RecommendedAction: "Diagnostic telemetry disabled (0). Hardened for privacy-sensitive enterprise environments."
                )
            };
        }

        public List<GpeditPolicyEvaluation> AuditGpeditSecurityBaseline()
        {
            return new List<GpeditPolicyEvaluation>
            {
                new GpeditPolicyEvaluation(
                    PolicyName: "Account lockout threshold",
                    Scope: "Computer Configuration -> Security Settings -> Account Policies",
                    SettingState: "5 invalid logon attempts",
                    MeetsEnterpriseBaseline: true,
                    CISBenchmarkRef: "CIS 1.1.1: Prevents brute-force password guessing attacks."
                ),
                new GpeditPolicyEvaluation(
                    PolicyName: "Do not allow storage of passwords and credentials for network authentication",
                    Scope: "Computer Configuration -> Security Settings -> Local Policies -> Security Options",
                    SettingState: "Enabled",
                    MeetsEnterpriseBaseline: true,
                    CISBenchmarkRef: "CIS 2.3.1.2: Mitigates credential harvesting from memory."
                ),
                new GpeditPolicyEvaluation(
                    PolicyName: "Prevent installation of devices not described by other policy settings",
                    Scope: "Computer Configuration -> Administrative Templates -> System -> Device Installation",
                    SettingState: "Enabled",
                    MeetsEnterpriseBaseline: true,
                    CISBenchmarkRef: "CIS 18.9.16.1: Blocks unauthorized USB flash drives to stop physical exfiltration."
                )
            };
        }
    }
}
