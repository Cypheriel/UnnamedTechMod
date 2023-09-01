using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Xna.Framework;
using UnnamedTechMod.Common.Systems;
using UnnamedTechMod.Common.TileData;

namespace UnnamedTechMod;

public class TransportNetwork
{
    public List<CapacitiveTileEntity> Loads = new();
    public List<CapacitiveTileEntity> Sources = new();
    public ObservableCollection<Point> TransportMedia = new();
    public TransportType TransportType;

    public TransportNetwork(TransportType transportType, params Point[] positions)
    {
        TransportType = transportType;
        foreach (var pos in positions)
        {
            TransportMedia.Add(pos);
        }

        TransportMedia.CollectionChanged += OnRemoveMedium;
    }
    
    /// <summary>
    /// Called upon altering <see cref="TransportMedia"/>.
    /// </summary>
    private void OnRemoveMedium(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Verify removal of a single medium
        if (e.Action != NotifyCollectionChangedAction.Remove || e.OldItems is null || e.OldItems.Count != 1)
            return;

        if (TransportMedia.Count == 0)
        {
            TransportDataSaveSystem.TransportNetworks.Remove(this);
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
        foreach (var pos in adjacentTilePositions.Where(p => TransportMedia.Contains(p)))
        {
            if (splitNetworks.Any(n => n.Contains(pos)))
                continue;
            splitNetworks.Add(ConnectedMedia(pos));
        }

        TransportDataSaveSystem.TransportNetworks.Remove(this);
        
        foreach (var splitNetwork in splitNetworks)
        {
            TransportDataSaveSystem.TransportNetworks.Add(new TransportNetwork(TransportType, splitNetwork.ToArray()));
        }
    }
    
    /// <summary>
    /// Used to get transport media that are connected in a group.
    /// </summary>
    /// <returns>All transport media connected to the current medium</returns>
    public List<Point> ConnectedMedia(Point current, HashSet<Point> connected = null)
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

        foreach (var pos in adjacentTilePositions.Where(p => TransportMedia.Contains(p) && !connected.Contains(p)))
        {
            ConnectedMedia(pos, connected);
        }

        return connected.ToList();
    }


    #nullable enable
    public static TransportNetwork? TryFromPosition(Point position, TransportType transportType)
    {
        return TransportDataSaveSystem.TransportNetworks
            .FirstOrDefault(
                network => network.TransportType == transportType && network.TransportMedia.Contains(position)
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
            TransportDataSaveSystem.TransportNetworks.Remove(network);
            Loads = Loads.Concat(network.Loads).ToList();
            Sources = Sources.Concat(network.Sources).ToList();
            foreach (var medium in network.TransportMedia)
            {
                TransportMedia.Add(medium);
            }
        }

        Console.WriteLine($"Total networks: {TransportDataSaveSystem.TransportNetworks.Count}");
    }
}