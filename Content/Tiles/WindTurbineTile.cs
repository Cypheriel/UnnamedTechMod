using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using UnnamedTechMod.Content.TileEntities;

namespace UnnamedTechMod.Content.Tiles;

public class WindTurbineTile : CapacitiveModTile<WindTurbineTileEntity>
{
    protected override void SafeSetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;
        
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

        AddMapEntry(Color.White, this.GetLocalization("MapEntry"));
    }

    public override string GetHoverText(int i, int j)
    {
        if (!TileUtils.TryGetTileEntityAs(i, j, out WindTurbineTileEntity entity))
            return null;

        var capacitor = entity.Capacitor;
        
        // TODO: Change depending whether tile is discharging or not.
        StringBuilder text = new();
        text.AppendLine(TileName);
        text.AppendLine(Language.GetTextValue(
            "Mods.UnnamedTechMod.Common.CapacityText",
            capacitor.CapacityJoules, capacitor.MaxCapacityJoules
        ));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.GenerationRateText", WindTurbineTileEntity.GenerationRate));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.VoltageText", capacitor.Voltage));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.CurrentText", capacitor.Current));
        text.AppendLine(Language.GetTextValue(entity.Active
            ? "Mods.UnnamedTechMod.Common.CapacitorActiveText"
            : "Mods.UnnamedTechMod.Common.CapacitorInactiveText"));

        return text.ToString();
    }
}