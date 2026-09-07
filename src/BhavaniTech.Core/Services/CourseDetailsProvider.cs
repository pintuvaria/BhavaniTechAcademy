using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public record CourseDetails(
        string CourseId,
        string CourseTitle,
        string Category,
        string Summary,
        string Prerequisites,
        string HardwareRequirements,
        string InstallationSteps,
        string EnvironmentVerification,
        string RecommendedTools,
        string CareerAndCertifications
    );

    public static partial class CourseDetailsProvider
    {
        private static readonly Dictionary<string, CourseDetails> Details = new(StringComparer.OrdinalIgnoreCase)
        {
            ["CS101"] = new CourseDetails(
                CourseId: "CS101",
                CourseTitle: "Computer Fundamentals & Architecture",
                Category: "Fundamentals",
                Summary: "Deep dive into binary representation, digital logic, CPU ALU execution cycles, virtual memory paging, and speculative execution.",
                Prerequisites: "• Basic arithmetic (powers of 2, addition, multiplication)\n• Familiarity with personal computers (files, folders, keyboard, mouse)\n• No prior coding experience required — this course starts from absolute ground zero!",
                HardwareRequirements: "• CPU: 1.0 GHz or faster\n• RAM: 512 MB minimum\n• Storage: 50 MB disk space\n• OS: Windows 7/8/10/11 or Linux (x64)",
                InstallationSteps: 
                    "1. OFFLINE BUILT-IN WORKBENCH (Recommended):\n" +
                    "   - Bhavani Technology includes built-in interactive binary logic simulators and CPU cycle viewers with zero host installation needed.\n\n" +
                    "2. OPTIONAL EXTERNAL TOOLCHAIN:\n" +
                    "   - Windows: Install MinGW-w64 compiler suite via winget:\n" +
                    "     winget install MSYS2.MSYS2\n" +
                    "     pacman -S mingw-w64-ucrt-x86_64-gcc gdb\n" +
                    "   - Linux (Debian/Ubuntu):\n" +
                    "     sudo apt update && sudo apt install build-essential gdb valgrind -y",
                EnvironmentVerification: 
                    "Run in Terminal:\n" +
                    "  gcc --version\n" +
                    "  gdb --version\n" +
                    "Expected: GCC and GDB banner output confirming x86_64 target compiler is operational.",
                RecommendedTools: "Bhavani Logic Gate Simulator, GCC/Clang, GDB debugger, HxD Hex Editor",
                CareerAndCertifications: "CompTIA IT Fundamentals (ITF+), CompTIA A+ Core 1, Systems Architect Associate"
            ),

            ["PROG101"] = new CourseDetails(
                CourseId: "PROG101",
                CourseTitle: "Polyglot Programming Academy",
                Category: "Programming",
                Summary: "Master C#, Python, JavaScript, and SQL. Learn variables, control flow, functions, memory stack vs heap, OOP principles, and algorithm efficiency.",
                Prerequisites: "• Basic logical reasoning and mathematical problem solving\n• Completed CS101 or familiar with binary and variables\n• Comfortable using a code editor or typing commands",
                HardwareRequirements: "• CPU: 1.2 GHz single-core or multi-core\n• RAM: 1 GB minimum (2 GB recommended)\n• Storage: 200 MB for local runtimes\n• OS: Windows or Linux",
                InstallationSteps: 
                    "1. OFFLINE BUILT-IN SANDBOX (Instant - Zero Setup):\n" +
                    "   - Click the 'Programming Code IDE' tab in Bhavani Academy to run C#, Python, JavaScript, and SQLite code in-memory.\n\n" +
                    "2. LOCAL PYTHON INSTALLATION:\n" +
                    "   - Windows: winget install Python.Python.3.12 (Check 'Add python.exe to PATH')\n" +
                    "   - Linux: sudo apt install python3 python3-pip python3-venv -y\n\n" +
                    "3. LOCAL .NET 9 C# SDK INSTALLATION:\n" +
                    "   - Windows: winget install Microsoft.DotNet.SDK.9\n" +
                    "   - Linux: sudo apt install dotnet-sdk-9.0 -y\n\n" +
                    "4. CODE EDITOR:\n" +
                    "   - Install VS Code: winget install Microsoft.VisualStudioCode",
                EnvironmentVerification: 
                    "Verify compilers:\n" +
                    "  python --version   -> Python 3.12+\n" +
                    "  dotnet --version   -> 9.0.xxx\n" +
                    "  node --version     -> Optional: v20+\n" +
                    "Run test script: python -c \"print('Polyglot Ready!')\"",
                RecommendedTools: "Bhavani In-Memory Code IDE, VS Code, Python IDLE, .NET CLI, SQLite Studio",
                CareerAndCertifications: "Microsoft Certified: C# Developer, PCEP (Certified Associate in Python), OpenEDG Python Institute"
            ),

            ["SW101"] = new CourseDetails(
                CourseId: "SW101",
                CourseTitle: "Software Engineering, Compilers & DevOps",
                Category: "Software Engineering",
                Summary: "Understand software architecture, version control with Git, lexers, AST compiler theory, OS CPU scheduling algorithms, and distributed systems.",
                Prerequisites: "• Completion of PROG101 or proficiency in at least one programming language\n• Basic understanding of functions, loops, and data structures\n• Familiarity with command line terminals",
                HardwareRequirements: "• CPU: 1.5 GHz multi-core\n• RAM: 2 GB RAM\n• Storage: 500 MB disk space\n• OS: Windows or Linux",
                InstallationSteps: 
                    "1. GIT VERSION CONTROL SETUP:\n" +
                    "   - Windows: winget install Git.Git\n" +
                    "   - Linux: sudo apt install git -y\n" +
                    "   - Configure Identity:\n" +
                    "     git config --global user.name \"Student Name\"\n" +
                    "     git config --global user.email \"student@bhavanitech.org\"\n\n" +
                    "2. COMPILER DEVELOPMENT TOOLS:\n" +
                    "   - Linux: sudo apt install llvm clang cmake make bison flex -y\n\n" +
                    "3. CONTAINER & RUNTIME SANDBOX:\n" +
                    "   - Use Bhavani Software Simulation tab for in-memory container cgroups v2 and AST visualizer.",
                EnvironmentVerification: 
                    "Verify Git and Clang:\n" +
                    "  git --version\n" +
                    "  clang --version\n" +
                    "Test git repo creation: git init test-repo",
                RecommendedTools: "Git CLI, GitHub Desktop, Clang / LLVM, CMake, Bhavani AST Simulator",
                CareerAndCertifications: "Git Certified Associate, Linux Foundation Certified Developer (LFCD), Certified Kubernetes Application Developer (CKAD)"
            ),

            ["HW101"] = new CourseDetails(
                CourseId: "HW101",
                CourseTitle: "Hardware Assembly & PC Builder",
                Category: "Hardware",
                Summary: "Hands-on component selection and diagnostics: CPUs, motherboards, RAM, GPUs, power supplies, thermal cooling, and POST beep troubleshooting.",
                Prerequisites: "• Elementary curiosity about physical computer components\n• Understanding of basic electrical safety (avoid working on plugged-in power supplies)\n• Anti-static handling fundamentals",
                HardwareRequirements: "• Any Windows computer running Bhavani Academy\n• Access to hardware components or the built-in 2D PC Builder",
                InstallationSteps: 
                    "1. OFFLINE 2D VIRTUAL PC BUILDER:\n" +
                    "   - Navigate to 'Hardware & PC Builder' in Bhavani Academy to simulate component compatibility and TDP power consumption.\n\n" +
                    "2. HARDWARE DIAGNOSTIC UTILITIES:\n" +
                    "   - CPU-Z: Hardware profiling tool for processor and motherboard specs.\n" +
                    "   - GPU-Z: Graphics processor diagnostics, PCIe lane usage, and VRAM bandwidth.\n" +
                    "   - HWiNFO64: Comprehensive sensor monitoring (voltage, temperatures, wattage).\n" +
                    "   - CrystalDiskInfo: S.M.A.R.T. health checking for NVMe/SATA storage.",
                EnvironmentVerification: 
                    "Launch Bhavani Virtual PC Builder -> Select CPU (LGA1700) + Motherboard (AM4) -> Verify that the diagnostic engine flags the socket mismatch error.",
                RecommendedTools: "Bhavani 2D PC Builder, CPU-Z, HWiNFO, MemTest86, Anti-static wrist strap, #2 Phillips screwdriver",
                CareerAndCertifications: "CompTIA A+ (Core 1: 220-1101), BICSI Installer, PC Repair & Bench Technician"
            ),

            ["HW201"] = new CourseDetails(
                CourseId: "HW201",
                CourseTitle: "Advanced Microarchitecture & Buses",
                Category: "Microarchitecture",
                Summary: "Master microarchitectural timing: DDR4/DDR5 CAS latency calculations, PCIe Gen 1-5 throughput, cache hierarchies (L1/L2/L3), and 5-stage RISC instruction pipelines.",
                Prerequisites: "• Completion of CS101 and HW101\n• Familiarity with clock frequencies (GHz, MHz) and nanosecond calculations\n• Basic understanding of assembly instructions (ADD, SUB, LW, SW)",
                HardwareRequirements: "• CPU: 1.2 GHz x86_64 processor\n• RAM: 1 GB RAM\n• OS: Windows or Linux",
                InstallationSteps: 
                    "1. OFFLINE SIMULATION SUITE:\n" +
                    "   - Bhavani Academy includes full DDR latency calculators and 5-stage RISC instruction pipeline hazard simulators.\n\n" +
                    "2. LINUX HARDWARE PERFORMANCE PROFILING:\n" +
                    "   - Install perf and msr-tools:\n" +
                    "     sudo apt install linux-tools-common linux-tools-generic msr-tools -y\n" +
                    "   - Run CPU cache benchmark:\n" +
                    "     perf stat -e L1-dcache-loads,L1-dcache-load-misses ls",
                EnvironmentVerification: 
                    "In Bhavani Academy -> Navigate to 'Hardware & PC Builder' -> '5-Stage RISC Pipeline Simulator' -> Click 'Clock Cycle Simulation' -> Confirm that RAW hazards and forwarding bypass paths are mapped.",
                RecommendedTools: "Bhavani RISC Pipeline Simulator, Linux perf, Valgrind Cachegrind, CPUID",
                CareerAndCertifications: "VLSI Architecture Fundamentals, IEEE Microprocessor Architecture Specialist"
            ),

            ["NET101"] = new CourseDetails(
                CourseId: "NET101",
                CourseTitle: "Networking & CCNA/CCNP Essentials",
                Category: "Networking",
                Summary: "Understand the OSI 7-layer model, TCP/IP protocol suite, IPv4/IPv6 CIDR subnetting, packet switching, routing tables, and DNS hierarchy.",
                Prerequisites: "• Basic computer literacy and command-line familiarity\n• Completion of CS101 recommended for binary understanding\n• Ability to perform decimal-to-binary calculations (powers of 2)",
                HardwareRequirements: "• CPU: 1.2 GHz\n• RAM: 1 GB minimum\n• Network Interface Card (NIC) with Ethernet or WiFi",
                InstallationSteps: 
                    "1. OFFLINE PACKET & SUBNETTING WORKBENCH:\n" +
                    "   - Use Bhavani Networking Academy for instant visual multi-hop packet tracing, CIDR subnet math, and DNS resolution simulation.\n\n" +
                    "2. WIRESHARK PACKET ANALYZER:\n" +
                    "   - Windows: winget install WiresharkFoundation.Wireshark\n" +
                    "   - Linux: sudo apt install wireshark tshark -y\n\n" +
                    "3. NETWORK COMMANDS TERMINAL SETUP:\n" +
                    "   - Windows: ping, tracert, ipconfig, nslookup, netstat, arp -a\n" +
                    "   - Linux: ip addr, ip route, traceroute, dig, ss -tulpn",
                EnvironmentVerification: 
                    "Test terminal commands:\n" +
                    "  ping 127.0.0.1\n" +
                    "  nslookup learn.bhavanitech.org\n" +
                    "Verify that local loopback responds in < 1ms.",
                RecommendedTools: "Bhavani Network Topology Packet Tracer, Wireshark, Cisco Packet Tracer, Nmap, PuTTY",
                CareerAndCertifications: "Cisco CCNA (200-301), CompTIA Network+ (N10-008), Cisco CCNP Enterprise"
            ),

            ["SEC101"] = new CourseDetails(
                CourseId: "SEC101",
                CourseTitle: "Cybersecurity & Ethical Hacking Mastery",
                Category: "Cybersecurity",
                Summary: "Learn penetration testing, reconnaissance, Nmap port scanning, SQL injection, XSS defense, hash cracking, ROP chain exploits, and RSA/AES cryptography.",
                Prerequisites: "• Completion of NET101 (IP, ports, TCP handshake) and LNX101 (Linux shell)\n• Understanding of web protocols (HTTP GET/POST, cookies)\n• Strong ethical commitment: only test systems you own or have explicit permission to test!",
                HardwareRequirements: "• CPU: 1.5 GHz or faster\n• RAM: 2 GB minimum\n• Storage: 2 GB disk space",
                InstallationSteps: 
                    "1. OFFLINE SAFE CYBER LABS (100% Isolated & Safe):\n" +
                    "   - Bhavani Academy contains an offline CTF Arena, Cryptographic Cipher Lab (RSA/Diffie-Hellman/AES-GCM), and ROP Chain simulator.\n\n" +
                    "2. OPTIONAL KALI LINUX / WSL ENVIRONMENT:\n" +
                    "   - Install Kali on Windows WSL 2:\n" +
                    "     wsl --install -d kali-linux\n" +
                    "   - Update and install essential security tools:\n" +
                    "     sudo apt update && sudo apt install nmap nikto hydra john sqlmap wireshark -y",
                EnvironmentVerification: 
                    "Run in Kali/Linux Terminal:\n" +
                    "  nmap -v\n" +
                    "  john --version\n" +
                    "Test local port scan: nmap -sT -p 80,443 127.0.0.1",
                RecommendedTools: "Bhavani CTF Arena & Cryptography Lab, Nmap, Burp Suite Community, Wireshark, Hashcat, Ghidra",
                CareerAndCertifications: "CompTIA Security+ (SY0-701), CEH (Certified Ethical Hacker), OSCP (Offensive Security Certified Professional), eJPT"
            ),

            ["LNX101"] = new CourseDetails(
                CourseId: "LNX101",
                CourseTitle: "Linux Systems & Kernel Administration",
                Category: "Linux",
                Summary: "Master the Linux command line, Filesystem Hierarchy Standard (FHS), octal permissions (chmod 755/SUID), process management, systemd, and container cgroups.",
                Prerequisites: "• Basic computer familiarity\n• Willingness to navigate via keyboard and text commands\n• Understanding of files and directory paths",
                HardwareRequirements: "• CPU: 1.0 GHz or faster\n• RAM: 512 MB minimum\n• Storage: 1 GB disk space (or 0 GB when using Bhavani in-memory terminal)",
                InstallationSteps: 
                    "1. IN-MEMORY VIRTUAL TERMINAL (Zero Installation Required):\n" +
                    "   - Click 'Linux Terminal Lab' in Bhavani Academy to access an interactive in-memory Unix filesystem with root privilege escalation.\n\n" +
                    "2. WSL 2 INSTALLATION ON WINDOWS:\n" +
                    "   - Open PowerShell as Administrator and run:\n" +
                    "     wsl --install -d Ubuntu\n" +
                    "   - Restart your machine if prompted, then set up your username and password.\n\n" +
                    "3. STANDALONE LINUX DUAL-BOOT / VIRTUALBOX:\n" +
                    "   - Download Ubuntu LTS or Debian Minimal ISO and run in Oracle VirtualBox.",
                EnvironmentVerification: 
                    "Inside Linux terminal:\n" +
                    "  uname -a\n" +
                    "  whoami\n" +
                    "  ls -la /\n" +
                    "Confirm system kernel release and user id.",
                RecommendedTools: "Bhavani In-Memory Shell, Ubuntu LTS, Bash, Nano / Vim, htop, systemctl",
                CareerAndCertifications: "Red Hat Certified System Administrator (RHCSA), LPIC-1 (Linux Professional Institute), LFCS"
            ),

            ["ELE101"] = new CourseDetails(
                CourseId: "ELE101",
                CourseTitle: "Electronics, Circuits & Robotics",
                Category: "Electronics",
                Summary: "Learn electrical circuits, Ohm's Law (V=IR), LED current limiting resistors, digital logic gates, microcontrollers (Arduino/ESP32), PWM, and servo motor control.",
                Prerequisites: "• Elementary physics: concepts of voltage (V), current (I), and resistance (R)\n• Safe handling of low-voltage DC electricity (under 12V)\n• High school algebra for simple circuit equations",
                HardwareRequirements: "• Access to Bhavani Breadboard & Microcontroller Studio\n• Optional: Arduino Uno Starter Kit with breadboard, LEDs, and 220Ω/330Ω resistors",
                InstallationSteps: 
                    "1. OFFLINE BREADBOARD & MICROCONTROLLER STUDIO:\n" +
                    "   - Open 'Electronics & Circuits' in Bhavani Academy for real-time circuit loop solving and PWM oscilloscope visualization.\n\n" +
                    "2. ARDUINO IDE INSTALLATION:\n" +
                    "   - Windows: winget install Arduino.ArduinoIDE\n" +
                    "   - Linux: Download the Arduino IDE AppImage from the official Arduino site:\n" +
                    "     chmod +x arduino-ide_*_Linux_64bit.AppImage && ./arduino-ide_*_Linux_64bit.AppImage\n\n" +
                    "3. HARDWARE CONNECTION:\n" +
                    "   - Connect Arduino Uno via USB cable; select Board: 'Arduino Uno' and appropriate COM port.",
                EnvironmentVerification: 
                    "In Arduino IDE -> File -> Examples -> 01.Basics -> Blink -> Upload -> Verify that onboard LED (Pin 13) blinks at 1 Hz.",
                RecommendedTools: "Bhavani Breadboard Simulator, Arduino IDE, Tinkercad Circuits, Digital Multimeter, 10-bit ADC sensors",
                CareerAndCertifications: "IPC Certified Electronics Technician, IEEE Robotics & Automation Specialist, Embedded Firmware Developer"
            ),

            ["AI101"] = new CourseDetails(
                CourseId: "AI101",
                CourseTitle: "Artificial Intelligence & Prompt Engineering",
                Category: "Artificial Intelligence",
                Summary: "Explore machine learning, perceptrons, backpropagation, activation functions (Sigmoid, Tanh, ReLU), computer vision convolutions, Transformers, and vector RAG.",
                Prerequisites: "• Completion of PROG101 (Python fundamentals)\n• High-school algebra (linear equations, matrix multiplication)\n• Understanding of probabilities (0.0 to 1.0)",
                HardwareRequirements: "• CPU: 1.5 GHz or faster\n• RAM: 2 GB minimum\n• GPU: Optional (Bhavani AI simulators are CPU-optimized)",
                InstallationSteps: 
                    "1. OFFLINE AI & COMPUTER VISION LABS (Zero Installation):\n" +
                    "   - Access Bhavani AI Academy for interactive 2D Neural Network decision boundary mapping, 3x3 convolution filtering, and offline vector RAG.\n\n" +
                    "2. PYTHON AI & MACHINE LEARNING ENVIRONMENT:\n" +
                    "   - Create a dedicated virtual environment:\n" +
                    "     python -m venv ai_env\n" +
                    "     Windows: ai_env\\Scripts\\activate\n" +
                    "     Linux: source ai_env/bin/activate\n" +
                    "   - Install standard libraries:\n" +
                    "     pip install numpy matplotlib scikit-learn pandas jupyterlab",
                EnvironmentVerification: 
                    "Run in Python:\n" +
                    "  python -c \"import numpy as np; print('NumPy Vector Ready:', np.array([1, 2, 3]))\"\n" +
                    "Expected: Confirmation that tensor array allocations execute cleanly.",
                RecommendedTools: "Bhavani Neural Net & Vector RAG Engine, JupyterLab, Python NumPy, Scikit-Learn, PyTorch",
                CareerAndCertifications: "Google Professional Machine Learning Engineer, AWS Certified Machine Learning, DeepLearning.AI Specialization"
            ),

            ["IT101"] = new CourseDetails(
                CourseId: "IT101",
                CourseTitle: "IT Troubleshooting & Systems Engineering",
                Category: "Troubleshooting",
                Summary: "Master systematic diagnostics using the CompTIA 6-step troubleshooting methodology: diagnosing boot failures, BSOD dumps, DNS drops, slow disks, and event logs.",
                Prerequisites: "• Broad foundational understanding across hardware (HW101), operating systems (LNX101/Windows), and networking (NET101)\n• Analytical problem-solving mindset",
                HardwareRequirements: "• CPU: 1.2 GHz\n• RAM: 1 GB minimum\n• Administrator / root privileges to run system diagnostics",
                InstallationSteps: 
                    "1. OFFLINE 500+ SCENARIOS TROUBLESHOOTING LAB:\n" +
                    "   - Access 'IT Troubleshooting Lab' in Bhavani Academy to practice with 500+ categorized diagnostic scenarios.\n\n" +
                    "2. WINDOWS SYSINTERNALS SUITE:\n" +
                    "   - Install Microsoft Sysinternals:\n" +
                    "     winget install Microsoft.SysinternalsSuite\n" +
                    "   - Key tools: Process Explorer (procexp.exe), Autoruns (autoruns.exe), TCPView (tcpview.exe).\n\n" +
                    "3. LINUX DIAGNOSTIC UTILITIES:\n" +
                    "   - sudo apt install htop iotop net-tools dmesg strace sysstat -y",
                EnvironmentVerification: 
                    "Open PowerShell/Command Prompt:\n" +
                    "  systeminfo\n" +
                    "  chkdsk C:\n" +
                    "Verify hardware inventory and filesystem integrity reports.",
                RecommendedTools: "Bhavani 500+ Scenarios Lab, Sysinternals Suite, Event Viewer, Wireshark, MemTest86",
                CareerAndCertifications: "CompTIA A+ (Core 2: 220-1102), Microsoft 365 Certified: Endpoint Administrator, SRE Foundation"
            )
        };

        public static CourseDetails GetDetails(string courseId)
        {
            if (Details.TryGetValue(courseId, out var det))
            {
                return det;
            }

            if (GetAdvancedDetails().TryGetValue(courseId, out var adv))
            {
                return adv;
            }

            if (GetGreyHatDetails().TryGetValue(courseId, out var gh))
            {
                return gh;
            }

            return new CourseDetails(
                CourseId: courseId,
                CourseTitle: "Technology Course",
                Category: "Technology",
                Summary: "Comprehensive technology course in the Bhavani Academy curriculum.",
                Prerequisites: "• Basic computer literacy and logical problem-solving ability.",
                HardwareRequirements: "• 1.2 GHz CPU, 1 GB RAM, 100% offline.",
                InstallationSteps: "• Use the built-in Bhavani Technology learning sandboxes.",
                EnvironmentVerification: "• Verify that the application is running normally.",
                RecommendedTools: "Bhavani Academy Core Tools",
                CareerAndCertifications: "Bhavani Technology Academy Mastery Certificate"
            );
        }
    }
}
