using System;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria.Localization;
using Terraria.ModLoader;

namespace UnnamedTechMod.Common.Systems;

/// <summary>
/// Refuse to call <see cref="Terraria.ModLoader.LocalizationLoader.UpdateLocalizationFilesForMod"/> for this mod.
/// This solves issues with tModLoader's autoformatting for hjson.
/// </summary>
internal class LocalizationILEdit : ModSystem
{
    private static ILHook _updateLocalizationFilesForModHook;

    public override void Load()
    {
        var updateLocalizationFilesInfo = typeof(LocalizationLoader)
            .GetMethod(
                "UpdateLocalizationFiles",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Default
            ) ?? throw new InvalidOperationException("UpdateLocalizationFiles method not found.");

        Mod.Logger.Debug("tModLoader wouldn't keep its grubby little hands off of my poor localization files.");
        _updateLocalizationFilesForModHook = new ILHook(updateLocalizationFilesInfo, UpdateLocalizationFiles_ILEdit);
    }

    public override void Unload()
    {
        _updateLocalizationFilesForModHook.Dispose();
    }

    private static void UpdateLocalizationFiles_ILEdit(
        ILContext context
    )
    {
        var updateLocalizationFilesForModInfo = typeof(LocalizationLoader)
            .GetMethod(
                "UpdateLocalizationFilesForMod",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
            ) ?? throw new InvalidOperationException("UpdateLocalizationFilesForMod method not found.");
        var updateLocalizationFilesForMod = updateLocalizationFilesForModInfo
            .CreateDelegate<Action<Mod, string, GameCulture>>();

        var cursor = new ILCursor(context);
        cursor.GotoNext(MoveType.Before,
            instruction => instruction.MatchCall(updateLocalizationFilesForModInfo)
        );
        cursor.Remove();
        cursor.EmitDelegate<Action<Mod, string, GameCulture>>((mod, _, _) =>
        {
            if (mod is UnnamedTechMod)
                return;
            updateLocalizationFilesForMod.Invoke(mod, null, null);
        });
    }
}