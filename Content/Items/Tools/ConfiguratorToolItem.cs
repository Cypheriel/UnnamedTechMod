using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UnnamedTechMod.Common.Players;
using UnnamedTechMod.Content.TileEntities;

namespace UnnamedTechMod.Content.Items.Tools;

public class ConfiguratorToolItem : ModItem
{
    public override string Texture => $"Terraria/Images/Item_{ItemID.AcornAxe}";

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

        var connectionPlayer = player.GetModPlayer<EnergyConnectionPlayer>();
        var tilePosition = new Point16(Player.tileTargetX, Player.tileTargetY);
        // var tile = Main.tile[tilePosition.X, tilePosition.Y];

        TileUtils.TryGetTileEntityAs(tilePosition.X, tilePosition.Y, out CapacitiveTileEntity entity);

        if (Main.mouseLeft) return OnLeftClick(connectionPlayer, entity);
        if (Main.mouseRight) return OnRightClick(connectionPlayer, entity);

        return false;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    private static bool OnLeftClick(EnergyConnectionPlayer player, CapacitiveTileEntity entity)
    {
        if (entity is null || !player.InConnectionMode) return false;

        switch (player.ConnectionMode)
        {
            case ConnectionMode.LoadBearer:
                player.BoundRelay.Sources.Remove(entity);

                if (!player.BoundRelay.Loads.Contains(entity))
                {
                    player.BoundRelay.Loads.Add(entity);
                    
                    if (!entity.BoundRelays.Contains(player.BoundRelay))
                        entity.BoundRelays.Add(player.BoundRelay);
                }
                break;
            
            case ConnectionMode.Source:
                player.BoundRelay.Loads.Remove(entity);

                if (!player.BoundRelay.Sources.Contains(entity))
                {
                    player.BoundRelay.Sources.Add(entity);
                    
                    if (!entity.BoundRelays.Contains(player.BoundRelay))
                        entity.BoundRelays.Add(player.BoundRelay);
                }
                break;
            
            case ConnectionMode.Disconnect:
                player.BoundRelay.Loads.Remove(entity);
                player.BoundRelay.Sources.Remove(entity);
                entity.BoundRelays.Remove(player.BoundRelay);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    private static bool OnRightClick(EnergyConnectionPlayer player, CapacitiveTileEntity entity)
    {
        if (entity is RelayTileEntity)
        {
            if (player.InConnectionMode && player.BoundRelay == entity)
            {
                player.ConnectionModeTile = null;
            }
            else
            {
                player.ConnectionModeTile = entity.Position;
            }

            return true;
        }

        player.ConnectionMode = player.ConnectionMode switch
        {
            ConnectionMode.LoadBearer => ConnectionMode.Source,
            ConnectionMode.Source => ConnectionMode.Disconnect,
            ConnectionMode.Disconnect => ConnectionMode.LoadBearer,
            _ => ConnectionMode.LoadBearer
        };

        return true;
    }
}