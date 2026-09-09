using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record AliBabaRoundResult(
        int RoundNumber,
        string ProverPathEntered,
        string VerifierCallout,
        bool Passed,
        double CheatConfidence
    );

    public record SchnorrProofResult(
        long GeneratorG,
        long PrimeP,
        long PublicKeyY,
        long CommitmentT,
        long ChallengeC,
        long ResponseS,
        long VerifierLhs,
        long VerifierRhs,
        bool IsVerified,
        string MathematicalExplanation
    );

    public record ArithmeticCircuitVerification(
        int SecretWitnessX,
        int TargetOutput,
        Dictionary<string, int> WireValues,
        List<string> ConstraintsChecked,
        bool IsSatisfied,
        string ProofSummary
    );

    public class ZeroKnowledgeLabService
    {
        // Public domain parameters for Schnorr protocol
        public const long DefaultPrimeP = 10007; // Safe prime
        public const long DefaultGeneratorG = 5;

        public List<AliBabaRoundResult> SimulateAliBabaCave(int rounds, bool proverKnowsSecret)
        {
            var results = new List<AliBabaRoundResult>();
            var rnd = new Random(42); // Deterministic seed for reproducible testing

            for (int r = 1; r <= rounds; r++)
            {
                // Prover enters Path A or Path B
                string entered = rnd.Next(2) == 0 ? "A" : "B";
                // Verifier calls out A or B
                string called = rnd.Next(2) == 0 ? "A" : "B";

                bool passed;
                if (proverKnowsSecret)
                {
                    // If prover knows secret door pass, she can always switch paths and exit through called path
                    passed = true;
                }
                else
                {
                    // Cheater can only exit called path if she happened to enter it
                    passed = (entered == called);
                }

                // Probability of a cheater surviving r rounds = (0.5)^r
                double cheatConfidence = 1.0 - Math.Pow(0.5, r);

                results.Add(new AliBabaRoundResult(r, entered, called, passed, cheatConfidence));

                // If cheater failed, abort remaining rounds
                if (!passed) break;
            }

            return results;
        }

        public SchnorrProofResult ExecuteSchnorrProtocol(long secretX, long? customNonceR = null, long? customChallengeC = null)
        {
            long p = DefaultPrimeP;
            long g = DefaultGeneratorG;

            // 1. Key Generation: y = g^x mod p
            long y = ModPow(g, secretX, p);

            // 2. Prover Commitment: choose random r, compute t = g^r mod p
            long r = customNonceR ?? 731;
            long t = ModPow(g, r, p);

            // 3. Verifier Challenge: c
            long c = customChallengeC ?? 419;

            // 4. Prover Response: s = (r + c * x) mod (p - 1)
            long s = (r + (c * (secretX % (p - 1)))) % (p - 1);

            // 5. Verifier Checks: g^s == t * (y^c) mod p
            long lhs = ModPow(g, s, p);
            long yc = ModPow(y, c, p);
            long rhs = (t * yc) % p;

            bool isVerified = (lhs == rhs);

            var sb = new StringBuilder();
            sb.AppendLine("=== SCHNORR ZERO-KNOWLEDGE IDENTIFICATION ===");
            sb.AppendLine($"1. Public Parameters: Generator g={g}, Prime p={p}");
            sb.AppendLine($"2. Public Key y = g^x mod p = {g}^{secretX} mod {p} = {y}");
            sb.AppendLine($"3. Prover Commitment t = g^r mod p = {g}^{r} mod {p} = {t} (Nonce r={r} kept secret)");
            sb.AppendLine($"4. Verifier Challenge c = {c}");
            sb.AppendLine($"5. Prover Response s = (r + c * x) mod (p-1) = {s}");
            sb.AppendLine($"6. Verification: g^s mod p ({lhs}) == (t * y^c) mod p ({rhs}) -> {(isVerified ? "VERIFIED ✅" : "FAILED ❌")}");
            sb.AppendLine("✨ ZERO-KNOWLEDGE GUARANTEE: The verifier is completely convinced that the prover knows secret x, but learns zero information about x!");

            return new SchnorrProofResult(g, p, y, t, c, s, lhs, rhs, isVerified, sb.ToString());
        }

        public ArithmeticCircuitVerification VerifyArithmeticCircuit(int candidateX, int targetResult = 35)
        {
            // Circuit: x^3 + x + 5 = 35
            // Wires:
            // w1 = x * x
            // w2 = w1 * x = x^3
            // w3 = w2 + x = x^3 + x
            // out = w3 + 5
            int w1 = candidateX * candidateX;
            int w2 = w1 * candidateX;
            int w3 = w2 + candidateX;
            int actualOut = w3 + 5;

            var wires = new Dictionary<string, int>
            {
                ["x (witness)"] = candidateX,
                ["w1 (x * x)"] = w1,
                ["w2 (w1 * x)"] = w2,
                ["w3 (w2 + x)"] = w3,
                ["out (w3 + 5)"] = actualOut
            };

            var constraints = new List<string>
            {
                $"Gate 1 (Multiplier): x * x = {w1} (satisfied: {candidateX * candidateX == w1})",
                $"Gate 2 (Multiplier): w1 * x = {w2} (satisfied: {w1 * candidateX == w2})",
                $"Gate 3 (Adder): w2 + x = {w3} (satisfied: {w2 + candidateX == w3})",
                $"Gate 4 (Constant Adder): w3 + 5 = {actualOut} (satisfied: {w3 + 5 == actualOut})"
            };

            bool isSatisfied = (actualOut == targetResult);
            string summary = isSatisfied
                ? $"✅ R1CS Polynomial Satisfaction SUCCESS: Witness value x={candidateX} satisfies circuit x^3 + x + 5 = {targetResult}!"
                : $"❌ Circuit Unsatisfied: Witness x={candidateX} evaluates to {actualOut}, expected {targetResult}.";

            return new ArithmeticCircuitVerification(candidateX, targetResult, wires, constraints, isSatisfied, summary);
        }

        private static long ModPow(long baseVal, long exp, long mod)
        {
            return (long)BigInteger.ModPow(baseVal, exp, mod);
        }
    }
}
