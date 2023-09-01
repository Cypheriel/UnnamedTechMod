using System;
using Terraria;

namespace UnnamedTechMod.Common.TileData;

[Flags]
public enum TransportType : byte
{
    Cable = 0b100,
    Conduit = 0b010,
    PneumaticTube = 0b001
}


public struct TransportTileData : ITileData
{
    public TransportType CarriedMedia;
}