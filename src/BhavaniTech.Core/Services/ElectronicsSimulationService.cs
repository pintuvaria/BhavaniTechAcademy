using System;

namespace BhavaniTech.Core.Services
{
    public record OhmsLawResult(double Voltage, double CurrentAmps, double ResistanceOhms, double PowerWatts);
    public record LedResistorResult(double RequiredResistorOhms, double RecommendedStandardResistorOhms, double ResistorPowerWatts);

    public record QuantumGateResult(
        string GateApplied,
        double InitialAlpha,
        double InitialBeta,
        double FinalAlpha,
        double FinalBeta,
        double ProbZero,
        double ProbOne,
        int MeasuredState,
        string QuantumExplanation
    );

    public static class ElectronicsSimulationService
    {
        public static OhmsLawResult CalculateOhmsLaw(double? v, double? i, double? r)
        {
            double voltage = v ?? 0;
            double current = i ?? 0;
            double resistance = r ?? 0;

            if (v.HasValue && i.HasValue && !r.HasValue)
            {
                resistance = current > 0 ? voltage / current : 0;
            }
            else if (v.HasValue && r.HasValue && !i.HasValue)
            {
                current = resistance > 0 ? voltage / resistance : 0;
            }
            else if (i.HasValue && r.HasValue && !v.HasValue)
            {
                voltage = current * resistance;
            }

            double power = voltage * current;
            return new OhmsLawResult(
                Math.Round(voltage, 2),
                Math.Round(current, 4),
                Math.Round(resistance, 2),
                Math.Round(power, 4)
            );
        }

        public static LedResistorResult CalculateLedResistor(double supplyVoltage, double ledForwardVoltage, double ledCurrentAmps)
        {
            if (supplyVoltage <= ledForwardVoltage || ledCurrentAmps <= 0)
            {
                return new LedResistorResult(0, 0, 0);
            }

            double reqR = (supplyVoltage - ledForwardVoltage) / ledCurrentAmps;
            double pWatts = (supplyVoltage - ledForwardVoltage) * ledCurrentAmps;

            // Nearest standard E24 resistor value
            double[] standardResistors = { 100, 150, 220, 330, 470, 680, 1000, 2200, 4700, 10000 };
            double recR = reqR;
            foreach (var s in standardResistors)
            {
                if (s >= reqR)
                {
                    recR = s;
                    break;
                }
            }

            return new LedResistorResult(
                Math.Round(reqR, 1),
                recR,
                Math.Round(pWatts, 3)
            );
        }

        public static bool EvaluateLogicGate(string gateType, bool inA, bool inB)
        {
            return gateType.ToUpperInvariant() switch
            {
                "AND" => inA && inB,
                "OR" => inA || inB,
                "NOT" => !inA,
                "NAND" => !(inA && inB),
                "NOR" => !(inA || inB),
                "XOR" => inA ^ inB,
                _ => false
            };
        }

        // =====================================================================
        // QUANTUM QUBIT COMPUTING & SUPERPOSITION SIMULATOR
        // =====================================================================
        public static QuantumGateResult SimulateQuantumGate(string gateType = "Hadamard (H)", double alpha = 1.0, double beta = 0.0)
        {
            // Normalize state vector: |α|^2 + |β|^2 = 1
            double norm = Math.Sqrt((alpha * alpha) + (beta * beta));
            if (norm < 0.00001) { alpha = 1.0; beta = 0.0; norm = 1.0; }
            alpha /= norm;
            beta /= norm;

            double finalAlpha = alpha;
            double finalBeta = beta;
            string gateClean = gateType.ToUpperInvariant();

            if (gateClean.Contains("HADAMARD") || gateClean == "H")
            {
                // H = 1/sqrt(2) * [ [1, 1], [1, -1] ]
                double invSqrt2 = 1.0 / Math.Sqrt(2.0);
                finalAlpha = invSqrt2 * (alpha + beta);
                finalBeta = invSqrt2 * (alpha - beta);
            }
            else if (gateClean.Contains("PAULI-X") || gateClean.Contains("NOT") || gateClean == "X")
            {
                // X = [ [0, 1], [1, 0] ]
                finalAlpha = beta;
                finalBeta = alpha;
            }
            else if (gateClean.Contains("PAULI-Z") || gateClean.Contains("PHASE") || gateClean == "Z")
            {
                // Z = [ [1, 0], [0, -1] ]
                finalAlpha = alpha;
                finalBeta = -beta;
            }

            double p0 = Math.Round(finalAlpha * finalAlpha, 4);
            double p1 = Math.Round(finalBeta * finalBeta, 4);

            // Simulate quantum wave function collapse upon physical measurement
            var rand = new Random();
            int measured = (rand.NextDouble() < p0) ? 0 : 1;

            var expl = new System.Text.StringBuilder();
            expl.AppendLine($"=== QUANTUM STATE EVOLUTION: {gateType} ===");
            expl.AppendLine($"Initial State: |ψ⟩ = {Math.Round(alpha, 3)}|0⟩ + {Math.Round(beta, 3)}|1⟩");
            expl.AppendLine($"Final State:   |ψ'⟩ = {Math.Round(finalAlpha, 3)}|0⟩ + {Math.Round(finalBeta, 3)}|1⟩");
            expl.AppendLine($"Measurement Probabilities: P(|0⟩) = {p0 * 100:0.0}%, P(|1⟩) = {p1 * 100:0.0}%");
            expl.AppendLine($"Wave Function Collapse Result: Measured state |{measured}⟩");
            if (gateClean.Contains("HADAMARD") || gateClean == "H")
            {
                expl.AppendLine("Quantum Note: The Hadamard gate transformed a deterministic basis state into true 50/50 quantum superposition!");
            }
            else if (gateClean.Contains("X"))
            {
                expl.AppendLine("Quantum Note: The Pauli-X gate inverted the qubit amplitude, acting as a Quantum Bit-Flip NOT gate.");
            }

            return new QuantumGateResult(
                gateType,
                Math.Round(alpha, 4),
                Math.Round(beta, 4),
                Math.Round(finalAlpha, 4),
                Math.Round(finalBeta, 4),
                p0,
                p1,
                measured,
                expl.ToString()
            );
        }
    }
}
