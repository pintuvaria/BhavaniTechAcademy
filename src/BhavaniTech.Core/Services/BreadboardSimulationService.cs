using System;
using System.Collections.Generic;

namespace BhavaniTech.Core.Services
{
    public enum CircuitComponentType
    {
        VoltageSource,
        Resistor,
        Led,
        Capacitor,
        Switch,
        Potentiometer
    }

    public class BreadboardComponent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..6];
        public string Name { get; set; } = "Component";
        public CircuitComponentType Type { get; set; }
        public double Value { get; set; } // Volts, Ohms, Farads (microFarads stored in uF)
        public string Unit { get; set; } = "";
        public bool IsActive { get; set; } = true; // For switches
        public double VoltageDrop { get; set; }
        public double CurrentAmps { get; set; }
        public double PowerWatts { get; set; }
        public string Status { get; set; } = "Normal";
    }

    public record BreadboardCircuitState(
        double SourceVoltage,
        double TotalCurrentAmps,
        double TotalEquivalentResistanceOhms,
        double TotalPowerWatts,
        List<BreadboardComponent> Components,
        string AnalysisSummary,
        bool HasCircuitFault
    );

    public record RcTransientPoint(double TimeMilliseconds, double CapacitorVoltage, double CurrentMilliamps);

    public static class BreadboardSimulationService
    {
        public static BreadboardCircuitState SimulateSeriesCircuit(
            double supplyVoltage,
            List<BreadboardComponent> components)
        {
            if (components == null || components.Count == 0)
            {
                return new BreadboardCircuitState(
                    SourceVoltage: supplyVoltage,
                    TotalCurrentAmps: 0,
                    TotalEquivalentResistanceOhms: 0,
                    TotalPowerWatts: 0,
                    Components: new List<BreadboardComponent>(),
                    AnalysisSummary: "Open circuit: No components connected.",
                    HasCircuitFault: false
                );
            }

            // Check if any open switch exists in the series loop
            bool openSwitch = false;
            foreach (var comp in components)
            {
                if (comp.Type == CircuitComponentType.Switch && !comp.IsActive)
                {
                    openSwitch = true;
                    comp.Status = "OPEN (Circuit Broken)";
                }
            }

            if (openSwitch)
            {
                foreach (var c in components)
                {
                    c.CurrentAmps = 0;
                    c.VoltageDrop = 0;
                    c.PowerWatts = 0;
                    if (c.Type == CircuitComponentType.Led) c.Status = "OFF (No current)";
                }
                return new BreadboardCircuitState(
                    SourceVoltage: supplyVoltage,
                    TotalCurrentAmps: 0,
                    TotalEquivalentResistanceOhms: double.PositiveInfinity,
                    TotalPowerWatts: 0,
                    Components: components,
                    AnalysisSummary: "Current is 0 A because a series switch is in the OPEN state.",
                    HasCircuitFault: false
                );
            }

            double totalResistance = 0;
            double totalLedForwardDrop = 0;
            int ledCount = 0;

            foreach (var comp in components)
            {
                switch (comp.Type)
                {
                    case CircuitComponentType.Resistor:
                    case CircuitComponentType.Potentiometer:
                        totalResistance += Math.Max(0.1, comp.Value);
                        break;
                    case CircuitComponentType.Led:
                        ledCount++;
                        // Standard LED forward drops: Red: 1.8V, Green: 2.1V, Blue: 3.2V (default ~2.0V)
                        double vf = comp.Value > 0 ? comp.Value : 2.0;
                        totalLedForwardDrop += vf;
                        break;
                    case CircuitComponentType.Capacitor:
                        // In steady state DC, ideal capacitor is an open circuit (infinite resistance)
                        totalResistance += 1e9;
                        comp.Status = "DC Steady-State: Fully Charged (Acts as Open Circuit)";
                        break;
                    case CircuitComponentType.Switch:
                        comp.Status = "CLOSED (Conduction Path Active)";
                        break;
                }
            }

            // If no resistor in series with LEDs and supply > Vf, that's a short circuit / blown LED hazard!
            bool fault = false;
            string summary;

            if (totalResistance < 1.0 && ledCount > 0 && supplyVoltage > totalLedForwardDrop)
            {
                fault = true;
                summary = "CRITICAL FAULT: Short Circuit / Missing Current-Limiting Resistor! LEDs will burn out instantly from excessive current.";
                foreach (var c in components)
                {
                    if (c.Type == CircuitComponentType.Led)
                    {
                        c.Status = "DESTROYED / BURNT OUT (Exceeded 500mA rating!)";
                        c.CurrentAmps = (supplyVoltage - totalLedForwardDrop) / 0.5;
                        c.VoltageDrop = c.Value > 0 ? c.Value : 2.0;
                    }
                }
                return new BreadboardCircuitState(
                    SourceVoltage: supplyVoltage,
                    TotalCurrentAmps: 5.0,
                    TotalEquivalentResistanceOhms: totalResistance,
                    TotalPowerWatts: supplyVoltage * 5.0,
                    Components: components,
                    AnalysisSummary: summary,
                    HasCircuitFault: true
                );
            }

            double netVoltage = Math.Max(0, supplyVoltage - totalLedForwardDrop);
            double loopCurrent = totalResistance > 0 ? netVoltage / totalResistance : 0;
            double totalPower = supplyVoltage * loopCurrent;

            foreach (var comp in components)
            {
                comp.CurrentAmps = Math.Round(loopCurrent, 6);
                switch (comp.Type)
                {
                    case CircuitComponentType.Resistor:
                    case CircuitComponentType.Potentiometer:
                        comp.VoltageDrop = Math.Round(loopCurrent * comp.Value, 3);
                        comp.PowerWatts = Math.Round(loopCurrent * loopCurrent * comp.Value, 4);
                        comp.Status = comp.PowerWatts > 0.25 
                            ? $"Dissipating {comp.PowerWatts:F3}W (Caution: Exceeds 1/4W rating!)" 
                            : $"Operating normally ({comp.PowerWatts * 1000:F1} mW)";
                        break;

                    case CircuitComponentType.Led:
                        double vf = comp.Value > 0 ? comp.Value : 2.0;
                        if (supplyVoltage < vf)
                        {
                            comp.VoltageDrop = supplyVoltage;
                            comp.Status = "OFF (Supply voltage < Forward voltage drop)";
                        }
                        else if (loopCurrent > 0.030)
                        {
                            comp.VoltageDrop = vf;
                            comp.Status = $"OVERLOAD: {loopCurrent * 1000:F1} mA (Standard 20mA max! Shortened lifespan)";
                        }
                        else if (loopCurrent >= 0.005)
                        {
                            comp.VoltageDrop = vf;
                            comp.Status = $"ON (Luminance: Optimal {loopCurrent * 1000:F1} mA)";
                        }
                        else
                        {
                            comp.VoltageDrop = vf;
                            comp.Status = $"ON (Dim: Low current {loopCurrent * 1000:F1} mA)";
                        }
                        comp.PowerWatts = Math.Round(comp.VoltageDrop * loopCurrent, 4);
                        break;

                    case CircuitComponentType.Capacitor:
                        comp.VoltageDrop = Math.Min(supplyVoltage, 12.0);
                        comp.PowerWatts = 0;
                        break;
                }
            }

            summary = $"Circuit Active: $I = {loopCurrent * 1000:F2} mA$, Total $R = {totalResistance:F1}\\ \\Omega$, Power = {totalPower * 1000:F1} mW. Kirchhoff's Voltage Law: $\\sum V = {supplyVoltage:F1}V$.";

            return new BreadboardCircuitState(
                SourceVoltage: supplyVoltage,
                TotalCurrentAmps: loopCurrent,
                TotalEquivalentResistanceOhms: totalResistance,
                TotalPowerWatts: totalPower,
                Components: components,
                AnalysisSummary: summary,
                HasCircuitFault: fault
            );
        }

        public static List<RcTransientPoint> CalculateRcTransient(double supplyVoltage, double resistanceOhms, double capacitanceMicroFarads, int steps = 20)
        {
            var points = new List<RcTransientPoint>();
            double capFarads = capacitanceMicroFarads * 1e-6;
            double tauSeconds = resistanceOhms * capFarads; // RC time constant
            double totalTimeSeconds = 5 * tauSeconds; // 5*tau is ~99.3% charged
            if (totalTimeSeconds <= 0) totalTimeSeconds = 0.01;

            double dt = totalTimeSeconds / steps;

            for (int i = 0; i <= steps; i++)
            {
                double t = i * dt;
                // Vc(t) = Vs * (1 - e^(-t / RC))
                double exponent = tauSeconds > 0 ? -t / tauSeconds : 0;
                double vc = supplyVoltage * (1.0 - Math.Exp(exponent));
                // I(t) = (Vs / R) * e^(-t / RC)
                double icAmps = resistanceOhms > 0 ? (supplyVoltage / resistanceOhms) * Math.Exp(exponent) : 0;

                points.Add(new RcTransientPoint(
                    TimeMilliseconds: Math.Round(t * 1000.0, 3),
                    CapacitorVoltage: Math.Round(vc, 3),
                    CurrentMilliamps: Math.Round(icAmps * 1000.0, 3)
                ));
            }

            return points;
        }
    }
}
