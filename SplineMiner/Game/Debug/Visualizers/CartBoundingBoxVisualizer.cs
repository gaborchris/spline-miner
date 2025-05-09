using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Physics.Components;
using SplineMiner.Core.Utils;
using SplineMiner.Game.Cart;
using SplineMiner.Game.Debug.Interfaces;

namespace SplineMiner.Game.Debug.Visualizers
{
    /// <summary>
    /// Visualizes the bounding box of a cart for collision debugging.
    /// </summary>
    public class CartBoundingBoxVisualizer : ICartDebugVisualizer
    {
        private readonly Texture2D _debugTexture;
        private const int LINE_THICKNESS = 2;

        public CartBoundingBoxVisualizer(GraphicsDevice graphicsDevice)
        {
            _debugTexture = new Texture2D(graphicsDevice, 1, 1);
            _debugTexture.SetData([Color.White]);
        }

        public void Draw(SpriteBatch spriteBatch, CartController cart)
        {
            if (cart.BoundingBox is Core.Physics.Components.BoundingBox boundingBox)
            {
                // Draw the bounding box rectangle
                Vector2 topLeft = boundingBox.Position;
                Vector2 topRight = new(boundingBox.Position.X + boundingBox.Size.X, boundingBox.Position.Y);
                Vector2 bottomLeft = new(boundingBox.Position.X, boundingBox.Position.Y + boundingBox.Size.Y);
                Vector2 bottomRight = boundingBox.Position + boundingBox.Size;

                // Draw the four sides of the rectangle
                DrawingHelpers.DrawLine(spriteBatch, _debugTexture, topLeft, topRight, Color.Red, LINE_THICKNESS);
                DrawingHelpers.DrawLine(spriteBatch, _debugTexture, topRight, bottomRight, Color.Red, LINE_THICKNESS);
                DrawingHelpers.DrawLine(spriteBatch, _debugTexture, bottomRight, bottomLeft, Color.Red, LINE_THICKNESS);
                DrawingHelpers.DrawLine(spriteBatch, _debugTexture, bottomLeft, topLeft, Color.Red, LINE_THICKNESS);

                // Draw the center point of the bounding box
                Vector2 center = boundingBox.Position + (boundingBox.Size / 2);
                DrawingHelpers.DrawCircle(spriteBatch, _debugTexture, center, 3, Color.Yellow);
            }
        }
    }
} 