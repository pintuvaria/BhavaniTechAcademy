using System;
using System.Collections.Generic;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public record DigitalTimingWaveform(
        string SignalName,
        string WaveformAscii,
        List<int> LogicLevels
    );

    public record UartFrameAnalysis(
        char Character,
        byte AsciiByte,
        int BaudRate,
        double BitTimeMicroseconds,
        string BinaryLsbFirst,
        string TimingDiagram,
        string ProtocolTrace
    );

    public record I2cPacketTrace(
        byte SlaveAddress7Bit,
        bool IsRead,
        byte DataByte,
        bool AckReceived,
        List<DigitalTimingWaveform> Waveforms,
        string ProtocolTrace
    );

    public record SpiTransactionTrace(
        int SpiMode, // 0, 1, 2, 3
        int ClockPolarityCpol,
        int ClockPhaseCpha,
        byte MosiByte,
        byte MisoByte,
        List<DigitalTimingWaveform> Waveforms,
        string ProtocolTrace
    );

    public record JtagTapTrace(
        List<string> TapStatesVisited,
        string InstructionRegisterLoaded,
        string DataRegisterShifted,
        string ProtocolTrace
    );

    public class HardwareProtocolsService
    {
        public UartFrameAnalysis AnalyzeUart(char ch, int baudRate = 115200)
        {
            byte b = (byte)ch;
            double bitTimeUs = Math.Round(1_000_000.0 / baudRate, 2);

            // 8 Data bits, LSB first
            var lsbBits = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                lsbBits.Append((b & (1 << i)) != 0 ? "1" : "0");
            }
            string bitStr = lsbBits.ToString();

            // Form timing ASCII: Idle(1) -> Start(0) -> 8 Bits -> Stop(1)
            var wave = new StringBuilder();
            wave.Append("TX LINE: ¯¯¯|_START_|");
            for (int i = 0; i < 8; i++)
            {
                wave.Append(bitStr[i] == '1' ? "¯D" + i + "¯|" : "_D" + i + "_|");
            }
            wave.Append("¯STOP¯|¯¯¯");

            var sb = new StringBuilder();
            sb.AppendLine("=== UART SERIAL ASYNCHRONOUS FRAME DECODER ===");
            sb.AppendLine($"Transmitted Character : '{ch}' (ASCII: 0x{b:X2} / Decimal: {b})");
            sb.AppendLine($"Configured Baud Rate  : {baudRate} Baud (Symbols/sec)");
            sb.AppendLine($"Single Bit Time (T_bit): {bitTimeUs} µs");
            sb.AppendLine($"Total 10-Bit Frame Time: {Math.Round(bitTimeUs * 10, 2)} µs");
            sb.AppendLine($"Frame Structure       : 1 Start bit (LOW) + 8 Data bits (LSB First: {bitStr}) + 1 Stop bit (HIGH)");
            sb.AppendLine("\nTIMING WAVEFORM DIAGRAM:\n" + wave);

            return new UartFrameAnalysis(ch, b, baudRate, bitTimeUs, bitStr, wave.ToString(), sb.ToString());
        }

        public I2cPacketTrace SimulateI2c(byte address7Bit, bool isRead, byte dataByte)
        {
            var waveforms = new List<DigitalTimingWaveform>();

            // SDA & SCL waveforms representation
            string sdaWave = "SDA: ¯¯¯\\_START_/" + $"[ADDR:0x{address7Bit:X2}]" + $"[R/W:{(isRead ? 1 : 0)}]" + "[ACK:0]" + $"[DATA:0x{dataByte:X2}]" + "[ACK:0]" + "_/¯STOP¯";
            string sclWave = "SCL: ¯¯¯_CLK_1_2_3_4_5_6_7_8_[9:ACK]_CLK_1_2_3_4_5_6_7_8_[9:ACK]¯¯¯¯¯¯¯";

            waveforms.Add(new DigitalTimingWaveform("SDA (Serial Data)", sdaWave, new List<int> { 1, 0, 1, 1, 0, 0, 1 }));
            waveforms.Add(new DigitalTimingWaveform("SCL (Serial Clock)", sclWave, new List<int> { 1, 0, 1, 0, 1, 0, 1 }));

            var sb = new StringBuilder();
            sb.AppendLine("=== I2C (INTER-INTEGRATED CIRCUIT) 2-WIRE BUS TRACE ===");
            sb.AppendLine("1. [START CONDITION] : SDA transitioned HIGH -> LOW while SCL remained HIGH.");
            sb.AppendLine($"2. [SLAVE ADDRESSING] : 7-bit Address 0x{address7Bit:X2} transmitted on bus.");
            sb.AppendLine($"3. [DIRECTION BIT]    : {(isRead ? "1 (READ from Slave)" : "0 (WRITE to Slave)")}.");
            sb.AppendLine("4. [ACKNOWLEDGE 1]   : Target peripheral pulled SDA LOW on 9th clock pulse (ACK confirmed).");
            sb.AppendLine($"5. [DATA PAYLOAD]     : Byte 0x{dataByte:X2} (Decimal {dataByte}) transferred MSB first.");
            sb.AppendLine("6. [ACKNOWLEDGE 2]   : Receiver pulled SDA LOW on 18th clock pulse (ACK confirmed).");
            sb.AppendLine("7. [STOP CONDITION]  : SDA transitioned LOW -> HIGH while SCL remained HIGH (Bus Released).");

            return new I2cPacketTrace(address7Bit, isRead, dataByte, true, waveforms, sb.ToString());
        }

        public SpiTransactionTrace SimulateSpi(int mode, byte mosiByte, byte misoByte = 0xAA)
        {
            int cpol = (mode == 2 || mode == 3) ? 1 : 0;
            int cpha = (mode == 1 || mode == 3) ? 1 : 0;

            var waveforms = new List<DigitalTimingWaveform>();
            string csWave = "CS  : ¯¯¯\\__________________ACTIVE_LOW___________________/¯¯¯";
            string sckWave = cpol == 0 ? "SCK : ___/¯\\_/¯\\_/¯\\_/¯\\_/¯\\_/¯\\_/¯\\_/¯\\___________________" : "SCK : ¯¯¯\\_/_\\_/_\\_/_\\_/_\\_/_\\_/_\\_/_\\_/_¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯";
            string mosiWave = $"MOSI: ---[B7][B6][B5][B4][B3][B2][B1][B0]:0x{mosiByte:X2}-------------";
            string misoWave = $"MISO: ---[B7][B6][B5][B4][B3][B2][B1][B0]:0x{misoByte:X2}-------------";

            waveforms.Add(new DigitalTimingWaveform("CS (Chip Select)", csWave, new List<int> { 1, 0, 0, 1 }));
            waveforms.Add(new DigitalTimingWaveform("SCK (Serial Clock)", sckWave, new List<int> { 0, 1, 0, 1 }));
            waveforms.Add(new DigitalTimingWaveform("MOSI (Master Out)", mosiWave, new List<int> { 0, 1, 1, 0 }));
            waveforms.Add(new DigitalTimingWaveform("MISO (Master In)", misoWave, new List<int> { 1, 0, 1, 0 }));

            var sb = new StringBuilder();
            sb.AppendLine("=== SPI (SERIAL PERIPHERAL INTERFACE) 4-WIRE BUS TRACE ===");
            sb.AppendLine($"Configuration: SPI Mode {mode} (Clock Polarity CPOL={cpol}, Clock Phase CPHA={cpha})");
            sb.AppendLine($"Clock Idle State : {(cpol == 0 ? "LOW (0V)" : "HIGH (VCC)")}");
            sb.AppendLine($"Data Latch Edge  : {(cpha == 0 ? "Leading Clock Edge" : "Trailing Clock Edge")}");
            sb.AppendLine($"Full-Duplex Byte Transfer: Master sent 0x{mosiByte:X2} | Peripheral returned 0x{misoByte:X2}");
            sb.AppendLine("CS Line asserted LOW throughout all 8 clock cycles with zero addressing overhead.");

            return new SpiTransactionTrace(mode, cpol, cpha, mosiByte, misoByte, waveforms, sb.ToString());
        }

        public JtagTapTrace SimulateJtagBoundaryScan()
        {
            var states = new List<string>
            {
                "Test-Logic-Reset (TMS=1 for 5 cycles)",
                "Run-Test/Idle",
                "Select-DR-Scan",
                "Select-IR-Scan",
                "Capture-IR",
                "Shift-IR (Shifting opcode 'IDCODE' 0x02 into IR register)",
                "Exit1-IR -> Update-IR (Latching IDCODE)",
                "Select-DR-Scan",
                "Capture-DR (Capturing 32-bit Device ID into Shift Register)",
                "Shift-DR (Clocking out 32-bit Chip Silicon ID on TDO pin)",
                "Exit1-DR -> Update-DR",
                "Run-Test/Idle"
            };

            var sb = new StringBuilder();
            sb.AppendLine("=== JTAG (IEEE 1149.1) TAP CONTROLLER BOUNDARY-SCAN TRACE ===");
            sb.AppendLine("Navigating 16-State Finite State Machine via TMS (Test Mode Select) line:");
            foreach (var st in states)
            {
                sb.AppendLine($" -> State: {st}");
            }
            sb.AppendLine("\nTarget Silicon Identification Code Read (IDCODE):");
            sb.AppendLine("0x00280001 (Manufacturer: 0x01 | Part: 0x0280 | Version: 0x0)");

            return new JtagTapTrace(states, "IDCODE (0x02)", "0x00280001", sb.ToString());
        }
    }
}
