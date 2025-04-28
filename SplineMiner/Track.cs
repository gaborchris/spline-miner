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
        private const float CONTROL_POINT_RADIUS = 10f;
        private int _selectedPointIndex = -1;
        private Texture2D _pointTexture;
        private List<Vector2> _debugPoints = new List<Vector2>();
        private bool _enableDebugVisualization = true;
        private float _totalArcLength = 0f;
        private TrackPreview _preview;
        private const int SHADOW_NODES = 2; // Number of nodes at the end that act as shadow nodes

        public float TraversableLength => _totalArcLength * GetTraversableRatio();
        public bool HasShadowNodes => ControlPoints.Count > SHADOW_NODES;

        /*
        A track is a multidimensional line f(t) = (x(t), y(t))
        where t is a parameter that can be queried. 

        The player position will only change change by a constant dt

        For the first iteration, we can just make the player move at a constant speed even the track is steep.
        
        TODO: A lot of this code is buggy and experimental but does the basic features
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

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _pointTexture = new Texture2D(graphicsDevice, 1, 1);
            _pointTexture.SetData(new[] { Color.White });
            _preview = new TrackPreview(this, graphicsDevice);
        }

        public int GetHoveredPointIndex(Vector2 mousePosition)
        {
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                if (Vector2.Distance(ControlPoints[i], mousePosition) <= CONTROL_POINT_RADIUS)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SelectPoint(int index)
        {
            _selectedPointIndex = index;
            Debug.WriteLine("Point selected: " + _selectedPointIndex);
        }

        public void MoveSelectedPoint(Vector2 position)
        {
            if (_selectedPointIndex >= 0 && _selectedPointIndex < ControlPoints.Count)
            {
                ControlPoints[_selectedPointIndex] = position;
                Debug.WriteLine("Selected point moved to: " + position);
                
                // Recalculate arc length when points change
                RecalculateArcLength();
            }
        }

        public void ReleaseSelectedPoint()
        {
            _selectedPointIndex = -1;
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
            if (ControlPoints.Count < 4)
                return;

            // Draw the main track segments
            const int segments = 100;
            float traversableRatio = GetTraversableRatio();
            int traversableSegments = (int)(segments * traversableRatio);

            // Draw traversable track
            for (int i = 0; i < traversableSegments; i++)
            {
                float t1 = i / (float)segments;
                float t2 = (i + 1) / (float)segments;

                Vector2 point1 = GetPoint(t1 * (ControlPoints.Count - 1));
                Vector2 point2 = GetPoint(t2 * (ControlPoints.Count - 1));

                DrawLine(spriteBatch, point1, point2, Color.Black, 2);
            }

            // Draw shadow track with transparency
            for (int i = traversableSegments; i < segments; i++)
            {
                float t1 = i / (float)segments;
                float t2 = (i + 1) / (float)segments;

                Vector2 point1 = GetPoint(t1 * (ControlPoints.Count - 1));
                Vector2 point2 = GetPoint(t2 * (ControlPoints.Count - 1));

                DrawLine(spriteBatch, point1, point2, Color.Gray * 0.5f, 2);
            }

            // Draw debug points if enabled
            if (_enableDebugVisualization)
            {
                foreach (var point in _debugPoints)
                {
                    DrawCircle(spriteBatch, point, 3, Color.Yellow);
                }

                if (_debugPoints.Count > 200)
                {
                    _debugPoints.RemoveRange(0, 100);
                }
            }

            // Draw the control points with different colors for regular and shadow nodes
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                Color pointColor;
                if (i == _selectedPointIndex)
                {
                    pointColor = Color.Red;
                }
                else if (i >= ControlPoints.Count - SHADOW_NODES)
                {
                    pointColor = Color.Gray * 0.5f; // Shadow nodes are semi-transparent
                }
                else
                {
                    pointColor = Color.Blue;
                }
                DrawCircle(spriteBatch, ControlPoints[i], CONTROL_POINT_RADIUS, pointColor);
            }

            // Draw preview if available
            _preview?.Draw(spriteBatch);
        }

        private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color)
        {
            spriteBatch.Draw(
                texture: _pointTexture,
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

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)System.Math.Atan2(edge.Y, edge.X);
            spriteBatch.Draw(
                texture: _pointTexture,
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

        private float ComputeArcLength(float tStart, float tEnd, int steps = 40)
        {
            float arcLength = 0f;
            float dt = (tEnd - tStart) / steps;

            Vector2 previousPoint = GetPoint(tStart);
            for (int i = 1; i <= steps; i++)
            {
                float t = tStart + i * dt;
                Vector2 currentPoint = GetPoint(t);

                // Add the distance between the previous and current points
                float segmentLength = Vector2.Distance(previousPoint, currentPoint);
                arcLength += segmentLength;

                if (_enableDebugVisualization && i % 5 == 0)
                {
                    // Store points for debug visualization
                    _debugPoints.Add(currentPoint);
                }

                previousPoint = currentPoint;
            }

            return arcLength;
        }
        private float MapDistanceToT(float distance, float totalLength, int binarySearchSteps = 30)
        {
            // Ensure we have a valid totalLength
            if (totalLength <= 0)
            {
                Debug.WriteLine("Warning: Total arc length is zero or negative!");
                return 0f;
            }

            // Calculate normalized target distance (0 to 1)
            float normalizedTarget = distance / totalLength;
            
            // Clamp to valid range
            normalizedTarget = Math.Clamp(normalizedTarget, 0f, 1f);
            
            float tMin = 0f;
            float tMax = ControlPoints.Count - 1;
            float tMid = 0f;
            float lastArcLength = 0f;

            for (int i = 0; i < binarySearchSteps; i++)
            {
                tMid = (tMin + tMax) / 2f;

                // Compute the arc length from t=0 to tMid
                float arcLength = ComputeArcLength(0f, tMid);
                float normalizedArcLength = arcLength / totalLength;

                // Debug every few steps
                /*
                if (i % 10 == 0)
                {
                    Debug.WriteLine($"Binary search step {i}: t={tMid}, arcLength={arcLength}, target={distance}");
                }
                */

                // Check if we're close enough
                if (Math.Abs(normalizedArcLength - normalizedTarget) < 0.001f)
                {
                    break;
                }
                // Check if we're making progress
                else if (Math.Abs(lastArcLength - arcLength) < 0.0001f && i > 5)
                {
                    Debug.WriteLine("Warning: Binary search not making progress");
                    break;
                }
                else if (normalizedArcLength < normalizedTarget)
                {
                    tMin = tMid; // Search in the upper half
                }
                else
                {
                    tMax = tMid; // Search in the lower half
                }
                
                lastArcLength = arcLength;
            }

            return tMid;
        }
        public Vector2 GetPointByDistance(float distance)
        {
            // If arc length hasn't been calculated yet, do it once
            if (_totalArcLength <= 0)
            {
                RecalculateArcLength();
            }

            // Clamp the distance to prevent going into shadow node territory
            float maxDistance = TraversableLength;
            distance = Math.Min(distance, maxDistance);
            if (distance < 0) distance += maxDistance;

            // Map the distance to a `t` value
            float t = MapDistanceToT(distance, _totalArcLength);

            // Get the point corresponding to the `t` value
            return GetPoint(t);
        }

        // Add this new method to calculate the total arc length once
        public void RecalculateArcLength()
        {
            _totalArcLength = ComputeArcLength(0f, ControlPoints.Count - 1, 100);
            Debug.WriteLine($"Total arc length: {_totalArcLength}");
        }

        // Add a visualization to show equally spaced points along the track
        public void VisualizeEquallySpacedPoints(int count)
        {
            _debugPoints.Clear();
            
            float totalLength = _totalArcLength > 0 ? _totalArcLength : ComputeArcLength(0f, ControlPoints.Count - 1);
            float stepSize = totalLength / count;
            
            for (int i = 0; i < count; i++)
            {
                float distance = i * stepSize;
                Vector2 point = GetPointByDistance(distance);
                _debugPoints.Add(point);
                Debug.WriteLine($"Equal spaced point {i}: distance={distance}, point={point}");
            }
        }

        public void DeletePoint(int index)
        {
            if (index >= 0 && index < ControlPoints.Count)
            {
                ControlPoints.RemoveAt(index);
                RecalculateArcLength();
            }
        }

        public void AddPoint(Vector2 position)
        {
            ControlPoints.Add(position);
            RecalculateArcLength();
        }

        public void UpdatePreview(Vector2 mousePosition)
        {
            _preview.Update(mousePosition);
        }

        public bool IsHoveringEndpoint => _preview?.IsHoveringEndpoint ?? false;

        private float GetTraversableRatio()
        {
            if (ControlPoints.Count <= 4) // Minimum points needed for a valid track
                return 0f;

            // Calculate ratio excluding shadow nodes
            float traversableSegments = ControlPoints.Count - (SHADOW_NODES + 1);
            float totalSegments = ControlPoints.Count - 1;
            return traversableSegments / totalSegments;
        }
    }
}
