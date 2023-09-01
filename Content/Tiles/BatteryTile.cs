using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using UnnamedTechMod.Common.TileData;
using UnnamedTechMod.Content.TileEntities;

namespace UnnamedTechMod.Content.Tiles;

public class BatteryTile : CapacitiveModTile<BatteryTileEntity>
{
    public override List<IOPort> IOPorts => new()
    {
        new IOPort(TransportType.Cable, new Point16(0, 0), FlowType.Input),
        new IOPort(TransportType.Cable, new Point16(1, 0), FlowType.Output),
    };

    protected override void SafeSetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = false;
        Main.tileLighted[Type] = true;
        
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.DrawYOffset = 2;

        AddMapEntry(Color.DarkGray, this.GetLocalization("MapEntry"));
    }
    
    public override string GetHoverText(int i, int j)
    {
        if (!TileUtils.TryGetTileEntityAs(i, j, out BatteryTileEntity entity)) return null;

        var capacitor = entity.Capacitor;

        StringBuilder text = new();
        text.AppendLine(TileName);
        text.AppendLine(Language.GetTextValue(
            "Mods.UnnamedTechMod.Common.CapacityText",
            capacitor.CapacityJoules, capacitor.MaxCapacityJoules
        ));
        
        return text.ToString();
    }
}