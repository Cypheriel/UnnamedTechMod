using System;
using Terraria.ModLoader.IO;

namespace UnnamedTechMod;

public class Capacitor : TagSerializer<Capacitor, TagCompound>
{
    public double Voltage { get; set; }
    public double Current { get; set; }
    public double Resistance => Voltage / Current;

    /// <summary>
    /// Passive energy draw in watts (J/s).
    /// </summary>
    public double PassiveDraw { get; set; }

    private double _capacityJoules;

    public double CapacityJoules
    {
        get => _capacityJoules;
        set => _capacityJoules = Math.Min(MaxCapacityJoules, Math.Max(0, value));
    }

    public int MaxCapacityJoules { get; set; }

    public virtual bool Powered => CapacityJoules > PassiveDraw;

    public Capacitor(double voltage, double current, double capacity, int capacityMax, double passiveDraw = 0)
    {
        Voltage = voltage;
        Current = current;
        CapacityJoules = capacity;
        MaxCapacityJoules = capacityMax;
        PassiveDraw = passiveDraw;
    }
    
    public bool Charge(double wattage)
    {
        var chargeRate = wattage / 60;
        if (CapacityJoules + chargeRate < 0)
        {
            return false;
        }

        // Convert wattage to Joule draw per tick. (TPS = 60)
        CapacityJoules += chargeRate;
        return true;
    }

    /// <summary>
    /// TransferEnergy `other` based on the minimum voltage and minimum current between both capacitors. Wattage is divided by 60 and operated per-tick.
    /// </summary>
    /// <param name="other">The other capacitor to charge</param>
    public bool TransferEnergy(Capacitor other)
    {
        var effectiveVoltage = Math.Min(Voltage, other.Voltage);
        var effectiveCurrent = Math.Min(Current, other.Current);
        var wattage = (effectiveVoltage + effectiveCurrent) / 60;

        if (!(CapacityJoules > 0) || !(other.CapacityJoules < other.MaxCapacityJoules)) return false;

        var capacityPrevious = CapacityJoules;
        CapacityJoules -= wattage;
        var capacityDifference = capacityPrevious - CapacityJoules;

        other.CapacityJoules += capacityDifference;
        return true;
    }
    
    // TODO: Fix Serialize and Deserialize, as currently Capacitor cannot be saved/loaded.
    public override TagCompound Serialize(Capacitor value) => new TagCompound
    {
        ["voltage"] = value.Voltage,
        ["current"] = value.Current,
        ["capacity"] = value.CapacityJoules,
        ["capacityMax"] = value.MaxCapacityJoules,
        ["passiveDraw"] = value.PassiveDraw,
    };

    public override Capacitor Deserialize(TagCompound tag) => new Capacitor(
        tag.GetDouble("voltage"),
        tag.GetDouble("current"),
        tag.GetDouble("capacity"),
        tag.GetInt("capacityMax"),
        tag.GetDouble("passiveDraw")
    );
}