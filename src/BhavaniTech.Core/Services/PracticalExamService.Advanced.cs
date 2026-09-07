using System.Collections.Generic;
using BhavaniTech.Core.Models;

namespace BhavaniTech.Core.Services
{
    public partial class PracticalExamService
    {
        public static List<PracticalExam> GetAdvancedExams()
        {
            return new List<PracticalExam>
            {
                // QC101 EXAMS
                new(67, "L_QC_1", "Qubit State Vector Normalization Lab", 
                    "You are configuring a quantum simulator. An unnormalized quantum state vector has amplitudes alpha = 3/5 and beta = 4/5.",
                    "Verify the normalization condition: compute |alpha|^2 + |beta|^2 and write a C# or Python verification asserting that the sum of probabilities equals 1.0.",
                    "// Compute probability sum\ndouble alpha = 0.6;\ndouble beta = 0.8;\ndouble probSum = Math.Pow(alpha, 2) + Math.Pow(beta, 2);\nConsole.WriteLine($\"Probability Sum: {probSum}\");",
                    "probSum,Math.Pow,0.6,0.8", "CodeExecution",
                    "Remember: |alpha|^2 + |beta|^2 must equal 1.0. (0.6^2 = 0.36, 0.8^2 = 0.64).", 100, 150),

                new(68, "L_QC_2", "Quantum Circuit Hadamard & CNOT Matrix Transform",
                    "Simulate an entangled Bell pair generator circuit: |00> passed through a Hadamard gate on qubit 0, followed by a CNOT gate with qubit 0 controlling qubit 1.",
                    "Write the quantum state transformation sequence showing the transition from |00> to (|00> + |11>)/sqrt(2).",
                    "// Input state: |00>\n// Step 1: H|0> x |0> = (|0> + |1>)/sqrt(2) x |0> = (|00> + |10>)/sqrt(2)\n// Step 2: CNOT((|00> + |10>)/sqrt(2)) = (|00> + |11>)/sqrt(2)\nstring bellState = \"(|00> + |11>)/sqrt(2)\";",
                    "bellState,Hadamard,CNOT,|00>,|11>", "CodeExecution",
                    "Hadamard creates superposition on qubit 0, then CNOT flips qubit 1 only when qubit 0 is |1>.", 100, 150),

                new(69, "L_QC_3", "Quantum Entanglement Measurement Simulator",
                    "Alice and Bob share Bell pair |Phi+>. Alice performs a measurement in computational basis.",
                    "Write code simulating Alice's random 50/50 measurement collapse and demonstrating that Bob's measurement always yields the identical result.",
                    "int aliceResult = new Random().Next(0, 2);\nint bobResult = aliceResult; // Entanglement correlation\nConsole.WriteLine($\"Alice: {aliceResult}, Bob: {bobResult}\");",
                    "aliceResult,bobResult,Random", "CodeExecution",
                    "When Alice measures 0, the joint state collapses to |00>, forcing Bob's measurement to be 0 with 100% certainty.", 100, 150),

                new(70, "L_QC_4", "Quantum Teleportation Reconstruction Gate Selector",
                    "Alice transmitted classical measurement bits M1 and M2 to Bob across a classical channel.",
                    "Implement the reconstruction logic: given bits m1 and m2, determine whether to apply identity, X, Z, or ZX Pauli correction gates.",
                    "string GetCorrectionGate(int m1, int m2) {\n    if (m1 == 0 && m2 == 0) return \"I\";\n    if (m1 == 0 && m2 == 1) return \"X\";\n    if (m1 == 1 && m2 == 0) return \"Z\";\n    return \"ZX\";\n}",
                    "GetCorrectionGate,m1,m2,ZX", "CodeExecution",
                    "M1 controls Z phase flip, M2 controls X bit flip.", 100, 150),

                new(71, "L_QC_5", "Grover's Search Iteration Counter",
                    "Calculate the optimal number of Grover iterations R for an unsorted database with N = 1,048,576 (2^20) items.",
                    "Implement a function calculating R = round(pi / 4 * sqrt(N)).",
                    "int n = 1048576;\ndouble r = (Math.PI / 4.0) * Math.Sqrt(n);\nint optimalIterations = (int)Math.Round(r);\nConsole.WriteLine($\"Optimal Iterations: {optimalIterations}\");",
                    "Math.PI,Math.Sqrt,optimalIterations", "CodeExecution",
                    "For N = 1,048,576, sqrt(N) = 1,024. (pi / 4) * 1024 approx 804 iterations.", 100, 150),

                new(72, "L_QC_6", "Post-Quantum Cryptography Lattice Parameter Validator",
                    "Validate security parameters for CRYSTALS-Kyber (ML-KEM) key encapsulation.",
                    "Verify the polynomial degree n = 256 and prime modulus q = 3329 used in Kyber module lattices.",
                    "int n = 256;\nint q = 3329;\nbool isValid = (n == 256 && q == 3329);\nConsole.WriteLine($\"Kyber-768 Parameters Valid: {isValid}\");",
                    "256,3329,isValid", "CodeExecution",
                    "Kyber uses degree n = 256 polynomials in ring R_q with q = 3329.", 100, 150),

                // DIST101 EXAMS
                new(73, "L_DIST_1", "Vector Clock Update Algorithm",
                    "Implement the vector clock update rule when process i receives a message with vector timestamp V_m.",
                    "Write code updating V_local: taking element-wise max with V_m and incrementing V_local[i].",
                    "void UpdateVector(int[] local, int[] remote, int pid) {\n    for (int k = 0; k < local.Length; k++) local[k] = Math.Max(local[k], remote[k]);\n    local[pid]++;\n}",
                    "UpdateVector,Math.Max,local[pid]++", "CodeExecution",
                    "Take component-wise maximum across all processes, then increment the local process ID slot.", 100, 150),

                new(74, "L_DIST_2", "CAP/PACELC Tradeoff Classifier",
                    "Build an automated architectural classifier that evaluates distributed databases based on network partition behavior and normal latency behavior.",
                    "Implement a method returning 'PC/EC' for Spanner and 'PA/EL' for DynamoDB.",
                    "string ClassifySystem(string name) {\n    if (name == \"Spanner\") return \"PC/EC\";\n    if (name == \"DynamoDB\") return \"PA/EL\";\n    return \"Unknown\";\n}",
                    "ClassifySystem,PC/EC,PA/EL", "CodeExecution",
                    "Spanner chooses Consistency under partition and Consistency over Latency normally (PC/EC).", 100, 150),

                new(75, "L_DIST_3", "Raft Quorum Calculator & Election Safety",
                    "Calculate the minimum quorum required to win a Raft leader election in an N-node cluster and evaluate split-brain risk.",
                    "Implement a function `int GetQuorum(int totalNodes)` returning (totalNodes / 2) + 1.",
                    "int GetQuorum(int totalNodes) {\n    return (totalNodes / 2) + 1;\n}\nint q5 = GetQuorum(5); // 3\nint q7 = GetQuorum(7); // 4",
                    "GetQuorum,totalNodes / 2,3", "CodeExecution",
                    "Quorum is always floor(N / 2) + 1.", 100, 150),

                new(76, "L_DIST_4", "Paxos Phase 1b Promise Generator",
                    "Implement Acceptor logic for Paxos Phase 1: compare incoming proposal number n against maxPromisedN.",
                    "If n > maxPromisedN, update maxPromisedN and return Promise; otherwise reject.",
                    "bool HandlePrepare(int n, ref int maxPromisedN) {\n    if (n > maxPromisedN) { maxPromisedN = n; return true; }\n    return false;\n}",
                    "HandlePrepare,maxPromisedN,true", "CodeExecution",
                    "Acceptors only promise to ignore proposals numbered less than the highest proposal number received.", 100, 150),

                new(77, "L_DIST_5", "PBFT Byzantine Fault Tolerance Quorum Evaluator",
                    "Calculate the minimum cluster size N required to tolerate f Byzantine traitor nodes in PBFT.",
                    "Implement `int GetMinPbftNodes(int f)` returning 3 * f + 1.",
                    "int GetMinPbftNodes(int f) {\n    return (3 * f) + 1;\n}\nint nFor2 = GetMinPbftNodes(2); // 7 nodes",
                    "GetMinPbftNodes,3 * f,7", "CodeExecution",
                    "PBFT requires N >= 3f + 1 nodes to reach consensus despite f arbitrary malicious nodes.", 100, 150),

                new(78, "L_DIST_6", "Saga Compensating Transaction Orchestrator",
                    "Simulate an e-commerce Saga: Step 1 = AuthorizePayment, Step 2 = ReserveInventory, Step 3 = ShipOrder.",
                    "If Step 2 fails, execute Compensating Transaction RefundPayment and log rollback.",
                    "bool step1 = true; // Payment OK\nbool step2 = false; // Out of stock\nif (!step2) {\n    Console.WriteLine(\"Compensating: RefundPayment\");\n    Console.WriteLine(\"Saga Aborted Cleanly\");\n}",
                    "Compensating,RefundPayment,Saga", "CodeExecution",
                    "When a saga step fails, prior successful steps must be reversed using compensating transactions.", 100, 150),

                // KRN101 EXAMS
                new(79, "L_KRN_1", "Linux Syscall Register Inspection",
                    "Write an x86-64 assembly or C snippet simulating passing syscall arguments for `write(1, msg, 13)`.",
                    "Populate %rax with 1 (sys_write), %rdi with 1 (stdout), and invoke syscall.",
                    "// %rax = 1 (sys_write)\n// %rdi = 1 (stdout)\n// %rsi = buffer pointer\n// %rdx = length\nint syscall_nr = 1;\nint fd = 1;",
                    "sys_write,syscall,rax,rdi", "CodeExecution",
                    "x86-64 Linux syscall convention uses rax for call number and rdi, rsi, rdx for the first three arguments.", 100, 150),

                new(80, "L_KRN_2", "x86-64 4-Level Page Table Index Calculator",
                    "Given a 64-bit virtual address `0x00007FFF5FC00000`, extract the PGD (bits 47-39) and PMD index offsets.",
                    "Implement bitmasking and shifting logic in C# to parse 9-bit page table level indices.",
                    "ulong vaddr = 0x00007FFF5FC00000UL;\nulong pgd = (vaddr >> 39) & 0x1FF;\nulong pud = (vaddr >> 30) & 0x1FF;\nConsole.WriteLine($\"PGD: {pgd}, PUD: {pud}\");",
                    "vaddr,0x1FF,39,30", "CodeExecution",
                    "Each level of 4-level paging uses 9 bits (mask 0x1FF = 511), shifting by 39, 30, 21, and 12 bits.", 100, 150),

                new(81, "L_KRN_3", "VFS Inode & Hard Link Reference Counter",
                    "Simulate creating a hard link `ln file1.txt file2.txt` and inspect the inode link count `i_nlink`.",
                    "Demonstrate that deleting `file1.txt` decrements `i_nlink` from 2 to 1 without freeing storage blocks.",
                    "int i_nlink = 1;\ni_nlink++; // ln file1 file2\nConsole.WriteLine($\"Links after hardlink: {i_nlink}\");\ni_nlink--; // rm file1\nConsole.WriteLine($\"Links after delete: {i_nlink}\");",
                    "i_nlink,hardlink,delete", "CodeExecution",
                    "Hard links share the same inode number; blocks are only freed when i_nlink reaches 0.", 100, 150),

                new(82, "L_KRN_4", "eBPF Packet Counter Map Simulator",
                    "Write a C eBPF snippet defining a `BPF_MAP_TYPE_ARRAY` map that stores incoming packet counts.",
                    "Include the `SEC(\"xdp\")` hook and map lookup update logic.",
                    "// SEC(\"xdp\")\n// struct { __uint(type, BPF_MAP_TYPE_ARRAY); } pkt_count SEC(\".maps\");\nint count = 1;\nConsole.WriteLine(\"eBPF BPF_MAP_TYPE_ARRAY verified\");",
                    "BPF_MAP_TYPE_ARRAY,xdp,maps", "CodeExecution",
                    "eBPF maps provide kernel-space persistent state accessible from user space.", 100, 150),

                new(83, "L_KRN_5", "XDP Line-Rate DDoS Packet Filter",
                    "Write an XDP program that inspects IP packets: if source port equals 53 (DNS amplification attack), return `XDP_DROP`; otherwise return `XDP_PASS`.",
                    "Implement the filter branch logic returning XDP action codes.",
                    "int FilterPacket(int srcPort) {\n    if (srcPort == 53) return 1; // XDP_DROP\n    return 2; // XDP_PASS\n}\nint action = FilterPacket(53);",
                    "FilterPacket,XDP_DROP,XDP_PASS", "CodeExecution",
                    "XDP_DROP discards packets inside the NIC driver before kernel sk_buff allocation.", 100, 150),

                new(84, "L_KRN_6", "bpftrace Syscall Latency Profiler",
                    "Construct a `bpftrace` script that records latency of `sys_enter_write` to `sys_exit_write` and plots a histogram.",
                    "Use `@start[tid]` and `hist()` to capture execution microseconds.",
                    "// bpftrace script\n// tracepoint:syscalls:sys_enter_write { @start[tid] = nsecs; }\n// tracepoint:syscalls:sys_exit_write /@start[tid]/ { @latency = hist((nsecs - @start[tid])/1000); delete(@start[tid]); }",
                    "tracepoint,nsecs,hist,delete", "CodeExecution",
                    "bpftrace dynamic tracing measures in-kernel execution latency with zero downtime.", 100, 150),

                // HFT101 EXAMS
                new(85, "L_HFT_1", "Cache Line False Sharing Eliminator",
                    "Eliminate false sharing between two worker threads incrementing variables on separate cores.",
                    "Pad the struct members to ensure each variable occupies its own distinct 64-byte cache line.",
                    "struct PaddedData {\n    public long Counter1;\n    public fixed byte Pad[56]; // 64 bytes total\n    public long Counter2;\n}",
                    "PaddedData,Counter1,56,64", "CodeExecution",
                    "Adding 56 bytes of padding to an 8-byte variable fills the 64-byte cache line, eliminating MESI invalidations.", 100, 150),

                new(86, "L_HFT_2", "Lock-Free CAS Counter Implementation",
                    "Implement a lock-free thread-safe increment using `Interlocked.CompareExchange` (CAS loop).",
                    "Write the while loop that repeatedly attempts CAS until the atomic update succeeds.",
                    "void AtomicIncrement(ref int target) {\n    int current;\n    do {\n        current = target;\n    } while (Interlocked.CompareExchange(ref target, current + 1, current) != current);\n}",
                    "CompareExchange,target,current + 1", "CodeExecution",
                    "CAS loops re-read the expected value and retry atomically without acquiring OS kernel mutex locks.", 100, 150),

                new(87, "L_HFT_3", "LMAX Disruptor Power-of-2 Ring Buffer Indexer",
                    "In the Disruptor, ring buffer size must be a power of 2 so modulo can be replaced with bitwise AND.",
                    "Implement the index lookup: `sequence & (bufferSize - 1)` for a buffer of size 1024.",
                    "long bufferSize = 1024;\nlong mask = bufferSize - 1;\nlong seq = 1025;\nlong slot = seq & mask;\nConsole.WriteLine($\"Sequence {seq} maps to slot: {slot}\");",
                    "bufferSize - 1,seq & mask,1024", "CodeExecution",
                    "Bitwise AND with (size - 1) executes in 1 clock cycle, whereas integer division modulo takes 15-40 cycles.", 100, 150),

                new(88, "L_HFT_4", "Kernel Bypass DPDK Polling Loop",
                    "Simulate a DPDK user-space NIC polling loop that continuously polls packet burst queues without interrupts.",
                    "Implement a poll loop checking `rte_eth_rx_burst` and processing incoming descriptors.",
                    "// DPDK PMD Loop\nint totalPackets = 0;\nfor (int i = 0; i < 100; i++) {\n    int nb_rx = 32; // Simulating burst\n    totalPackets += nb_rx;\n}\nConsole.WriteLine($\"Packets processed via poll-mode: {totalPackets}\");",
                    "totalPackets,burst,poll", "CodeExecution",
                    "DPDK poll-mode drivers spin continuously on NIC ring buffers, eliminating interrupt latency.", 100, 150),

                new(89, "L_HFT_5", "Limit Order Book Matching Simulator",
                    "Implement price-time priority matching: an incoming Buy order of 50 units at $100.00 matches against an Ask of 30 units at $100.00.",
                    "Calculate the filled trade quantity and remaining order resting volume.",
                    "int buyQty = 50;\nint askQty = 30;\nint matchQty = Math.Min(buyQty, askQty);\nbuyQty -= matchQty;\naskQty -= matchQty;\nConsole.WriteLine($\"Trade Executed: {matchQty}, Remaining Buy: {buyQty}\");",
                    "Math.Min,matchQty,Remaining Buy", "CodeExecution",
                    "Trades match against existing orders at the maker's price; unfilled shares rest on the book.", 100, 150),

                new(90, "L_HFT_6", "SBE Zero-Copy Binary Field Unpacker",
                    "Read a 64-bit trade price and 32-bit volume directly from a byte buffer without object allocation.",
                    "Use `BitConverter` or direct pointer casts to extract binary fields at specific offsets.",
                    "byte[] buffer = new byte[16];\nlong price = BitConverter.ToInt64(buffer, 0);\nint volume = BitConverter.ToInt32(buffer, 8);\nConsole.WriteLine($\"Price: {price}, Volume: {volume}\");",
                    "BitConverter,ToInt64,ToInt32", "CodeExecution",
                    "SBE aligns binary primitives directly to CPU natural word boundaries for zero-copy deserialization.", 100, 150),

                // COMP101 EXAMS
                new(91, "L_COMP_1", "Regular Expression Lexer Tokenizer",
                    "Build a miniature tokenizer for arithmetic expressions identifying `NUMBER`, `PLUS`, and `MULTIPLY` tokens.",
                    "Scan input '12 + 34 * 5' and output the list of token types.",
                    "string input = \"12 + 34 * 5\";\nstring[] parts = input.Split(' ');\nforeach (var p in parts) {\n    string type = char.IsDigit(p[0]) ? \"NUMBER\" : \"OPERATOR\";\n    Console.WriteLine($\"{p}: {type}\");\n}",
                    "NUMBER,OPERATOR,Split", "CodeExecution",
                    "The lexer categorizes raw lexemes into token types with associated values and positions.", 100, 150),

                new(92, "L_COMP_2", "Recursive Descent Expression Parser",
                    "Implement a recursive descent function `ParseTerm()` that handles multiplication precedence over addition.",
                    "Demonstrate that `2 + 3 * 4` evaluates correctly to 14 rather than 20.",
                    "int ParseTerm(int a, int b, int c) {\n    return a + (b * c);\n}\nint result = ParseTerm(2, 3, 4);\nConsole.WriteLine($\"AST Eval Result: {result}\");",
                    "ParseTerm,AST,14", "CodeExecution",
                    "Multiplication binds tighter in the grammar, forming a deeper sub-tree in the AST.", 100, 150),

                new(93, "L_COMP_3", "Symbol Table Scope Resolver",
                    "Implement a scoped Symbol Table: verify that an inner block variable shadows an outer variable of the same name.",
                    "Demonstrate scope lookup checking current dictionary before falling back to parent dictionary.",
                    "var globalScope = new Dictionary<string, string> { [\"x\"] = \"global_int\" };\nvar localScope = new Dictionary<string, string> { [\"x\"] = \"local_int\" };\nstring resolved = localScope.ContainsKey(\"x\") ? localScope[\"x\"] : globalScope[\"x\"];\nConsole.WriteLine($\"Resolved x: {resolved}\");",
                    "localScope,globalScope,ContainsKey", "CodeExecution",
                    "Symbol tables search upwards through lexical scope enclosures, enabling variable shadowing.", 100, 150),

                new(94, "L_COMP_4", "SSA Form Transformation",
                    "Convert non-SSA code `x = 5; x = x + 1; x = x * 2;` into valid Static Single Assignment form.",
                    "Assign uniquely versioned variables `x1`, `x2`, `x3` such that no variable is defined twice.",
                    "int x1 = 5;\nint x2 = x1 + 1;\nint x3 = x2 * 2;\nConsole.WriteLine($\"SSA Variables: x1={x1}, x2={x2}, x3={x3}\");",
                    "x1,x2,x3,SSA", "CodeExecution",
                    "In SSA form, every variable is assigned a value exactly once, simplifying compiler optimization.", 100, 150),

                new(95, "L_COMP_5", "Loop-Invariant Code Motion (LICM) Optimizer",
                    "Optimize a loop computing `total += i * (width * 2)` by hoisting the invariant calculation `stride = width * 2` outside the loop.",
                    "Demonstrate that hoisting eliminates redundant multiplications on every iteration.",
                    "int width = 10;\nint stride = width * 2; // Hoisted invariant\nint total = 0;\nfor (int i = 0; i < 100; i++) { total += i * stride; }\nConsole.WriteLine($\"Optimized loop total: {total}\");",
                    "stride,hoisted,total", "CodeExecution",
                    "LICM moves invariant calculations outside loop headers to save CPU cycles.", 100, 150),

                new(96, "L_COMP_6", "LLVM IR Function Emitter",
                    "Write an LLVM IR function string defining `@add(i32 %a, i32 %b)` that returns the sum.",
                    "Include `%res = add i32 %a, %b` and `ret i32 %res`.",
                    "string llvmIr = @\"define i32 @add(i32 %a, i32 %b) {\n  %res = add i32 %a, %b\n  ret i32 %res\n}\";\nConsole.WriteLine(llvmIr);",
                    "define i32,add i32,ret i32", "CodeExecution",
                    "LLVM IR represents strongly-typed assembly instructions targeting universal backend codegen.", 100, 150),

                // LLM101 EXAMS
                new(97, "L_LLM_1", "Scaled Dot-Product Attention Calculator",
                    "Compute scaled dot-product attention score for Query vector Q = [1, 2] and Key vector K = [2, 1] with d_k = 4.",
                    "Calculate dotProduct / sqrt(d_k) = (1*2 + 2*1) / sqrt(4) = 4 / 2 = 2.0.",
                    "double dot = (1 * 2) + (2 * 1);\ndouble dk = 4.0;\ndouble score = dot / Math.Sqrt(dk);\nConsole.WriteLine($\"Attention Score: {score}\");",
                    "dot,Math.Sqrt,score", "CodeExecution",
                    "Scaling by 1/sqrt(d_k) stabilizes dot product variance before applying softmax.", 100, 150),

                new(98, "L_LLM_2", "RMSNorm Layer Normalization Implementation",
                    "Implement Root Mean Square Normalization (RMSNorm) for a hidden activation vector [2.0, 4.0, 4.0].",
                    "Compute RMS = sqrt(mean(x^2)) and normalize vector elements by dividing by RMS.",
                    "double[] x = { 2.0, 4.0, 4.0 };\ndouble sumSq = 4.0 + 16.0 + 16.0; // 36.0\ndouble rms = Math.Sqrt(sumSq / 3.0); // sqrt(12)\nConsole.WriteLine($\"RMS: {rms}\");",
                    "sumSq,Math.Sqrt,rms", "CodeExecution",
                    "RMSNorm avoids mean calculation, reducing memory operations in transformer decoder blocks.", 100, 150),

                new(99, "L_LLM_3", "LoRA Low-Rank Parameter Savings Calculator",
                    "Calculate the parameter reduction for a weight matrix W of size 4096 x 4096 using LoRA rank r = 16.",
                    "Original params = 4096 * 4096 = 16,777,216. LoRA params = (4096 * 16) + (16 * 4096) = 131,072. Calculate savings percentage.",
                    "long originalParams = 4096 * 4096;\nlong loraParams = (4096 * 16) + (16 * 4096);\ndouble reduction = (1.0 - ((double)loraParams / originalParams)) * 100.0;\nConsole.WriteLine($\"Parameter Reduction: {reduction:F2}%\");",
                    "originalParams,loraParams,reduction", "CodeExecution",
                    "LoRA reduces trainable parameter footprint by >99%, enabling fine-tuning on consumer hardware.", 100, 150),

                new(100, "L_LLM_4", "Symmetric 4-bit Quantization Scale Calculator",
                    "Compute the scale factor S for symmetric INT4 quantization of weights in range [-3.5, 3.5].",
                    "For 4-bit signed (-8 to 7), S = max(|x|) / 7 = 3.5 / 7 = 0.5. Quantize weight 2.0 to INT4.",
                    "double maxVal = 3.5;\ndouble scale = maxVal / 7.0; // 0.5\ndouble w = 2.0;\nsbyte quantized = (sbyte)Math.Round(w / scale); // 4\nConsole.WriteLine($\"Quantized weight: {quantized}\");",
                    "scale,quantized,Math.Round", "CodeExecution",
                    "Quantization maps continuous floats to discrete integers using calibrated scale factors.", 100, 150),

                new(101, "L_LLM_5", "KV-Cache VRAM Footprint Calculator",
                    "Calculate the KV-cache VRAM requirement for an 8-user batch with context length 2048 on a 32-layer model with hidden size 4096 in FP16 (2 bytes).",
                    "Formula: 2 * 2 * layers * hidden * seq * batch. Output total megabytes.",
                    "long layers = 32;\nlong hidden = 4096;\nlong seq = 2048;\nlong batch = 8;\nlong bytesTotal = 2 * 2 * layers * hidden * seq * batch;\nlong mb = bytesTotal / (1024 * 1024);\nConsole.WriteLine($\"KV-Cache VRAM: {mb} MB\");",
                    "layers,hidden,seq,batch,bytesTotal", "CodeExecution",
                    "PagedAttention eliminates fragmentation in KV-cache memory allocation across concurrent sessions.", 100, 150),

                new(102, "L_LLM_6", "Speculative Decoding Rejection Sampler",
                    "Simulate verification sampling: draft model probability is 0.70, target model probability is 0.85.",
                    "Since P_target >= P_draft, acceptance probability is 1.0 (Accepted). If target is 0.35, acceptance is 0.35 / 0.70 = 0.50.",
                    "double pDraft = 0.70;\ndouble pTarget = 0.85;\ndouble pAccept = Math.Min(1.0, pTarget / pDraft);\nbool accepted = pAccept >= 1.0;\nConsole.WriteLine($\"Token Accepted Speculatively: {accepted}\");",
                    "pDraft,pTarget,pAccept,Math.Min", "CodeExecution",
                    "Speculative decoding delivers 2x-3x speedup while maintaining target distribution fidelity.", 100, 150)
            };
        }
    }
}
