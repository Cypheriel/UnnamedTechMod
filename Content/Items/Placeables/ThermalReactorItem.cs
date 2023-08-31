using Terraria.ID;
using Terraria.ModLoader;
using UnnamedTechMod.Content.Tiles;

namespace UnnamedTechMod.Content.Items.Placeables;

public class ThermalReactorItem : ModItem
{
    public override void SetDefaults()
    {
        Item.createTile = ModContent.TileType<ThermalReactorTile>();
        Item.consumable = true;
        Item.maxStack = 9999;
        Item.useTime = 10;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.width = 12;
        Item.height = 12;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 15;
    }
}