# Bhavani Technology - Technology Learning & Coding Academy

![Dashboard View](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/dashboard.jpg)
*Main dashboard with Gamification, Streaks, and Course Pathways.*

![Bhavani Autonomous Neural Local AI](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/local_ai.jpg)
*Bhavani Autonomous Neural Local AI (100% Offline, Air-Gapped, CoT Reasoning Trace, Syntax-Highlighted Code Generation).*

![Quantum & Transformer Simulation Labs](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/quantum_ai.jpg)
*Quantum Gates 2-Qubit Entanglement & Transformer Multi-Head Attention Visualization.*

![Linux Containers, Docker & WebAssembly Studio](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/docker_wasm.jpg)
*Linux Containers (Docker CLI, cgroups v2 & OverlayFS) and WebAssembly (WASM) Stack Machine Studio.*

![Zero-Knowledge Proofs & Multi-Theme Customizer](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/zk_snarks.jpg)
*Zero-Knowledge Proofs (Ali Baba Cave, Schnorr Protocol, R1CS Circuit) and High-Contrast Themes (OLED Deep Matrix & Retro Amber CRT).*

![Sandbox View](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/sandbox.jpg)
*Interactive Grey Hat Cybersecurity and Polyglot Coding Sandbox.*

![CCNA Networking Labs](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/ccna.jpg)
*Interactive CCNA Networking Labs and Topology Simulation.*

![BCA & MCA University Portal](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/university.jpg)
*BCA & MCA University Degree Portals with specialized tracks.*

![Multi-Student Profile Selection](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/multiuser.jpg)
*Multi-student login: Select existing profile or register new student with Full Name and Date of Birth.*

![Certificate of Excellence](https://raw.githubusercontent.com/pintuvaria/BhavaniTechAcademy/master/assets/certificate.jpg)
*Automatically issued Certificate of Excellence after every exam pass, personalized with student name and DOB.*

**Founder & Chief Architect:** Dharmesh Varia  
**Application Type:** Standalone Windows Application (.NET 9 WPF, 100% Offline, Self-Contained)  
**Target Hardware:** 1.2 GHz Single-Core CPU | 2 GB RAM | HDD | Integrated Graphics  
**Runtime Footprint:** ~45 MB Working Set RAM | < 1% CPU Idle | Zero External Host Dependencies

---

## 🌟 Overview

**Bhavani Technology Academy** is a comprehensive, self-contained educational workstation designed to teach computer science and technology from **elementary basics to master/cutting-edge levels**. It requires **zero host prerequisites** (no pre-installed .NET runtime, Python, or Node.js required) and operates **100% offline** with zero external network calls.

---

## 🚀 Key Highlights & New Enhancements

### 🧠 1. Autonomous Self-Learning Local AI (100% Offline & Air-Gapped)
- **Zero Internet Requirement**: Completely self-contained neural-symbolic inference running in-memory with local SQLite persistence.
- **Autonomous Knowledge Expansion**: The AI actively learns and assimilates new concepts in-memory and stores them in encrypted local SQLite (`LocalAiLearnedKnowledge`), expanding its knowledge without external internet or model re-downloading.
- **🎓 Socratic Tutoring Mode**: Guided inquiry mode that asks probing questions and provides progressive conceptual hints instead of spoiling direct answers, cultivating critical problem-solving skills.
- **🔍 Offline Static Code Reviewer & Bug Hunter**: Audits student code snippets for SQL Injection, memory/resource leaks (unclosed streams/connections), unsafe buffer overflows (`strcpy`/`gets`), empty catch blocks, and weak cryptography. Outputs a security score (A+ to F), issue descriptions, and an automated hardened patch.
- **Interactive "Teach AI" Capability**: Students can teach the AI new lessons, algorithms, and concepts via `Teach AI: Topic | Domain | Details | Code` or direct conversation, fostering mastery through the **Protégé Effect**.
- **Pedagogical Student Guidance Engine**: Dynamically analyzes the student's questions and learning milestones to formulate an adaptive study roadmap with personalized next-step suggestions.
- **Chain-of-Thought (CoT) Reasoning Trace**: Live inspection showing step-by-step reasoning (Tokenization $\rightarrow$ Core & Learned Bank Retrieval $\rightarrow$ Vector RAG Similarity $\rightarrow$ Synthesized Explanation $\rightarrow$ Code Generation $\rightarrow$ Adaptive Guidance).
- **Curriculum Citations**: Displays exact domain and curriculum module links for every verified answer.

### ⚡ 2. Global Command Palette & Modern UX
- **🔍 Global Quick Jump (Ctrl+K)**: Instant fuzzy-search command palette accessible from anywhere in the app to jump between all 11 stations, workbenches, tools, and labs in milliseconds.
- **📜 Verifiable Printable Certificate Generator**: Generates beautiful, official offline certificates (HTML/PDF) featuring gold borders, Bhavani Technology Academy watermark, student name, track mastery, and cryptographic SHA-256 validation hash (`BTA-...`).
- **🎴 SuperMemo SM-2 Spaced Repetition Engine**: Calculates exact review intervals (`CalculateSm2Interval`) across all 11 disciplines based on ease factor and retention quality.
- **🎨 Multi-Theme Customizer**: Switch seamlessly between `🌙 Cyber Dark`, `☀️ Daylight Light`, `📟 Terminal Green`, `⚡ Cyberpunk Neon`, `🌌 OLED Deep Matrix`, and `📻 Retro Amber CRT`.

### 🐳 3. Linux Containers, Docker Engine & Systems Isolation
- **Linux Namespaces**: Deep-dive interactive inspection of PID, NET, MNT, UTS, IPC, and USER namespaces. Demonstrates container PID 1 translation to host PID space.
- **Control Groups (cgroups v2)**: Simulates CPU quota throttling (`cpu.max`) and memory limits (`memory.max`), including Linux kernel Out-Of-Memory (OOM) killer execution (Exit Code 137 / SIGKILL).
- **OverlayFS Layered Storage**: Visualizes `lowerdir` (read-only base image layers), `upperdir` (read-write container diff), and `merged` unified mount with Copy-on-Write (CoW) semantics.
- **Docker CLI Console**: Interactive execution for `docker ps`, `docker run`, `docker stop`, `docker inspect`, `docker top`, and container stress testing.

### ⚡ 4. WebAssembly (WASM) Text & Binary Stack Machine
- **WAT to WASM Bytecode Disassembler**: Analyzes WebAssembly Text (WAT) S-expressions and produces WASM binary headers (Magic `\0asm`, Version 1, and sections: Type, Function, Memory, Export, Code).
- **Virtual Stack Machine Execution**: Step-by-step operand stack visualization for `i32.const`, `i32.add`, `i32.sub`, `i32.mul`, `i32.div_s`, `i32.lt_s`, and local variables.
- **64 KB Linear Memory Inspector**: Full page memory allocator with hex dump inspection, string encoding, and byte-level memory operations (`i32.store8`, `i32.load8_u`).

### 🔐 5. Zero-Knowledge Proofs (ZK-SNARKs) Laboratory
- **Ali Baba Cave Interactive Protocol**: Interactive round-by-round simulation demonstrating Completeness, Soundness, and Zero-Knowledge with exponential error decay $(1/2)^k$ over $k$ rounds.
- **Schnorr Identification Protocol**: Discrete logarithm Zero-Knowledge identification ($y = g^x \pmod p$) with commitment $t = g^r$, challenge $c$, and response $s = (r + c \cdot x) \pmod{p-1}$, mathematically proving $g^s \equiv t \cdot y^c \pmod p$ without revealing secret witness $x$.
- **R1CS Arithmetic Circuit ($x^3 + x + 5 = 35$)**: Evaluates Rank-1 Constraint System wire allocations ($w_1, w_2, w_3, out$) and verifies quadratic arithmetic satisfaction without revealing the witness.

### ⚛️ 6. Advanced Quantum & AI Simulation Engines
- **2-Qubit Entangled Quantum Lab**: Pauli-X/Y/Z, Phase, T, Hadamard, and CNOT gates. Generates the maximally entangled Bell State $|\Phi^+\rangle = \frac{1}{\sqrt{2}}(|00\rangle + |11\rangle)$ with state amplitude vector calculations.
- **Transformer Multi-Head Attention (MHA)**: Simulates query/key/value projections ($Q, K, V$), $h$ parallel attention heads, scaled dot-product attention softmax distributions, context matrix concatenation, and LayerNorm residual connections.

### 🔬 4. Scientific Computing & Hardware Physics
- **Upgraded Scientific Calculator**: Full Dijkstra Shunting-Yard algorithm supporting parenthesis parsing, operator precedence, trigonometric functions (`sin`, `cos`, `tan`), logarithmic functions (`log`, `ln`), and powers (`^`).
- **Non-Linear Breadboard Physics**: Shockley Diode equation solver ($I = I_s(e^{V_D/nV_T} - 1)$), dynamic diode resistance curves, and parallel circuit equivalent resistance solvers alongside RC transient charging analysis.
- **Operating Systems Internals**: FIFO and Belady's Optimal (MIN) page replacement algorithms, and Preemptive Shortest Remaining Time First (SRTF) CPU scheduling.

### 🛡️ 5. Grey Hat Offensive Security & Forensics
- **SQLi AST Classification Engine**: Automatically analyzes payloads for Tautology, UNION-based, and Blind Time-based patterns, detecting inline comment (`/**/`) WAF evasion techniques.
- **Binary PCAP Frame Forensics**: Decodes Ethernet II, IPv4 headers, checksums, TCP flags (SYN/ACK/FIN), and extracts plaintext HTTP/FTP credentials from base64 authentication headers.
- **Compiler Stack Canary & ASLR Sandbox**: Visualizes buffer overflow detection, canary tripping (`0xDEADBEEF`, `__stack_chk_fail`), Address Space Layout Randomization (ASLR), and DEP/NX execution protection.

### 🎓 6. Academic & University Curriculums
- **Relational Database Normalization Engine**: Validates table schemas through 1NF, 2NF, 3NF, and BCNF, decomposing multi-valued attributes and resolving partial functional dependencies.
- **C Pointer & Memory Arithmetic Simulator**: Visualizes pointer variable addresses, dereferencing, and pointer increment steps in hexadecimal format.
- **CCNA Certification Exam Bank**: Expanded to a comprehensive 20-question certification bank with detailed explanations.
- **CIS Benchmark & Windows Registry Auditor**: Programmatic registry security inspection and Group Policy baseline auditing.

---

## 📂 Workspace Structure

```
D:\AntiGravity\Kids Game\
│
├── BhavaniTech.UI.exe              # Ready-to-run standalone portable executable (63 MB)
├── BhavaniTechAcademy.sln          # Visual Studio / .NET 9 Solution File
├── README.md                       # Comprehensive project documentation
├── WALKTHROUGH.md                  # Detailed architectural walkthrough & test results
├── IMPLEMENTATION_PLAN.md          # Multi-phase engineering roadmap
│
├── build/
│   └── portable/
│       ├── BhavaniTech.UI.exe      # Single-file self-contained distribution binary
│       ├── BhavaniTech.UI.pdb      # Debug symbols
│       └── BhavaniTech.Core.pdb    # Core library debug symbols
│
├── src/
│   ├── BhavaniTech.Core/           # Core logic, SQLite database, algorithms, and simulation services
│   │   ├── Database/               # SQLite curriculum database context & seeders
│   │   ├── Hardware/               # System diagnostics & hardware capability profiler
│   │   ├── Models/                 # Strongly-typed data models (Courses, Lessons, Quizzes, Users)
│   │   └── Services/               # Interactive laboratory service engines:
│   │       ├── CourseDetailsProvider.cs            # Academic blueprints, prerequisites, CLI setup & verify
│   │       ├── LanguageRegistryService.cs          # 10 programming languages with logos, brands, paradigms & templates
│   │       ├── CalculatorEngine.cs                 # Arithmetic, scientific math, memory store & history tape
│   │       ├── CryptographyLabService.cs           # RSA, Diffie-Hellman, AES-256-GCM AEAD
│   │       ├── VirtualTerminalService.cs           # In-memory Unix filesystem & CTF shell
│   │       ├── PolyglotExecutionService.cs         # In-memory SQLite runner & JS evaluator
│   │       ├── CodeExecutionService.cs             # Multi-language simulation, syntax & register emulator
│   │       ├── AlgorithmVisualizationService.cs    # QuickSort, BubbleSort, BST, Graph traces
│   │       ├── BreadboardSimulationService.cs      # 2D Breadboard circuit solver & RC transient
│   │       ├── MicrocontrollerStudioService.cs     # Arduino/ESP32 PWM oscilloscope & servo
│   │       ├── CpuPipelineService.cs               # 5-stage RISC hazard simulator & ALU forwarding
│   │       ├── NeuralNetPlaygroundService.cs       # 2D decision boundary & multi-layer perceptron
│   │       ├── ComputerVisionLabService.cs         # 3x3 convolution filtering & Max-Pooling
│   │       ├── VectorRagService.cs                 # Offline semantic vector search & RAG
│   │       ├── NetworkTopologyService.cs           # Multi-hop packet tracer & firewall inspection
│   │       ├── DnsResolutionService.cs             # Hierarchical recursive DNS tracer
│   │       ├── K8sSchedulerService.cs              # Kubernetes pod scheduling predicates & scoring
│   │       ├── CertificationExamService.cs         # Multi-disciplinary exam & verifiable diploma
│   │       ├── ProgressPortabilityService.cs       # Student progress JSON export/import
│   │       ├── LocalAiEngine.cs                    # 100% offline domain-specialized AI mentor
│   │       ├── CybersecurityLabService.cs          # CTF arena, packet sniffer, ROP exploit
│   │       └── SoftwareSimulationService.cs        # Linux container namespaces & cgroups v2
│   │
│   ├── BhavaniTech.UI/             # WPF modern desktop user interface
│   │   ├── Assets/                 # Branding icons and logo
│   │   ├── MainWindow.xaml         # High-contrast layouts, multi-language IDE, Web Studio & Calculator
│   │   └── MainWindow.xaml.cs      # Event dispatching, live web renderer & calculator engine
│   │
│   └── BhavaniTech.Benchmark/      # Performance benchmark and memory profiler tool
│
└── tests/
    └── VerifyApp/                  # Automated verification test suite (11 comprehensive test suites)
        ├── Program.cs              # Automated test runner validating all subsystems
        └── VerifyApp.csproj        # Test runner project definition
```

---

## 🎓 11 Curriculum Disciplines (Basics to Cutting-Edge)

Each course contains **6 mastery tiers** totaling **66 interactive lessons** and **66 validated quizzes**:

1. **`CS101` Computer Fundamentals**: Binary math $\rightarrow$ Virtual memory $\rightarrow$ Speculative execution & Quantum superposition.
2. **`PROG101` Polyglot Programming**: Variables & loops $\rightarrow$ Memory layout $\rightarrow$ SIMD AVX-512 & zero-allocation spans.
3. **`SW101` Software Engineering**: Version control $\rightarrow$ Lexers & ASTs $\rightarrow$ Compiler SSA & Raft distributed consensus.
4. **`HW101` Hardware & PC Assembly**: Component anatomy $\rightarrow$ Sockets/VRMs $\rightarrow$ 2nm GAAFET lithography.
5. **`HW201` Advanced Microarchitecture**: Clock cycles & SMT $\rightarrow$ Cache coherence $\rightarrow$ GPU Tensor Cores & HBM3e.
6. **`NET101` Networking & CCNA**: OSI model $\rightarrow$ Subnetting & VLANs $\rightarrow$ eBPF, XDP & HTTP/3 QUIC.
7. **`SEC101` Cybersecurity & Ethical Hacking**: Recon & Nmap $\rightarrow$ Web exploits $\rightarrow$ ROP chains, rootkits & Kyber PQC.
8. **`LNX101` Linux Administration**: Shell & FHS $\rightarrow$ Process control $\rightarrow$ Kernel LKMs & container namespaces.
9. **`ELE101` Electronics & Circuits**: Ohm's Law $\rightarrow$ Logic gates $\rightarrow$ Verilog HDL, PID controllers & FreeRTOS.
10. **`AI101` Artificial Intelligence**: Supervised ML $\rightarrow$ Perceptrons & CNNs $\rightarrow$ Transformer attention & INT4 quantization.
11. **`IT101` IT Troubleshooting**: CompTIA methodology $\rightarrow$ BSOD dumps $\rightarrow$ Chaos engineering & SRE.

---

## 🔬 Interactive Virtual Laboratories

- **🔐 Cryptography Lab**: Asymmetric RSA derivation, Diffie-Hellman shared secret math, AES-256-GCM authenticated encryption with tamper detection.
- **🐧 Linux Terminal Lab**: Full Unix filesystem in-memory with permission enforcement (`chmod`), text manipulation (`cat`, `grep`, `strings`), and `su root` authentication (`bhavani_root`).
- **🗄️ In-Memory SQL Console**: SQLite script runner with tabular formatted output.
- **🔌 2D Breadboard Circuit Lab**: Interactive Ohm's and Kirchhoff's loop calculations, LED overload prevention, and capacitor RC transient response.
- **🎛️ Microcontroller Studio**: Arduino pin simulation, 490 Hz PWM oscilloscope waveform generator, and servo motor rotation.
- **⚙️ 5-Stage RISC Pipeline Simulator**: Hazard detection, ALU forwarding bypass paths, load-use bubbles, and branch flushes.
- **🎯 Neural Network Playground**: Multi-layer perceptron with choice of activations (Sigmoid, Tanh, ReLU) and 2D decision boundary matrix visualization.
- **🖼️ Computer Vision Lab**: 3x3 spatial convolution kernels (Sobel, Sharpen, Ridge, Blur) and 2x2 Max-Pooling feature reduction.
- **🔍 Offline Vector Search & RAG**: 16-dimensional dense embedding vectors, cosine similarity ranking, and grounded offline synthesis.
- **🗺️ Network Topology Packet Tracer**: Multi-hop routing from Client to Cloud Server through L2 Switch, Router, and Firewall.
- **🌍 DNS Resolution Tracer**: Root $\rightarrow$ TLD $\rightarrow$ Authoritative hierarchical recursive lookups.
- **☸️ Kubernetes Scheduler Sandbox**: Pod resource request matching, node affinity, taints, tolerations, and scoring.
- **🔍 Course Search & Filter Engine**: Real-time course filtering by name, code, category, or description with instant match auto-selection and clear button (`✕`).
- **🛠️ Course Blueprint & Environment Setup**: Dedicated tab with prerequisites, hardware requirements, step-by-step CLI toolchain installation commands (Windows & Linux), and verification commands.
- **💻 Polyglot Code IDE (10 Languages with Brand Logos)**: Visual language logos, brand chips, version indicators, and educational starter templates for:
  - 🐍 **Python 3.12** (`#3776AB`)
  - 🔷 **C# 13 / .NET 9** (`#512BD4`)
  - ⚡ **JavaScript ES2024** (`#F7DF1E`)
  - 🌐 **HTML5 & CSS3** (`#E34F26`)
  - 🗄️ **SQL SQLite 3** (`#00758F`)
  - ⚙️ **ISO C++20** (`#00599C`)
  - 🦀 **Rust 2024 Edition** (`#DEA584`)
  - ☕ **Java 21 LTS** (`#ED8B00`)
  - 🐹 **Go 1.22** (`#00ADD8`)
  - 📟 **x86_64 Assembly** (`#4D5BCE`)
- **🌐 Website Creator & Live Output Sandbox**: Students write custom HTML5, CSS3, and JavaScript with instant offline DOM rendering via embedded browser engine, interactive click elements, CSS animations, and single-click export to standalone HTML files.
- **🧮 Interactive Calculator Project Studio**: Fully functional physical calculator UI with LED digital screen, memory functions (`MC`, `MR`, `M+`, `M-`), scientific operations (`x²`, `√x`, `1/x`, `%`, `±`), calculation history tape, and editable project source code in JavaScript, C# (.NET), and Python (Tkinter).
- **📜 Certification Exam & Verifiable Diploma**: Multi-disciplinary assessment, instant grading, and offline verifiable Certificate of Technology Mastery with SHA-256 tokens.
- **💾 Progress JSON Portability**: Single-click export/import of student profiles between offline machines.

---

## 🏛️ Logical Student Architecture (4 Streamlined Learning Stations)

The application is architecturally designed around a continuous, logical student learning journey:
**Launch Application $\rightarrow$ Greeted by Next Milestone $\rightarrow$ Study & Hands-on Lab $\rightarrow$ Save Knowledge $\rightarrow$ Reopen with Compounding Progress**:

1. **🏠 Learning HQ & Syllabus**:
   - **Hero Continuity Card**: Shows current learning streak, total academy progress bar (15% to 100%), and a prominent **`▶ CONTINUE LEARNING`** button that jumps directly into the student's next uncompleted lesson.
   - **Compounding Session Metrics**: Tracks completed lessons, project studios built, and diploma status.
   - **6 Structured Roadmap Tracks**: Quick-launch into Computer Fundamentals (`CS101`), Polyglot Coding (`PROG101`), Linux & Networking (`LNX101`), Cybersecurity (`SEC101`), AI & Robotics (`AI101`), and Technology Mastery (`MAST101`).

2. **📚 Active Study Hall**:
   - **4-Stage Mastery Workflow**:
     - *Stage 1: Concept & Theory*: Deep technical breakdown from beginner basics to cutting-edge architectures.
     - *Stage 2: Blueprint & Toolchain Setup*: Academic prerequisites, exact CLI commands for Windows & Linux, compiler flags, and career certifications.
     - *Stage 3: Interactive Knowledge Check*: Instant multiple-choice quiz with immediate pedagogical feedback (+30 XP).
     - *Stage 4: Lab & Milestone Advance*: Single-click jump to the relevant Creative Workbench, and **`✅ Mark Completed & Advance (+50 XP)`** which writes completion to SQLite, updates Hero status, and seamlessly loads the next milestone.

3. **🔬 Creative Workbenches (11 Project Studios)**:
   - All interactive builder tools and sandbox simulators neatly organized under 9 unified tabs:
     - 💻 **Polyglot Code IDE**: 10 languages with brand logos, color themes, and starter templates.
     - 🌐 **Website Creator & Live Browser**: Real-time DOM renderer with HTML/CSS/JS export.
     - 🧮 **Interactive Calculator Studio**: Tactile LED physical device and educational engine source code.
     - ⚙️ **Software Engineering & OS**: CPU scheduling, lexer tokenizer, Git commit graphs, and containers.
     - 🖥️ **Virtual PC Hardware Lab**: 2D custom PC builder and 5-stage RISC instruction pipeline simulator.
     - 🌐 **Networking & Subnetting**: Multi-hop packet tracer, CIDR engine, and DNS resolver.
     - 🛡️ **Cybersecurity & Ethical Hacking**: Safe RSA math, Diffie-Hellman, AES-GCM, CTF shell, and offline AI cyber mentor.
     - 🤖 **AI & Neural Networks**: 2D decision boundary training, 3x3 CV convolution, and offline Vector RAG.
     - ⚡ **Electronics & Circuits**: Breadboard circuit solver and Arduino PWM oscilloscope.
     - 🐧 **Linux Terminal**: In-memory Unix shell with file system permissions.
     - 🔧 **IT Troubleshooting**: 500+ real-world diagnostic scenarios.

4. **🏆 Progress & Diploma Station**:
   - **Skill Tree & 12 Badges**: Demonstrates progression across disciplines.
   - **Comprehensive Certification Exam**: Rigorous 8-domain evaluation issuing SHA-256 verifiable diplomas.
   - **Progress Portability**: JSON export/import for transfer between offline computers.
   - **Performance Settings**: GDI+ software rendering switch, low-hardware profiling, and RAM optimization.

---

## 🚀 Running & Building

### Direct Execution (No Setup Required)
Simply double-click the portable executable in the project root:
```powershell
.\BhavaniTech.UI.exe
```

### Running the Automated Verification Suite
Run the 29-test verification suite to validate database persistence, continuity resume, local AI inference, quantum circuits, transformer MHA, offensive security classifiers, CCNA exams, university modules, and calculator engine:
```powershell
dotnet run --project tests/VerifyApp/VerifyApp.csproj -c Release
```

### Building the Entire Solution
```powershell
dotnet build BhavaniTechAcademy.sln -c Release
```

### Publishing the Portable Single-File Executable
```powershell
dotnet publish src/BhavaniTech.UI/BhavaniTech.UI.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o build/portable
```






