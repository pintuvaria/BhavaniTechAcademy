using System;
using System.Collections.Generic;
using BhavaniTech.Core.Models;

namespace BhavaniTech.Core.Database
{
    public static partial class CurriculumSeeder
    {
        public static List<CourseSeed> GetAdvancedCourses()
        {
            return new List<CourseSeed>
            {
                new("QC101", "Quantum Computing & Quantum Information Foundations", CourseCategory.Fundamentals, 
                    "Master qubits, superposition, Bloch sphere geometry, entanglement, Bell states, Grover's search, and Shor's algorithm.", "Atom", 12),

                new("DIST101", "Distributed Systems, Consensus & Fault Tolerance", CourseCategory.Networking, 
                    "Master Lamport clocks, vector clocks, CAP/PACELC theorems, Raft leader election, Paxos, BFT, and Saga orchestration.", "Network", 13),

                new("KRN101", "Linux Kernel Internals, eBPF & Systems Tracing", CourseCategory.Linux, 
                    "Explore kernel rings, syscall dispatch, virtual memory paging, VFS buffer cache, eBPF bytecode, XDP, and bpftrace.", "Terminal", 14),

                new("HFT101", "Ultra-Low-Latency Systems & High-Frequency Trading", CourseCategory.Programming, 
                    "Master CPU cache line alignment, lock-free CAS, LMAX Disruptor, DPDK kernel bypass, and limit order book matching engines.", "Zap", 15),

                new("COMP101", "Compiler Construction, LLVM IR & Code Generation", CourseCategory.Programming, 
                    "Master DFA/NFA lexing, recursive descent AST parsing, Static Single Assignment (SSA) form, CFGs, and LLVM native codegen.", "Code", 16),

                new("LLM101", "Modern LLM Architecture, Fine-Tuning & Quantization", CourseCategory.ArtificialIntelligence, 
                    "Master Transformer self-attention, RoPE embeddings, LoRA/QLoRA fine-tuning, GGUF/AWQ quantization, and PagedAttention.", "Brain", 17)
            };
        }

        public static List<ModuleSeed> GetAdvancedModules()
        {
            return new List<ModuleSeed>
            {
                new("M_QC", "QC101", "Quantum Mechanics, Gates & Quantum Algorithms", 1),
                new("M_DIST", "DIST101", "Time, Consensus & Distributed Storage Architecture", 1),
                new("M_KRN", "KRN101", "Kernel Memory, VFS & In-Kernel eBPF Observability", 1),
                new("M_HFT", "HFT101", "Microarchitecture, Lock-Free Concurrency & Market Engines", 1),
                new("M_COMP", "COMP101", "Parsing, Intermediate Representations & LLVM Codegen", 1),
                new("M_LLM", "LLM101", "Attention Mechanics, Quantization & High-Throughput Serving", 1)
            };
        }

        public static List<LessonSeed> GetAdvancedLessons()
        {
            return new List<LessonSeed>
            {
                // =========================================================================
                // QC101: QUANTUM COMPUTING (6 LESSONS)
                // =========================================================================
                new("L_QC_1", "M_QC", "Qubits, Superposition & Bloch Sphere Geometry",
                    "Understand the fundamental unit of quantum information: quantum state vectors, Dirac bra-ket notation, and superposition.",
                    @"# Qubits, Superposition & Bloch Sphere Geometry

Classical computing uses bits that are either **0** or **1**. In contrast, a **Qubit** (quantum bit) is a two-level quantum mechanical system described by a state vector $|\psi\rangle$ living in a 2-dimensional complex Hilbert space $\mathbb{C}^2$.

---

## 1. Dirac Bra-Ket Notation & State Vectors
The standard computational basis states are denoted as:
- $|0\rangle = \begin{bmatrix} 1 \\ 0 \end{bmatrix}$
- $|1\rangle = \begin{bmatrix} 0 \\ 1 \end{bmatrix}$

Any arbitrary single-qubit state $|\psi\rangle$ is represented as a linear combination (superposition) of basis states:
$$|\psi\rangle = \alpha |0\rangle + \beta |1\rangle$$

where $\alpha, \beta \in \mathbb{C}$ are probability amplitudes satisfying the normalization constraint:
$$|\alpha|^2 + |\beta|^2 = 1$$

When measured in the computational basis:
- The probability of obtaining outcome $0$ is $P(0) = |\alpha|^2$
- The probability of obtaining outcome $1$ is $P(1) = |\beta|^2$
- The quantum state instantly collapses to the measured basis state!

---

## 2. Geometric Representation: The Bloch Sphere
Every pure single-qubit state can be uniquely mapped to a point on the surface of a unit sphere in $\mathbb{R}^3$:
$$|\psi\rangle = \cos\left(\frac{\theta}{2}\right) |0\rangle + e^{i\phi} \sin\left(\frac{\theta}{2}\right) |1\rangle$$

Where:
- $\theta \in [0, \pi]$ represents the polar angle (colatitude)
- $\phi \in [0, 2\pi)$ represents the azimuthal angle (longitude)
- The North Pole ($\theta = 0$) corresponds to $|0\rangle$
- The South Pole ($\theta = \pi$) corresponds to $|1\rangle$
- The Equator ($\theta = \frac{\pi}{2}$) represents balanced superpositions like $|+\rangle = \frac{|0\rangle + |1\rangle}{\sqrt{2}}$ ($\phi = 0$) and $|-\rangle = \frac{|0\rangle - |1\rangle}{\sqrt{2}}$ ($\phi = \pi$).

---

## 3. Physical Realizations of Qubits
1. **Superconducting Transmon Qubits** (IBM, Google Sycamore): Josephson junctions cooled to ~15 millikelvin in dilution refrigerators.
2. **Trapped Ion Qubits** (IonQ, Quantinuum): Individual ionized ytterbium/calcium atoms manipulated via laser pulses in electromagnetic Paul traps.
3. **Photonic Qubits** (Xanadu, PsiQuantum): Polarization or path states of individual photons operating at room temperature.",
                    25, "Master"),

                new("L_QC_2", "M_QC", "Quantum Logic Gates: Pauli, Hadamard & CNOT",
                    "Learn unitary transformation matrices that manipulate qubit states and create multi-qubit entanglement.",
                    @"# Quantum Logic Gates: Pauli, Hadamard & CNOT

Quantum operations are reversible linear transformations represented by **unitary matrices** ($U^\dagger U = I$).

---

## 1. Single-Qubit Gates

### The Pauli Matrices
1. **Pauli-X (Quantum NOT Gate)**:
$$X = \begin{bmatrix} 0 & 1 \\ 1 & 0 \end{bmatrix}, \quad X|0\rangle = |1\rangle, \quad X|1\rangle = |0\rangle$$
Performs a $180^\circ$ rotation around the X-axis of the Bloch sphere.

2. **Pauli-Z (Phase Flip Gate)**:
$$Z = \begin{bmatrix} 1 & 0 \\ 0 & -1 \end{bmatrix}, \quad Z|0\rangle = |0\rangle, \quad Z|1\rangle = -|1\rangle$$
Leaves $|0\rangle$ untouched and inverts the phase of $|1\rangle$.

3. **Hadamard Gate ($H$)**: Creates equal superposition from computational basis states:
$$H = \frac{1}{\sqrt{2}} \begin{bmatrix} 1 & 1 \\ 1 & -1 \end{bmatrix}$$
$$H|0\rangle = \frac{|0\rangle + |1\rangle}{\sqrt{2}} = |+\rangle, \quad H|1\rangle = \frac{|0\rangle - |1\rangle}{\sqrt{2}} = |-\rangle$$

---

## 2. Multi-Qubit Controlled-NOT Gate (CNOT)
The CNOT gate acts on a 2-qubit tensor product space $\mathbb{C}^4$:
$$\text{CNOT} = \begin{bmatrix} 1 & 0 & 0 & 0 \\ 0 & 1 & 0 & 0 \\ 0 & 0 & 0 & 1 \\ 0 & 0 & 1 & 0 \end{bmatrix}$$

- Target qubit is flipped if and only if the control qubit is $|1\rangle$.
- In combination with single-qubit rotations, CNOT forms a **universal quantum gate set** capable of approximating any unitary operation.",
                    25, "Master"),

                new("L_QC_3", "M_QC", "Quantum Entanglement, Bell States & No-Cloning Theorem",
                    "Explore non-local quantum correlations, Bell inequality violations, and the proof of the No-Cloning Theorem.",
                    @"# Quantum Entanglement, Bell States & No-Cloning Theorem

Quantum entanglement is a phenomenon where composite quantum states cannot be factored into the tensor product of individual subsystem states.

---

## 1. The Four Bell States (EPR Pairs)
Starting with two qubits $|00\rangle$, passing qubit 0 through a Hadamard gate and then both through a CNOT gate produces the maximally entangled Bell state:
$$|\Phi^+\rangle = \frac{|00\rangle + |11\rangle}{\sqrt{2}}$$

The complete orthonormal Bell basis consists of:
1. $|\Phi^+\rangle = \frac{|00\rangle + |11\rangle}{\sqrt{2}}$
2. $|\Phi^-\rangle = \frac{|00\rangle - |11\rangle}{\sqrt{2}}$
3. $|\Psi^+\rangle = \frac{|01\rangle + |10\rangle}{\sqrt{2}}$
4. $|\Psi^-\rangle = \frac{|01\rangle - |10\rangle}{\sqrt{2}}$

If Alice measures qubit A and observes $0$, Bob's qubit B collapses to $0$ instantly, regardless of spatial distance!

---

## 2. The No-Cloning Theorem
**Theorem**: It is physically impossible to create an identical copy of an arbitrary unknown quantum state.

### Proof by Contradiction:
Assume a unitary operator $U$ exists such that for any state $|\psi\rangle$ and blank state $|0\rangle$:
$$U(|\psi\rangle |0\rangle) = |\psi\rangle |\psi\rangle$$
For two distinct non-orthogonal states $|\psi\rangle$ and $|\phi\rangle$:
$$\langle \psi | \phi \rangle = \langle \psi | \langle 0 | U^\dagger U | \phi \rangle |0\rangle = (\langle \psi | \langle \psi |)(|\phi\rangle |\phi\rangle) = (\langle \psi | \phi \rangle)^2$$
This implies $\langle \psi | \phi \rangle - (\langle \psi | \phi \rangle)^2 = 0 \implies \langle \psi | \phi \rangle \in \{0, 1\}$.
Hence, cloning is only possible if the states are identical or completely orthogonal. General quantum states cannot be cloned!",
                    25, "Master"),

                new("L_QC_4", "M_QC", "Quantum Teleportation & Superdense Coding",
                    "Transmit arbitrary quantum states using shared entanglement and classical communication channels.",
                    @"# Quantum Teleportation & Superdense Coding

Quantum Teleportation transmits an unknown qubit $|\psi\rangle = \alpha|0\rangle + \beta|1\rangle$ from Alice to Bob by consuming one pre-shared Bell pair and sending two classical bits.

---

## 1. The Teleportation Protocol
1. **Shared Entanglement**: Alice and Bob share Bell pair $|\Phi^+\rangle_{AB} = \frac{|00\rangle + |11\rangle}{\sqrt{2}}$. Alice holds $|\psi\rangle_C$ and qubit $A$.
2. **Total State (3 Qubits)**:
$$|\Psi_{\text{total}}\rangle = (\alpha|0\rangle + \beta|1\rangle)_C \otimes \frac{|00\rangle + |11\rangle}{\sqrt{2}}_{AB}$$
3. **Bell State Measurement**: Alice performs CNOT between $C$ (control) and $A$ (target), then applies $H$ to $C$, and measures both qubits in the computational basis.
4. **Classical Transmission**: Alice sends the 2 classical bits $M_1, M_2$ to Bob.
5. **Reconstruction**: Bob applies Pauli gates based on the classical bits:
   - `00`: Do nothing ($I$)
   - `01`: Apply Pauli-X ($X$)
   - `10`: Apply Pauli-Z ($Z$)
   - `11`: Apply $ZX$
Bob's qubit is now identically $|\psi\rangle$! Notice: The original state in Alice's possession is destroyed by measurement, fully adhering to the No-Cloning Theorem.

---

## 2. Superdense Coding
The reciprocal protocol: Alice transmits **two classical bits** of information to Bob by sending only **one physical qubit**, provided they share a pre-existing Bell pair.",
                    25, "Master"),

                new("L_QC_5", "M_QC", "Grover's Quantum Search Algorithm",
                    "Achieve quadratic speedup O(sqrt(N)) for searching unsorted databases via quantum amplitude amplification.",
                    @"# Grover's Quantum Search Algorithm

Searching an unsorted database of $N = 2^n$ items classically requires $\mathcal{O}(N)$ checks in the worst case. **Grover's Algorithm** finds the target element in only $\mathcal{O}(\sqrt{N})$ evaluations.

---

## 1. Algorithm Architecture
1. **State Initialization**: Prepare $n$ qubits in uniform superposition:
$$|s\rangle = H^{\otimes n} |0\rangle^{\otimes n} = \frac{1}{\sqrt{N}} \sum_{x=0}^{N-1} |x\rangle$$

2. **The Oracle ($U_\omega$)**: Marks the target state $|\omega\rangle$ by inverting its quantum phase:
$$U_\omega |x\rangle = (-1)^{f(x)} |x\rangle = \begin{cases} -|x\rangle & \text{if } x = \omega \\ |x\rangle & \text{if } x \neq \omega \end{cases}$$

3. **The Diffusion Operator ($U_s$)**: Performs an inversion about the mean amplitude:
$$U_s = 2|s\rangle\langle s| - I = H^{\otimes n}(2|0\rangle^{\otimes n}\langle 0|^{\otimes n} - I)H^{\otimes n}$$

4. **Iteration**: Repeat the Grover step $G = U_s U_\omega$ approximately:
$$R \approx \frac{\pi}{4}\sqrt{N} \text{ times}$$

5. **Measurement**: Measure the register in the computational basis. The target state $|\omega\rangle$ is observed with near $100\%$ probability!",
                    25, "Master"),

                new("L_QC_6", "M_QC", "Shor's Algorithm & Post-Quantum Cryptography",
                    "Examine polynomial-time integer factorization via Quantum Fourier Transform and the transition to lattice-based cryptography.",
                    @"# Shor's Algorithm & Post-Quantum Cryptography

Peter Shor's 1994 algorithm solves the discrete logarithm and integer factorization problems in **polynomial time** $\mathcal{O}((\log N)^3)$, rendering classical public-key cryptography (RSA, Diffie-Hellman, ECC) vulnerable once fault-tolerant quantum computers emerge.

---

## 1. The Core Mechanism: Period Finding
Factoring $N = p \cdot q$ reduces classically to finding the period $r$ of the modular exponential function:
$$f(x) = a^x \pmod N$$
where $\gcd(a, N) = 1$. The period $r$ satisfies $a^r \equiv 1 \pmod N$. Once $r$ is known (and even):
$$a^r - 1 \equiv 0 \pmod N \implies (a^{r/2} - 1)(a^{r/2} + 1) \equiv 0 \pmod N$$
Computing $\gcd(a^{r/2} \pm 1, N)$ using Euclid's classical algorithm reveals the non-trivial prime factors $p$ and $q$!

---

## 2. Quantum Fourier Transform (QFT)
The Quantum Fourier Transform maps computational basis states $|j\rangle$ to frequency states:
$$\text{QFT}|j\rangle = \frac{1}{\sqrt{2^n}} \sum_{k=0}^{2^n-1} e^{2\pi i j k / 2^n} |k\rangle$$
Constructed from single-qubit Hadamard gates and controlled phase-rotation gates $R_k$. QFT extracts the period $r$ with high probability in $\mathcal{O}(n^2)$ gate operations.

---

## 3. The Post-Quantum Cryptography (PQC) Era
To resist quantum cryptanalysis, NIST standardized quantum-resistant algorithms based on mathematical hard problems:
- **CRYSTALS-Kyber (ML-KEM)**: Key Encapsulation based on Learning With Errors (LWE) over module lattices.
- **CRYSTALS-Dilithium (ML-DSA)**: Digital Signature Algorithm based on lattice vectors.
- **SPHINCS+ (SLH-DSA)**: Stateless hash-based signatures relying strictly on cryptographic hash function collision resistance.",
                    30, "Cutting-Edge"),

                // =========================================================================
                // DIST101: DISTRIBUTED SYSTEMS & CONSENSUS (6 LESSONS)
                // =========================================================================
                new("L_DIST_1", "M_DIST", "Distributed Time, Lamport Clocks & Vector Clocks",
                    "Understand time synchronization anomalies, physical clock drift, Lamport logical timestamps, and vector clocks for partial ordering.",
                    @"# Distributed Time, Lamport Clocks & Vector Clocks

In distributed systems, physical quartz clocks drift due to temperature and voltage fluctuations. Network latency prevents nodes from sharing a synchronized physical clock without skew ($\pm 1-10\text{ ms}$ even with NTP).

---

## 1. Leslie Lamport's 'Happened-Before' Relation ($\to$)
The relation $a \to b$ defines causal ordering:
1. If $a$ and $b$ are events in the same process, and $a$ occurs before $b$, then $a \to b$.
2. If $a$ is the sending of a message and $b$ is the receipt of that message, then $a \to b$.
3. If $a \to b$ and $b \to c$, then $a \to c$ (transitivity).
If neither $a \to b$ nor $b \to a$, the events are **concurrent** ($a \parallel b$).

---

## 2. Lamport Logical Timestamps
Each process $P_i$ maintains a scalar counter $C_i$:
- Before executing an event: $C_i = C_i + 1$
- When sending message $m$, attach timestamp $T_m = C_i$
- When receiving message $(m, T_m)$, update:
$$C_j = \max(C_j, T_m) + 1$$
**Limitation**: $a \to b \implies C(a) < C(b)$, but $C(a) < C(b)$ **does not imply** $a \to b$ (cannot distinguish concurrency from causality).

---

## 3. Vector Clocks
Each process in a system of $N$ nodes maintains an array $V[1..N]$:
- Local event in $P_i$: $V_i[i] = V_i[i] + 1$
- Message send: Attach full vector $V_i$
- Message receive with vector $V_m$:
$$V_j[k] = \max(V_j[k], V_m[k]) \quad \forall k \in [1..N], \quad V_j[j] = V_j[j] + 1$$
**Guarantee**: $a \to b \iff V(a) < V(b)$. If neither $V(a) \le V(b)$ nor $V(b) \le V(a)$, then $a \parallel b$. Used in Amazon DynamoDB and Git DAG history tracking.",
                    25, "Master"),

                new("L_DIST_2", "M_DIST", "CAP Theorem, PACELC & Consistency Models",
                    "Analyze trade-offs between consistency, availability, and partition tolerance, plus CRDT state merging.",
                    @"# CAP Theorem, PACELC & Consistency Models

The CAP theorem proves that in an asynchronous network subject to partitions, a distributed datastore can guarantee at most two of three properties.

---

## 1. The CAP Theorem (Brewer / Gilbert & Lynch)
- **Consistency (Linearizability)**: Every read receives the most recent write or an error.
- **Availability**: Every non-failing node returns a non-error response for every request.
- **Partition Tolerance**: The system continues to operate despite arbitrary dropped or delayed messages between nodes.

In any real-world network, network partitions ($P$) are inevitable due to fiber cuts, switch reboots, or firewall misconfigurations. Therefore, distributed architectures must choose between **CP** (Consistency under Partition) or **AP** (Availability under Partition).

---

## 2. The PACELC Theorem (Daniel Abadi)
Extends CAP to describe behavior under normal operating conditions:
- **If Partition ($P$)**: Choose between Availability ($A$) or Consistency ($C$).
- **Else ($E$)**: Choose between Latency ($L$) or Consistency ($C$).

Examples:
- **CockroachDB / Google Spanner**: PC/EC (Strongly consistent always, higher latency).
- **Cassandra / DynamoDB**: PA/EL (Available during partitions, lowest latency in normal state).

---

## 3. Conflict-Free Replicated Data Types (CRDTs)
CRDTs guarantee eventual consistency mathematically without coordination locks:
- **State-based (CvRDT)**: Replicas periodically send their entire state and merge via a Join-Semilattice operation ($\sqcup$) that is commutative, associative, and idempotent.
- **Operation-based (CmRDT)**: Replicas broadcast concurrent operations over a causal delivery transport.",
                    25, "Master"),

                new("L_DIST_3", "M_DIST", "Raft Consensus Protocol & Leader Elections",
                    "Deconstruct Raft: randomized election timeouts, heartbeat RPCs, log replication, and split-brain prevention.",
                    @"# Raft Consensus Protocol & Leader Elections

Raft is a distributed consensus algorithm designed for understandability, dividing consensus into three subproblems: Leader Election, Log Replication, and Safety.

---

## 1. Node Roles & Lifecycle
Every node exists in one of three states:
1. **Follower**: Passive; only responds to incoming RPCs from Candidates or the Leader.
2. **Candidate**: Initiates election after an election timeout expires.
3. **Leader**: Handles all client read/write requests and replicates logs.

```
       [ Follower ]
         |       ^
Timeout  |       | Discovers higher term / valid leader
         v       |
       [ Candidate ]
         |
Wins     |
Majority |
         v
       [ Leader ]
```

---

## 2. Leader Election
- Nodes use **randomized election timeouts** (e.g., 150ms - 300ms) to prevent split votes.
- When timeout expires, node increments its `currentTerm`, transitions to Candidate, votes for itself, and broadcasts `RequestVote` RPCs.
- Candidate becomes Leader if it receives votes from a strict quorum majority:
$$\text{Quorum} = \left\lfloor \frac{N}{2} \right\rfloor + 1$$
In a 5-node cluster, Quorum is $\lfloor 5/2 \rfloor + 1 = 3$ nodes.

---

## 3. Log Replication & Commit Safety
- Leader appends new command to local log, sends `AppendEntries` RPCs to followers.
- When an entry is replicated on a majority of nodes, the Leader commits it and applies it to its state machine.
- **Election Safety Rule**: A follower will deny a vote if the candidate's log is less up-to-date than its own log (evaluated by `lastLogTerm`, then `lastLogIndex`).",
                    25, "Master"),

                new("L_DIST_4", "M_DIST", "Paxos & Multi-Paxos Deep-Dive",
                    "Master Leslie Lamport's foundational consensus protocol: Phase 1 Prepare/Promise and Phase 2 Accept/Accepted.",
                    @"# Paxos & Multi-Paxos Deep-Dive

Paxos is the foundational consensus protocol underlying systems like Google Chubby and Apache ZooKeeper.

---

## 1. Single-Decree Paxos
Reaches agreement on a single proposed value among a set of unreliable processors:
- **Proposers**: Propose values from clients.
- **Acceptors**: Form the quorum consensus memory.
- **Learners**: Read the agreed-upon value.

### Two-Phase Execution:
1. **Phase 1a (Prepare)**: Proposer chooses unique, monotonic proposal number $n$ and broadcasts `Prepare(n)` to a majority of Acceptors.
2. **Phase 1b (Promise)**: If $n > \text{all previously promised } n$, Acceptor responds with `Promise(n, max_accepted_n, max_accepted_val)`.
3. **Phase 2a (Accept)**: If Proposer receives promises from a majority, it chooses $v$ (the value with the highest proposal number among responses, or its own value if none), and sends `Accept(n, v)`.
4. **Phase 2b (Accepted)**: Acceptor accepts $(n, v)$ if it has not promised to ignore $n$. If a majority accepts, the value is permanently decided!

---

## 2. Multi-Paxos Optimization
Single-Decree Paxos requires 2 round-trips for every write. Multi-Paxos elects a stable leader once, skipping Phase 1 for subsequent log instances. This reduces latency to a single round-trip (`Accept` -> `Accepted`), achieving optimal throughput for production distributed state machines.",
                    25, "Master"),

                new("L_DIST_5", "M_DIST", "Byzantine Fault Tolerance & Gossip Protocols",
                    "Handle malicious node attacks with PBFT (3f+1 quorum) and explore peer-to-peer epidemic gossip replication.",
                    @"# Byzantine Fault Tolerance & Gossip Protocols

Crash fault tolerant (CFT) protocols (Raft, Paxos) assume nodes only fail by stopping. **Byzantine Fault Tolerance (BFT)** handles arbitrary, malicious, or compromised nodes that send conflicting or forged messages.

---

## 1. The Byzantine Generals Problem & Quorum Bounds
To tolerate $f$ Byzantine (traitorous) nodes in a synchronous or partially synchronous network:
$$N \ge 3f + 1$$
- To tolerate $1$ malicious node, a minimum of $4$ nodes is mathematically required.
- **PBFT (Practical Byzantine Fault Tolerance)**: Castro & Liskov (1999) protocol utilizing cryptographic signatures with three phases:
  1. `Pre-Prepare`: Primary announces sequence number.
  2. `Prepare`: Replicas verify and broadcast agreement.
  3. `Commit`: Replicas broadcast when $2f+1$ valid prepares are received.

---

## 2. Gossip Protocols (Epidemic Dissemination)
Used in Apache Cassandra and Consul for cluster membership and failure detection:
- Each node periodically selects $k$ random peer nodes and shares cluster state.
- Spreads state with logarithmic time complexity $\mathcal{O}(\log N)$.
- **Anti-Entropy**: Compares Merkle trees (cryptographic hash trees) between nodes to identify and sync differing data ranges in minimum network transfers.",
                    25, "Master"),

                new("L_DIST_6", "M_DIST", "Distributed Transactions: 2PC, Saga & Event Sourcing",
                    "Coordinate atomic updates across microservices using Two-Phase Commit, Sagas with compensating actions, and CQRS.",
                    @"# Distributed Transactions: 2PC, Saga & Event Sourcing

Traditional ACID transactions rely on database locks. In distributed microservice architectures with separate databases, cross-service transactions require distinct coordination patterns.

---

## 1. Two-Phase Commit (2PC)
A centralized Coordinator orchestrates across participants:
1. **Phase 1 (Prepare)**: Coordinator sends `PREPARE`. Participants acquire local locks, write to undo/redo logs, and vote `YES` or `NO`.
2. **Phase 2 (Commit)**: If all vote `YES`, Coordinator sends `COMMIT`. If any vote `NO` or timeout occurs, Coordinator sends `ABORT`.
- **Pitfall**: 2PC is a blocking protocol! If the coordinator crashes after participants vote `YES`, participants remain locked indefinitely.

---

## 2. The Saga Pattern
Decomposes a distributed transaction into a sequence of local transactions:
$$T_1 \to T_2 \to T_3 \to \dots \to T_n$$
Each step $T_i$ has a corresponding **Compensating Transaction** $C_i$ that reverses its side effects (e.g., refunding a credit card if inventory reservation fails):
$$\text{If } T_3 \text{ fails} \implies C_2 \to C_1 \text{ executed in reverse order}$$

- **Orchestrated Saga**: Central saga orchestrator state machine directs services.
- **Choreographed Saga**: Services react to asynchronous domain events published via Apache Kafka or RabbitMQ.

---

## 3. Event Sourcing & CQRS
- **Event Sourcing**: Instead of storing mutable current state, persist an append-only log of immutable domain events (`OrderCreated`, `PaymentAuthorized`).
- **CQRS (Command Query Responsibility Segregation)**: Separates read and write models, allowing high-scale reads via denormalized read-projected databases.",
                    30, "Cutting-Edge"),

                // =========================================================================
                // KRN101: LINUX KERNEL INTERNALS & eBPF (6 LESSONS)
                // =========================================================================
                new("L_KRN_1", "M_KRN", "Kernel Space, Syscall Dispatch & Interrupt Handling",
                    "Understand privilege rings, hardware Interrupt Descriptor Tables, top/bottom halves, and syscall entry mechanisms.",
                    @"# Kernel Space, Syscall Dispatch & Interrupt Handling

Modern CPUs enforce hardware isolation via Privilege Rings. The x86-64 architecture defines Ring 0 (Kernel Mode) and Ring 3 (User Mode).

---

## 1. Privilege Separation & System Call Dispatch
User-space applications cannot execute privileged hardware instructions (`cli`, `in`, `out`, `mov cr3`). To request OS services, applications trigger a **System Call** via the `syscall` instruction:
1. User space loads system call number into register `%rax` (e.g., `1` for `sys_write` on Linux x86-64).
2. Arguments are placed in registers `%rdi`, `%rsi`, `%rdx`, `%r10`, `%r8`, `%r9`.
3. CPU transitions from Ring 3 to Ring 0, jumping to the entry point stored in the `MSR_LSTAR` (Model-Specific Register).
4. Kernel executes `entry_SYSCALL_64`, saves registers to the kernel stack (`struct pt_regs`), indexes `sys_call_table`, and invokes the kernel handler.
5. On completion, `sysretq` restores user state and returns to Ring 3.

---

## 2. Interrupt Handling: Top vs Bottom Halves
When peripheral hardware (NIC, NVMe controller) signals the CPU, an interrupt is triggered via the APIC:
- **Top Half (Hard IRQ)**: Executes with interrupts disabled. Acknowledges hardware, reads critical device state into memory, and schedules deferred work. Must finish in microseconds!
- **Bottom Half (Deferred Work)**: Executes with interrupts enabled:
  - **SoftIRQs**: High-performance core kernel subsystems (Network RX/TX, block devices).
  - **Tasklets**: Dynamically allocated SoftIRQs bound to a single CPU.
  - **Workqueues**: Kernel threads running in process context, capable of sleeping and acquiring mutexes.",
                    25, "Master"),

                new("L_KRN_2", "M_KRN", "Virtual Memory, Paging & Page Fault Handling",
                    "Master multi-level 4-level/5-level page tables, TLB shootdowns, kswapd reclamation, and the OOM killer.",
                    @"# Virtual Memory, Paging & Page Fault Handling

Linux maps 64-bit virtual addresses to physical RAM frames through multi-level hierarchical page tables managed by the hardware Memory Management Unit (MMU).

---

## 1. 4-Level Page Table Traversal
On x86-64, a 48-bit virtual address is partitioned into:
- Bits 47-39: Page Global Directory (PGD) index (9 bits)
- Bits 38-30: Page Upper Directory (PUD) index (9 bits)
- Bits 29-21: Page Middle Directory (PMD) index (9 bits)
- Bits 20-12: Page Table Entry (PTE) index (9 bits)
- Bits 11-0: Byte offset within physical 4 KB frame (12 bits)

```
Virtual Address [47..0]
  [PGD: 9] -> [PUD: 9] -> [PMD: 9] -> [PTE: 9] -> [4KB Physical Frame + Offset: 12]
```
The base address of the active process's PGD is stored in CPU register `CR3`. During context switches, updating `CR3` switches the virtual address space.

---

## 2. Page Fault Handling (`do_page_fault`)
When an instruction accesses an unmapped virtual address, the MMU fires interrupt vector 14 (Page Fault):
1. CPU saves faulting virtual address to register `CR2`.
2. Kernel looks up virtual memory area (`struct vm_area_struct`) in the process `mm_struct`.
3. If address is invalid: kernel issues `SIGSEGV` (Segmentation Fault).
4. If address is valid but not loaded (Demand Paging / Anonymous zero-fill / Swap):
   - Allocates physical page from slab/buddy allocator.
   - Reads data from disk backing store if necessary.
   - Updates PTE with physical frame address and valid bit.
   - CPU retries the faulting instruction transparently!

---

## 3. OOM Killer (Out-Of-Memory)
When free physical memory drops below the emergency threshold and `kswapd` cannot reclaim enough pages, `out_of_memory()` computes badness scores for all processes based on RAM consumption and `oom_score_adj`, terminating the highest-scoring process with `SIGKILL`.",
                    25, "Master"),

                new("L_KRN_3", "M_KRN", "Virtual File System (VFS), Inodes & Buffer Cache",
                    "Deconstruct VFS objects (superblocks, inodes, dentries, files) and the unified page cache writeback engine.",
                    @"# Virtual File System (VFS), Inodes & Buffer Cache

The Linux Virtual File System (VFS) provides a unified abstraction layer over disparate filesystems (ext4, XFS, btrfs, NFS, procfs).

---

## 1. Core VFS Abstractions
Every mounted filesystem implements four fundamental object types:
1. **Superblock**: Describes the entire mounted filesystem (block size, total inodes, filesystem flags, dirty status).
2. **Inode**: Represents a specific file or directory on storage. Stores file metadata (size, permissions, timestamps, block pointers). **Does not contain the filename!**
3. **Dentry (Directory Entry)**: Connects directory paths to Inodes, forming the hierarchical tree (`/home/user/code`). Cached aggressively in the `dcache`.
4. **File Object**: Represents an open file instance created when a process calls `sys_open()`. Tracks current file read/write offset (`f_pos`) and access modes.

---

## 2. Hard Links vs Symbolic Links
- **Hard Link**: A new dentry pointing to an existing inode number (`inode->i_nlink++`). If the original filename is deleted, data persists until `i_nlink == 0`. Cannot cross filesystem boundaries.
- **Symbolic Link**: A distinct file whose data payload contains the text string of another target path.

---

## 3. The Unified Page Cache
File I/O does not write directly to disk:
- `read()` checks the Page Cache. On cache hit, data copies directly to user buffer without disk access.
- `write()` marks pages as 'dirty' in RAM and returns immediately (`O_DIRECT` or `fsync()` forces synchronous flush).
- Kernel background threads (`flushers` / `pdflush`) write dirty pages asynchronously to disk storage.",
                    25, "Master"),

                new("L_KRN_4", "M_KRN", "eBPF Architecture, Verifier & BPF Maps",
                    "Execute verified sandboxed bytecode inside the Linux kernel without recompiling kernel modules.",
                    @"# eBPF Architecture, Verifier & BPF Maps

Extended Berkeley Packet Filter (eBPF) transforms the Linux kernel into a programmable runtime, executing sandboxed bytecode in response to kernel events.

---

## 1. eBPF Virtual Machine Architecture
- **Registers**: 11 64-bit registers: `r0` (return value), `r1`-`r5` (arguments), `r6`-`r9` (callee-saved), `r10` (read-only frame pointer).
- **Instruction Set**: 64-bit RISC architecture supporting arithmetic, jump, load/store, and helper function calls.
- **JIT Compilation**: Converts eBPF bytecode into native machine instructions (x86-64 / ARM64) at load time.

```
C Source Code (Clang/LLVM) 
       |
       v
eBPF Bytecode (.o)
       |
bpf() syscall -> [ In-Kernel Verifier ] -> [ JIT Compiler ] -> [ Hook Execution ]
                         |
                 (Checks Safety)
```

---

## 2. The In-Kernel Static Verifier
Before any eBPF program is loaded, the kernel verifier proves memory and execution safety:
- Explores all possible execution paths (Directed Acyclic Graph analysis).
- Proves the program **cannot crash the kernel**:
  - No dereferencing uninitialized memory or out-of-bounds pointers.
  - No unbounded loops (ensures termination).
  - Maximum instruction limit (1 million instructions for privileged users).

---

## 3. BPF Maps: Kernel-to-User Communication
eBPF programs cannot call arbitrary user-space libraries. They share state using **BPF Maps**:
- `BPF_MAP_TYPE_HASH`: Key-value lookup tables.
- `BPF_MAP_TYPE_ARRAY`: Fast indexed arrays.
- `BPF_MAP_TYPE_RINGBUF`: Lock-free multi-producer single-consumer ring buffer for streaming events to user space.",
                    25, "Master"),

                new("L_KRN_5", "M_KRN", "High-Performance Networking with XDP",
                    "Process 40+ million packets per second via eXpress Data Path directly in the network device driver.",
                    @"# High-Performance Networking with XDP

Standard Linux network processing allocates a `struct sk_buff` (socket buffer) for every incoming packet, traversing the entire TCP/IP stack. **XDP (eXpress Data Path)** intercepts packets at the lowest possible layer—inside the network device driver before memory allocation.

---

## 1. XDP Packet Processing Pipeline
When a network interface card (NIC) receives a frame via DMA:
1. Driver rings trigger XDP hook.
2. eBPF program receives raw memory pointer `struct xdp_md` containing `data` and `data_end`.
3. Program inspects Ethernet, IP, and TCP/UDP headers.
4. Program returns an immediate XDP action code:
   - `XDP_DROP`: Discards the packet instantly at the NIC driver layer (ideal for multi-terabit DDoS mitigation).
   - `XDP_TX`: Bounces the packet back out the same interface (line-rate load balancing).
   - `XDP_REDIRECT`: Forwards packet directly to another NIC or to user space via `AF_XDP`.
   - `XDP_PASS`: Passes packet up to standard Linux TCP/IP kernel stack.

---

## 2. AF_XDP Zero-Copy Sockets
`AF_XDP` bypasses the kernel network stack, streaming raw Ethernet packets directly into user-space memory buffers (UMEM) via shared memory ring buffers, achieving line-rate processing with minimal CPU utilization.",
                    25, "Master"),

                new("L_KRN_6", "M_KRN", "Kernel Tracing with bpftrace, BCC & FlameGraphs",
                    "Observe production systems with dynamic kprobes, uprobes, tracepoints, and CPU profiling FlameGraphs.",
                    @"# Kernel Tracing with bpftrace, BCC & FlameGraphs

Dynamic tracing allows deep observability of running production Linux systems without modifying binaries or rebooting.

---

## 1. Trace Points & Probes
- **Kernel Tracepoints**: Static, stable instrumentation points embedded by kernel developers in kernel source code (e.g., `sched:sched_switch`, `net:netif_receive_skb`). Zero overhead when inactive.
- **Kprobes / Kretprobes**: Dynamic kernel instrumentation that splices a breakpoint instruction into the entry or exit of almost any kernel function.
- **Uprobes / Uretprobes**: Dynamic user-space probes attached to binaries (e.g., tracing `malloc` in `libc.so` or HTTP request parsing in NGINX).

---

## 2. One-Liners with `bpftrace`
`bpftrace` provides an expressive awk-like DSL:

```bash
# Measure read() syscall latency distribution:
bpftrace -e 'kprobe:vfs_read { @start[tid] = nsecs; } 
             kretprobe:vfs_read /@start[tid]/ { 
                 @latency_us = hist((nsecs - @start[tid]) / 1000); 
                 delete(@start[tid]); 
             }'

# Count syscalls by process name:
bpftrace -e 'tracepoint:raw_syscalls:sys_enter { @[comm] = count(); }'
```

---

## 3. Brendan Gregg's CPU Flame Graphs
Flame Graphs visualize hierarchical stack traces sampled periodically (e.g., 99 Hz):
- X-axis: Population of samples sorted alphabetically (width indicates CPU time spent).
- Y-axis: Stack depth from bottom (root callers) to top (active leaf functions). Identifies CPU hotspots at a glance.",
                    30, "Cutting-Edge"),

                // =========================================================================
                // HFT101: ULTRA-LOW-LATENCY SYSTEMS & HFT (6 LESSONS)
                // =========================================================================
                new("L_HFT_1", "M_HFT", "CPU Cache Lines, Memory Hierarchies & False Sharing",
                    "Optimize memory access for L1/L2/L3 caches, prevent cache-line bouncing, and enforce MESI protocol coherence.",
                    @"# CPU Cache Lines, Memory Hierarchies & False Sharing

In modern high-performance systems, RAM access is orders of magnitude slower than CPU computation. Performance is dictated by cache efficiency.

---

## 1. The Memory Latency Hierarchy
Typical latency costs on modern x86-64 CPUs:
- **CPU Registers**: 1 clock cycle (~0.25 ns)
- **L1 Cache (32-64 KB per core)**: 4-5 cycles (~1 ns)
- **L2 Cache (512 KB - 1 MB per core)**: 12-14 cycles (~3 ns)
- **L3 Cache (Shared LLC 32-128 MB)**: 40-60 cycles (~12 ns)
- **Main DDR5 RAM**: 150-250 cycles (~50-80 ns)
- **PCIe / NVMe / Network**: Microseconds (1,000+ ns)

A single L3 cache miss that goes to RAM costs hundreds of wasted clock cycles!

---

## 2. The 64-Byte Cache Line & False Sharing
CPUs transfer memory between RAM and caches in atomic units called **Cache Lines** (almost universally 64 bytes).

### The False Sharing Bug
Occurs when two independent threads running on separate CPU cores modify distinct variables that happen to reside in the **same 64-byte cache line**:
```c
struct ThreadData {
    uint64_t counter1; // Modified by Thread 1 on Core 0
    uint64_t counter2; // Modified by Thread 2 on Core 1
};
```
Because both variables share the same cache line, the CPU's **MESI cache coherence protocol** continuously invalidates the cache line between cores (Cache Bouncing / Ping-Pong), degrading throughput by up to 20x!

### Resolution: Cache Line Padding
```c
struct alignas(64) ThreadDataPadded {
    uint64_t counter1;
    char pad[56]; // Pad to full 64 bytes
    uint64_t counter2;
};
```",
                    25, "Master"),

                new("L_HFT_2", "M_HFT", "Lock-Free Programming & Atomic Memory Ordering",
                    "Eliminate OS mutex locks with Compare-And-Swap (CAS), memory barriers, and acquire-release semantics.",
                    @"# Lock-Free Programming & Atomic Memory Ordering

Operating system mutexes (`pthread_mutex_lock`, C# `lock`) rely on kernel context switches when contention occurs, adding 2,000 to 10,000 nanoseconds of latency. Low-latency systems use **Lock-Free Concurrency**.

---

## 1. Hardware Atomic Primitives
The x86-64 `LOCK CMPXCHG` instruction performs an atomic **Compare-And-Swap (CAS)**:
```c
bool compare_and_swap(uint64_t* ptr, uint64_t expected, uint64_t desired) {
    // Atomically: if (*ptr == expected) { *ptr = desired; return true; } else return false;
}
```

### The ABA Problem
Thread 1 reads value $A$. Thread 2 changes $A \to B \to A$. Thread 1 executes CAS, observes value $A$, and erroneously succeeds even though the underlying data structure was modified.
**Solution**: Tagged pointers / Versioned references combining a 64-bit counter with the 64-bit pointer (`CMPXCHG16B`).

---

## 2. Memory Ordering Models
CPUs and optimizing compilers reorder memory reads and writes to maximize pipelining. C++11 and modern languages define fine-grained memory orders:
1. `memory_order_relaxed`: Guarantees atomicity only; no ordering constraints.
2. `memory_order_acquire`: Prevents reads and writes from being reordered *before* this operation (used when reading flags).
3. `memory_order_release`: Prevents reads and writes from being reordered *after* this operation (used when publishing data).
4. `memory_order_seq_cst`: Full sequentially consistent order with memory barriers (`MFENCE`).",
                    25, "Master"),

                new("L_HFT_3", "M_HFT", "The LMAX Disruptor & Zero-Copy Ring Buffers",
                    "Understand mechanical sympathy, pre-allocated bounded ring buffers, and lock-free sequence barriers.",
                    @"# The LMAX Disruptor & Zero-Copy Ring Buffers

Created by LMAX Exchange to process millions of financial orders per second with sub-microsecond latency, the Disruptor pattern replaces traditional linked-node blocking queues.

---

## 1. Why Traditional Blocking Queues Fail
1. **Garbage Collection / Memory Allocation**: `Queue.push(new Node())` causes constant heap allocations and cache misses.
2. **Lock Contention**: Head and tail pointers require mutexes or CAS contention.
3. **Cache Invalidation**: Head and tail pointers frequently cause false sharing.

---

## 2. The Disruptor Architecture
The Disruptor uses a pre-allocated, power-of-2 sized continuous circular array:

```
[Slot 0] [Slot 1] [Slot 2] [Slot 3] ... [Slot N-1]
   ^
   |--- Indexed by: sequence & (buffer_size - 1)
```

- **Zero Allocation**: Buffer elements are instantiated at startup and reused infinitely.
- **Single Writer Principle**: A single thread increments a 64-bit sequence counter monotonically with zero locks or contention.
- **Sequence Barriers**: Consumer threads poll sequence counters using memory barriers without blocking the producer thread.",
                    25, "Master"),

                new("L_HFT_4", "M_HFT", "Kernel Bypass: DPDK & Solarflare OpenOnload",
                    "Bypass the OS kernel network stack to read packets directly from the NIC in hundreds of nanoseconds.",
                    @"# Kernel Bypass: DPDK & Solarflare OpenOnload

The traditional Linux network stack incurs:
1. Context switches from user mode to kernel mode.
2. Memory copies between kernel socket buffers (`sk_buff`) and user buffers.
3. Interrupt processing latency and CPU core rescheduling.

**Kernel Bypass** maps the network interface card's PCIe registers and DMA buffers directly into the user application's memory space.

---

## 1. Data Plane Development Kit (DPDK)
- Uses Poll Mode Drivers (PMD) that run in a continuous polling loop, eliminating hardware interrupt latency.
- Uses HugePages (2 MB or 1 GB pages) to eliminate TLB cache misses.
- Binds worker threads to dedicated isolated CPU cores (`isolcpus`, `taskset`).

---

## 2. Solarflare EF_VI & OpenOnload
Solarflare NICs support the EF_VI user-space API:
- Application posts receive descriptors directly to the NIC's hardware queues.
- As soon as Ethernet frames arrive on the wire, the NIC DMA-transfers bytes directly into application memory.
- Sub-700 nanosecond tick-to-trade wire-to-wire packet processing!",
                    25, "Master"),

                new("L_HFT_5", "M_HFT", "Limit Order Book (LOB) Matching Engine Architecture",
                    "Build a deterministic price-time priority order book matching engine with nanosecond execution.",
                    @"# Limit Order Book (LOB) Matching Engine Architecture

The core of every financial exchange (NASDAQ, CME, Binance) is the **Matching Engine**, which pairs buy orders (Bids) and sell orders (Asks).

---

## 1. Data Structures for Price-Time Priority
Orders at the same price level are executed in First-Come, First-Served order:
- **Price Levels**: Maintained in an intrusive doubly-linked list or contiguous fixed array for cache locality.
- **Best Bid & Offer (BBO)**: Pointers to the highest Bid price and lowest Ask price.
- **Order Lookup**: Fast hash table or flat direct-indexed array mapping `OrderId` to order node pointers for $\mathcal{O}(1)$ cancellations.

```
       ASKS (Sell Orders)
Level 2: $100.02 -> [Order 4: 50 qty]
Level 1: $100.01 -> [Order 2: 10 qty] -> [Order 3: 20 qty]
------------------------------------------------------------ SPREAD ($0.02)
Level 1: $99.99  -> [Order 1: 100 qty]
Level 2: $99.98  -> [Order 5: 40 qty]
       BIDS (Buy Orders)
```

---

## 2. The Matching Algorithm
When an incoming Limit Buy Order arrives at price $P_{\text{in}}$ with quantity $Q_{\text{in}}$:
1. While $Q_{\text{in}} > 0$ and $P_{\text{in}} \ge \text{BestAsk}$:
   - Match with head order of `BestAsk`.
   - Trade execution executed at maker price.
   - Decrement filled quantities.
2. If remaining $Q_{\text{in}} > 0$: Insert remainder into Bid book at price level $P_{\text{in}}$.",
                    25, "Master"),

                new("L_HFT_6", "M_HFT", "Simple Binary Encoding (SBE) & FPGA Acceleration",
                    "Master zero-copy binary serialization protocols and FPGA gate-array hardware acceleration.",
                    @"# Simple Binary Encoding (SBE) & FPGA Acceleration

Financial protocols like FIX (Financial Information eXchange) historically used ASCII key-value tags (`35=D|49=CLIENT|56=EXCHANGE`). Parsing strings wastes hundreds of CPU cycles.

---

## 1. Simple Binary Encoding (SBE)
SBE serializes data directly in CPU-native binary representation:
- **Direct Memory Access**: Fields are aligned to natural byte boundaries (e.g., 64-bit integers aligned to 8-byte offsets). Reading a field is a simple pointer cast without copying or deserialization:
```c
struct MessageHeader {
    uint16_t blockLength;
    uint16_t templateId;
    uint16_t schemaId;
    uint16_t version;
};
```

---

## 2. FPGA Hardware Acceleration
Trading firms flash hardware logic directly onto Field Programmable Gate Arrays (FPGAs) mounted in PCIe slots:
- Physical network fiber connects directly to the FPGA's SFP+ optical transceivers.
- The FPGA parses Ethernet frames, verifies risk limits in hardware logic gates, and generates trade responses in **less than 50 nanoseconds** before software on the CPU even receives an interrupt!",
                    30, "Cutting-Edge"),

                // =========================================================================
                // COMP101: COMPILER CONSTRUCTION & LLVM IR (6 LESSONS)
                // =========================================================================
                new("L_COMP_1", "M_COMP", "Lexical Analysis & Finite Automata (DFA/NFA)",
                    "Build tokenizers using regular grammars, non-deterministic finite automata, and Thompson's construction.",
                    @"# Lexical Analysis & Finite Automata (DFA/NFA)

The first phase of a compiler, the **Lexer (Tokenizer)**, transforms a stream of characters into a stream of meaningful atomic units called **Tokens**.

---

## 1. Tokens, Lexemes & Patterns
- **Lexeme**: The raw character substring in source code (e.g., `42`, `while`, `myVariable`).
- **Token**: An abstract symbol categorized by type with an optional semantic attribute:
  `Token(TokenType.KEYWORD_WHILE, 'while', line=1, col=5)`

---

## 2. Regular Expressions to NFA: Thompson's Construction
Any regular grammar can be converted into a Non-deterministic Finite Automaton (NFA):
1. **Base Case $\epsilon$**: Start state transitions to accept state on empty input.
2. **Base Case $a$**: Start state transitions to accept state on character $a$.
3. **Concatenation ($ab$)**: Accept state of $a$ connects via $\epsilon$-transition to start state of $b$.
4. **Alternation ($a | b$)**: New start state branches to $a$ and $b$ via $\epsilon$-transitions.
5. **Kleene Star ($a^*$)**: Adds looping $\epsilon$-transitions.

---

## 3. NFA to DFA: The Subset Construction
NFAs can occupy multiple states simultaneously. The **Subset Construction Algorithm** determinizes an NFA into an equivalent Deterministic Finite Automaton (DFA) where every state-transition pair maps to exactly one state, allowing $\mathcal{O}(N)$ linear scan tokenization.",
                    25, "Master"),

                new("L_COMP_2", "M_COMP", "Syntax Analysis & Abstract Syntax Trees (AST)",
                    "Construct recursive descent parsers and produce Abstract Syntax Tree hierarchical representations.",
                    @"# Syntax Analysis & Abstract Syntax Trees (AST)

The Parser verifies that the stream of tokens conforms to the grammar rules of the language (defined via Context-Free Grammars in BNF notation) and constructs an **Abstract Syntax Tree (AST)**.

---

## 1. Context-Free Grammars & Ambiguity
Consider arithmetic expressions:
$$E \to E + E \mid E \times E \mid (E) \mid \text{id}$$
Without precedence rules, `2 + 3 * 4` can be parsed into two distinct parse trees. Grammars resolve ambiguity by establishing operator precedence and associativity:
$$E \to T + E \mid T$$
$$T \to F \times T \mid F$$
$$F \to (E) \mid \text{id}$$

---

## 2. Recursive Descent Parsing
A top-down parsing technique where each non-terminal grammar rule maps to a recursive function:

```csharp
Expr ParseExpression() {
    Expr left = ParseTerm();
    while (Match(TokenType.Plus, TokenType.Minus)) {
        Token op = Previous();
        Expr right = ParseTerm();
        left = new BinaryExpr(left, op, right);
    }
    return left;
}
```

---

## 3. Abstract Syntax Trees (AST)
Unlike concrete parse trees that store every semicolon and parenthesis, the AST retains only essential semantic relationships:

```
        BinaryExpr (+)
        /            \
Literal(2)        BinaryExpr (*)
                  /            \
             Literal(3)     Literal(4)
```",
                    25, "Master"),

                new("L_COMP_3", "M_COMP", "Semantic Analysis & Type Checking Systems",
                    "Enforce lexical scoping, symbol tables, type validation, and Hindley-Milner type inference.",
                    @"# Semantic Analysis & Type Checking Systems

Semantic analysis verifies that the AST obeys language rules that cannot be captured by context-free grammars (e.g., variable declarations before use, type compatibility).

---

## 1. Symbol Tables & Lexical Scoping
The Symbol Table tracks identifiers (variables, functions, classes) across nested scopes:
- Organized as a tree of dictionaries where each scope links to its parent enclosure.
- `Lookup(name)` checks current scope, then traverses upwards to global scope.
- Detects errors like re-declaring variables in the same block or accessing out-of-scope locals.

---

## 2. Type Systems & Checking
The type checker recursively walks the AST, assigning types to expressions:
- For `BinaryExpr(left, op, right)`:
  - If `op == '+'` and both operands are `Int`, result is `Int`.
  - If operands are `String` and `Int`, emit compile error: `Type mismatch: cannot add Int to String`.

---

## 3. Hindley-Milner Type Inference
Used in modern languages (Rust, Haskell, ML) to automatically deduce the most general types of expressions without explicit annotations, utilizing unification (Robinson's unification algorithm) and type variables.",
                    25, "Master"),

                new("L_COMP_4", "M_COMP", "Intermediate Representations & SSA Form",
                    "Transform ASTs into Static Single Assignment form, Control Flow Graphs, and phi-nodes.",
                    @"# Intermediate Representations & SSA Form

Modern compilers decouple frontend parsing from backend target architectures by translating ASTs into an architecture-independent **Intermediate Representation (IR)**.

---

## 1. Static Single Assignment (SSA) Form
In SSA form, **every variable is assigned a value exactly once**, and every variable use refers to a single definition:

### Non-SSA:
```c
y = 1;
y = y + 2;
```

### SSA Form:
```c
y1 = 1;
y2 = y1 + 2;
```

---

## 2. The $\phi$ (Phi) Function
When multiple control flow paths converge (e.g., after an `if-else` statement), the compiler inserts a pseudo-function $\phi$ that selects the appropriate definition depending on which branch executed:

```c
if (condition) {
    x1 = 10;
} else {
    x2 = 20;
}
x3 = phi(x1, x2);
```

SSA simplifies data-flow analysis, transforming complex variable tracking into explicit Directed Acyclic Graphs.",
                    25, "Master"),

                new("L_COMP_5", "M_COMP", "Compiler Optimization Passes & Loop Invariants",
                    "Implement Dead Code Elimination, Common Subexpression Elimination, and Loop-Invariant Code Motion.",
                    @"# Compiler Optimization Passes & Loop Invariants

Compilers execute iterative optimization passes over the SSA IR to produce faster, smaller machine code.

---

## 1. Key SSA Optimization Passes
1. **Constant Folding & Propagation**: Computes known values at compile time:
   `x = 3 + 4 * 2` $\implies$ `x = 11`.
2. **Common Subexpression Elimination (CSE)**: Identifies identical subcomputations and reuses results:
   `a = x + y; b = x + y` $\implies$ `a = x + y; b = a`.
3. **Dead Code Elimination (DCE)**: Removes computations whose results are never read or have no side effects.

---

## 2. Loop Optimizations
Loops consume the majority of execution cycles in software:
- **Loop-Invariant Code Motion (LICM)**: Hoists calculations that produce the same value on every iteration outside the loop body:
```c
// Before LICM:
for (int i = 0; i < N; i++) { arr[i] = i * (width * 2); }
// After LICM:
int stride = width * 2;
for (int i = 0; i < N; i++) { arr[i] = i * stride; }
```
- **Loop Unrolling**: Duplicates loop bodies to reduce branch instructions and enable vectorization.
- **Auto-Vectorization**: Replaces scalar operations with SIMD instructions (SSE/AVX).",
                    25, "Master"),

                new("L_COMP_6", "M_COMP", "LLVM Infrastructure & Native Code Generation",
                    "Generate native machine code via LLVM IR, instruction selection, and Chaitin-Briggs graph coloring register allocation.",
                    @"# LLVM Infrastructure & Native Code Generation

The LLVM Compiler Infrastructure powers Clang (C/C++), Rustc, Swift, and Julia.

---

## 1. LLVM Intermediate Representation (LLVM IR)
A strongly-typed, infinite-register RISC assembly language:

```llvm
define i32 @factorial(i32 %n) {
entry:
  %cmp = icmp sle i32 %n, 1
  br i1 %cmp, label %base, label %recurse

base:
  ret i32 1

recurse:
  %sub = sub i32 %n, 1
  %call = call i32 @factorial(i32 %sub)
  %res = mul i32 %n, %call
  ret i32 %res
}
```

---

## 2. Register Allocation via Graph Coloring
LLVM IR assumes an infinite number of virtual registers (`%0, %1, %2...`), but physical CPUs have only 16 general-purpose registers (x86-64):
1. **Liveness Analysis**: Computes when each virtual register is created and last read.
2. **Interference Graph**: Nodes represent variables; edges connect variables that are live at the same time.
3. **$K$-Coloring**: If the graph can be colored with $K$ colors (where $K$ is the number of physical registers), no two interfering variables share a register.
4. **Register Spilling**: If graph coloring fails, excess variables are spilled to the stack memory.",
                    30, "Cutting-Edge"),

                // =========================================================================
                // LLM101: MODERN LLM ARCHITECTURE & QUANTIZATION (6 LESSONS)
                // =========================================================================
                new("L_LLM_1", "M_LLM", "Scaled Dot-Product & Multi-Head Self-Attention",
                    "Master the mathematical formulation of Self-Attention, Query/Key/Value projections, and RoPE positional embeddings.",
                    @"# Scaled Dot-Product & Multi-Head Self-Attention

The core breakthrough of the Transformer architecture (Vaswani et al., 2017) is the **Self-Attention Mechanism**, enabling tokens to dynamically relate to all other tokens in a sequence.

---

## 1. Scaled Dot-Product Attention Formula
Given input embeddings, linear projection matrices $W_Q, W_K, W_V$ generate Queries ($Q$), Keys ($K$), and Values ($V$):
$$\text{Attention}(Q, K, V) = \text{softmax}\left(\frac{QK^T}{\sqrt{d_k}} + M\right) V$$

- $QK^T$: Computes the pairwise similarity score between every query and key token.
- $\frac{1}{\sqrt{d_k}}$: Scaling factor preventing dot products from exploding for large hidden dimensions ($d_k$), avoiding vanishing gradients in the softmax.
- $M$: Causal attention mask setting upper-triangular elements to $-\infty$ so tokens cannot attend to future tokens during auto-regressive generation.
- $\text{softmax}$: Normalizes scores into a probability distribution summing to 1.0.

---

## 2. Multi-Head Attention (MHA)
Instead of a single attention function, project queries, keys, and values into $h$ parallel subspace heads:
$$\text{MultiHead}(Q, K, V) = \text{Concat}(\text{head}_1, \dots, \text{head}_h) W_O$$
Allows the model to jointly attend to information at different positions from different representation subspaces (e.g., grammatical syntax, pronouns, factual associations).

---

## 3. Rotary Position Embeddings (RoPE)
Modern LLMs (Llama 3, Mistral) abandon absolute sinusoidal position encodings in favor of RoPE:
- Rotates the query and key vectors in the 2D complex plane by an angle proportional to token position $m$:
$$R_{\Theta, m}^d = \text{diag}(R_{\theta_1, m}, R_{\theta_2, m}, \dots)$$
- Guarantees that the dot product $q_m^T k_n$ depends purely on the relative distance $(m - n)$, enabling robust context-length extrapolation.",
                    25, "Master"),

                new("L_LLM_2", "M_LLM", "Transformer Decoder Architectures & RoPE",
                    "Explore modern decoder-only designs: Pre-RMSNorm, SwiGLU activation functions, and FlashAttention tiling.",
                    @"# Transformer Decoder Architectures & RoPE

Contemporary state-of-the-art LLMs (Llama 3, DeepSeek, Gemma) adopt a decoder-only architecture with specific structural enhancements.

---

## 1. Pre-Normalization with RMSNorm
Instead of standard LayerNorm which normalizes by mean and variance:
$$\text{RMSNorm}(x) = \frac{x}{\text{RMS}(x)} \odot \gamma, \quad \text{where } \text{RMS}(x) = \sqrt{\frac{1}{d} \sum_{i=1}^d x_i^2 + \epsilon}$$
RMSNorm eliminates mean calculation, saving 7% to 15% compute time per transformer layer while preserving gradient stability.

---

## 2. SwiGLU Activation Function
Replaces standard ReLU or GELU in the Feed-Forward Network (FFN) layers:
$$\text{SwiGLU}(x) = \text{Swish}(x W_1) \otimes (x W_2) \cdot W_3$$
Significantly improves perplexity and factual recall at equivalent parameter counts.

---

## 3. FlashAttention (Dao et al.)
Standard attention writes the $N \times N$ attention matrix to high-bandwidth GPU memory (HBM). For an 8K context, this consumes gigabytes of memory and saturates memory bandwidth.
- **FlashAttention** tiles the softmax computation into blocks, executing entirely inside ultra-fast GPU SRAM cache without materializing the intermediate $N \times N$ attention matrix in HBM, accelerating training by 2-4x.",
                    25, "Master"),

                new("L_LLM_3", "M_LLM", "Parameter-Efficient Fine-Tuning: LoRA & QLoRA",
                    "Fine-tune 70B parameter models on single consumer GPUs using Low-Rank Adaptation and 4-bit NormalFloat (NF4).",
                    @"# Parameter-Efficient Fine-Tuning: LoRA & QLoRA

Full fine-tuning of large foundation models requires updating all weights, requiring massive cluster compute (e.g., fine-tuning a 70B FP16 model requires >700 GB of VRAM for weights, gradients, and optimizer states).

---

## 1. Low-Rank Adaptation (LoRA)
Hypothesis (Aghajanyan et al.): Weight updates during adaptation have a low intrinsic dimension.
For a pre-trained weight matrix $W_0 \in \mathbb{R}^{d \times k}$, freeze $W_0$ and decompose the update $\Delta W$ into two low-rank matrices:
$$W = W_0 + \Delta W = W_0 + \frac{\alpha}{r} (B \cdot A)$$
where $B \in \mathbb{R}^{d \times r}$ and $A \in \mathbb{R}^{r \times k}$, with rank $r \ll \min(d, k)$ (e.g., $r = 8$ or $16$).
- Trainable parameters are reduced by over **99%**!
- At inference time, weights can be merged into $W_0$ with zero latency overhead.

---

## 2. QLoRA (Quantized LoRA)
Introduces three key innovations to train directly on 4-bit quantized base models:
1. **NF4 (NormalFloat 4)**: An information-theoretically optimal quantile quantization data type for normally distributed weights.
2. **Double Quantization**: Quantizes the quantization constants themselves, saving 0.37 bits per parameter.
3. **Paged Optimizers**: Uses CUDA Unified Memory to page memory between GPU VRAM and CPU RAM during peak gradient allocations.",
                    25, "Master"),

                new("L_LLM_4", "M_LLM", "Weight Quantization: GGUF, AWQ & GPTQ",
                    "Compress models from 16-bit floats to 4-bit and 2-bit representations with minimal perplexity degradation.",
                    @"# Weight Quantization: GGUF, AWQ & GPTQ

Quantization compresses model weights from 16-bit floating-point (FP16/BF16) to lower bit-depth representations (INT8, INT4, INT2), enabling massive models to run on edge devices and consumer workstations.

---

## 1. Post-Training Quantization (PTQ) vs Quantization-Aware Training (QAT)
- **PTQ**: Takes an already-trained model and quantizes weights without retraining.
- **Symmetric vs Asymmetric Quantization**:
$$x_q = \text{round}\left(\frac{x}{S}\right) + Z$$
where $S$ is the scale factor and $Z$ is the zero-point offset.

---

## 2. Major Quantization Methodologies
1. **GGUF (llama.cpp)**: CPU-friendly unified file format supporting k-quants (e.g., `Q4_K_M`). Packs weights, tokenizer, and architecture metadata into a single portable binary.
2. **GPTQ (Generalized Post-Training Quantization)**: Second-order Taylor series error compensation utilizing inverse Hessian matrices to minimize output error layer-by-layer.
3. **AWQ (Activation-aware Weight Quantization)**: Recognizes that only ~1% of weights are critical (salient weights that protect activations). Quantizes 99% of weights aggressively while preserving salient weights with higher precision.",
                    25, "Master"),

                new("L_LLM_5", "M_LLM", "High-Throughput Serving & PagedAttention",
                    "Analyze KV-cache memory footprints and eliminate memory fragmentation using vLLM PagedAttention.",
                    @"# High-Throughput Serving & PagedAttention

In LLM serving, generation is bounded by GPU memory bandwidth rather than compute. The primary consumer of dynamic VRAM is the **Key-Value (KV) Cache**.

---

## 1. KV-Cache Memory Calculation
For a model with $L$ layers, hidden dimension $H$, running batch size $B$, sequence length $S$, in FP16 (2 bytes):
$$\text{Memory} = 2 \times 2 \times L \times H \times S \times B \text{ bytes}$$
For Llama 3 70B ($L=80, H=8192$) with 128 concurrent users at 4K context:
$$\text{KV Cache} \approx 4 \times 80 \times 8192 \times 4096 \times 128 \approx 1.37 \text{ Terabytes of VRAM!}$$

---

## 2. Memory Fragmentation & PagedAttention (vLLM)
Traditional serving pre-allocates contiguous memory chunks for maximum sequence lengths, resulting in **60% to 80% memory waste** due to internal and external fragmentation!

### PagedAttention Solution
Inspired by OS virtual memory paging:
- Divides the KV-cache into fixed-size **KV Blocks** (e.g., 16 tokens per block).
- A block table maps logical token positions to non-contiguous physical GPU memory frames.
- Enables **near-zero memory waste (<4%)**, boosting serving throughput by 2x-4x!",
                    25, "Master"),

                new("L_LLM_6", "M_LLM", "Speculative Decoding & Edge Model Inference",
                    "Accelerate auto-regressive generation using speculative draft models and local edge quantization runtimes.",
                    @"# Speculative Decoding & Edge Model Inference

Auto-regressive token generation produces one token per forward pass: generating 100 tokens requires 100 sequential passes through the entire model, severely constrained by memory bandwidth.

---

## 1. Speculative Decoding (Leviathan et al.)
Pairs a large target model (e.g., Llama-3-70B) with an ultra-fast small draft model (e.g., Llama-3-8B):
1. **Draft Phase**: The small draft model quickly generates $K$ candidate tokens auto-regressively: $[x_1, x_2, \dots, x_K]$.
2. **Verification Phase**: The target model runs a **single parallel forward pass** over all $K$ tokens simultaneously, computing exact probability distributions.
3. **Rejection Sampling**: Accept or reject tokens using modified acceptance thresholds:
$$P(\text{accept } x_i) = \min\left(1, \frac{P_{\text{target}}(x_i)}{P_{\text{draft}}(x_i)}\right)$$
If the first $m$ tokens are accepted, the target model generates $m+1$ tokens in the time of a single forward pass!
**Guarantee**: The output distribution is mathematically identical to running the large model alone, with 2x-3x latency reduction.

---

## 2. Edge Deployments with ONNX Runtime & WebGPU
Deploying quantized models locally on consumer PCs using NPU and DirectML acceleration enables completely private, zero-latency inference with zero server operational costs.",
                    30, "Cutting-Edge")
            };
        }

        public static List<QuizSeed> GetAdvancedQuizzes()
        {
            return new List<QuizSeed>
            {
                // QC101 QUIZZES
                new("L_QC_1", "What mathematical constraint must probability amplitudes alpha and beta satisfy in a normalized qubit state?", 
                    "|alpha| + |beta| = 1", "|alpha|^2 + |beta|^2 = 1", "alpha * beta = 0", "alpha / beta = pi", "B", 
                    "Total probability must equal 1, meaning |alpha|^2 + |beta|^2 = 1."),

                new("L_QC_2", "Which quantum logic gate creates an equal superposition state from a computational basis state |0>?", 
                    "Pauli-X", "Hadamard (H)", "Pauli-Z", "Phase Gate (S)", "B", 
                    "The Hadamard gate transforms |0> into (|0> + |1>)/sqrt(2)."),

                new("L_QC_3", "According to the No-Cloning Theorem, which condition allows a quantum state to be copied?", 
                    "Only if the state is in superposition", "Only if the states are mutually orthogonal or identical", "Any state can be cloned using lasers", "States can be cloned if cooled to absolute zero", "B", 
                    "The No-Cloning Theorem proves that arbitrary unknown quantum states cannot be duplicated; only orthogonal states can be distinguished and copied."),

                new("L_QC_4", "How many classical bits must be transmitted to complete the Quantum Teleportation protocol for one unknown qubit?", 
                    "0 bits", "1 bit", "2 bits", "64 bits", "C", 
                    "Alice sends 2 classical bits to inform Bob which Pauli correction (I, X, Z, or ZX) to apply."),

                new("L_QC_5", "What is the computational complexity of Grover's quantum search algorithm for an unsorted database of N items?", 
                    "O(N)", "O(log N)", "O(sqrt(N))", "O(N^2)", "C", 
                    "Grover's algorithm achieves quadratic speedup O(sqrt(N)) compared to classical O(N)."),

                new("L_QC_6", "Which mathematical primitive forms the foundation of NIST's primary post-quantum key encapsulation standard (CRYSTALS-Kyber)?", 
                    "Elliptic Curve Discrete Logarithms", "Integer Factorization", "Learning With Errors (LWE) over Module Lattices", "SHA-256 Collision Search", "C", 
                    "CRYSTALS-Kyber relies on the hardness of Learning With Errors (LWE) over module lattices."),

                // DIST101 QUIZZES
                new("L_DIST_1", "What limitation of Lamport logical timestamps is resolved by Vector Clocks?", 
                    "Lamport timestamps cannot be stored on disk", "Lamport timestamps can only count to 100", "Lamport timestamps cannot determine whether two events are causally related or concurrent", "Lamport timestamps require GPS satellites", "C", 
                    "Vector clocks allow identifying both causal order (V(a) < V(b)) and concurrency (a || b)."),

                new("L_DIST_2", "According to the PACELC theorem, what trade-off must a distributed system make when no partition exists (Else)?", 
                    "Between Cost and Security", "Between Latency and Consistency", "Between Bandwidth and Encryption", "Between RAM and CPU", "B", 
                    "PACELC states: if Partition (P), choose Availability (A) or Consistency (C); Else (E), choose Latency (L) or Consistency (C)."),

                new("L_DIST_3", "In the Raft consensus protocol, what mechanism prevents multiple candidates from splitting votes indefinitely?", 
                    "Randomized election timeouts", "Alphabetical node sorting", "Manual administrator intervention", "Hashing node IP addresses", "A", 
                    "Randomized election timeouts ensure one node times out and requests votes before competitors."),

                new("L_DIST_4", "How many network round-trips are required for Multi-Paxos steady-state writes once a stable leader is established?", 
                    "1 round-trip (Phase 2 Accept/Accepted)", "2 round-trips (Phase 1 + Phase 2)", "4 round-trips", "0 round-trips", "A", 
                    "Multi-Paxos skips Phase 1 for steady-state writes, reducing latency to 1 round-trip."),

                new("L_DIST_5", "What is the minimum number of total nodes required in Practical Byzantine Fault Tolerance (PBFT) to tolerate f traitorous nodes?", 
                    "f + 1", "2f + 1", "3f + 1", "4f + 1", "C", 
                    "PBFT requires N >= 3f + 1 nodes to achieve consensus despite f Byzantine nodes."),

                new("L_DIST_6", "In a distributed microservice Saga, what action is taken when a participating step fails?", 
                    "The database is formatted", "Compensating transactions are executed in reverse order to undo prior steps", "The entire network is shut down", "A 2PC lock is acquired indefinitely", "B", 
                    "Sagas undo partial work by executing compensating transactions backwards from the point of failure."),

                // KRN101 QUIZZES
                new("L_KRN_1", "On x86-64 Linux, which CPU register holds the system call number during the 'syscall' instruction?", 
                    "%rax", "%rbx", "%rsp", "%rip", "A", 
                    "The system call number is passed in register %rax (e.g., 0 for read, 1 for write)."),

                new("L_KRN_2", "Which CPU control register contains the base physical memory address of the active Page Global Directory (PGD)?", 
                    "CR0", "CR2", "CR3", "CR4", "C", 
                    "Register CR3 holds the physical base address of the top-level page table directory."),

                new("L_KRN_3", "In the Linux Virtual File System (VFS), which object represents a file metadata record but NOT its filename?", 
                    "Superblock", "Inode", "Dentry", "File descriptor", "B", 
                    "Inodes hold file metadata (permissions, size, blocks); dentries associate filenames with inodes."),

                new("L_KRN_4", "What safety guarantee is enforced by the Linux in-kernel eBPF verifier before bytecode is loaded?", 
                    "That the program is written in Python", "That the program terminates and will not crash the kernel or access invalid memory", "That the network speed is at least 1 Gbps", "That the CPU fan is running", "B", 
                    "The verifier proves memory safety, bounds checking, and termination to ensure kernel stability."),

                new("L_KRN_5", "Which XDP return code discards a network packet immediately at the NIC driver layer for maximum DDoS mitigation?", 
                    "XDP_PASS", "XDP_TX", "XDP_DROP", "XDP_ABORTED", "C", 
                    "XDP_DROP drops the packet before socket buffer allocation, enabling 40+ million pps filtering."),

                new("L_KRN_6", "In Linux tracing, what is the difference between a Tracepoint and a Kprobe?", 
                    "Tracepoints are static, stable points; Kprobes are dynamic instruction breakpoints", "Tracepoints only trace Python; Kprobes trace C++", "Tracepoints require rebooting the PC", "Kprobes are slower than hard drives", "A", 
                    "Tracepoints are compiled directly into kernel source; Kprobes dynamically insert breakpoints at runtime."),

                // HFT101 QUIZZES
                new("L_HFT_1", "What performance hazard occurs when two CPU cores modify different variables located on the same 64-byte cache line?", 
                    "Stack Overflow", "False Sharing", "Memory Leak", "Buffer Underflow", "B", 
                    "False sharing causes continuous cache-line invalidation (cache ping-pong) between cores."),

                new("L_HFT_2", "Which hardware instruction is the primary building block for building lock-free data structures on x86-64 CPUs?", 
                    "LOCK CMPXCHG (Compare-And-Swap)", "NOP", "JMP", "POPF", "A", 
                    "LOCK CMPXCHG atomically updates a memory location if it matches an expected value."),

                new("L_HFT_3", "How does the LMAX Disruptor achieve high throughput compared to standard queues?", 
                    "By using linked lists on the heap", "By using a pre-allocated bounded ring buffer with lock-free sequence barriers", "By saving all messages to a remote SQL database", "By compressing messages with ZIP", "B", 
                    "The Disruptor uses a pre-allocated circular ring buffer with cache-aligned sequence counters."),

                new("L_HFT_4", "Why does Kernel Bypass (such as DPDK or Solarflare EF_VI) dramatically reduce network packet latency?", 
                    "It uses satellite internet", "It bypasses OS kernel interrupts and socket buffers, reading packets directly in user-space", "It encrypts packets twice", "It increases MTU size to 1 GB", "B", 
                    "Kernel bypass maps NIC DMA queues directly to user memory, eliminating context switches and interrupts."),

                new("L_HFT_5", "In a financial matching engine, what order priority rule executes orders at the same price based on arrival time?", 
                    "Pro-Rata Priority", "Price-Time Priority (FIFO)", "Random Selection", "Alphabetical Priority", "B", 
                    "Price-Time priority executes orders with the best price first, breaking ties by earliest arrival."),

                new("L_HFT_6", "Why is Simple Binary Encoding (SBE) preferred over JSON or XML in high-frequency trading?", 
                    "SBE is encrypted with AES", "SBE aligns binary fields to native CPU byte boundaries for zero-copy memory reads", "SBE uses fewer CPU registers", "SBE is human readable", "B", 
                    "SBE aligns binary integers and structs so applications can cast memory pointers directly without parsing."),

                // COMP101 QUIZZES
                new("L_COMP_1", "What is the primary output of the Lexical Analysis (tokenizer) phase of a compiler?", 
                    "Machine assembly code", "A stream of categorized Tokens", "A binary executable", "An Abstract Syntax Tree", "B", 
                    "The lexer consumes source characters and emits a stream of tokens (type, value, position)."),

                new("L_COMP_2", "What parsing technique implements grammar production rules as mutually recursive functions in code?", 
                    "Recursive Descent Parsing", "LR(1) Table Parsing", "Shift-Reduce Parsing", "CYK Algorithm", "A", 
                    "Recursive descent maps each non-terminal grammar rule to a recursive function."),

                new("L_COMP_3", "In compiler semantic analysis, what data structure maintains variable names, types, and lexical scope?", 
                    "Call Stack", "Symbol Table", "Priority Queue", "Bloom Filter", "B", 
                    "The Symbol Table manages variable names, types, and scope nesting across blocks."),

                new("L_COMP_4", "What is the defining invariant of Static Single Assignment (SSA) form in optimizing compilers?", 
                    "No variables may be assigned more than once", "All functions must return integers", "Loops are unrolled to 10 iterations", "Pointers cannot be used", "A", 
                    "SSA requires that every variable is assigned a value exactly once in intermediate representation."),

                new("L_COMP_5", "Which compiler optimization moves calculations that yield identical results outside of loop bodies?", 
                    "Dead Code Elimination", "Loop-Invariant Code Motion (LICM)", "Register Spilling", "Constant Folding", "B", 
                    "Loop-Invariant Code Motion hoists loop-independent computations outside the loop header."),

                new("L_COMP_6", "What classic graph algorithm is used by compilers to assign unlimited virtual registers to a fixed set of physical CPU registers?", 
                    "Dijkstra's Shortest Path", "Graph Coloring (Chaitin-Briggs)", "Kruskal's Minimum Spanning Tree", "Breadth-First Search", "B", 
                    "Register allocation maps variable interference graphs to K-colorable register assignments."),

                // LLM101 QUIZZES
                new("L_LLM_1", "Why is the dot product scaled by 1 / sqrt(d_k) in the Transformer scaled dot-product attention formula?", 
                    "To invert matrix dimensions", "To prevent large dot products from pushing the softmax function into regions with vanishingly small gradients", "To round numbers to integers", "To compress weights to 4-bit", "B", 
                    "For large d_k, dot products grow large in magnitude, driving softmax gradients to near zero."),

                new("L_LLM_2", "What optimization does FlashAttention use to achieve 2x-4x speedups in Transformer training and inference?", 
                    "It runs on CPU instead of GPU", "It tiles attention computation into GPU SRAM blocks to avoid materializing the full N x N matrix in HBM", "It converts text into images", "It drops half of the vocabulary", "B", 
                    "FlashAttention uses SRAM tiling to compute exact softmax without writing the large attention matrix to HBM."),

                new("L_LLM_3", "In Low-Rank Adaptation (LoRA), how are weight updates delta_W parameterized for a d x k weight matrix?", 
                    "As a single 1D vector", "As the product of two low-rank matrices B (d x r) and A (r x k) where r << min(d, k)", "As a diagonal identity matrix", "As a random hash table", "B", 
                    "LoRA decomposes updates into B * A with small rank r (e.g., 8 or 16), reducing parameters by 99%."),

                new("L_LLM_4", "Which quantization method uses activation calibration to identify and protect the top 1% salient weights from aggressive quantization?", 
                    "AWQ (Activation-aware Weight Quantization)", "GGUF Q4_0", "Standard Round-to-Nearest", "ZIP compression", "A", 
                    "AWQ protects the 1% most salient weights based on activation magnitudes to preserve accuracy."),

                new("L_LLM_5", "What problem in high-throughput LLM serving is solved by vLLM's PagedAttention architecture?", 
                    "Network packet loss", "KV-cache GPU memory fragmentation and waste", "Slow disk write speeds", "Screen refresh rate drops", "B", 
                    "PagedAttention eliminates KV-cache memory waste by paging key-value states in non-contiguous blocks."),

                new("L_LLM_6", "What is the key advantage of Speculative Decoding in large language model inference?", 
                    "It guarantees 100% correct factual answers", "It accelerates token generation latency by 2x-3x while preserving the exact probability distribution of the target model", "It translates text into 50 languages", "It eliminates the need for a GPU", "B", 
                    "Speculative decoding verifies draft tokens in a single parallel step, delivering speedups with zero quality loss.")
            };
        }
    }
}
