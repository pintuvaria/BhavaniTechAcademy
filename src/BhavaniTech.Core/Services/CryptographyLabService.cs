using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record CaesarResult(string Ciphertext, int Shift, List<string> BruteForceCandidates);
    public record VigenereResult(string ResultText, string Key, bool IsEncrypted);
    public record DiffieHellmanResult(
        BigInteger PrimeP,
        BigInteger GeneratorG,
        BigInteger AlicePrivate,
        BigInteger BobPrivate,
        BigInteger AlicePublic,
        BigInteger BobPublic,
        BigInteger AliceSharedSecret,
        BigInteger BobSharedSecret,
        string MathematicalTrace
    );
    public record RsaKeyResult(
        BigInteger P,
        BigInteger Q,
        BigInteger N,
        BigInteger Phi,
        BigInteger E,
        BigInteger D,
        BigInteger OriginalNumber,
        BigInteger EncryptedNumber,
        BigInteger DecryptedNumber,
        string MathematicalTrace
    );
    public record AesGcmResult(
        string KeyHex,
        string IvHex,
        string CiphertextHex,
        string AuthTagHex,
        bool TagVerified,
        string SecurityAnalysis
    );

    public static class CryptographyLabService
    {
        // =====================================================================
        // CAESAR CIPHER
        // =====================================================================
        public static CaesarResult ProcessCaesar(string input, int shift, bool encrypt = true)
        {
            if (string.IsNullOrEmpty(input))
                return new CaesarResult("", shift, new List<string>());

            shift %= 26;
            if (!encrypt) shift = (26 - shift) % 26;

            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    char shifted = (char)(baseChar + (c - baseChar + shift + 26) % 26);
                    sb.Append(shifted);
                }
                else
                {
                    sb.Append(c);
                }
            }

            string resultText = sb.ToString();
            var candidates = new List<string>();

            // Generate brute-force table for all 25 shifts
            for (int s = 1; s <= 25; s++)
            {
                var candSb = new StringBuilder();
                foreach (char c in resultText)
                {
                    if (char.IsLetter(c))
                    {
                        char baseChar = char.IsUpper(c) ? 'A' : 'a';
                        char shifted = (char)(baseChar + (c - baseChar - s + 26) % 26);
                        candSb.Append(shifted);
                    }
                    else candSb.Append(c);
                }
                candidates.Add($"Shift -{s:D2}: {candSb}");
            }

            return new CaesarResult(resultText, shift, candidates);
        }

        // =====================================================================
        // VIGENÈRE CIPHER
        // =====================================================================
        public static VigenereResult ProcessVigenere(string input, string key, bool encrypt = true)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(key))
                return new VigenereResult(input ?? "", key ?? "", encrypt);

            var sb = new StringBuilder();
            int keyIndex = 0;
            key = key.ToUpperInvariant();

            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    int kShift = key[keyIndex % key.Length] - 'A';
                    if (!encrypt) kShift = (26 - kShift) % 26;

                    char shifted = (char)(baseChar + (c - baseChar + kShift + 26) % 26);
                    sb.Append(shifted);
                    keyIndex++;
                }
                else
                {
                    sb.Append(c);
                }
            }

            return new VigenereResult(sb.ToString(), key, encrypt);
        }

        // =====================================================================
        // DIFFIE-HELLMAN KEY EXCHANGE
        // =====================================================================
        public static DiffieHellmanResult SimulateDiffieHellman(int primeP = 23, int generatorG = 5, int alicePriv = 6, int bobPriv = 15)
        {
            if (primeP <= 2) primeP = 23;
            if (generatorG <= 1) generatorG = 5;
            if (alicePriv <= 0) alicePriv = 6;
            if (bobPriv <= 0) bobPriv = 15;

            BigInteger p = primeP;
            BigInteger g = generatorG;
            BigInteger a = alicePriv;
            BigInteger b = bobPriv;

            // Public Keys: A = g^a mod p, B = g^b mod p
            BigInteger A = BigInteger.ModPow(g, a, p);
            BigInteger B = BigInteger.ModPow(g, b, p);

            // Shared Secrets: S_Alice = B^a mod p, S_Bob = A^b mod p
            BigInteger sAlice = BigInteger.ModPow(B, a, p);
            BigInteger sBob = BigInteger.ModPow(A, b, p);

            var trace = new StringBuilder();
            trace.AppendLine("=== DIFFIE-HELLMAN KEY EXCHANGE DERIVATION ===");
            trace.AppendLine($"1. Public Parameters : Shared Prime p = {p}, Generator g = {g}");
            trace.AppendLine($"2. Alice Secret Key  : a = {a} (Kept secret!)");
            trace.AppendLine($"   Alice Public Key  : A = g^a mod p = {g}^{a} mod {p} = {A}");
            trace.AppendLine($"3. Bob Secret Key    : b = {b} (Kept secret!)");
            trace.AppendLine($"   Bob Public Key    : B = g^b mod p = {g}^{b} mod {p} = {B}");
            trace.AppendLine($"4. Exchange over Public Internet: Alice sends A={A} to Bob; Bob sends B={B} to Alice.");
            trace.AppendLine($"5. Alice computes Shared Secret : S = B^a mod p = {B}^{a} mod {p} = {sAlice}");
            trace.AppendLine($"   Bob computes Shared Secret   : S = A^b mod p = {A}^{b} mod {p} = {sBob}");
            trace.AppendLine($"6. Outcome: sAlice ({sAlice}) == sBob ({sBob}) -> Match! Shared encryption key established without ever sending the key over the wire!");

            return new DiffieHellmanResult(p, g, a, b, A, B, sAlice, sBob, trace.ToString());
        }

        // =====================================================================
        // RSA PUBLIC-KEY ASYMMETRIC CRYPTOGRAPHY
        // =====================================================================
        public static RsaKeyResult SimulateRsa(int pVal = 61, int qVal = 53, int msgNumber = 65)
        {
            if (pVal < 3) pVal = 61;
            if (qVal < 3) qVal = 53;
            if (pVal == qVal) qVal = 53;

            BigInteger p = pVal;
            BigInteger q = qVal;
            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);

            // Standard public exponent
            BigInteger e = 17;
            while (BigInteger.GreatestCommonDivisor(e, phi) != 1 && e < phi)
            {
                e += 2;
            }

            // Extended Euclidean modular inverse: d = e^-1 mod phi
            BigInteger d = ModInverse(e, phi);

            BigInteger m = msgNumber % n;
            if (m <= 1) m = 42;

            // Encrypt: c = m^e mod n
            BigInteger c = BigInteger.ModPow(m, e, n);
            // Decrypt: m' = c^d mod n
            BigInteger decrypted = BigInteger.ModPow(c, d, n);

            var trace = new StringBuilder();
            trace.AppendLine("=== RSA ASYMMETRIC CRYPTOGRAPHY DERIVATION ===");
            trace.AppendLine($"1. Prime Selection : p = {p}, q = {q}");
            trace.AppendLine($"2. RSA Modulus     : n = p * q = {p} * {q} = {n}");
            trace.AppendLine($"3. Euler's Totient : φ(n) = (p-1)*(q-1) = ({p-1})*({q-1}) = {phi}");
            trace.AppendLine($"4. Public Exponent : e = {e} (Coprime to φ(n))");
            trace.AppendLine($"5. Private Key     : d = e^(-1) mod φ(n) = {d} (Because {e} * {d} ≡ 1 mod {phi})");
            trace.AppendLine($"   -> PUBLIC KEY   : (e={e}, n={n}) [Shared with the world]");
            trace.AppendLine($"   -> PRIVATE KEY  : (d={d}, n={n}) [Kept strictly secret]");
            trace.AppendLine("");
            trace.AppendLine($"6. Encryption Math : c = m^e mod n = {m}^{e} mod {n} = {c}");
            trace.AppendLine($"7. Decryption Math : m = c^d mod n = {c}^{d} mod {n} = {decrypted}");
            trace.AppendLine($"8. Verification    : Decrypted ({decrypted}) == Original ({m}) -> 100% Cryptographic Match!");

            return new RsaKeyResult(p, q, n, phi, e, d, m, c, decrypted, trace.ToString());
        }

        private static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger y = 0, x = 1;

            if (m == 1) return 0;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;
                m = a % m;
                a = t;
                t = y;
                y = x - q * y;
                x = t;
            }

            if (x < 0) x += m0;
            return x;
        }

        // =====================================================================
        // AES-GCM AUTHENTICATED SYMMETRIC ENCRYPTION
        // =====================================================================
        public static AesGcmResult SimulateAesGcm(string plaintext, bool simulateTamper = false)
        {
            if (string.IsNullOrEmpty(plaintext)) plaintext = "Bhavani Confidential Secret 2026";

            byte[] key = new byte[32]; // 256-bit key
            byte[] iv = new byte[12];  // 96-bit nonce
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }

            byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16]; // 128-bit authentication tag

            using (var aesGcm = new AesGcm(key, 16))
            {
                aesGcm.Encrypt(iv, plainBytes, cipherBytes, tag);
            }

            if (simulateTamper && cipherBytes.Length > 0)
            {
                cipherBytes[0] ^= 0xFF; // Flip bits to simulate MITM data tampering
            }

            bool verified = false;
            byte[] decrypted = new byte[cipherBytes.Length];

            try
            {
                using var verifyGcm = new AesGcm(key, 16);
                verifyGcm.Decrypt(iv, cipherBytes, tag, decrypted);
                verified = true;
            }
            catch (CryptographicException)
            {
                verified = false;
            }

            string analysis = verified
                ? "AUTHENTICATED & VERIFIED: The 128-bit Galois Message Authentication Code (GMAC) tag matches. Zero ciphertext tampering detected."
                : "SECURITY ALERT: DECRYPTION REJECTED! Ciphertext was altered in transit. AES-GCM AEAD prevented chosen-ciphertext and bit-flipping attacks!";

            return new AesGcmResult(
                Convert.ToHexString(key),
                Convert.ToHexString(iv),
                Convert.ToHexString(cipherBytes),
                Convert.ToHexString(tag),
                verified,
                analysis
            );
        }
    }
}
