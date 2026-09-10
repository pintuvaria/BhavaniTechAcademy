using System;
using System.IO;
using System.Text;

namespace BhavaniTech.Core.Services
{
    public enum SoundType
    {
        LevelUp,
        FlagCapture,
        Laser,
        ErrorBuzzer,
        MorseCode,
        Click
    }

    public class SoundSynthesizerService
    {
        private const int SampleRate = 22050; // 22.05 kHz standard retro sample rate

        /// <summary>
        /// Generates a standard RIFF WAVE PCM stream in memory.
        /// </summary>
        public byte[] GenerateWavBuffer(SoundType type, string? extraParam = null)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            // Compute audio samples based on sound type
            short[] samples = type switch
            {
                SoundType.LevelUp => SynthesizeArpeggio(new[] { 523.25, 659.25, 783.99, 1046.50 }, 0.12), // C5-E5-G5-C6
                SoundType.FlagCapture => SynthesizeMajorChords(new[] { 440.0, 554.37, 659.25, 880.0 }, 0.15), // A4-C#5-E5-A5
                SoundType.Laser => SynthesizeFrequencySweep(1800.0, 300.0, 0.18),
                SoundType.ErrorBuzzer => SynthesizeBuzzer(110.0, 0.25),
                SoundType.MorseCode => SynthesizeMorseCode(extraParam ?? "SOS"),
                SoundType.Click => SynthesizeClick(),
                _ => SynthesizeArpeggio(new[] { 440.0, 880.0 }, 0.1)
            };

            int dataChunkSize = samples.Length * sizeof(short);

            // 1. RIFF header
            bw.Write(Encoding.ASCII.GetBytes("RIFF"));
            bw.Write(36 + dataChunkSize); // Total file size - 8
            bw.Write(Encoding.ASCII.GetBytes("WAVE"));

            // 2. "fmt " subchunk
            bw.Write(Encoding.ASCII.GetBytes("fmt "));
            bw.Write(16); // Subchunk1Size for PCM
            bw.Write((short)1); // AudioFormat: 1 = PCM
            bw.Write((short)1); // NumChannels: 1 = Mono
            bw.Write(SampleRate); // SampleRate
            bw.Write(SampleRate * sizeof(short)); // ByteRate (SampleRate * NumChannels * BitsPerSample/8)
            bw.Write((short)2); // BlockAlign (NumChannels * BitsPerSample/8)
            bw.Write((short)16); // BitsPerSample: 16-bit PCM

            // 3. "data" subchunk
            bw.Write(Encoding.ASCII.GetBytes("data"));
            bw.Write(dataChunkSize);

            // Write 16-bit PCM audio samples
            for (int i = 0; i < samples.Length; i++)
            {
                bw.Write(samples[i]);
            }

            bw.Flush();
            return ms.ToArray();
        }

        [System.Runtime.InteropServices.DllImport("winmm.dll", SetLastError = true)]
        private static extern bool PlaySound(byte[] ptrToSound, UIntPtr hmod, uint fdwSound);

        private const uint SND_ASYNC = 0x0001;
        private const uint SND_MEMORY = 0x0004;

        /// <summary>
        /// Plays retro audio effect asynchronously without blocking.
        /// </summary>
        public void PlaySound(SoundType type, string? extraParam = null)
        {
            try
            {
                byte[] wavData = GenerateWavBuffer(type, extraParam);
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    PlaySound(wavData, UIntPtr.Zero, SND_ASYNC | SND_MEMORY);
                }
            }
            catch
            {
                // Silently ignore in headless test environments or systems without audio devices
            }
        }

        #region Chiptune Waveform Synthesizers

        private static short[] SynthesizeArpeggio(double[] frequencies, double noteDurationSec)
        {
            int noteSamples = (int)(SampleRate * noteDurationSec);
            short[] result = new short[frequencies.Length * noteSamples];

            for (int n = 0; n < frequencies.Length; n++)
            {
                double freq = frequencies[n];
                int offset = n * noteSamples;

                for (int i = 0; i < noteSamples; i++)
                {
                    double t = (double)i / SampleRate;
                    // Chiptune Square Wave (duty cycle 50%)
                    double phase = (t * freq) % 1.0;
                    double amp = (phase < 0.5) ? 0.45 : -0.45;

                    // Smooth decay envelope
                    double decay = 1.0 - ((double)i / noteSamples);
                    result[offset + i] = (short)(amp * decay * short.MaxValue);
                }
            }

            return result;
        }

        private static short[] SynthesizeMajorChords(double[] frequencies, double chordDurationSec)
        {
            int totalSamples = (int)(SampleRate * chordDurationSec * 2);
            short[] result = new short[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                double t = (double)i / SampleRate;
                double sum = 0;

                foreach (var freq in frequencies)
                {
                    double phase = (t * freq) % 1.0;
                    sum += (phase < 0.5) ? 0.25 : -0.25;
                }

                double envelope = Math.Exp(-2.5 * t);
                result[i] = (short)(sum * envelope * short.MaxValue);
            }

            return result;
        }

        private static short[] SynthesizeFrequencySweep(double startFreq, double endFreq, double durationSec)
        {
            int totalSamples = (int)(SampleRate * durationSec);
            short[] result = new short[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                double progress = (double)i / totalSamples;
                double currentFreq = startFreq + (endFreq - startFreq) * progress;
                double t = (double)i / SampleRate;

                // Sine wave frequency sweep
                double wave = Math.Sin(2.0 * Math.PI * currentFreq * t);
                double decay = 1.0 - progress;
                result[i] = (short)(wave * decay * 0.6 * short.MaxValue);
            }

            return result;
        }

        private static short[] SynthesizeBuzzer(double freq, double durationSec)
        {
            int totalSamples = (int)(SampleRate * durationSec);
            short[] result = new short[totalSamples];

            for (int i = 0; i < totalSamples; i++)
            {
                double t = (double)i / SampleRate;
                // Harsh sawtooth wave for error
                double wave = 2.0 * ((t * freq) % 1.0) - 1.0;
                result[i] = (short)(wave * 0.5 * short.MaxValue);
            }

            return result;
        }

        private static short[] SynthesizeClick()
        {
            int totalSamples = (int)(SampleRate * 0.02); // 20 ms click
            short[] result = new short[totalSamples];
            var rand = new Random(123);

            for (int i = 0; i < totalSamples; i++)
            {
                double noise = (rand.NextDouble() * 2.0) - 1.0;
                double decay = 1.0 - ((double)i / totalSamples);
                result[i] = (short)(noise * decay * 0.4 * short.MaxValue);
            }

            return result;
        }

        private static short[] SynthesizeMorseCode(string message)
        {
            message = message.ToUpperInvariant();
            const double dotDuration = 0.06;
            const double morseFreq = 750.0; // 750 Hz standard telegraph beep

            var samples = new System.Collections.Generic.List<short>();

            foreach (char c in message)
            {
                string? code = c switch
                {
                    'S' => "...",
                    'O' => "---",
                    'A' => ".-",
                    'B' => "-...",
                    'C' => "-.-.",
                    'D' => "-..",
                    'E' => ".",
                    'H' => "....",
                    'P' => ".--.",
                    _ => "."
                };

                foreach (char symbol in code)
                {
                    double dur = (symbol == '-') ? dotDuration * 3 : dotDuration;
                    int count = (int)(SampleRate * dur);

                    for (int i = 0; i < count; i++)
                    {
                        double t = (double)i / SampleRate;
                        double wave = Math.Sin(2.0 * Math.PI * morseFreq * t);
                        samples.Add((short)(wave * 0.45 * short.MaxValue));
                    }

                    // Intra-symbol pause (1 dot)
                    int pauseCount = (int)(SampleRate * dotDuration);
                    for (int p = 0; p < pauseCount; p++) samples.Add(0);
                }

                // Inter-letter pause (3 dots)
                int letterPause = (int)(SampleRate * dotDuration * 2);
                for (int p = 0; p < letterPause; p++) samples.Add(0);
            }

            return samples.ToArray();
        }

        #endregion
    }
}
