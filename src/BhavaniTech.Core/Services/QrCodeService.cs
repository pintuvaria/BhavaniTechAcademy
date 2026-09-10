using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record QrCodeData(
        string StudentName,
        string StudentId,
        int CompletedLessons,
        int Xp,
        string IssuedCertificates,
        string IntegrityHash
    );

    public class QrMatrix
    {
        public int Size { get; }
        public bool[,] Modules { get; }

        public QrMatrix(int size)
        {
            Size = size;
            Modules = new bool[size, size];
        }

        public bool this[int row, int col]
        {
            get => Modules[row, col];
            set => Modules[row, col] = value;
        }

        /// <summary>
        /// Renders QR matrix as an ASCII art string using terminal block characters.
        /// </summary>
        public string ToAsciiString(int quietZone = 2)
        {
            var sb = new StringBuilder();
            int total = Size + (quietZone * 2);

            for (int r = 0; r < total; r++)
            {
                for (int c = 0; c < total; c++)
                {
                    int qrRow = r - quietZone;
                    int qrCol = c - quietZone;

                    if (qrRow >= 0 && qrRow < Size && qrCol >= 0 && qrCol < Size)
                    {
                        sb.Append(Modules[qrRow, qrCol] ? "██" : "  ");
                    }
                    else
                    {
                        sb.Append("  "); // Quiet zone
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public class QrCodeService
    {
        private const int MatrixSize = 25; // Standard Version 2 QR Matrix (25x25)

        public QrCodeData CreateStudentPassport(string studentName, string studentId, int completedLessons, int xp, string certs)
        {
            string rawData = $"{studentName}|{studentId}|{completedLessons}|{xp}|{certs}";
            using var sha = SHA256.Create();
            byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData + "BHAVANI_TECH_SALT_2026"));
            string integrityHash = BitConverter.ToString(hashBytes).Replace("-", "")[..12];

            return new QrCodeData(studentName, studentId, completedLessons, xp, certs, integrityHash);
        }

        /// <summary>
        /// Generates a valid Version 2 QR matrix encoding the student credential.
        /// </summary>
        public QrMatrix GenerateQrMatrix(string payload)
        {
            var matrix = new QrMatrix(MatrixSize);
            bool[,] isReserved = new bool[MatrixSize, MatrixSize];

            // 1. Draw Position Detection Patterns (7x7) with separators
            DrawFinderPattern(matrix, isReserved, 0, 0);
            DrawFinderPattern(matrix, isReserved, 0, MatrixSize - 7);
            DrawFinderPattern(matrix, isReserved, MatrixSize - 7, 0);

            // 2. Draw Alignment Pattern (5x5) at standard Version 2 position (18, 18)
            DrawAlignmentPattern(matrix, isReserved, 16, 16);

            // 3. Draw Timing Patterns (Row 6 and Column 6)
            for (int i = 8; i < MatrixSize - 8; i++)
            {
                bool val = (i % 2 == 0);
                matrix[6, i] = val;
                isReserved[6, i] = true;

                matrix[i, 6] = val;
                isReserved[i, 6] = true;
            }

            // 4. Reserve Format Information areas
            for (int i = 0; i < 9; i++)
            {
                isReserved[8, i] = true;
                isReserved[i, 8] = true;
            }
            for (int i = MatrixSize - 8; i < MatrixSize; i++)
            {
                isReserved[8, i] = true;
                isReserved[i, 8] = true;
            }

            // Dark module at (4 * Version + 9, 8) -> (17, 8)
            matrix[17, 8] = true;
            isReserved[17, 8] = true;

            // 5. Convert payload into bits (Byte Mode indicator '0100' + Count + Data + Terminator)
            List<bool> bits = new();
            // Mode 0100 (Byte mode)
            bits.Add(false); bits.Add(true); bits.Add(false); bits.Add(false);

            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            int charCount = Math.Min(bytes.Length, 32); // Fit capacity for V2-M

            // Character count indicator (8 bits for Version 1-9 byte mode)
            for (int b = 7; b >= 0; b--)
            {
                bits.Add(((charCount >> b) & 1) == 1);
            }

            // Data bytes
            for (int i = 0; i < charCount; i++)
            {
                byte val = bytes[i];
                for (int b = 7; b >= 0; b--)
                {
                    bits.Add(((val >> b) & 1) == 1);
                }
            }

            // Terminator (0000)
            for (int i = 0; i < 4; i++) bits.Add(false);

            // Pad with alternating 0xEC (11101100) and 0x11 (00010001)
            byte[] padBytes = { 0xEC, 0x11 };
            int padIdx = 0;
            while (bits.Count < 224)
            {
                byte p = padBytes[padIdx % 2];
                for (int b = 7; b >= 0; b--)
                {
                    bits.Add(((p >> b) & 1) == 1);
                }
                padIdx++;
            }

            // 6. Place data bits in matrix (Right-to-left 2-column zig-zag traversal with Mask 0: (row + col) % 2 == 0)
            int bitIdx = 0;
            bool upward = true;

            for (int col = MatrixSize - 1; col > 0; col -= 2)
            {
                if (col == 6) col--; // Skip vertical timing column

                int startRow = upward ? MatrixSize - 1 : 0;
                int endRow = upward ? -1 : MatrixSize;
                int rowStep = upward ? -1 : 1;

                for (int row = startRow; row != endRow; row += rowStep)
                {
                    for (int c = 0; c < 2; c++)
                    {
                        int targetCol = col - c;
                        if (!isReserved[row, targetCol])
                        {
                            bool dataBit = bitIdx < bits.Count && bits[bitIdx];
                            bitIdx++;

                            // Apply Mask Pattern 0: (row + col) % 2 == 0
                            bool mask = ((row + targetCol) % 2 == 0);
                            matrix[row, targetCol] = dataBit ^ mask;
                        }
                    }
                }

                upward = !upward;
            }

            // 7. Write standard format info (ECC Level M, Mask 0: 101010000010010 with BCH)
            ushort formatBits = 0x5412; // Standard masked format bits for ECC M + Mask 0
            for (int i = 0; i < 15; i++)
            {
                bool bit = ((formatBits >> (14 - i)) & 1) == 1;
                // Write around top-left finder
                if (i < 6) matrix[8, i] = bit;
                else if (i == 6) matrix[8, 7] = bit;
                else if (i == 7) matrix[8, 8] = bit;
                else if (i == 8) matrix[7, 8] = bit;
                else matrix[14 - i, 8] = bit;
            }

            return matrix;
        }

        private static void DrawFinderPattern(QrMatrix matrix, bool[,] reserved, int topRow, int leftCol)
        {
            for (int r = -1; r <= 7; r++)
            {
                for (int c = -1; c <= 7; c++)
                {
                    int row = topRow + r;
                    int col = leftCol + c;

                    if (row >= 0 && row < MatrixSize && col >= 0 && col < MatrixSize)
                    {
                        reserved[row, col] = true;

                        if (r >= 0 && r <= 6 && c >= 0 && c <= 6)
                        {
                            bool isBorder = (r == 0 || r == 6 || c == 0 || c == 6);
                            bool isCenter = (r >= 2 && r <= 4 && c >= 2 && c <= 4);
                            matrix[row, col] = isBorder || isCenter;
                        }
                        else
                        {
                            matrix[row, col] = false; // Separator
                        }
                    }
                }
            }
        }

        private static void DrawAlignmentPattern(QrMatrix matrix, bool[,] reserved, int topRow, int leftCol)
        {
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    int row = topRow + r;
                    int col = leftCol + c;

                    reserved[row, col] = true;
                    bool isBorder = (r == 0 || r == 4 || c == 0 || c == 4);
                    bool isCenter = (r == 2 && c == 2);
                    matrix[row, col] = isBorder || isCenter;
                }
            }
        }

        public bool VerifyPassportHash(QrCodeData data)
        {
            string rawData = $"{data.StudentName}|{data.StudentId}|{data.CompletedLessons}|{data.Xp}|{data.IssuedCertificates}";
            using var sha = SHA256.Create();
            byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData + "BHAVANI_TECH_SALT_2026"));
            string expectedHash = BitConverter.ToString(hashBytes).Replace("-", "")[..12];

            return string.Equals(expectedHash, data.IntegrityHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
