using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace UnnamedTechMod;

/// <summary>
/// Base class for tile entities with capacitance.
/// </summary>
public abstract class CapacitiveTileEntity : ModTileEntity
{
    public readonly Capacitor Capacitor;

    protected CapacitiveTileEntity(double voltage, double current, int capacityMax, double passiveDraw = 0)
    {
        Capacitor = new Capacitor(voltage, current, 0, capacityMax, passiveDraw);
    }

    public sealed override void Update()
    {
        if (Capacitor.Powered)
        {
            Capacitor.Charge(-Capacitor.PassiveDraw);
        }

        SafeUpdate();
    }

    protected virtual void SafeUpdate()
    {
    }

    public override bool IsTileValidForEntity(int x, int y)
    {
        return TileLoader.GetTile(Main.tile[x, y].TileType) is CapacitiveModTile;
    }
    
    public override int Hook_AfterPlacement(int i, int j, int tileEntityType, int style, int direction, int alternate)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
        }

        return Place(i, j);
    }

    public override void OnNetPlace()
    {
        if (Main.netMode == NetmodeID.Server)
        {
            NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag["capacity"] = Capacitor.CapacityJoules;
    }

    public override void LoadData(TagCompound tag)
    {
        Capacitor.CapacityJoules = tag.GetDouble("capacity");
    }
}