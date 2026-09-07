# CEH (Certified Ethical Hacker) - Complete Defensive Tutorial

*Disclaimer: This tutorial is strictly for educational and defensive purposes to understand how attackers operate so you can secure systems against them.*

## Module 1: Reconnaissance & Footprinting
Footprinting is the first phase where attackers gather maximum intelligence about a target.
- **Passive Footprinting:** Gathering data without directly interacting with the target (e.g., WHOIS lookups, OSINT, Shodan, Google Dorks).
- **Active Footprinting:** Direct interaction with the target (e.g., DNS Zone Transfers, Social Engineering).

## Module 2: Scanning & Enumeration
Once the perimeter is defined, attackers map the internal structure.
- **Nmap Scanning:** 
  - `-sS` (TCP SYN Stealth Scan): Sends a SYN, waits for SYN-ACK, then drops the connection with an RST to avoid logging.
  - `-sV` (Version Detection): Probes open ports to determine the exact software and version running.
- **Enumeration:** Extracting valid usernames, machine names, and network resources using protocols like SMB (Port 445), SNMP (Port 161), and LDAP.

## Module 3: System Hacking & Privilege Escalation
After gaining initial access (usually via unpatched vulnerabilities or phishing), the attacker attempts to gain Administrative or Root privileges.
- **Password Cracking:** Offline cracking of hashed passwords using tools like Hashcat or John the Ripper. Defended by using strong, salted hashing algorithms like Argon2.
- **Privilege Escalation:** Exploiting misconfigured services, unquoted service paths, or kernel vulnerabilities to move from a standard user to `NT AUTHORITY\SYSTEM`.

## Module 4: Web Application Vulnerabilities
- **SQL Injection (SQLi):** Injecting malicious SQL statements into input fields (e.g., `' OR 1=1--`) to bypass authentication or dump databases. Defended by **Parameterized Queries**.
- **Cross-Site Scripting (XSS):** Injecting malicious JavaScript into web pages viewed by other users to steal session cookies. Defended by **Input Validation** and **Output Encoding**.
- **Insecure Direct Object References (IDOR):** Manipulating parameters (e.g., `user_id=5` to `user_id=6`) to access unauthorized data. Defended by strict server-side authorization checks.

## Module 5: Malware, Sniffing & Social Engineering
- **Malware Types:** Trojans (hidden inside legitimate software), Ransomware (encrypts files for extortion), Rootkits (hides deep in the OS kernel).
- **Sniffing:** Capturing network packets (using Wireshark/tcpdump). Man-in-the-Middle (MitM) attacks use ARP Spoofing to intercept traffic. Defended by enforcing HTTPS/TLS and using dynamic ARP inspection.
- **Social Engineering:** Manipulating human psychology (Phishing, Pretexting, Baiting). The only defense is comprehensive employee security awareness training.
