using System;
using System.Collections.Generic;
using System.Linq;

namespace BhavaniTech.Core.Services
{
    public class KnowledgeDocument
    {
        public string Id { get; set; } = "";
        public string Category { get; set; } = "";
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public double[] Embedding { get; set; } = Array.Empty<double>();
    }

    public record RetrievalResult(
        KnowledgeDocument Document,
        double CosineSimilarity,
        int Rank
    );

    public record RagResponse(
        string Query,
        double[] QueryEmbedding,
        List<RetrievalResult> TopMatches,
        string AugmentedPrompt,
        string SynthesizedAnswer
    );

    public static class VectorRagService
    {
        private const int EmbeddingDim = 64;
        private static readonly List<KnowledgeDocument> KnowledgeBase;

        static VectorRagService()
        {
            KnowledgeBase = new List<KnowledgeDocument>
            {
                new KnowledgeDocument
                {
                    Id = "doc_os_vm",
                    Category = "Operating Systems",
                    Title = "Virtual Memory & Page Translation",
                    Content = "Virtual memory decouples the programmer's address space from physical RAM using page tables and the Memory Management Unit (MMU). When a requested virtual page is absent from physical frames, the CPU triggers a Page Fault interrupt, swapping the required 4KB page from disk swap space."
                },
                new KnowledgeDocument
                {
                    Id = "doc_crypto_rsa",
                    Category = "Cryptography",
                    Title = "RSA Asymmetric Public Key Cryptography",
                    Content = "RSA relies on the computational hardness of factoring the product of two large prime numbers p and q. The public key is (e, n) where n = p*q, while the private key d is the modular multiplicative inverse of e modulo phi(n). Encryption computes c = m^e mod n and decryption m = c^d mod n."
                },
                new KnowledgeDocument
                {
                    Id = "doc_cpu_pipe",
                    Category = "Microarchitecture",
                    Title = "Classical 5-Stage RISC Instruction Pipeline",
                    Content = "Instruction pipelining overlaps the execution of instructions across 5 stages: IF (Instruction Fetch), ID (Instruction Decode), EX (Execute), MEM (Memory Access), and WB (Writeback). Data hazards (RAW) can be resolved using hardware forwarding bypass paths or pipeline stall bubbles."
                },
                new KnowledgeDocument
                {
                    Id = "doc_net_dns",
                    Category = "Networking",
                    Title = "Hierarchical DNS Resolution",
                    Content = "The Domain Name System (DNS) translates human-readable hostnames into IP addresses. A recursive resolver queries the Root server (.), then the Top-Level Domain server (.com, .org), and finally the Authoritative Nameserver, caching results according to Time-To-Live (TTL)."
                },
                new KnowledgeDocument
                {
                    Id = "doc_ai_cnn",
                    Category = "Artificial Intelligence",
                    Title = "Convolutional Neural Networks (CNNs) & Feature Extraction",
                    Content = "CNNs use spatial convolution kernels (such as Sobel or Gaussian filters) to extract local edge, texture, and shape representations from multi-dimensional matrices. Pooling layers (Max Pooling) downsample spatial dimensions to achieve translation invariance."
                },
                new KnowledgeDocument
                {
                    Id = "doc_cyber_bof",
                    Category = "Cyber Security",
                    Title = "Stack Buffer Overflow & Control Flow Hijacking",
                    Content = "A stack buffer overflow occurs when untrusted input exceeds the bounds of a stack-allocated buffer (e.g. via strcpy or gets). The overflow overwrites the stored base pointer (RBP) and instruction pointer (RIP/EIP), diverting execution flow to arbitrary shellcode."
                },
                new KnowledgeDocument
                {
                    Id = "doc_db_sql",
                    Category = "Database Systems",
                    Title = "Relational ACID Properties & B-Tree Indexing",
                    Content = "Relational databases guarantee ACID: Atomicity, Consistency, Isolation, and Durability. Indexes are typically organized as balanced B+ Trees to achieve O(log N) logarithmic search, insertion, and range queries on primary and foreign keys."
                }
            };

            // Compute embeddings for all documents
            foreach (var doc in KnowledgeBase)
            {
                doc.Embedding = ComputeTextEmbedding(doc.Title + " " + doc.Content);
            }
        }

        private static int DeterministicHash(string s)
        {
            unchecked
            {
                int hash = 5381;
                foreach (char c in s)
                {
                    hash = ((hash << 5) + hash) + c;
                }
                return Math.Abs(hash);
            }
        }

        public static double[] ComputeTextEmbedding(string text)
        {
            var vec = new double[EmbeddingDim];
            if (string.IsNullOrWhiteSpace(text)) return vec;

            string clean = text.ToLowerInvariant();
            var words = clean.Split(new[] { ' ', '.', ',', ';', '(', ')', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Hash words into embedding dimension bins with semantic keyword weighting
            foreach (var w in words)
            {
                int hash = DeterministicHash(w);
                int bin = hash % EmbeddingDim;
                
                // Weight length and significant domain tokens
                double weight = 1.0 + (w.Length * 0.1);
                if (w.Contains("crypto") || w.Contains("rsa") || w.Contains("cipher") || w.Contains("key")) weight += 2.5;
                if (w.Contains("pipeline") || w.Contains("cpu") || w.Contains("hazard") || w.Contains("risc")) weight += 2.5;
                if (w.Contains("dns") || w.Contains("network") || w.Contains("packet") || w.Contains("ip")) weight += 2.5;
                if (w.Contains("ai") || w.Contains("neural") || w.Contains("convolution") || w.Contains("cnn")) weight += 2.5;
                if (w.Contains("memory") || w.Contains("virtual") || w.Contains("page") || w.Contains("fault")) weight += 2.5;
                if (w.Contains("overflow") || w.Contains("stack") || w.Contains("security") || w.Contains("buffer")) weight += 2.5;
                if (w.Contains("sql") || w.Contains("database") || w.Contains("acid") || w.Contains("btree")) weight += 2.5;

                vec[bin] += weight;
            }

            // L2 normalize the vector
            double norm = 0;
            for (int i = 0; i < EmbeddingDim; i++) norm += vec[i] * vec[i];
            norm = Math.Sqrt(norm);

            if (norm > 0)
            {
                for (int i = 0; i < EmbeddingDim; i++) vec[i] = Math.Round(vec[i] / norm, 4);
            }

            return vec;
        }

        public static double CalculateCosineSimilarity(double[] vecA, double[] vecB)
        {
            if (vecA == null || vecB == null || vecA.Length != vecB.Length) return 0.0;
            double dot = 0;
            double normA = 0;
            double normB = 0;

            for (int i = 0; i < vecA.Length; i++)
            {
                dot += vecA[i] * vecB[i];
                normA += vecA[i] * vecA[i];
                normB += vecB[i] * vecB[i];
            }

            double denom = Math.Sqrt(normA) * Math.Sqrt(normB);
            return denom > 0 ? Math.Round(dot / denom, 4) : 0.0;
        }

        public static RagResponse ExecuteRagQuery(string query, int topK = 2)
        {
            var queryVec = ComputeTextEmbedding(query);

            var ranked = KnowledgeBase
                .Select(doc => new
                {
                    Doc = doc,
                    Score = CalculateCosineSimilarity(queryVec, doc.Embedding)
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .Select((x, idx) => new RetrievalResult(x.Doc, x.Score, idx + 1))
                .ToList();

            // Construct augmented prompt
            var topDoc = ranked.FirstOrDefault()?.Document;
            string context = topDoc != null ? $"[{topDoc.Title}]: {topDoc.Content}" : "No direct offline context found.";

            string prompt = $"[SYSTEM PROMPT: Grounded Offline AI RAG Engine]\n" +
                            $"--- RETRIEVED CONTEXT FROM LOCAL VECTOR STORE ---\n" +
                            $"{context}\n" +
                            $"-------------------------------------------------\n" +
                            $"STUDENT QUESTION: {query}\n" +
                            $"SYNTHESIZED GROUNDED ANSWER:";

            string answer = topDoc != null
                ? $"Based on retrieved knowledge '{topDoc.Title}' (Cosine Similarity {ranked[0].CosineSimilarity:P1}):\n{topDoc.Content}"
                : "Unable to retrieve high-confidence vector matches offline.";

            return new RagResponse(
                Query: query,
                QueryEmbedding: queryVec,
                TopMatches: ranked,
                AugmentedPrompt: prompt,
                SynthesizedAnswer: answer
            );
        }
    }
}
