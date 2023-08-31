using System;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using UnnamedTechMod.Content.TileEntities;

namespace UnnamedTechMod.Content.Tiles;

public class AutomatedRefinerTile : CapacitiveModTile<AutomatedRefinerTileEntity>
{
    public override string Texture => $"Terraria/Images/Tiles_{TileID.Extractinator}";

    protected override void SafeSetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;
        Main.tileLighted[Type] = true;

        TileID.Sets.IsAContainer[Type] = true;
        
        AdjTiles = new int[] { TileID.Containers };

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.Origin = new Point16(0, 0);
        TileObjectData.newTile.DrawYOffset = 2;

        AddMapEntry(Color.DarkGray, this.GetLocalization("MapEntry"));
    }

    public override void SafeKillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (!TileUtils.TryGetTileEntityAs(i, j, out AutomatedRefinerTileEntity entity)) return;

        while (entity.InputBuffer.Buffer.Count > 0)
        {
            var item = entity.InputBuffer.Buffer.Dequeue();
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j).ToWorldCoordinates(), item.type, item.stack, false, item.prefix);
            Console.WriteLine($"Dropping {item.Name} x{item.stack} from INPUT.");
        }
        
        while (entity.OutputBuffer.Buffer.Count > 0)
        {
            var item = entity.OutputBuffer.Buffer.Dequeue();
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j).ToWorldCoordinates(), item.type, item.stack, false, item.prefix);
            Console.WriteLine($"Dropping {item.Name} x{item.stack} from OUTPUT.");
        }
    }

    protected override bool SafeRightClick(int i, int j)
    {
        if (!TileUtils.TryGetTileEntityAs(i, j, out AutomatedRefinerTileEntity entity)) return false;

        entity.InputBuffer.Insert(new Item(ItemID.SiltBlock, 100));
        
        return true;
    }
    
    public override string GetHoverText(int i, int j)
    {
        if (!TileUtils.TryGetTileEntityAs(i, j, out AutomatedRefinerTileEntity entity)) return null;

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
}