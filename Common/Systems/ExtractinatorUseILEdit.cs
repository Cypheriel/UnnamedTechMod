using System;
using System.Linq;
using System.Reflection;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using UnnamedTechMod.Content.TileEntities;

namespace UnnamedTechMod.Common.Systems;

public class ExtractinatorUseILEdit : ModSystem
{
    public override void Load()
    {
        IL_Player.ExtractinatorUse += IL_PlayerOnExtractinatorUse;
    }

    private static void IL_PlayerOnExtractinatorUse(ILContext il)
    {
        var dropItemFromExtractinatorMethodInfo = typeof(Player).GetMethod("DropItemFromExtractinator",
                                                      BindingFlags.Public | BindingFlags.NonPublic |
                                                      BindingFlags.Instance)
                                                  ?? throw new InvalidOperationException(
                                                      "Unable to acquire Player.DropItemFromExtractinator method.");
        var dropItemFromExtractinatorMethod = dropItemFromExtractinatorMethodInfo
            .CreateDelegate<Action<Player, int, int>>();

        var cursor = new ILCursor(il);
        cursor.GotoNext(MoveType.Before, instruction => instruction.MatchCall(dropItemFromExtractinatorMethodInfo));
        cursor.Remove();
        cursor.EmitDelegate<Action<Player, int, int>>((player, type, stack) =>
        {
            var tilePosition = player.Center.ToTileCoordinates();

            if (!TileUtils.TryGetTileEntityAs(tilePosition.X, tilePosition.Y, out AutomatedRefinerTileEntity entity) ||
                Main.player.Contains(player))
            {
                dropItemFromExtractinatorMethod.Invoke(player, type, stack);
                return;
            }

            entity.OutputBuffer.Insert(new Item(type, stack));
        });
    }
}
