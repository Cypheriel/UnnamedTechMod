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

public class RelayTile : CapacitiveModTile<RelayTileEntity>
{
    protected override void SafeSetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);

        AnimationFrameHeight = 18;

        AddMapEntry(Color.White, this.GetLocalization("MapEntry"));
    }
    
    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        if (frameCounter % 45 != 0) return;
        
        frame = ++frame % 6;
    }
    
    /// <summary>
    /// <inheritdoc cref="Terraria.Localization.LocalizedText.Format"/>
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public override string GetHoverText(int i, int j)
    {
        if (!TileUtils.TryGetTileEntityAs(i, j, out RelayTileEntity entity)) return null;

        var capacitor = entity.Capacitor;

        StringBuilder text = new();
        text.AppendLine(TileName);
        text.AppendLine(Language.GetTextValue(
            "Mods.UnnamedTechMod.Common.CapacityText",
            capacitor.CapacityJoules, capacitor.MaxCapacityJoules
        ));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.VoltageText", capacitor.Voltage));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.CurrentText", capacitor.Current));
        text.AppendLine(Language.GetTextValue(entity.Active
            ? "Mods.UnnamedTechMod.Common.CapacitorActiveText"
            : "Mods.UnnamedTechMod.Common.CapacitorInactiveText"));

        return text.ToString();
    }

    public override void RandomUpdate(int i, int j)
    {
        
    }
}