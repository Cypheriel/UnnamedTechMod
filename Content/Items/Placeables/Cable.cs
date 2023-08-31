using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UnnamedTechMod.Common.TileData;

namespace UnnamedTechMod.Content.Items.Placeables;

public class Cable : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.Wire}";

    public override void SetDefaults()
    {
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.useStyle = ItemUseStyleID.Swing;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return null;

        var tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
        ref var data = ref tile.Get<TransportTileData>();
        Mod.Logger.Debug($"Pre-place data: {data}");
        if (data.CarriedMediums.HasFlag(TransportType.Cable))
            return null;
        
        data.CarriedMediums |= TransportType.Cable;
        Mod.Logger.Debug($"Post-place data: {data}");
        return true;
    }
}