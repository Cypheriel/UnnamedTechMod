﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using UnnamedTechMod.Common.TileData;

namespace UnnamedTechMod.Common.Systems;

public class TransportDataSaveSystem : ModSystem
{
    public static readonly List<TransportNetwork> TransportNetworks = new();

    public override void ClearWorld()
    {
        TransportNetworks.Clear();
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["tileData"] = MemoryMarshal.Cast<TransportTileData, byte>(Main.tile.GetData<TransportTileData>()).ToArray();
    }

    public override void LoadWorldData(TagCompound tag)
    {
        MemoryMarshal.Cast<byte, TransportTileData>(tag.GetByteArray("tileData")).CopyTo(Main.tile.GetData<TransportTileData>());
    }
}