using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace UnnamedTechMod;

public static class TileUtils
{
    public static bool TryGetTileEntityAs<T>(int i, int j, out T entity) where T : TileEntity
    {
        var position = new Point16(i, j);
        var tile = Main.tile[position.ToPoint()];
        var tileObjectData = TileObjectData.GetTileData(tile);

        if (tileObjectData is null)
        {
            entity = null;
            return false;
        }
        
        var size = 16 + tileObjectData.CoordinatePadding;
        
        var frameX = tile.TileFrameX % (size * tileObjectData.Width) / size;
        var frameY = tile.TileFrameY % (size * tileObjectData.Height) / size;

        var origin = new Point16(i - frameX, j - frameY);

        if (TileEntity.ByPosition.TryGetValue(origin, out var existing) && existing is T existingAsT)
        {
            entity = existingAsT;
            return true;
        }

        entity = null;
        return false;
    }
}