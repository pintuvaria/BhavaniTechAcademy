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
    }
}
