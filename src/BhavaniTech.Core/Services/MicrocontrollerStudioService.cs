using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public enum PinModeType
    {
        Input,
        Output,
        Pwm,
        AnalogInput
    }

    public class MicroPin
    {
        public int PinNumber { get; set; }
        public string Label { get; set; } = "";
        public PinModeType Mode { get; set; } = PinModeType.Output;
        public bool DigitalValue { get; set; } = false; // HIGH / LOW
        public byte PwmValue { get; set; } = 0; // 0 to 255
        public int AnalogValue { get; set; } = 0; // 0 to 1023 (10-bit)
        public double Voltage => Mode switch
        {
            PinModeType.AnalogInput => Math.Round((AnalogValue / 1023.0) * 5.0, 2),
            PinModeType.Pwm => Math.Round((PwmValue / 255.0) * 5.0, 2),
            PinModeType.Output => DigitalValue ? 5.0 : 0.0,
            _ => 0.0
        };
    }

    public record OscilloscopeSample(double TimeMicroseconds, double Voltage);

    public record MicrocontrollerState(
        string BoardType,
        double OperatingVoltage,
        int ClockSpeedMhz,
        List<MicroPin> Pins,
        double ServoAngleDegrees,
        double UltrasonicDistanceCm,
        string ConsoleLog
    );

    public static class MicrocontrollerStudioService
    {
        public static List<MicroPin> InitializeUnoPins()
        {
            var pins = new List<MicroPin>();
            // Digital Pins 0 - 13 (PWM on 3, 5, 6, 9, 10, 11)
            for (int i = 0; i <= 13; i++)
            {
                bool isPwmCapable = (i == 3 || i == 5 || i == 6 || i == 9 || i == 10 || i == 11);
                pins.Add(new MicroPin
                {
                    PinNumber = i,
                    Label = isPwmCapable ? $"~D{i} (PWM)" : $"D{i}",
                    Mode = isPwmCapable ? PinModeType.Pwm : PinModeType.Output,
                    DigitalValue = false,
                    PwmValue = 0
                });
            }
            // Analog Pins A0 - A5
            for (int a = 0; a <= 5; a++)
            {
                pins.Add(new MicroPin
                {
                    PinNumber = 100 + a,
                    Label = $"A{a}",
                    Mode = PinModeType.AnalogInput,
                    AnalogValue = 512
                });
            }
            return pins;
        }

        public static List<OscilloscopeSample> GeneratePwmWaveform(byte pwmValue, double frequencyHz = 490.0, int cycles = 3)
        {
            var samples = new List<OscilloscopeSample>();
            double periodUs = (1.0 / frequencyHz) * 1_000_000.0;
            double dutyCycleRatio = pwmValue / 255.0;
            double highTimeUs = periodUs * dutyCycleRatio;

            int sampleCount = 60;
            double dt = (periodUs * cycles) / sampleCount;

            for (int i = 0; i <= sampleCount; i++)
            {
                double t = i * dt;
                double timeInCycle = t % periodUs;
                double voltage = (timeInCycle < highTimeUs) ? 5.0 : 0.0;
                samples.Add(new OscilloscopeSample(Math.Round(t, 1), voltage));
            }

            return samples;
        }

        public static double CalculateServoAngle(byte pwmValue)
        {
            // Standard hobby servo: 0 to 255 maps to 0° to 180°
            return Math.Round((pwmValue / 255.0) * 180.0, 1);
        }

        public static double CalculateUltrasonicDistance(double pulseEchoMicroseconds)
        {
            // Speed of sound = 343 m/s = 0.0343 cm/us
            // Distance = (Time * 0.0343) / 2
            double dist = (pulseEchoMicroseconds * 0.0343) / 2.0;
            return Math.Round(dist, 1);
        }

        public static MicrocontrollerState ExecuteSketch(string sketchName, byte sliderPwm, int sensorAdc)
        {
            var pins = InitializeUnoPins();
            string log = "";
            double servoAngle = 0;
            double distCm = 0;

            switch (sketchName.ToLowerInvariant())
            {
                case "blink":
                    // Pin 13 built-in LED
                    var d13 = pins.Find(p => p.PinNumber == 13);
                    if (d13 != null) d13.DigitalValue = true;
                    log = "[SKETCH: Blink]\nvoid setup() { pinMode(13, OUTPUT); }\nvoid loop() {\n  digitalWrite(13, HIGH); // LED ON (5V)\n  delay(1000);\n  digitalWrite(13, LOW);  // LED OFF (0V)\n  delay(1000);\n}\n>> D13 LED is illuminated.";
                    break;

                case "servo":
                    // Servo on PWM Pin 9
                    var d9 = pins.Find(p => p.PinNumber == 9);
                    if (d9 != null)
                    {
                        d9.Mode = PinModeType.Pwm;
                        d9.PwmValue = sliderPwm;
                    }
                    servoAngle = CalculateServoAngle(sliderPwm);
                    log = $"[SKETCH: Servo Motor Control]\n#include <Servo.h>\nServo myServo;\nmyServo.attach(9);\nmyServo.write({(int)servoAngle}); // Pulse Width: {1000 + (sliderPwm * 1000 / 255)}us\n>> Servo horn rotated to {servoAngle} degrees.";
                    break;

                case "ultrasonic":
                    // HC-SR04 Trigger D7, Echo D8
                    double echoUs = 1457.0; // Simulated echo time for ~25cm
                    distCm = CalculateUltrasonicDistance(echoUs);
                    var d7 = pins.Find(p => p.PinNumber == 7);
                    var d8 = pins.Find(p => p.PinNumber == 8);
                    if (d7 != null) d7.DigitalValue = true;
                    if (d8 != null) d8.DigitalValue = true;
                    log = $"[SKETCH: HC-SR04 Ultrasonic Ranging]\ntriggerPulse(pin 7);\nechoDuration = pulseIn(pin 8, HIGH); // {echoUs} us\ndistance = (echoDuration * 0.0343) / 2;\n>> Detected obstacle distance: {distCm} cm";
                    break;

                case "adc_read":
                default:
                    // Read Potentiometer on A0, Output PWM on D3
                    var a0 = pins.Find(p => p.PinNumber == 100);
                    var d3 = pins.Find(p => p.PinNumber == 3);
                    if (a0 != null) a0.AnalogValue = sensorAdc;
                    byte mappedPwm = (byte)(sensorAdc / 4); // 0-1023 -> 0-255
                    if (d3 != null)
                    {
                        d3.PwmValue = mappedPwm;
                    }
                    log = $"[SKETCH: Analog Potentiometer to PWM Dimmer]\nint rawVal = analogRead(A0); // {sensorAdc} (Voltage: {sensorAdc * 5.0 / 1023:F2}V)\nint pwmVal = map(rawVal, 0, 1023, 0, 255); // {mappedPwm}\nanalogWrite(3, pwmVal);\n>> LED brightness driven by PWM duty cycle: {(mappedPwm / 255.0) * 100:F1}%.";
                    break;
            }

            return new MicrocontrollerState(
                BoardType: "ATmega328P (Arduino Uno R3)",
                OperatingVoltage: 5.0,
                ClockSpeedMhz: 16,
                Pins: pins,
                ServoAngleDegrees: servoAngle,
                UltrasonicDistanceCm: distCm,
                ConsoleLog: log
            );
        }
    }
}
