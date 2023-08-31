using System;
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

    private TransportNetwork(TransportType transportType)
    {
        if (transportType.ToString().Count(c => c == '1') != 1)
            throw new ArgumentException("transportType should only be a single flag");
        transportType = TransportType;
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

        return (
            from position in adjacentTilePositions
            from network in UnnamedTechMod.TransportNetworks
            where transportType == network.TransportType && network.TransportMediums.Contains(position)
            select network
        ).ToList();
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