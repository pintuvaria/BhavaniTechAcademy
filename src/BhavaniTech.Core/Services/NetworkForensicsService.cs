using System;
using System.Collections.Generic;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public class NetworkForensicsService
    {
        public record PcapAnalysisResult(bool FoundCleartextAuth, string ExtractedData, string HexDump);

        public record DecodedPacketFrame(
            string EthernetSourceMac,
            string EthernetDestMac,
            string EtherType,
            string IpSource,
            string IpDest,
            int IpTtl,
            string Protocol,
            int SourcePort,
            int DestPort,
            uint SequenceNumber,
            uint AckNumber,
            List<string> TcpFlags,
            string PayloadAscii
        );

        public record AdvancedPcapResult(
            bool FoundCleartextAuth,
            string ExtractedData,
            string HexDump,
            DecodedPacketFrame? DecodedFrame
        );

        public AdvancedPcapResult AnalyzeSimulatedPcap(string protocol, string payload)
        {
            // Simulate generating a hex dump
            var hexDump = new StringBuilder();
            byte[] bytes = Encoding.ASCII.GetBytes(payload);
            for (int i = 0; i < bytes.Length; i += 16)
            {
                hexDump.Append($"{i:X4}  ");
                for (int j = 0; j < 16; j++)
                {
                    if (i + j < bytes.Length)
                        hexDump.Append($"{bytes[i + j]:X2} ");
                    else
                        hexDump.Append("   ");
                }
                hexDump.Append(" ");
                for (int j = 0; j < 16; j++)
                {
                    if (i + j < bytes.Length)
                    {
                        char c = (char)bytes[i + j];
                        hexDump.Append(char.IsControl(c) ? '.' : c);
                    }
                }
                hexDump.AppendLine();
            }

            bool foundCleartext = false;
            string extracted = "No sensitive data found.";

            string protoUpper = protocol.ToUpperInvariant();
            if (protoUpper is "FTP" or "HTTP" or "TELNET")
            {
                if (payload.Contains("USER ") || payload.Contains("PASS ") || payload.Contains("Authorization: Basic"))
                {
                    foundCleartext = true;
                    if (payload.Contains("Authorization: Basic "))
                    {
                        int start = payload.IndexOf("Authorization: Basic ") + "Authorization: Basic ".Length;
                        int end = payload.IndexOf("\r\n", start);
                        string b64 = end > start ? payload[start..end].Trim() : payload[start..].Trim();
                        try
                        {
                            string creds = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
                            extracted = $"Warning: Basic Auth Credentials decoded: '{creds}'";
                        }
                        catch
                        {
                            extracted = "Warning: Cleartext credentials detected on unencrypted protocol!";
                        }
                    }
                    else
                    {
                        extracted = "Warning: Cleartext credentials detected on unencrypted protocol!";
                    }
                }
            }

            var tcpFlags = new List<string> { "ACK", "PSH" };
            if (payload.Contains("SYN")) tcpFlags.Add("SYN");
            if (payload.Contains("FIN")) tcpFlags.Add("FIN");

            var frame = new DecodedPacketFrame(
                EthernetSourceMac: "00:1A:2B:3C:4D:5E",
                EthernetDestMac: "00:11:22:33:44:55",
                EtherType: "IPv4 (0x0800)",
                IpSource: "192.168.1.105",
                IpDest: "10.0.0.1",
                IpTtl: 64,
                Protocol: protoUpper,
                SourcePort: 54321,
                DestPort: protoUpper == "FTP" ? 21 : protoUpper == "TELNET" ? 23 : 80,
                SequenceNumber: 10024501,
                AckNumber: 9948210,
                TcpFlags: tcpFlags,
                PayloadAscii: payload.Trim()
            );

            return new AdvancedPcapResult(foundCleartext, extracted, hexDump.ToString(), frame);
        }
    }
}
