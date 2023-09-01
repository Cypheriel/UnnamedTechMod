using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Xna.Framework;
using UnnamedTechMod.Common.TileData;

namespace UnnamedTechMod;

public class TransportNetwork
{
    public List<CapacitiveTileEntity> Loads = new();
    public List<CapacitiveTileEntity> Sources = new();
    public ObservableCollection<Point> TransportMediums = new();
    public TransportType TransportType;

    public TransportNetwork(TransportType transportType, params Point[] positions)
    {
        TransportType = transportType;
        foreach (var pos in positions)
        {
            TransportMediums.Add(pos);
        }

        TransportMediums.CollectionChanged += OnRemoveMedium;
    }
    
    /// <summary>
    /// Called upon altering <see cref="TransportMediums"/>.
    /// </summary>
    private void OnRemoveMedium(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Verify removal of a single medium
        if (e.Action != NotifyCollectionChangedAction.Remove || e.OldItems is null || e.OldItems.Count != 1)
            return;

        if (TransportMediums.Count == 0)
        {
            UnnamedTechMod.TransportNetworks.Remove(this);
            return;
        }

        var removedMedium = (Point)e.OldItems[0]!;
        Point[] adjacentTilePositions =
        {
            new(removedMedium.X + 1, removedMedium.Y),
            new(removedMedium.X, removedMedium.Y + 1),
            new(removedMedium.X - 1, removedMedium.Y),
            new(removedMedium.X, removedMedium.Y - 1),
        };

        var splitNetworks = new List<List<Point>>();
        foreach (var pos in adjacentTilePositions.Where(p => TransportMediums.Contains(p)))
        {
            if (splitNetworks.Any(n => n.Contains(pos)))
                continue;
            splitNetworks.Add(ConnectedMediums(pos));
        }

        UnnamedTechMod.TransportNetworks.Remove(this);
        
        foreach (var splitNetwork in splitNetworks)
        {
            UnnamedTechMod.TransportNetworks.Add(new TransportNetwork(TransportType, splitNetwork.ToArray()));
        }
    }
    
    /// <summary>
    /// Used to get transport mediums that are connected in a group.
    /// </summary>
    /// <returns>All transport mediums connected to the current medium</returns>
    public List<Point> ConnectedMediums(Point current, HashSet<Point> connected = null)
    {
        connected ??= new HashSet<Point>();
        connected.Add(current);
        
        Point[] adjacentTilePositions =
        {
            new(current.X + 1, current.Y),
            new(current.X, current.Y + 1),
            new(current.X - 1, current.Y),
            new(current.X, current.Y - 1),
        };

        foreach (var pos in adjacentTilePositions.Where(p => TransportMediums.Contains(p) && !connected.Contains(p)))
        {
            ConnectedMediums(pos, connected);
        }

        return connected.ToList();
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
            foreach (var medium in network.TransportMediums)
            {
                TransportMediums.Add(medium);
            }
        }

        Console.WriteLine($"Total networks: {UnnamedTechMod.TransportNetworks.Count}");
    }
}