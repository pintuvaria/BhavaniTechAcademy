using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public record DnsResolutionStep(
        int StepNumber,
        string QueryServerType,
        string ServerName,
        string Question,
        string ResponseType,
        string ResponseData,
        int TtlSeconds,
        string Explanation
    );

    public record DnsResolutionResult(
        string DomainName,
        string ResolvedIp,
        int TotalLatencyMs,
        List<DnsResolutionStep> Steps,
        string Summary
    );

    public static class DnsResolutionService
    {
        public static DnsResolutionResult ResolveDomain(string domain, bool simulateCacheHit = false)
        {
            var steps = new List<DnsResolutionStep>();
            string cleanDomain = string.IsNullOrWhiteSpace(domain) ? "learn.bhavanitech.org" : domain.Trim().ToLowerInvariant();
            string resolvedIp = "104.21.55.92";

            if (simulateCacheHit)
            {
                steps.Add(new DnsResolutionStep(
                    StepNumber: 1,
                    QueryServerType: "Local OS DNS Cache",
                    ServerName: "localhost (Windows dnscache svc)",
                    Question: $"A {cleanDomain}",
                    ResponseType: "CACHE HIT (A Record)",
                    ResponseData: resolvedIp,
                    TtlSeconds: 240,
                    Explanation: "OS found existing valid DNS record in in-memory resolver cache. No network queries required."
                ));

                return new DnsResolutionResult(
                    DomainName: cleanDomain,
                    ResolvedIp: resolvedIp,
                    TotalLatencyMs: 1,
                    Steps: steps,
                    Summary: $"Resolved {cleanDomain} to {resolvedIp} instantly via Local OS DNS Cache (1ms)."
                );
            }

            // Step 1: Local Cache Miss
            steps.Add(new DnsResolutionStep(
                StepNumber: 1,
                QueryServerType: "Local OS Resolver",
                ServerName: "localhost",
                Question: $"A {cleanDomain}",
                ResponseType: "CACHE MISS",
                ResponseData: "Forwarding to Recursive Resolver...",
                TtlSeconds: 0,
                Explanation: "Record not found in local client cache. Initiating outbound recursive query."
            ));

            // Step 2: Recursive Resolver
            steps.Add(new DnsResolutionStep(
                StepNumber: 2,
                QueryServerType: "Recursive Resolver",
                ServerName: "1.1.1.1 (Cloudflare Public DNS)",
                Question: $"A {cleanDomain}",
                ResponseType: "Iterative Lookup Started",
                ResponseData: "Querying Root DNS Servers...",
                TtlSeconds: 0,
                Explanation: "Recursive resolver accepts request and begins traversing the DNS hierarchy from the root."
            ));

            // Step 3: Root Nameserver (.)
            steps.Add(new DnsResolutionStep(
                StepNumber: 3,
                QueryServerType: "Root Nameserver (.)",
                ServerName: "a.root-servers.net (198.41.0.4)",
                Question: $"A {cleanDomain}",
                ResponseType: "DELEGATION (NS referral)",
                ResponseData: "Referral to .org TLD servers (e.g. a0.org.afilias-nst.info)",
                TtlSeconds: 172800,
                Explanation: "Root server does not know the final IP, but directs the resolver to the responsible Top-Level Domain (TLD) nameserver."
            ));

            // Step 4: TLD Nameserver (.org / .com)
            string tld = cleanDomain.Contains('.') ? cleanDomain[(cleanDomain.LastIndexOf('.') + 1)..] : "org";
            steps.Add(new DnsResolutionStep(
                StepNumber: 4,
                QueryServerType: $"TLD Nameserver (.{tld})",
                ServerName: $"a0.{tld}.afilias-nst.info",
                Question: $"A {cleanDomain}",
                ResponseType: "DELEGATION (Authoritative referral)",
                ResponseData: "Referral to ns1.bhavanitech.org (173.245.58.112)",
                TtlSeconds: 86400,
                Explanation: $"The .{tld} TLD nameserver refers the resolver to the domain's Authoritative Nameservers."
            ));

            // Step 5: Authoritative Nameserver
            steps.Add(new DnsResolutionStep(
                StepNumber: 5,
                QueryServerType: "Authoritative Nameserver",
                ServerName: "ns1.bhavanitech.org",
                Question: $"A {cleanDomain}",
                ResponseType: "ANSWER (A Record)",
                ResponseData: resolvedIp,
                TtlSeconds: 3600,
                Explanation: $"The authoritative server holds the true record for {cleanDomain} and returns the IPv4 address {resolvedIp}."
            ));

            return new DnsResolutionResult(
                DomainName: cleanDomain,
                ResolvedIp: resolvedIp,
                TotalLatencyMs: 48,
                Steps: steps,
                Summary: $"Hierarchical resolution completed across 5 stages in 48ms. Record cached with 3600s TTL."
            );
        }
    }
}
