using System;
using System.IO;

namespace BhavaniTech.Core
{
    public static class IconHelper
    {
        public static void ConvertJpgToIco(string inputJpgPath, string outputIcoPath)
        {
            if (!File.Exists(inputJpgPath)) return;

            byte[] imageBytes = File.ReadAllBytes(inputJpgPath);

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);

            // 1. ICONDIR Header (6 bytes)
            writer.Write((ushort)0); // Reserved
            writer.Write((ushort)1); // Resource Type (1 = Icon)
            writer.Write((ushort)1); // Image Count (1)

            // 2. ICONDIRENTRY (16 bytes)
            writer.Write((byte)0);   // Width 256 (0 = 256)
            writer.Write((byte)0);   // Height 256 (0 = 256)
            writer.Write((byte)0);   // Color Count
            writer.Write((byte)0);   // Reserved
            writer.Write((ushort)1); // Color Planes
            writer.Write((ushort)32);// Bits per Pixel (32-bit)
            writer.Write((uint)imageBytes.Length); // Size of Image Data
            writer.Write((uint)22);  // Offset of Image Data (6 + 16 = 22)

            // 3. Image Data (PNG/JPG byte payload)
            writer.Write(imageBytes);

            writer.Flush();
            File.WriteAllBytes(outputIcoPath, ms.ToArray());
        }
    }
}
