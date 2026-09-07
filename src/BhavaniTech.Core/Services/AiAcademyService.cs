using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BhavaniTech.Core.Services
{
    public record PromptAnalysisResult(string Rating, int EstimatedTokens, string Feedback, string OptimizedPrompt);

    public record TokenAttentionScore(string Token, int Position, double RawDotProduct, double AttentionWeightPercent);
    public record TransformerAttentionResult(
        string FocusToken,
        int VectorDimension,
        List<TokenAttentionScore> TokenWeights,
        string MathematicalTrace,
        string ArchitecturalInsight
    );

    public static class AiAcademyService
    {
        public static PromptAnalysisResult AnalyzePrompt(string promptText)
        {
            if (string.IsNullOrWhiteSpace(promptText))
                return new PromptAnalysisResult("Weak", 0, "Prompt is empty.", "");

            int tokenEstimate = (int)Math.Ceiling(promptText.Length / 4.0);
            int score = 0;
            var feedback = new System.Text.StringBuilder();

            // Evaluate Role specification
            if (Regex.IsMatch(promptText, @"\b(act as|you are|role|expert|developer|teacher|specialist)\b", RegexOptions.IgnoreCase))
            {
                score += 25;
                feedback.AppendLine(" - ✅ Role Framing Specified (e.g. 'Act as a senior C# developer')");
            }
            else
            {
                feedback.AppendLine(" - ⚠️ Add Role Framing (e.g. 'You are an expert Linux sysadmin')");
            }

            // Evaluate Context & Directives
            if (Regex.IsMatch(promptText, @"\b(context|background|given|assuming|environment|specification)\b", RegexOptions.IgnoreCase))
            {
                score += 25;
                feedback.AppendLine(" - ✅ Background Context Provided");
            }

            // Evaluate Output Format instruction
            if (Regex.IsMatch(promptText, @"\b(format|json|markdown|table|list|bullet|code|step-by-step)\b", RegexOptions.IgnoreCase))
            {
                score += 25;
                feedback.AppendLine(" - ✅ Output Format Specified");
            }
            else
            {
                feedback.AppendLine(" - ⚠️ Specify Desired Output Format (e.g. 'Provide output in Markdown table')");
            }

            // Length check
            if (promptText.Length > 40)
            {
                score += 25;
            }

            string rating = score switch
            {
                >= 80 => "EXCELLENT (High-Precision Prompt)",
                >= 50 => "GOOD (Clear Intent)",
                _ => "NEEDS IMPROVEMENT (Vague or Underspecified)"
            };

            string optimized = $"Act as a Technology Mentor. {promptText.Trim()} Please explain step-by-step and format the response in clean Markdown with examples.";

            return new PromptAnalysisResult(rating, tokenEstimate, feedback.ToString(), optimized);
        }

        public static string QueryOfflineRuleBasedAi(string userQuestion)
        {
            userQuestion = userQuestion.ToLowerInvariant();

            if (userQuestion.Contains("python"))
                return "AI Tutor: Python is an interpreted, high-level language known for clean syntax. Key concepts include variables, lists, dictionaries, loops, functions, and object-oriented programming.";

            if (userQuestion.Contains("c#") || userQuestion.Contains("csharp"))
                return "AI Tutor: C# is a strongly-typed object-oriented language developed by Microsoft for .NET applications. It features auto memory management (GC), async/await, and LINQ.";

            if (userQuestion.Contains("network") || userQuestion.Contains("ip") || userQuestion.Contains("subnet"))
                return "AI Tutor: Computer networks transfer data via packets using the OSI model. IPv4 uses 32-bit addresses split into network and host bits defined by CIDR netmasks.";

            if (userQuestion.Contains("hardware") || userQuestion.Contains("cpu") || userQuestion.Contains("ram"))
                return "AI Tutor: CPU executes instructions via clock cycles, RAM stores volatile runtime data at multi-gigabyte speeds, and storage (SSD/HDD) persists files permanently.";

            if (userQuestion.Contains("security") || userQuestion.Contains("password") || userQuestion.Contains("hash"))
                return "AI Tutor: Cybersecurity relies on the CIA Triad (Confidentiality, Integrity, Availability). Never store plain text passwords; always use cryptographic hashes like SHA-256 with salt.";

            return "AI Tutor: Great question! Technology learning thrives on understanding core fundamentals: CPU clock cycles, RAM memory allocation, packet routing, and clean algorithm design.";
        }

        // =====================================================================
        // TRANSFORMER MULTI-HEAD ATTENTION MATHEMATICAL SIMULATOR
        // =====================================================================
        public static TransformerAttentionResult SimulateTransformerAttention(string sentence = "The robot completed the task because it was fast", string queryWord = "it")
        {
            if (string.IsNullOrWhiteSpace(sentence))
                sentence = "The robot completed the task because it was fast";
            if (string.IsNullOrWhiteSpace(queryWord))
                queryWord = "it";

            var tokens = sentence.Split(new[] { ' ', ',', '.', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) tokens = new[] { "robot", "fast" };

            int d_k = 64; // Embedding dimension for attention head
            double scale = Math.Sqrt(d_k);

            // Calculate simulated semantic dot products
            var rawScores = new List<double>();
            for (int i = 0; i < tokens.Length; i++)
            {
                string t = tokens[i].ToLowerInvariant();
                string q = queryWord.ToLowerInvariant();

                double baseScore = 1.0;
                if (t == q) baseScore += 3.5; // High self-attention
                else if ((q == "it" || q == "he" || q == "she" || q == "they") &&
                         (t == "robot" || t == "cat" || t == "dog" || t == "computer" || t == "car" || t == "server"))
                {
                    baseScore += 6.8; // High coreference resolution attention!
                }
                else if (t == "fast" || t == "slow" || t == "smart" || t == "completed")
                {
                    baseScore += 4.2; // Predicate association
                }
                else
                {
                    baseScore += (Math.Abs((t + q).GetHashCode()) % 200) / 100.0;
                }

                // Scaled score = (Q * K^T) / sqrt(d_k)
                rawScores.Add(baseScore / scale);
            }

            // Softmax: exp(s_i) / sum(exp(s_j))
            double maxScore = rawScores.Max();
            var expScores = rawScores.Select(s => Math.Exp(s - maxScore)).ToList();
            double sumExp = expScores.Sum();
            var weights = expScores.Select(e => e / sumExp).ToList();

            var tokenWeights = new List<TokenAttentionScore>();
            for (int i = 0; i < tokens.Length; i++)
            {
                tokenWeights.Add(new TokenAttentionScore(
                    tokens[i],
                    i,
                    Math.Round(rawScores[i] * scale, 3),
                    Math.Round(weights[i] * 100.0, 1)
                ));
            }

            var trace = new System.Text.StringBuilder();
            trace.AppendLine($"=== SCALED DOT-PRODUCT ATTENTION FORMULA ===");
            trace.AppendLine("Attention(Q, K, V) = softmax( (Q * K^T) / √d_k ) * V");
            trace.AppendLine($"Query Token: \"{queryWord}\" | Embedding Dimension d_k = {d_k} | Scale Factor √d_k = {scale:0.00}");
            trace.AppendLine("");
            trace.AppendLine("Softmax Distribution Across Tokens:");
            foreach (var tw in tokenWeights.OrderByDescending(t => t.AttentionWeightPercent))
            {
                string bar = new string('█', (int)(tw.AttentionWeightPercent / 3));
                trace.AppendLine($"  Token [{tw.Position:D2}] \"{tw.Token,-10}\": {tw.AttentionWeightPercent,5:0.0}% | {bar}");
            }

            string insight = $"Coreference Resolution Success: When processing pronoun \"{queryWord}\", the highest attention weight " +
                $"automatically focuses on the antecedent subject (e.g. \"robot\") and descriptive adjectives (e.g. \"fast\"). " +
                $"This allows Transformers to capture context dynamically across unlimited sequence lengths without RNN recurrence!";

            return new TransformerAttentionResult(queryWord, d_k, tokenWeights, trace.ToString(), insight);
        }
    }
}
