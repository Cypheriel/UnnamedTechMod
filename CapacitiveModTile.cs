using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using UnnamedTechMod.Content.Items;

namespace UnnamedTechMod;

/// <summary>
/// Base class of <see cref="CapacitiveModTile{T}"/> to allow <c>is CapacitiveModTile</c> checks.
/// </summary>
public abstract class CapacitiveModTile : ModTile
{
}

public abstract class CapacitiveModTile<T> : CapacitiveModTile where T : CapacitiveTileEntity
{
    /// <summary>
    /// The tile name used primarily for capacitor info.
    /// </summary>
    /// <remarks>
    /// Defaults to localized text of <c>Mods.{Mod.Name}.Tiles.{Name}.MapEntry</c>
    /// </remarks>
    protected virtual string TileName => Language.GetTextValue($"Mods.{Mod.Name}.Tiles.{Name}.MapEntry");

    public virtual List<IOPort> IOPorts => new();

    public sealed override void SetStaticDefaults()
    {
        SafeSetStaticDefaults();

        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(
            ModContent.GetInstance<T>().Hook_AfterPlacement, -1, 0, false
        );
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.addTile(Type);
    }

    /// <summary>
    /// Called by <see cref="SetStaticDefaults"/>, which additionally places down a CapacitiveTileEntity of type <c>T</c>.<br />
    /// </summary>
    /// <remarks>
    /// Use in place of <see cref="SetStaticDefaults"/>.
    /// </remarks>
    protected virtual void SafeSetStaticDefaults()
    {
    }

    public sealed override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        SafeKillTile(i, j, ref fail, ref effectOnly, ref noItem);
        
        if (fail)
            return;

        ModContent.GetInstance<T>().Kill(i, j);
    }
    
    /// <summary>
    /// Called by <see cref="KillTile"/>.
    /// </summary>
    /// <remarks>
    /// Use in place of <see cref="KillTile"/>.
    /// </remarks>
    public virtual void SafeKillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
    }
    
    // TODO: Add a safe variant.
    public sealed override void MouseOver(int i, int j)
    {
        var heldItem = Main.LocalPlayer.HeldItem.type;
        if (heldItem == 0) heldItem = ModContent.ItemType<DummyItem>();

        Main.LocalPlayer.cursorItemIconEnabled = true;
        Main.LocalPlayer.cursorItemIconID = heldItem;
        // TODO: Additional formatting depending on whether capacitor is charging/discharging
        Main.LocalPlayer.cursorItemIconText = GetHoverText(i, j) ?? "Not found";
    }

    public sealed override bool RightClick(int i, int j)
    {
        return SafeRightClick(i, j);
    }

    /// <summary>
    /// Called by `RightClick(i, j)`, won't fire in relay connection mode unless the tile is the relay that's being configured.
    /// </summary>
    /// <param name="i">x coordinate</param>
    /// <param name="j">y coordinate</param>
    /// <returns>Whether an interaction has occurred.</returns>
    protected virtual bool SafeRightClick(int i, int j)
    {
        return false;
    }
    
    /// <summary>
    /// Generates the text used by the tile to change <see cref="Player.cursorItemIconText"/>. <br />
    /// This is the text shown when hovering over a <see cref="CapacitiveModTile{T}"/>.
    /// </summary>
    /// <param name="i">Tile's x-coordinate</param>
    /// <param name="j">Tile's y-coordinate</param>
    /// <returns></returns>
    public virtual string GetHoverText(int i, int j)
    {
        if (!TileUtils.TryGetTileEntityAs(i, j, out CapacitiveTileEntity entity)) return null;

        var capacitor = entity.Capacitor;

        StringBuilder text = new();
        text.AppendLine(TileName);
        text.AppendLine(Language.GetTextValue(
            "Mods.UnnamedTechMod.Common.CapacityText",
            capacitor.CapacityJoules, capacitor.MaxCapacityJoules
        ));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.VoltageText", capacitor.Voltage));
        text.AppendLine(Language.GetTextValue("Mods.UnnamedTechMod.Common.CurrentText", capacitor.Current));

        return text.ToString();
    }
}