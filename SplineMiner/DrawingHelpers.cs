using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
            if (points == null) throw new ArgumentNullException(nameof(points));
            if (points.Length < 4) return;
            if (segments < 1) throw new ArgumentException("Segments must be greater than 0", nameof(segments));

            // Calculate how many curve segments we can draw
            int numCurveSegments = points.Length - 3;

            // For each curve segment
            for (int curveSegment = 0; curveSegment < numCurveSegments; curveSegment++)
            {
                // Draw the subdivided segments for this curve segment
                for (int i = 0; i < segments; i++)
                {
                    float t1 = i / (float)segments;
                    float t2 = (i + 1) / (float)segments;

                    Vector2 point1 = SplineUtils.CatmullRom(
                        points[curveSegment],
                        points[curveSegment + 1],
                        points[curveSegment + 2],
                        points[curveSegment + 3],
                        t1);

                    Vector2 point2 = SplineUtils.CatmullRom(
                        points[curveSegment],
                        points[curveSegment + 1],
                        points[curveSegment + 2],
                        points[curveSegment + 3],
                        t2);

                    DrawLine(spriteBatch, texture, point1, point2, color, thickness);
                }
            }
        }
    }
} 