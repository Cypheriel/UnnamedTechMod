using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace UnnamedTechMod.Content.TileEntities;

public class AutomatedRefinerTileEntity : CapacitiveTileEntity
{
    private ushort _frameCounter;
    private Player _dummyPlayer = new();

    public ItemBuffer InputBuffer = new();
    public ItemBuffer OutputBuffer = new();

    public static ushort ExtractionCost = 125;

    public virtual bool Active => Capacitor.Powered
                                  && InputBuffer.Buffer.Count > 0
                                  && Capacitor.CapacityJoules > ExtractionCost + Capacitor.PassiveDraw;

    public AutomatedRefinerTileEntity() : base(15, 15, 5_000)
    {
    }

    protected override void SafeUpdate()
    {
        _frameCounter++;
        _frameCounter %= 60;

        if (_frameCounter != 0) return;
        
        // TODO: Figure out a better place to change the dummy player's position. This might be a small bit to weird.
        _dummyPlayer.Center = Position.ToWorldCoordinates();
        
        if (InputBuffer.Buffer.Count == 0 || !Active || OutputBuffer.Buffer.Count >= OutputBuffer.Capacity) return;
        var item = InputBuffer.RetrieveFirst(1);
        Capacitor.CapacityJoules -= ExtractionCost;

        var extractinatorUseMethod =
            typeof(Player).GetMethod("ExtractinatorUse",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                ?.CreateDelegate<Action<Player, int, int>>()
            ?? throw new InvalidOperationException("Unable to acquire Player.ExtractinatorUse method.");
        extractinatorUseMethod.Invoke(_dummyPlayer, item.type, TileID.Extractinator);
    }
}