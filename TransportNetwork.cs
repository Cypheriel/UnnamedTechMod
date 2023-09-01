using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using UnnamedTechMod.Common.TileData;

namespace UnnamedTechMod;

public class TransportNetwork
{
    public List<CapacitiveTileEntity> Loads = new();
    public List<CapacitiveTileEntity> Sources = new();
    public List<Point> TransportMediums = new();
    public TransportType TransportType;

    public TransportNetwork(TransportType transportType, Point position)
    {
        TransportType = transportType;
        TransportMediums.Add(position);
    }
    
    #nullable enable
    public static TransportNetwork? TryFromPosition(Point position, TransportType transportType)
    {
        return UnnamedTechMod.TransportNetworks
            .FirstOrDefault(
                network => network.TransportType == transportType && network.TransportMediums.Contains(position)
            );
    }

    public static List<TransportNetwork> AdjacentNetworks(int x, int y, TransportType transportType)
    {
        Point[] adjacentTilePositions =
        {
            new(x + 1, y),
            new(x, y + 1),
            new(x - 1, y),
            new(x, y - 1),
        };

        var result = new List<TransportNetwork>();
        // Nullability seems to cause issues with converting this to a LINQ expression.
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var position in adjacentTilePositions)
        {
            var network = TryFromPosition(position, transportType);
            if (network is not null && network.TransportType == transportType)
            {
                result.Add(network);
            }
        }

        return result;
    }

    public void MergeNetworks(params TransportNetwork[] others)
    {
        foreach (var network in others)
        {
            UnnamedTechMod.TransportNetworks.Remove(network);
            Loads = Loads.Concat(network.Loads).ToList();
            Sources = Sources.Concat(network.Sources).ToList();
            TransportMediums = TransportMediums.Concat(network.TransportMediums).ToList();
        }
    }
}