using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BhavaniTech.Core.Services
{
    public enum SameSitePolicy
    {
        None,
        Lax,
        Strict
    }

    public record CsrfEvaluationResult(
        bool CookieAttached,
        bool RequestAllowed,
        bool AttackSucceeded,
        string AnalysisDetails,
        string RemediationRecommendation
    );

    public record SsrfEvaluationResult(
        bool Exploited,
        string ResolvedIp,
        string ExtractedData,
        string AttackType,
        string DefenseVerdict,
        string Remediation
    );

    public record JwtTamperResult(
        bool ServerAccepted,
        string ModifiedToken,
        string VulnerabilityExploited,
        string Explanation,
        string PreventionRecommendation
    );

    public class AdvancedWebSecurityService
    {
        #region CSRF Simulation

        public CsrfEvaluationResult SimulateCsrfRequest(
            string originDomain,
            string targetDomain,
            string httpMethod,
            bool isTopLevelNavigation,
            SameSitePolicy sameSite,
            bool hasAntiCsrfTokenDefense,
            bool isSensitiveAction = true)
        {
            originDomain = originDomain.Trim().ToLowerInvariant();
            targetDomain = targetDomain.Trim().ToLowerInvariant();
            httpMethod = httpMethod.Trim().ToUpperInvariant();

            bool isSameSiteDomain = originDomain == targetDomain;
            bool cookieAttached = false;

            if (isSameSiteDomain)
            {
                cookieAttached = true;
            }
            else
            {
                // Cross-site request
                switch (sameSite)
                {
                    case SameSitePolicy.Strict:
                        // Strict cookies are NEVER sent on cross-site requests, even top-level navigation
                        cookieAttached = false;
                        break;

                    case SameSitePolicy.Lax:
                        // Lax cookies are sent on top-level GET navigation, but withheld on cross-origin POST / PUT / DELETE or AJAX/fetch/iframes
                        if (isTopLevelNavigation && (httpMethod == "GET" || httpMethod == "HEAD"))
                        {
                            cookieAttached = true;
                        }
                        else
                        {
                            cookieAttached = false;
                        }
                        break;

                    case SameSitePolicy.None:
                        // Sent across all cross-site contexts (requires Secure in modern browsers)
                        cookieAttached = true;
                        break;
                }
            }

            bool attackSucceeded = false;
            bool requestAllowed = false;
            string analysis;

            if (!cookieAttached)
            {
                requestAllowed = false;
                attackSucceeded = false;
                analysis = $"BLOCKED BY BROWSER: SameSite={sameSite} prevented session cookie transmission from cross-origin '{originDomain}' to '{targetDomain}'. The request carried no authenticated session.";
            }
            else if (!isSameSiteDomain && isSensitiveAction && !hasAntiCsrfTokenDefense)
            {
                // Cookie was transmitted across origins to a sensitive endpoint without anti-CSRF token verification!
                requestAllowed = true;
                attackSucceeded = true;
                analysis = $"CRITICAL VULNERABILITY! CSRF attack succeeded: Browser sent victim's session cookie under SameSite={sameSite} and server executed unauthorized '{httpMethod}' state change from origin '{originDomain}' without token verification.";
            }
            else if (!isSameSiteDomain && isSensitiveAction && hasAntiCsrfTokenDefense)
            {
                // Server defended using anti-CSRF token verification
                requestAllowed = false;
                attackSucceeded = false;
                analysis = "BLOCKED BY SERVER: Session cookie was present, but anti-CSRF Synchronizer Token defense verified that the forged cross-site request lacked a valid token.";
            }
            else
            {
                // First-party or non-sensitive request
                requestAllowed = true;
                attackSucceeded = false;
                analysis = isSameSiteDomain
                    ? "NORMAL: Legitimate first-party request within same origin."
                    : "PERMITTED: Cross-site non-sensitive read operation permitted.";
            }

            string remediation = "Use SameSite=Lax or Strict cookies, enforce anti-CSRF tokens (Synchronizer Token Pattern or Double-Submit Cookie) for all state-changing verbs (POST/PUT/DELETE), and verify Sec-Fetch-Site / Origin headers.";

            return new CsrfEvaluationResult(
                CookieAttached: cookieAttached,
                RequestAllowed: requestAllowed,
                AttackSucceeded: attackSucceeded,
                AnalysisDetails: analysis,
                RemediationRecommendation: remediation);
        }

        #endregion

        #region SSRF Simulation

        public SsrfEvaluationResult SimulateSsrf(string targetUrlInput, bool enableImdsV2 = false, bool enableDnsPreflightFilter = false)
        {
            string url = targetUrlInput.Trim();
            string resolvedIp = "UNKNOWN";
            string attackType = "Direct Access";

            // Extract host
            string host = url;
            try
            {
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(url);
                    host = uri.Host;
                }
            }
            catch
            {
                // Fallback basic host extraction
                host = url.Replace("http://", "").Replace("https://", "").Split('/')[0].Split(':')[0];
            }

            // Check for IP obfuscations
            if (uint.TryParse(host, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint decimalIp))
            {
                // Decimal representation of IPv4 e.g. 2852039166 -> 169.254.169.254, 2130706433 -> 127.0.0.1
                byte b1 = (byte)((decimalIp >> 24) & 0xFF);
                byte b2 = (byte)((decimalIp >> 16) & 0xFF);
                byte b3 = (byte)((decimalIp >> 8) & 0xFF);
                byte b4 = (byte)(decimalIp & 0xFF);
                resolvedIp = $"{b1}.{b2}.{b3}.{b4}";
                attackType = $"Decimal IP Obfuscation ({decimalIp} -> {resolvedIp})";
            }
            else if (host.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                // Hex IP e.g. 0xA9FEA9FE or 0x7F000001
                try
                {
                    string cleanHex = host[2..];
                    if (uint.TryParse(cleanHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint hexVal))
                    {
                        byte b1 = (byte)((hexVal >> 24) & 0xFF);
                        byte b2 = (byte)((hexVal >> 16) & 0xFF);
                        byte b3 = (byte)((hexVal >> 8) & 0xFF);
                        byte b4 = (byte)(hexVal & 0xFF);
                        resolvedIp = $"{b1}.{b2}.{b3}.{b4}";
                        attackType = $"Hex IP Obfuscation ({host} -> {resolvedIp})";
                    }
                }
                catch { }
            }
            else if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || host == "127.0.0.1" || host == "::1")
            {
                resolvedIp = "127.0.0.1";
                attackType = "Loopback Localhost Access";
            }
            else if (host == "169.254.169.254" || host.Contains("metadata.google.internal") || host.Contains("169.254."))
            {
                resolvedIp = "169.254.169.254";
                attackType = "Cloud Metadata Service Access (AWS/GCP/Azure)";
            }
            else if (IPAddress.TryParse(host, out var parsedIp))
            {
                resolvedIp = parsedIp.ToString();
            }
            else
            {
                resolvedIp = "93.184.216.34"; // Simulated external public internet host
                attackType = "External Domain";
            }

            bool isCloudMetadata = resolvedIp == "169.254.169.254";
            bool isPrivateSubnet = isCloudMetadata || resolvedIp.StartsWith("127.") || resolvedIp.StartsWith("10.") || resolvedIp.StartsWith("192.168.") || (resolvedIp.StartsWith("172.") && Is172Private(resolvedIp));

            if (enableDnsPreflightFilter && isPrivateSubnet)
            {
                return new SsrfEvaluationResult(
                    Exploited: false,
                    ResolvedIp: resolvedIp,
                    ExtractedData: "[BLOCKED: Request aborted prior to socket connection]",
                    AttackType: attackType,
                    DefenseVerdict: "DEFENSE SUCCESS: Server resolved host IP pre-flight and dropped connection matching RFC 1918 / Cloud link-local blocklist.",
                    Remediation: "Always resolve hostnames before connection, reject any IP matching 0.0.0.0/8, 10.0.0.0/8, 127.0.0.0/8, 169.254.0.0/16, 172.16.0.0/12, 192.168.0.0/16."
                );
            }

            if (isCloudMetadata)
            {
                if (enableImdsV2)
                {
                    return new SsrfEvaluationResult(
                        Exploited: false,
                        ResolvedIp: resolvedIp,
                        ExtractedData: "HTTP/1.1 401 Unauthorized - Missing IMDSv2 Session Token (X-aws-ec2-metadata-token header required)",
                        AttackType: attackType,
                        DefenseVerdict: "BLOCKED BY IMDSv2: Cloud metadata rejected unauthenticated request lacking pre-fetched session token.",
                        Remediation: "Enforce IMDSv2 (HttpTokens=required, HopLimit=1) across cloud instances to defeat blind SSRF."
                    );
                }
                else
                {
                    string leakedCredentials = "{\n  \"AccessKeyId\": \"ASIAV3EXAMPLESECRETKEY\",\n  \"SecretAccessKey\": \"wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY\",\n  \"Token\": \"AQoDYXdzEJr...\",\n  \"Expiration\": \"2026-10-01T12:00:00Z\"\n}";
                    return new SsrfEvaluationResult(
                        Exploited: true,
                        ResolvedIp: resolvedIp,
                        ExtractedData: leakedCredentials,
                        AttackType: attackType,
                        DefenseVerdict: "EXPLOITED (CRITICAL): Attacker extracted Cloud IAM instance profile credentials via unhardened IMDSv1!",
                        Remediation: "Migrate immediately to IMDSv2, enforce egress firewall filtering, and use an allowlist for outbound HTTP fetchers."
                    );
                }
            }

            if (isPrivateSubnet)
            {
                return new SsrfEvaluationResult(
                    Exploited: true,
                    ResolvedIp: resolvedIp,
                    ExtractedData: "HTTP/1.1 200 OK\nInternal Admin Microservice: Status Healthy, Cluster Master: node-01, Vault Key: vlt_master_9921",
                    AttackType: attackType,
                    DefenseVerdict: "EXPLOITED: Attacker bypassed external perimeter and accessed internal intranet microservice.",
                    Remediation: "Implement network micro-segmentation, mutual TLS (mTLS), and application-level egress URL validation."
                );
            }

            return new SsrfEvaluationResult(
                Exploited: false,
                ResolvedIp: resolvedIp,
                ExtractedData: "Public HTTP 200 OK - Standard web content.",
                AttackType: attackType,
                DefenseVerdict: "SAFE: Public external internet resource fetched normally.",
                Remediation: "Continue using hardened outbound HTTP clients with strict timeouts and URL allowlisting."
            );
        }

        private static bool Is172Private(string ip)
        {
            var parts = ip.Split('.');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int secondOctet))
            {
                return secondOctet >= 16 && secondOctet <= 31;
            }
            return false;
        }

        #endregion

        #region JWT Tampering & Algorithm Confusion

        public record JwtTokenParts(string HeaderBase64, string PayloadBase64, string SignatureBase64);

        public string CreateSampleToken(string username, string role, string secretKey = "BhavaniTechSecretKey2026")
        {
            string headerJson = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
            string payloadJson = $"{{\"sub\":\"{username}\",\"role\":\"{role}\",\"iat\":1773000000}}";

            string headerB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            string payloadB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
            string signingInput = $"{headerB64}.{payloadB64}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput));
            string signatureB64 = Base64UrlEncode(signature);

            return $"{headerB64}.{payloadB64}.{signatureB64}";
        }

        public JwtTamperResult SimulateAlgNoneAttack(string originalToken, string desiredRole = "admin")
        {
            var parts = originalToken.Split('.');
            if (parts.Length < 2)
            {
                return new JwtTamperResult(false, originalToken, "Invalid Token", "Token does not have 3 parts.", "");
            }

            // Decode payload and tamper role
            string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            string tamperedPayload = payloadJson.Replace("\"role\":\"student\"", $"\"role\":\"{desiredRole}\"")
                                                .Replace("\"role\":\"user\"", $"\"role\":\"{desiredRole}\"");

            // Forged alg=none header
            string algNoneHeader = "{\"alg\":\"none\",\"typ\":\"JWT\"}";
            string headerB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(algNoneHeader));
            string payloadB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(tamperedPayload));

            // Alg none attack specifies an empty signature (trailing dot)
            string forgedToken = $"{headerB64}.{payloadB64}.";

            string explanation = "In the 'alg: none' vulnerability (CVE-2015-9235), naive JWT verification libraries trust the header's declared algorithm. When alg is 'none', the parser skips HMAC/RSA signature verification entirely, accepting arbitrary tampered claims!";
            string remediation = "Hardcode the expected algorithm on the server (e.g. Algorithms.HMAC_SHA256) and explicitly reject any tokens where alg is 'none' or mismatched.";

            return new JwtTamperResult(
                ServerAccepted: true, // Demonstrating the vulnerability when alg none is allowed
                ModifiedToken: forgedToken,
                VulnerabilityExploited: "Algorithm Confusion (alg: none / CVE-2015-9235)",
                Explanation: explanation,
                PreventionRecommendation: remediation
            );
        }

        public JwtTamperResult SimulateSignatureTamper(string originalToken, string desiredRole = "admin", string serverSecret = "BhavaniTechSecretKey2026")
        {
            var parts = originalToken.Split('.');
            if (parts.Length != 3)
            {
                return new JwtTamperResult(false, originalToken, "Invalid Token", "Token malformed.", "");
            }

            // Attacker changes role in payload without the real server secret
            string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            string tamperedPayload = payloadJson.Replace("\"role\":\"student\"", $"\"role\":\"{desiredRole}\"")
                                                .Replace("\"role\":\"user\"", $"\"role\":\"{desiredRole}\"");

            string payloadB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(tamperedPayload));
            string forgedToken = $"{parts[0]}.{payloadB64}.{parts[2]}"; // Keep original signature

            // Server validates token with its secret
            string signingInput = $"{parts[0]}.{payloadB64}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(serverSecret));
            byte[] expectedSig = hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput));
            string expectedSigB64 = Base64UrlEncode(expectedSig);

            bool accepted = expectedSigB64 == parts[2];

            return new JwtTamperResult(
                ServerAccepted: accepted,
                ModifiedToken: forgedToken,
                VulnerabilityExploited: "Direct Payload Tampering (Signature Verification Active)",
                Explanation: accepted 
                    ? "UNEXPECTED: Signature accepted." 
                    : "BLOCKED: Server recalculated HMAC-SHA256 over tampered header.payload and found signature mismatch. Request rejected with 401 Unauthorized.",
                PreventionRecommendation: "Always enforce cryptographic signature verification using constant-time comparison (FixedTimeEquals) with a 256-bit cryptographically random secret."
            );
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string padded = input.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }

        #endregion
    }
}
