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
        where t is a parameter that can be queried. 

        The player position will only change change by a constant dt

        For the first iteration, we can just make the player move at a constant speed even the track is steep.
        
        TODO: A lot of this code is buggy and experimental but does the basic features
        Focus on implementing control of track first with UX and then get a proper interpolator function
        Right now the cart jumps from point to point and will require some close debugging and mathematical
        derivations.

        It might be worth completely rewriting this class at some point, but the GetPoint method should remain.

        In future, might add a gravity feature so that the amount a player can move will be constrained
        by the steepness of the track at that point.

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
        private float ComputeArcLength(float tStart, float tEnd, int steps = 10)
        {
            float arcLength = 0f;
            float dt = (tEnd - tStart) / steps;

            Vector2 previousPoint = GetPoint(tStart);
            for (int i = 1; i <= steps; i++)
            {
                float t = tStart + i * dt;
                Vector2 currentPoint = GetPoint(t);

                // Add the distance between the previous and current points
                arcLength += Vector2.Distance(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            return arcLength;
        }
        private float MapDistanceToT(float distance, float totalLength, int binarySearchSteps = 20)
        {
            float tMin = 0f;
            float tMax = ControlPoints.Count - 1;
            float tMid = 0f;

            for (int i = 0; i < binarySearchSteps; i++)
            {
                tMid = (tMin + tMax) / 2f;

                // Compute the arc length from t=0 to tMid
                float arcLength = ComputeArcLength(0f, tMid);

                if (Math.Abs(arcLength - distance) < 0.01f) // Close enough
                {
                    break;
                }
                else if (arcLength < distance)
                {
                    tMin = tMid; // Search in the upper half
                }
                else
                {
                    tMax = tMid; // Search in the lower half
                }
            }

            return tMid;
        }
        public Vector2 GetPointByDistance(float distance)
        {
            // Compute the total length of the curve on the fly
            float totalLength = ComputeArcLength(0f, ControlPoints.Count - 1);

            // Wrap the distance to ensure it loops around the track
            distance = distance % totalLength;
            if (distance < 0) distance += totalLength;

            // Map the distance to a `t` value
            float t = MapDistanceToT(distance, totalLength);

            // Get the point corresponding to the `t` value
            return GetPoint(t);
        }
    }
}
