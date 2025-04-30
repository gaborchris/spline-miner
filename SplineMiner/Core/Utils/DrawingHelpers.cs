using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SplineMiner.Core.Utils
{
    public static class DrawingHelpers
    {
        public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
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

        public static void DrawCircle(SpriteBatch spriteBatch, Texture2D texture, Vector2 center, float radius, Color color)
        {
            spriteBatch.Draw(
                texture: texture,
                position: center,
                sourceRectangle: null,
                color: color,
                rotation: 0f,
                origin: new Vector2(0.5f, 0.5f),
                scale: new Vector2(radius * 2, radius * 2),
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }

        public static void DrawSplineCurve(SpriteBatch spriteBatch, Texture2D texture, Vector2[] points, int segments, Color color, int thickness)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));
            if (points.Length < 4) return;
            if (segments < 1) throw new ArgumentException("Segments must be greater than 0", nameof(segments));

            // For each curve segment between the points
            for (int i = 0; i < points.Length - 1; i++)
            {
                // Get 4 points needed for the Catmull-Rom spline
                Vector2 p0 = points[Math.Max(i - 1, 0)];
                Vector2 p1 = points[i];
                Vector2 p2 = points[i + 1];
                Vector2 p3 = points[Math.Min(i + 2, points.Length - 1)];

                // Draw the subdivided segments
                for (int j = 0; j < segments; j++)
                {
                    float t1 = j / (float)segments;
                    float t2 = (j + 1) / (float)segments;

                    Vector2 point1 = SplineUtils.CatmullRom(p0, p1, p2, p3, t1);
                    Vector2 point2 = SplineUtils.CatmullRom(p0, p1, p2, p3, t2);

                    DrawLine(spriteBatch, texture, point1, point2, color, thickness);
                }
            }
        }
    }
}