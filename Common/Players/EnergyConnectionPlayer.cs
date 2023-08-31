using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using UnnamedTechMod.Content.TileEntities;

namespace UnnamedTechMod.Common.Players;

public enum ConnectionMode
{
    LoadBearer,
    Source,
    Disconnect
}

public class EnergyConnectionPlayer : ModPlayer
{
    public RelayTileEntity BoundRelay;

    private ConnectionMode _connectionMode = ConnectionMode.LoadBearer;

    public ConnectionMode ConnectionMode
    {
        get => _connectionMode;
        set
        {
            _connectionMode = value;
            
            string message;
            Color color;
            var position = Player.position;
            
            switch (value)
            {
                case ConnectionMode.LoadBearer:
                    message = "Assigning load-bearers";
                    color = Color.Aqua;
                    break;
                case ConnectionMode.Source:
                    message = "Assigning sources";
                    color = Color.LightYellow;
                    break;
                case ConnectionMode.Disconnect:
                    message = "Disconnecting tiles";
                    color = Color.IndianRed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            
            PopupText.ClearAll();
            PopupText.NewText(new AdvancedPopupRequest
                {
                    Text = message,
                    Color = color,
                    DurationInFrames = 60,
                    Velocity = default
                },
                new Vector2(position.X + 0.5f,  position.Y + 0.5f)
            );
        }
    }
    
    private Point16? _connectionModeTile;

    public Point16? ConnectionModeTile
    {
        get => _connectionModeTile;
        set
        {
            if (_connectionModeTile == value) return;

            var message = "This ain't supposed to happen...";
            var color = Color.DarkRed;
            var location = _connectionModeTile ?? new Point16(0, 0);

            if (_connectionModeTile is null && value is not null)
            {
                message = Language.GetTextValue("Mods.UnnamedTechMod.Common.RelayModeEnabled", value);
                color = Color.LightGreen;
                location = value.Value;

                if (!TileUtils.TryGetTileEntityAs(value.Value.X, value.Value.Y, out RelayTileEntity entity))
                    return;

                BoundRelay = entity;
            }
            else if (_connectionModeTile is not null && value is null)
            {
                message = Language.GetTextValue("Mods.UnnamedTechMod.Common.RelayModeDisabled");
                color = Color.IndianRed;
                location = _connectionModeTile.Value;
            }
            else if (_connectionModeTile is not null && value is not null)
            {
                message = Language.GetTextValue("Mods.UnnamedTechMod.Common.RelaySwitched");
                color = Color.LightYellow;
                location = value.Value;
                
                if (!TileUtils.TryGetTileEntityAs(value.Value.X, value.Value.Y, out RelayTileEntity entity))
                    return;

                BoundRelay = entity;
            }

            _connectionModeTile = value;
            
            PopupText.ClearAll();
            PopupText.NewText(new AdvancedPopupRequest
                {
                    Text = message,
                    Color = color,
                    DurationInFrames = 60
                },
                location.ToWorldCoordinates()
            );
        }
    }

    public bool InConnectionMode => ConnectionModeTile is not null;
}