using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;
using UnnamedTechMod.Common.TileData;

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

    public static bool PlaceTransportMedium(int x, int y, TransportType transportType)
    {
        var tile = Main.tile[x, y];
        ref var data = ref tile.Get<TransportTileData>();

        if (data.CarriedMedia.HasFlag(transportType))
            return false;

        data.CarriedMedia |= transportType;

        var network = TransportNetwork.TryFromPosition(new Point(x, y), transportType);
        if (network is not null)
            return false;

        network = new TransportNetwork(transportType, new Point(x, y));
        
        UnnamedTechMod.TransportNetworks.Add(network);
        network.MergeNetworks(TransportNetwork.AdjacentNetworks(x, y, transportType).ToArray());
        
        return true;
    }
    
    public static bool DestroyTransportMedium(int x, int y, TransportType transportType)
    {
        var tile = Main.tile[x, y];
        ref var data = ref tile.Get<TransportTileData>();

        if (!data.CarriedMedia.HasFlag(transportType))
            return false;

        data.CarriedMedia ^= transportType;
        
        var network = TransportNetwork.TryFromPosition(new Point(x, y), transportType);
        
        return network is not null && network.TransportMedia.Remove(new Point(x, y));
    }
}