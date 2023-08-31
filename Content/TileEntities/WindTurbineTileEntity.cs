using System;
using Terraria;

namespace UnnamedTechMod.Content.TileEntities;

public class WindTurbineTileEntity : CapacitiveTileEntity
{
    private static float BaseRate => 50f;
    public static float GenerationRate => BaseRate * Math.Abs(Main.windSpeedCurrent) / 1.2f;
    public virtual bool Active => GenerationRate > 0;

    public WindTurbineTileEntity() : base(10, 5, 5_000)
    {
    }

    protected override void SafeUpdate()
    {
        Capacitor.Charge(GenerationRate);
    }
}