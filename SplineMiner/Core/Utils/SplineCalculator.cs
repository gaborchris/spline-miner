using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using SplineMiner.Core.Interfaces;
using SplineMiner.Game.Track;

namespace SplineMiner.Core.Utils
{
    /// <summary>
    /// Handles calculations for Catmull-Rom spline curves used in track generation and manipulation.
    /// </summary>
    /// <remarks>
    /// TODO: Consider implementing caching for frequently calculated values
    /// TODO: Add support for different spline types (Bezier, B-spline)
    /// TODO: Implement parallel processing for arc length calculations
    /// TODO: Add validation for input parameters
    /// </remarks>
    public class SplineCalculator : ISplineCalculator
    {
        private readonly List<PlacedTrackNode> _placedNodes;
        private const int MIN_NODES = 4;

        /// <summary>
        /// Initializes a new instance of the SplineCalculator.
        /// </summary>
        /// <param name="placedNodes">List of track nodes that define the spline curve.</param>
        /// <exception cref="ArgumentNullException">Thrown when placedNodes is null.</exception>
        public SplineCalculator(List<PlacedTrackNode> placedNodes)
        {
            _placedNodes = placedNodes ?? throw new ArgumentNullException(nameof(placedNodes));
        }

        /// <summary>
        /// Calculates a point on the spline at the given parameter value.
        /// </summary>
        /// <param name="t">Parameter value. Integer part determines segment index, fractional part determines position within segment.</param>
        /// <returns>A Vector2 position on the spline curve.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there are fewer than 4 control points.</exception>
        public Vector2 GetPoint(float t)
        {
            if (_placedNodes.Count < MIN_NODES)
                throw new InvalidOperationException("At least 4 control points are required for Catmull-Rom splines.");

            int segment = (int)Math.Floor(t);
            float localT = t - segment;

            int p0 = Math.Clamp(segment - 1, 0, _placedNodes.Count - 1);
            int p1 = Math.Clamp(segment, 0, _placedNodes.Count - 1);
            int p2 = Math.Clamp(segment + 1, 0, _placedNodes.Count - 1);
            int p3 = Math.Clamp(segment + 2, 0, _placedNodes.Count - 1);

            return SplineUtils.CatmullRom(
                _placedNodes[p0].Position,
                _placedNodes[p1].Position,
                _placedNodes[p2].Position,
                _placedNodes[p3].Position,
                localT
            );
        }

        /// <summary>
        /// Converts a distance along the spline into a parameter value.
        /// </summary>
        /// <param name="distance">The distance along the spline.</param>
        /// <param name="totalLength">The total length of the spline.</param>
        /// <returns>The parameter value t that corresponds to the given distance.</returns>
        /// <remarks>
        /// Uses binary search to find the parameter value. The accuracy improves with more iterations,
        /// but the default maximum iterations should be sufficient for most cases.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when totalLength is less than or equal to 0.</exception>
        public float GetParameterForDistance(float distance, float totalLength)
        {
            if (totalLength <= 0) return 0f;

            float normalizedTarget = distance / totalLength;
            normalizedTarget = Math.Clamp(normalizedTarget, 0f, 1f);

            float tMin = 0f;
            float tMax = _placedNodes.Count - 1;
            float tMid = 0f;
            float lastArcLength = 0f;

            int maxIterations = 30 + (int)(_placedNodes.Count * 0.5f);

            for (int i = 0; i < maxIterations; i++)
            {
                tMid = (tMin + tMax) / 2f;
                float arcLength = ComputeArcLength(0f, tMid);
                float normalizedArcLength = arcLength / totalLength;

                if (Math.Abs(normalizedArcLength - normalizedTarget) < 0.0001f)
                    break;
                else if (Math.Abs(lastArcLength - arcLength) < 0.00001f && i > 5)
                    break;
                else if (normalizedArcLength < normalizedTarget)
                    tMin = tMid;
                else
                    tMax = tMid;

                lastArcLength = arcLength;
            }

            return tMid;
        }

        /// <summary>
        /// Calculates the arc length of a portion of the spline curve.
        /// </summary>
        /// <param name="tStart">Starting parameter value.</param>
        /// <param name="tEnd">Ending parameter value.</param>
        /// <param name="baseSteps">Base number of steps for length calculation. Higher values give more accurate results.</param>
        /// <returns>The approximate length of the spline segment.</returns>
        /// <remarks>
        /// Uses adaptive sampling to improve accuracy for segments with high curvature.
        /// When segment length exceeds 10 units, additional midpoints are calculated.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when tStart is greater than tEnd.</exception>
        public float ComputeArcLength(float tStart, float tEnd, int baseSteps = 40)
        {
            float arcLength = 0f;
            int steps = baseSteps * (int)Math.Ceiling((tEnd - tStart) / 2.0f);
            float dt = (tEnd - tStart) / steps;

            Vector2 previousPoint = GetPoint(tStart);
            for (int i = 1; i <= steps; i++)
            {
                float t = tStart + i * dt;
                Vector2 currentPoint = GetPoint(t);
                float segmentLength = Vector2.Distance(previousPoint, currentPoint);

                if (segmentLength > 10.0f)
                {
                    float midT = tStart + (i - 0.5f) * dt;
                    Vector2 midPoint = GetPoint(midT);
                    arcLength += Vector2.Distance(previousPoint, midPoint);
                    arcLength += Vector2.Distance(midPoint, currentPoint);
                }
                else
                {
                    arcLength += segmentLength;
                }

                previousPoint = currentPoint;
            }
            return arcLength;
        }
    }
}