using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public static partial class CourseDetailsProvider
    {
        public static Dictionary<string, CourseDetails> GetGreyHatDetails()
        {
            return new Dictionary<string, CourseDetails>(System.StringComparer.OrdinalIgnoreCase)
            {
                ["GH101"] = new CourseDetails(
                    CourseId: "GH101",
                    CourseTitle: "Grey Hat Hacking: Professional Penetration Testing & Security Research",
                    Category: "Cybersecurity",
                    Summary: "Master the complete ethical hacking lifecycle — passive OSINT reconnaissance, active scanning, web exploitation (OWASP Top 10), network penetration testing, Linux & Windows privilege escalation, Metasploit post-exploitation, and professional bug-bounty report writing with CVSS scoring. Prepares for eJPT, OSCP, CEH, and BSCP certifications.",
                    Prerequisites:
                        "• Completed NET101 (Networking & CCNA/CCNP Essentials)\n" +
                        "• Completed LNX101 (Linux Systems & Kernel Administration)\n" +
                        "• Completed SEC101 (Cybersecurity & Ethical Hacking Mastery)\n" +
                        "• Basic comfort with command-line terminals (Linux bash / Windows CMD)\n" +
                        "• Understanding of TCP/IP protocols and HTTP request-response cycle",
                    HardwareRequirements:
                        "• CPU: x86-64 dual-core 1.8 GHz or faster\n" +
                        "• RAM: 4 GB minimum (8 GB recommended — running 2 VMs simultaneously)\n" +
                        "• Storage: 30 GB free (Kali Linux VM + target VMs)\n" +
                        "• Virtualisation: Intel VT-x or AMD-V enabled in BIOS\n" +
                        "• Network: NAT or Host-Only adapter for safe isolated lab",
                    InstallationSteps:
                        "1. INSTALL HYPERVISOR (choose one):\n" +
                        "   - VirtualBox: https://www.virtualbox.org/wiki/Downloads (free)\n" +
                        "   - VMware Workstation Player: https://www.vmware.com (free for personal use)\n\n" +
                        "2. DOWNLOAD KALI LINUX (attacker machine):\n" +
                        "   - https://www.kali.org/get-kali/#kali-virtual-machines\n" +
                        "   - Import pre-built .ova — no manual install needed\n\n" +
                        "3. DOWNLOAD PRACTICE TARGETS (intentionally vulnerable):\n" +
                        "   - Metasploitable 3: https://github.com/rapid7/metasploitable3\n" +
                        "   - DVWA via Docker: docker run -d -p 80:80 vulnerables/web-dvwa\n" +
                        "   - VulnHub (free offline machines): https://www.vulnhub.com\n\n" +
                        "4. CREATE FREE ACCOUNTS:\n" +
                        "   - HackerOne: https://hackerone.com\n" +
                        "   - TryHackMe: https://tryhackme.com (beginner rooms: free)\n" +
                        "   - HackTheBox: https://www.hackthebox.com (Starting Point: free)",
                    EnvironmentVerification:
                        "Run in Kali Linux Terminal:\n" +
                        "  nmap --version\n" +
                        "  sqlmap --version\n" +
                        "  msfconsole --version\n" +
                        "  burpsuite &\n" +
                        "Expected: Nmap 7.x, sqlmap 1.x, Metasploit 6.x banners, Burp Suite GUI launches.\n\n" +
                        "Verify DVWA target:\n" +
                        "  curl http://localhost/dvwa/login.php\n" +
                        "Expected: DVWA HTML login page response.",
                    RecommendedTools:
                        "Kali Linux, Burp Suite Community, Nmap, sqlmap, Gobuster, theHarvester, Metasploit, " +
                        "Hydra, Aircrack-ng, Wireshark, LinPEAS, WinPEAS, CrackMapExec, Impacket Suite, " +
                        "OWASP ZAP, subfinder, amass, nuclei, ffuf, John the Ripper, Hashcat",
                    CareerAndCertifications:
                        "CAREER PATHS:\n" +
                        "• Penetration Tester (VAPT Analyst)\n" +
                        "• Red Team Operator\n" +
                        "• Bug Bounty Hunter (freelance / full-time)\n" +
                        "• Application Security Engineer\n" +
                        "• Offensive Security Consultant\n" +
                        "• Security Researcher / CVE Hunter\n\n" +
                        "RECOMMENDED CERTIFICATIONS:\n" +
                        "• eJPT (eLearnSecurity) — entry level, INR ~5,000\n" +
                        "• CEH (EC-Council) — industry recognised, ~INR 50,000\n" +
                        "• OSCP (OffSec) — gold standard for pentesters, ~INR 1,20,000\n" +
                        "• BSCP (PortSwigger Burp Suite) — elite web-app specialist cert\n\n" +
                        "SALARY RANGES (India):\n" +
                        "• Junior VAPT Analyst: INR 3–6 LPA\n" +
                        "• Mid-Level Pentester: INR 8–18 LPA\n" +
                        "• Senior Red Team Operator: INR 20–45 LPA\n" +
                        "• Bug Bounty (top researchers): USD 100K–2M/year global"
                )
            };
        }
    }
}
