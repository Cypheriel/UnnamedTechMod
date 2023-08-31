using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;
using UnnamedTechMod.Common.Players;
using UnnamedTechMod.Content.Items.Tools;

namespace UnnamedTechMod.Common.Systems;

public class SelectionDrawSystem : ModSystem
{
    public override void PostDrawTiles()
    {
        if (Main.LocalPlayer.HeldItem.ModItem is not ConfiguratorToolItem) return;

        var connectionPlayer = Main.LocalPlayer.GetModPlayer<EnergyConnectionPlayer>();

        if (!connectionPlayer.InConnectionMode) return;


        var brightness =
            Lighting.Brightness(connectionPlayer.BoundRelay.Position.X, connectionPlayer.BoundRelay.Position.Y);


        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
            DepthStencilState.None, RasterizerState.CullCounterClockwise, null,
            Main.GameViewMatrix.TransformationMatrix);


        DrawRectangle(
            connectionPlayer.BoundRelay.Position,
            2,
            new Color(99, 23, 169) * 0.45f * brightness,
            new Color(99, 23, 169) * 0.9f * brightness
        );

        foreach (var source in connectionPlayer.BoundRelay.Sources)
        {
            DrawRectangle(
                source.Position,
                2,
                Color.CornflowerBlue * 0.45f * brightness,
                Color.CornflowerBlue * 0.95f * brightness
            );
        }

        foreach (var load in connectionPlayer.BoundRelay.Loads)
        {
            DrawRectangle(
                load.Position,
                2,
                Color.IndianRed * 0.45f * brightness,
                Color.IndianRed * 0.95f * brightness
            );
        }


        Main.spriteBatch.End();
    }
    
    // TODO: Extract into a utility class(?)
    private static void DrawRectangle(Point16 tilePosition, int outlineThickness,
        Color infillColor, Color outlineColor)
    {
        var origin = tilePosition.ToWorldCoordinates(0, 0) - Main.screenPosition;
        var tileObjectData = TileObjectData.GetTileData(Main.tile[tilePosition.ToPoint()]);

        if (tileObjectData is null) return;

        var lineWidth = (tileObjectData.Width * 16) - outlineThickness;
        var lineHeight = (tileObjectData.Height * 16) - outlineThickness;

        // Draw infill
        Main.spriteBatch.Draw(
            TextureAssets.MagicPixel.Value,
            origin + new Vector2(outlineThickness),
            new Rectangle(0, 0, lineWidth - outlineThickness, lineHeight - outlineThickness),
            infillColor,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            0
        );

        // Draw outline
        // Top (minus top right outlineThickness)
        Main.spriteBatch.Draw(
            TextureAssets.MagicPixel.Value,
            origin,
            new Rectangle(0, 0, lineWidth, outlineThickness),
            outlineColor,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            0
        );

        // Right (minus bottom right outlineThickness)
        Main.spriteBatch.Draw(
            TextureAssets.MagicPixel.Value,
            origin + new Vector2(lineWidth, 0),
            new Rectangle(0, 0, outlineThickness, lineHeight),
            outlineColor,
            0,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            0
        );

        // Bottom (minus bottom left outlineThickness)
        Main.spriteBatch.Draw(
            TextureAssets.MagicPixel.Value,
            origin + new Vector2(tileObjectData.Width * 16, tileObjectData.Height * 16),
            new Rectangle(0, 0, lineWidth, outlineThickness),
            outlineColor,
            MathHelper.Pi,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            0
        );

        // Left (minus top left outlineThickness)
        Main.spriteBatch.Draw(
            TextureAssets.MagicPixel.Value,
            origin + new Vector2(outlineThickness, tileObjectData.Height * 16),
            new Rectangle(0, 0, outlineThickness, lineHeight),
            outlineColor,
            MathHelper.Pi,
            Vector2.Zero,
            1,
            SpriteEffects.None,
            0
        );
    }
}