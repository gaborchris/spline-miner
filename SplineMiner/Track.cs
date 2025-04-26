using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SplineMiner
{
    public class Track
    {
        public List<Vector2> ControlPoints { get; private set; }

        /*
        A track is a multidimensional line f(t) = (x(t), y(t))
        where t is a parameter that varies over time.
        The player position will initially change by a constant dt
        but as gravity is added to the game, the amount a player can move will be constrained
        by the steepness of the track at that point.

        For the first iteration, we can just make the player move at a constant speed even the track is steep.
         */

        public Track(List<Vector2> points)
        {
            ControlPoints = points;
        }

        public Vector2 GetPoint(float t)
        {

            // Ensure there are at least 4 control points for Catmull-Rom
            if (ControlPoints.Count < 4)
                throw new InvalidOperationException("At least 4 control points are required for Catmull-Rom splines.");

            // Determine which segment of the spline `t` is in
            int segment = (int)Math.Floor(t);
            float localT = t - segment;

            // Clamp segment index to valid range
            int p0 = Math.Clamp(segment - 1, 0, ControlPoints.Count - 1);
            int p1 = Math.Clamp(segment, 0, ControlPoints.Count - 1);
            int p2 = Math.Clamp(segment + 1, 0, ControlPoints.Count - 1);
            int p3 = Math.Clamp(segment + 2, 0, ControlPoints.Count - 1);

            // Perform Catmull-Rom interpolation
            return CatmullRom(ControlPoints[p0], ControlPoints[p1], ControlPoints[p2], ControlPoints[p3], localT);
        }
        // Catmull-Rom spline interpolation formula
        private Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                (2 * p1) +
                (-p0 + p2) * t +
                (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                (-p0 + 3 * p1 - 3 * p2 + p3) * t3
            );
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the track as a series of small line segments
            const int segments = 100;
            for (int i = 0; i < segments; i++)
            {
                float t1 = i / (float)segments;
                float t2 = (i + 1) / (float)segments;

                Vector2 point1 = GetPoint(t1 * (ControlPoints.Count - 1));
                Vector2 point2 = GetPoint(t2 * (ControlPoints.Count - 1));

                DrawLine(spriteBatch, point1, point2, Color.Black, 2);
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
