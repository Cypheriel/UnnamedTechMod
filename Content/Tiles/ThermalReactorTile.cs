using System;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using UnnamedTechMod.Content.TileEntities;

namespace UnnamedTechMod.Content.Tiles;

public class ThermalReactorTile : CapacitiveModTile<ThermalReactorTileEntity>
{
    protected override void SafeSetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);

        AddMapEntry(Color.White, this.GetLocalization("MapEntry"));
    }

    public override string GetHoverText(int i, int j)
    {
        if (!TileUtils.TryGetTileEntityAs(i, j, out ThermalReactorTileEntity entity)) return null;

        var capacitor = entity.Capacitor;
        // TODO: Change depending whether tile is discharging or not.
        // var plusMinus = "+";

        StringBuilder text = new();
        text.AppendLine(TileName);
        text.AppendLine(Language.GetTextValue(
            "Mods.UnnamedTechMod.Common.CapacityText",
            capacitor.CapacityJoules, capacitor.MaxCapacityJoules
        ));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.HeatLevelText", entity.HeatLevel));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.GenerationRateText", entity.GenerationRate));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.VoltageText", capacitor.Voltage));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.CurrentText", capacitor.Current));
        text.AppendLine(Language.GetTextValue(entity.Active
            ? "Mods.UnnamedTechMod.Common.CapacitorActiveText"
            : "Mods.UnnamedTechMod.Common.CapacitorInactiveText"));

        return text.ToString();
    }
}