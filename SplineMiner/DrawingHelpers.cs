using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplineMiner
{
    public static class DrawingHelpers
    {
        public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)System.Math.Atan2(edge.Y, edge.X);
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
            if (points.Length < 4) return;

            for (int i = 0; i < segments; i++)
            {
                float t1 = i / (float)segments;
                float t2 = (i + 1) / (float)segments;

                Vector2 point1 = CatmullRomPoint(points, t1);
                Vector2 point2 = CatmullRomPoint(points, t2);

                DrawLine(spriteBatch, texture, point1, point2, color, thickness);
            }
        }

        private static Vector2 CatmullRomPoint(Vector2[] points, float t)
        {
            int numPoints = points.Length;
            if (numPoints < 4) return Vector2.Zero;

            float t2 = t * t;
            float t3 = t2 * t;

            // Scale t to the number of segments
            float scaledT = t * (numPoints - 3);
            int segment = (int)System.Math.Floor(scaledT);
            float localT = scaledT - segment;

            // Get the points for interpolation
            int p0 = System.Math.Clamp(segment, 0, numPoints - 4);
            int p1 = p0 + 1;
            int p2 = p0 + 2;
            int p3 = p0 + 3;

            return SplineUtils.CatmullRom(points[p0], points[p1], points[p2], points[p3], localT);
        }
    }
} 