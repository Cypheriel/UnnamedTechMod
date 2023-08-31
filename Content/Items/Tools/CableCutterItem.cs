using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UnnamedTechMod.Common.TileData;

namespace UnnamedTechMod.Content.Items.Tools;

public class CableCutterItem : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.WireCutter}";

    public override void SetDefaults()
    {
        Item.maxStack = 1;
        Item.width = 12;
        Item.height = 12;
        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.useStyle = ItemUseStyleID.Swing;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer) return false;

        return TileUtils.DestroyTransportMedium(Player.tileTargetX, Player.tileTargetY, TransportType.Cable)
            ? true
            : null;
    }
}