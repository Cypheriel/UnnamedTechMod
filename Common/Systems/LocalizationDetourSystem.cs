using System.Reflection;
using Terraria.Localization;
using Terraria.ModLoader;

namespace UnnamedTechMod.Common.Systems;

internal class LocalizationDetourSystem : ModSystem
{
    private delegate void OriginalUpdateLocalizationFilesForMod(
        Mod mod,
        string outputPath = null,
        GameCulture specificCulture = null
    );

    public override void Load()
    {
        var updateLocalizationFilesForModInfo = typeof(LocalizationLoader)
            .GetMethod(
                "UpdateLocalizationFilesForMod",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                );

        Mod.Logger.Debug("tModLoader wouldn't keep its grubby little hands off of my poor localization files.");
        MonoModHooks.Add(updateLocalizationFilesForModInfo, UpdateLocalizationFilesForMod_Detour);
    }

    private static void UpdateLocalizationFilesForMod_Detour(
        OriginalUpdateLocalizationFilesForMod orig,
        Mod mod,
        string outputPath = null,
        GameCulture specificCulture = null)
    {
        if (mod is not UnnamedTechMod)
            orig(mod, outputPath, specificCulture);
    }
}