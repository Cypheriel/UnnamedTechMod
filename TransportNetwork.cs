using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
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
        TransportMedia.CollectionChanged += Handler;

        TransportType = transportType;
        foreach (var pos in positions)
        {
            TransportMedia.Add(pos);
        }
    }

    /// <summary>
    /// Called upon altering <see cref="TransportMedia"/>.
    /// </summary>
    private void Handler(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            // Verify removal of a single medium
            case NotifyCollectionChangedAction.Remove when e.OldItems is not null && e.OldItems.Count == 1:
                OnRemoveMedium((Point)e.OldItems[0]!);
                break;
            case NotifyCollectionChangedAction.Add when e.NewItems is not null && e.NewItems.Count == 1:
                OnAddMedium((Point)e.NewItems[0]!);
                break;
        }
    }

    private void OnAddMedium(Point addedMedium)
    {
        if (!TileUtils.TryGetTileEntityAs(addedMedium.X, addedMedium.Y, out CapacitiveTileEntity entity))
            return;

        var tile = Main.tile[addedMedium];
        var modTileType = typeof(CapacitiveModTile<>).MakeGenericType(entity.GetType());
        var modTile = TileLoader.GetTile(tile.TileType);

        if (modTile.GetType().BaseType != modTileType)
        {
            Console.WriteLine($"{modTile} is not {modTileType}");
            return;
        }

        foreach (IOPort port in ((dynamic)modTile).IOPorts)
        {
            if (port.Type == TransportType && (entity.Position + port.Position).ToPoint() == addedMedium)
            {
                switch (port.FlowType)
                {
                    case FlowType.Input:
                        Console.WriteLine($"Adding {entity.Name} as load");
                        Loads.Add(entity);
                        break;
                    case FlowType.Output:
                        Console.WriteLine($"Adding {entity.Name} as source");
                        Sources.Add(entity);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid flow type specified");
                }
            }
        }
    }

    private void OnRemoveMedium(Point removedMedium)
    {
        if (TransportMedia.Count == 0)
        {
            TransportDataSaveSystem.TransportNetworks.Remove(this);
            return;
        }

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
    
    public static TransportNetwork TryFromPosition(Point position, TransportType transportType)
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

    public void Transfer()
    {
        foreach (var source in Sources)
        {
            foreach (var load in Loads)
            {
                if (TransportType != TransportType.Cable)
                    return;

                source.Capacitor.TransferEnergy(load.Capacitor, 1f / Sources.Count);
            }
        }
    }
}