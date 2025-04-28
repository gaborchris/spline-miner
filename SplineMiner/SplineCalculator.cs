using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using SplineMiner.Core;

namespace SplineMiner
{
    public class SplineCalculator : ISplineCalculator
    {
        private readonly List<PlacedTrackNode> _placedNodes;
        private const int MIN_NODES = 4;

        public SplineCalculator(List<PlacedTrackNode> placedNodes)
        {
            _placedNodes = placedNodes;
        }

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