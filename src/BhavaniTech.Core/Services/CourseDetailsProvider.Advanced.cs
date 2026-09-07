using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public static partial class CourseDetailsProvider
    {
        public static Dictionary<string, CourseDetails> GetAdvancedDetails()
        {
            return new Dictionary<string, CourseDetails>(StringComparer.OrdinalIgnoreCase)
            {
                ["QC101"] = new CourseDetails(
                    CourseId: "QC101",
                    CourseTitle: "Quantum Computing & Quantum Information Foundations",
                    Category: "Cutting-Edge",
                    Summary: "Deep dive into quantum mechanical principles of computing: Hilbert space state vectors, Bloch sphere rotations, multi-qubit entanglement, Bell tests, Grover's search, and Shor's algorithm.",
                    Prerequisites: "• Linear algebra (vector dot products, matrix multiplication, complex numbers)\n• Basic probability theory\n• Completed CS101 or familiar with binary logic gates",
                    HardwareRequirements: "• CPU: 1.5 GHz or faster\n• RAM: 1 GB minimum\n• Storage: 50 MB\n• 100% offline quantum simulator included in Bhavani Academy",
                    InstallationSteps:
                        "1. BUILT-IN WORKBENCH (Zero Setup):\n" +
                        "   - Practice qubit state transformations and Bell pair measurements in the Bhavani Academy Sandbox.\n\n" +
                        "2. OPTIONAL PYTHON QUANTUM TOOLCHAIN:\n" +
                        "   - Windows/Linux:\n" +
                        "     pip install qiskit qiskit-aer cirq\n" +
                        "   - Verify: python -c \"import qiskit; print(qiskit.__version__)\"",
                    EnvironmentVerification:
                        "Run in Terminal:\n" +
                        "  python -c \"import qiskit; qc = qiskit.QuantumCircuit(2); qc.h(0); qc.cx(0,1); print(qc)\"\n" +
                        "Expected: Bell state quantum circuit diagram rendered in terminal.",
                    RecommendedTools: "Bhavani Quantum Lab, IBM Quantum Composer, Qiskit Aer, Quirk Quantum Simulator",
                    CareerAndCertifications: "IBM Certified Associate Developer - Quantum Computation, Quantum Algorithms Researcher"
                ),

                ["DIST101"] = new CourseDetails(
                    CourseId: "DIST101",
                    CourseTitle: "Distributed Systems, Consensus & Fault Tolerance",
                    Category: "Architecture",
                    Summary: "Master Lamport clocks, vector clocks, the CAP and PACELC theorems, Raft leader election, Paxos state machines, Byzantine Fault Tolerance (PBFT), and distributed Saga transactions.",
                    Prerequisites: "• Solid understanding of client-server networking (TCP/IP, sockets)\n• Concurrency fundamentals (threads, locks, race conditions)\n• Completed NET101 or equivalent network background",
                    HardwareRequirements: "• CPU: Dual-core 2.0 GHz\n• RAM: 2 GB minimum\n• Storage: 100 MB\n• 100% offline cluster simulation built-in",
                    InstallationSteps:
                        "1. BUILT-IN DISTRIBUTED SIMULATOR:\n" +
                        "   - Simulate network partition splits and Raft leader elections inside Bhavani Academy.\n\n" +
                        "2. OPTIONAL LOCAL RAFT/ETCD CLUSTER:\n" +
                        "   - Linux: sudo apt install etcd-server etcd-client -y\n" +
                        "   - Windows: winget install CoreOS.etcd",
                    EnvironmentVerification:
                        "Run in Terminal:\n" +
                        "  etcdctl version\n" +
                        "  etcdctl endpoint health\n" +
                        "Expected: etcd server version and healthy raft consensus cluster response.",
                    RecommendedTools: "Bhavani Raft Simulator, etcd, HashiCorp Consul, Jepsen Distributed Testing Framework",
                    CareerAndCertifications: "Distributed Systems Architect, Senior Infrastructure Engineer, CNCF Certified Kubernetes Administrator"
                ),

                ["KRN101"] = new CourseDetails(
                    CourseId: "KRN101",
                    CourseTitle: "Linux Kernel Internals, eBPF & Systems Tracing",
                    Category: "Systems",
                    Summary: "Explore CPU privilege rings, syscall entrypoints, multi-level 4-level page tables, VFS buffer cache, eBPF in-kernel verifier, XDP line-rate packet filtering, and bpftrace flame graphs.",
                    Prerequisites: "• Competency in C programming and pointer arithmetic\n• Familiarity with Linux terminal and shell commands (LNX101)\n• Understanding of computer architecture (CS101)",
                    HardwareRequirements: "• CPU: 64-bit x86-64 or ARM64\n• RAM: 2 GB minimum (4 GB recommended)\n• Storage: 500 MB (or WSL2 on Windows)",
                    InstallationSteps:
                        "1. WSL2 / NATIVE LINUX SETUP:\n" +
                        "   - Windows: wsl --install -d Ubuntu-24.04\n" +
                        "   - Linux: sudo apt update && sudo apt install linux-headers-$(uname -r) bpfcc-tools bpftrace clang llvm libbpf-dev -y",
                    EnvironmentVerification:
                        "Run in Terminal:\n" +
                        "  uname -r\n" +
                        "  sudo bpftrace -e 'BEGIN { printf(\"eBPF is active on Linux %s!\\n\", curtask->comm); exit(); }'\n" +
                        "Expected: Kernel version reported and eBPF bytecode compiled and executed in kernel space.",
                    RecommendedTools: "bpftrace, BCC tools, perf, FlameGraph, GDB kernel debugging, QEMU",
                    CareerAndCertifications: "Linux Kernel Developer, SRE Performance Engineer, eBPF Systems Observability Specialist"
                ),

                ["HFT101"] = new CourseDetails(
                    CourseId: "HFT101",
                    CourseTitle: "Ultra-Low-Latency Systems & High-Frequency Trading",
                    Category: "High-Performance",
                    Summary: "Engineer sub-microsecond trading systems: CPU cache-line alignment, false sharing elimination, lock-free CAS atomics, LMAX Disruptor ring buffers, DPDK kernel bypass, and LOB matching.",
                    Prerequisites: "• Advanced C++ or C# multi-threading\n• Knowledge of CPU memory hierarchies and assembly instructions\n• Completed PROG101 and HW201",
                    HardwareRequirements: "• CPU: Multi-core x86-64 processor\n• RAM: 4 GB minimum\n• Storage: 200 MB",
                    InstallationSteps:
                        "1. LOW-LATENCY C++ / C# TOOLCHAIN:\n" +
                        "   - Windows: Visual Studio 2022 C++ Tools or .NET 9 SDK\n" +
                        "   - Linux: sudo apt install g++ clang numactl libnuma-dev -y",
                    EnvironmentVerification:
                        "Run in Terminal:\n" +
                        "  lscpu\n" +
                        "Inspect L1d, L1i, L2, and L3 cache sizes and core topology.",
                    RecommendedTools: "Bhavani Low-Latency Benchmarker, QuickFIX, DPDK, Intel VTune Profiler, Google Benchmark",
                    CareerAndCertifications: "Quantitative Systems Engineer, Ultra-Low-Latency C++ Developer, Exchange Infrastructure Architect"
                ),

                ["COMP101"] = new CourseDetails(
                    CourseId: "COMP101",
                    CourseTitle: "Compiler Construction, LLVM IR & Code Generation",
                    Category: "Programming",
                    Summary: "Build complete optimizing compilers: Lexical analysis (DFA/NFA), Recursive Descent AST parsing, semantic type systems, Static Single Assignment (SSA) form, and LLVM IR native codegen.",
                    Prerequisites: "• Data structures (Trees, Hash Tables, Graphs, Stacks)\n• Proficiency in C#, C++, or Rust\n• Completed CS101 and SW101",
                    HardwareRequirements: "• CPU: 1.5 GHz or faster\n• RAM: 2 GB minimum\n• Storage: 300 MB",
                    InstallationSteps:
                        "1. OFFLINE COMPILER LAB:\n" +
                        "   - Use the built-in Bhavani AST parser and SSA visualizer.\n\n" +
                        "2. LLVM COMPILER INFRASTRUCTURE:\n" +
                        "   - Windows: winget install LLVM.LLVM\n" +
                        "   - Linux: sudo apt install llvm clang llvm-dev -y",
                    EnvironmentVerification:
                        "Run in Terminal:\n" +
                        "  clang --version\n" +
                        "  llc --version\n" +
                        "Expected: LLVM target machine code generator and Clang frontend banner.",
                    RecommendedTools: "LLVM, Clang, ANTLR4, Compiler Explorer (Godbolt), Graphviz",
                    CareerAndCertifications: "Compiler Engineer, Language Runtime Developer, LLVM Infrastructure Specialist"
                ),

                ["LLM101"] = new CourseDetails(
                    CourseId: "LLM101",
                    CourseTitle: "Modern LLM Architecture, Fine-Tuning & Quantization",
                    Category: "Artificial Intelligence",
                    Summary: "Master contemporary Generative AI engineering: Scaled dot-product self-attention, RoPE, RMSNorm, SwiGLU, FlashAttention, LoRA/QLoRA parameter-efficient fine-tuning, GGUF/AWQ, and PagedAttention.",
                    Prerequisites: "• Python or C# proficiency\n• Matrix algebra and calculus (partial derivatives, gradients)\n• Completed AI101 or introductory machine learning",
                    HardwareRequirements: "• CPU: Quad-core or better\n• RAM: 8 GB minimum (16 GB recommended)\n• GPU: Optional (works 100% on CPU with quantized GGUF models)",
                    InstallationSteps:
                        "1. OFFLINE LLM INFERENCE SANDBOX:\n" +
                        "   - Built-in Bhavani Local AI vector and prompt playground.\n\n" +
                        "2. LOCAL RUNTIME TOOLCHAIN:\n" +
                        "   - Windows: winget install Ollama.Ollama\n" +
                        "   - Python: pip install torch transformers peft accelerate bitsandbytes",
                    EnvironmentVerification:
                        "Run in Terminal:\n" +
                        "  python -c \"import torch; print('PyTorch Version:', torch.__version__, 'CUDA Available:', torch.cuda.is_available())\"\n" +
                        "Expected: PyTorch version and hardware acceleration status.",
                    RecommendedTools: "Bhavani Vector RAG Lab, Ollama, llama.cpp, vLLM, Hugging Face Transformers, Axolotl",
                    CareerAndCertifications: "Senior Generative AI Engineer, LLM Fine-Tuning Specialist, AI Inference Systems Architect"
                )
            };
        }
    }
}
