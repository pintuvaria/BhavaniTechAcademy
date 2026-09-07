using System;
using System.Collections.Generic;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public class NetworkForensicsService
    {
        public record PcapAnalysisResult(bool FoundCleartextAuth, string ExtractedData, string HexDump);

        public PcapAnalysisResult AnalyzeSimulatedPcap(string protocol, string payload)
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

            if (protocol.ToUpperInvariant() == "FTP" || protocol.ToUpperInvariant() == "HTTP")
            {
                if (payload.Contains("USER ") || payload.Contains("PASS ") || payload.Contains("Authorization: Basic"))
                {
                    foundCleartext = true;
                    extracted = "Warning: Cleartext credentials detected on unencrypted protocol!";
                }
            }

            return new PcapAnalysisResult(foundCleartext, extracted, hexDump.ToString());
        }
    }
}
