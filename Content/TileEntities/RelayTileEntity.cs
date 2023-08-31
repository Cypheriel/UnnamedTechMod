using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace UnnamedTechMod.Content.TileEntities;

public class RelayTileEntity : CapacitiveTileEntity
{
    public List<CapacitiveTileEntity> Loads = new();
    public List<CapacitiveTileEntity> Sources = new();
    
    private List<Point16> _loadLocations = new();
    private List<Point16> _sourceLocations = new();
    private bool _transferringEnergy;

    private bool _loaded;

    public virtual bool Active => _transferringEnergy;

    public RelayTileEntity() : base(15, 15, 2_500)
    {
    }

    protected override void SafeUpdate()
    {
        if (!_loaded) InitConnections();

        _transferringEnergy = false;
        
        foreach (var source in Sources)
        {
            if (source.Capacitor.TransferEnergy(Capacitor))
                _transferringEnergy = true;
        }
        
        foreach (var load in Loads)
        {
            if (Capacitor.TransferEnergy(load.Capacitor))
                _transferringEnergy = true;
        }
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add("loads", Loads.Select(cte => cte.Position).ToList());
        tag.Add("sources", Sources.Select(cte => cte.Position).ToList());
        
        Mod.Logger.Debug($"Saving {Loads.Count} load(s) and {Sources.Count} source(s).");
    }

    public override void LoadData(TagCompound tag)
    {
        tag.TryGet("loads", out _loadLocations);
        tag.TryGet("sources", out _sourceLocations);
        
        Mod.Logger.Debug($"Loading {_loadLocations.Count} load(s) and {_sourceLocations.Count} source(s).");
    }

    private void InitConnections()
    {
        Mod.Logger.Debug("Attempting to find load/source entities.");
        
        foreach (var loadLocation in _loadLocations)
        {
            if (!TileUtils.TryGetTileEntityAs(loadLocation.X, loadLocation.Y, out CapacitiveTileEntity entity))
            {
                Mod.Logger.Debug("Entity is not Capacitive");
                continue;
            }

            Mod.Logger.Debug($"Loaded {entity.Name} as a load-bearer.");

            Loads.Add(entity);
            entity.BoundRelays.Add(this);
        }

        foreach (var sourceLocation in _sourceLocations)
        {
            if (!TileUtils.TryGetTileEntityAs(sourceLocation.X, sourceLocation.Y, out CapacitiveTileEntity entity))
            {
                Mod.Logger.Debug("Entity is not Capacitive");
                continue;
            }

            Mod.Logger.Debug($"Loaded {entity.Name} as a source.");

            Sources.Add(entity);
            entity.BoundRelays.Add(this);
        }
        
        Mod.Logger.Debug($"{Loads.Count}/{_loadLocations.Count} loads | {Sources.Count}/{_sourceLocations.Count}");
        _loaded = true;
    }
}
