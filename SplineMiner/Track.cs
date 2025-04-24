using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace SplineMiner
{
    public class Track
    {
        public List<Vector2> Points { get; private set; }

        public Track(List<Vector2> points)
        {
            Points = points;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: note this just draws a line and not a curve
            // In order to draw a curve need to implement a Bezier curve or Catmull-Rom spline
            for (int i = 0; i < Points.Count - 1; i++)
            {
                DrawLine(spriteBatch, Points[i], Points[i + 1], Color.Black, 2);
            }
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)System.Math.Atan2(edge.Y, edge.X);
            var texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White }); 
            spriteBatch.Draw(
                texture: texture,
                position: start,
                sourceRectangle: null,
                color: color,
                rotation: angle,
                origin: Vector2.Zero,
                scale: new Vector2(edge.Length(), thickness),
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }
    }
}
