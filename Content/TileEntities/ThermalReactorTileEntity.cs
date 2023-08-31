using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace UnnamedTechMod.Content.TileEntities;

public class ThermalReactorTileEntity : CapacitiveTileEntity
{
    private const float BaseRate = 5.0f;

    public float HeatLevel
    {
        get
        {
            // TODO: Multi-tile stuff
            var adjacentTilePositions = new List<Point16>()
            {
                Position,
                new(Position.X + 1, Position.Y),
                new(Position.X, Position.Y + 1),
                new(Position.X - 1, Position.Y),
                new(Position.X, Position.Y - 1)
            };

            var result = 0;

            foreach (var tile in adjacentTilePositions.Select(pos => Main.tile[pos.X, pos.Y]))
            {
                if (tile.LiquidAmount > 0)
                {
                    switch (tile.LiquidType)
                    {
                        case LiquidID.Lava:
                            result += 1;
                            break;
                        case LiquidID.Water:
                            result -= 3;
                            break;
                    }
                }

                if (tile.HasTile)
                {
                    switch (tile.TileType)
                    {
                        case TileID.Hellstone:
                            result += 3;
                            break;
                        case TileID.Meteorite:
                            result += 2;
                            break;
                        case TileID.IceBlock:
                        case TileID.IceBrick:
                        case TileID.SnowBlock:
                        case TileID.SnowBrick:
                            result -= 5;
                            break;
                    }
                }
            }

            var dummyPlayer = UnnamedTechMod.DummyPlayer;
            dummyPlayer.Center = new Vector2(Position.X * 16, Position.Y * 16);

            dummyPlayer.ForceUpdateBiomes();

            if (dummyPlayer.ZoneUnderworldHeight) result += 5;
            else
            {
                if (dummyPlayer.ZoneDesert) result += 3;
                if (dummyPlayer.ZoneMeteor) result += 2;
                if (dummyPlayer.ZoneSnow) result -= 5;
            }

            Main.LocalPlayer.ForceUpdateBiomes();

            return result;
        }
    }

    public float GenerationRate => Math.Max(0, BaseRate * HeatLevel);
    public bool Active => GenerationRate > 0;

    public ThermalReactorTileEntity() : base(10, 5, 4_000)
    {
    }

    protected override void SafeUpdate()
    {
        Capacitor.Charge(GenerationRate);
    }
}