using System;
using System.Net;

namespace BhavaniTech.Core.Services
{
    public static class SubnettingEngine
    {
        public static Models.SubnetResult CalculateSubnet(string ipAddress, int cidr)
        {
            if (cidr < 0 || cidr > 32) throw new ArgumentOutOfRangeException(nameof(cidr), "CIDR must be between 0 and 32.");
            if (!IPAddress.TryParse(ipAddress, out var parsedIp)) throw new ArgumentException("Invalid IP address format.");

            uint ipBytes = IpToUint(parsedIp);
            uint maskBytes = cidr == 0 ? 0 : 0xFFFFFFFF << (32 - cidr);
            uint netBytes = ipBytes & maskBytes;
            uint bcastBytes = netBytes | ~maskBytes;
            uint wildcardBytes = ~maskBytes;

            uint firstUsable = (cidr >= 31) ? netBytes : netBytes + 1;
            uint lastUsable = (cidr >= 31) ? bcastBytes : bcastBytes - 1;
            long usableHosts = (cidr >= 31) ? 0 : (long)(bcastBytes - netBytes - 1);

            return new Models.SubnetResult(
                NetworkAddress: UintToIp(netBytes).ToString(),
                BroadcastAddress: UintToIp(bcastBytes).ToString(),
                FirstUsableIP: UintToIp(firstUsable).ToString(),
                LastUsableIP: UintToIp(lastUsable).ToString(),
                UsableHosts: Math.Max(0, usableHosts),
                Netmask: UintToIp(maskBytes).ToString(),
                CIDR: cidr,
                WildcardMask: UintToIp(wildcardBytes).ToString()
            );
        }

        private static uint IpToUint(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();
            Array.Reverse(bytes); // Big-endian to Little-endian uint
            return BitConverter.ToUInt32(bytes, 0);
        }

        private static IPAddress UintToIp(uint address)
        {
            byte[] bytes = BitConverter.GetBytes(address);
            Array.Reverse(bytes);
            return new IPAddress(bytes);
        }
    }
}
