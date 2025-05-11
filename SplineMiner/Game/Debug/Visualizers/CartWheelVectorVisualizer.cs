using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Utils;
using SplineMiner.Game.Cart;
using SplineMiner.Game.Debug.Interfaces;

namespace SplineMiner.Game.Debug.Visualizers
{
    /// <summary>
    /// Visualizes wheel positions and normal/tangent vectors for a cart.
    /// </summary>
    public class CartWheelVectorVisualizer : ICartDebugVisualizer
    {
        private readonly Texture2D _debugTexture;
        private const float INDICATOR_LENGTH = 20f;
        private const float VECTOR_LENGTH = 30f;

        public CartWheelVectorVisualizer(GraphicsDevice graphicsDevice)
        {
            _debugTexture = new Texture2D(graphicsDevice, 1, 1);
            _debugTexture.SetData([Color.White]);
        }

        public void Draw(SpriteBatch spriteBatch, CartModel cart)
        {
            System.Diagnostics.Debug.WriteLine("Drawing cart wheel vectors");
            // Draw wheel positions
            DrawingHelpers.DrawCircle(spriteBatch, _debugTexture, cart.WheelSystem.FrontWheelPosition, 3, Color.Green);
            DrawingHelpers.DrawCircle(spriteBatch, _debugTexture, cart.WheelSystem.BackWheelPosition, 3, Color.Blue);

            // Draw line between wheels
            DrawingHelpers.DrawLine(spriteBatch, _debugTexture, cart.WheelSystem.FrontWheelPosition, cart.WheelSystem.BackWheelPosition, Color.Yellow, 1);

            // Draw cart's position point
            DrawingHelpers.DrawCircle(spriteBatch, _debugTexture, cart.Position, 3, Color.White);

            // Draw rotation change indicator
            Vector2 indicatorEnd = cart.Position + new Vector2(
                (float)Math.Cos(cart.Rotation) * INDICATOR_LENGTH,
                (float)Math.Sin(cart.Rotation) * INDICATOR_LENGTH
            );
            DrawingHelpers.DrawLine(spriteBatch, _debugTexture, cart.Position, indicatorEnd, Color.Purple, 2);

            // Draw normal and tangent vectors
            float normalAngle = cart.Rotation + MathHelper.PiOver2;
            float tangentAngle = cart.Rotation;

            // Draw normal vector (perpendicular to track)
            Vector2 normalEnd = cart.Position + new Vector2(
                (float)Math.Cos(normalAngle) * VECTOR_LENGTH,
                (float)Math.Sin(normalAngle) * VECTOR_LENGTH
            );
            DrawingHelpers.DrawLine(spriteBatch, _debugTexture, cart.Position, normalEnd, Color.Red, 2);

            // Draw tangent vector (along track)
            Vector2 tangentEnd = cart.Position + new Vector2(
                (float)Math.Cos(tangentAngle) * VECTOR_LENGTH,
                (float)Math.Sin(tangentAngle) * VECTOR_LENGTH
            );
            DrawingHelpers.DrawLine(spriteBatch, _debugTexture, cart.Position, tangentEnd, Color.Green, 2);
        }
    }
} 