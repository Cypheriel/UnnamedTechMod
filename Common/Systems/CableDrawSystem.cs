using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;
using UnnamedTechMod.Common.TileData;

namespace UnnamedTechMod.Common.Systems;

public class CableDrawSystem : ModSystem
{
    private delegate void GetScreenDrawAreaDelegate(
        TileDrawing self,
        Vector2 screenPosition,
        Vector2 offSet,
        out int firstTileX,
        out int lastTileX,
        out int firstTileY,
        out int lastTileY);
    
    private static GetScreenDrawAreaDelegate _getScreenDrawArea = typeof(TileDrawing)
        .GetMethod("GetScreenDrawArea", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        ?.CreateDelegate<GetScreenDrawAreaDelegate>()
            ?? throw new InvalidOperationException("GetScreenDrawArea method not found.");
    
    public override void PostDrawTiles()
    {
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
            DepthStencilState.None, RasterizerState.CullCounterClockwise, null,
            Main.GameViewMatrix.TransformationMatrix);
        
        var offset = new Vector2(Main.offScreenRange, Main.offScreenRange);
        if (Main.drawToScreen)
            offset = Vector2.Zero;
        
        _getScreenDrawArea.Invoke(
            Main.instance.TilesRenderer,
            Main.Camera.UnscaledPosition,
            offset + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition),
            out var firstTileX,
            out var lastTileX,
            out var firstTileY,
            out var lastTileY
        );

        for (var x = firstTileX; x <= lastTileX; x++)
        {
            for (var y = firstTileY; y <= lastTileY; y++)
            {
                var tileData = Main.tile[x, y].Get<TransportTileData>();
                if (tileData.CarriedMediums.HasFlag(TransportType.Cable))
                {
                    Main.spriteBatch.Draw(
                        TextureAssets.MagicPixel.Value,
                        new Point(x, y).ToWorldCoordinates(0, 0) - Main.screenPosition,
                        new Rectangle(0, 0, 16, 16),
                        Color.IndianRed,
                        0,
                        Vector2.Zero,
                        1,
                        SpriteEffects.None,
                        0
                    );
                }
            }
        }
        
        Main.spriteBatch.End();
    }
}