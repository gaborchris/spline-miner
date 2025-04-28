using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SplineMiner
{
    public class Track
    {
        private List<PlacedTrackNode> _placedNodes;
        private List<ShadowTrackNode> _shadowNodes;
        private int _selectedNodeIndex = -1;
        private Texture2D _pointTexture;
        private List<Vector2> _debugPoints = new List<Vector2>();
        private bool _enableDebugVisualization = true;
        private float _totalArcLength = 0f;
        private TrackPreview _preview;
        private const int SHADOW_NODE_COUNT = 2;

        public IReadOnlyList<ITrackNode> AllNodes => _placedNodes.Cast<ITrackNode>().Concat(_shadowNodes.Cast<ITrackNode>()).ToList();
        public IReadOnlyList<PlacedTrackNode> PlacedNodes => _placedNodes.AsReadOnly();
        public bool HasShadowNodes => _shadowNodes.Count == SHADOW_NODE_COUNT;
        public float TraversableLength => _totalArcLength * ((_placedNodes.Count - 1f) / (AllNodes.Count - 1f));
        public float TotalArcLength => _totalArcLength;

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

        public Track(List<Vector2> initialPoints)
        {
            _placedNodes = initialPoints.Select(p => new PlacedTrackNode(p)).ToList();
            _shadowNodes = new List<ShadowTrackNode>();
            UpdateShadowNodes();
        }

        private void UpdateShadowNodes()
        {
            _shadowNodes.Clear();
            if (_placedNodes.Count >= 2)
            {
                // Create shadow nodes based on the direction of the last two placed nodes
                Vector2 lastPos = _placedNodes[^1].Position;
                Vector2 direction = lastPos - _placedNodes[^2].Position;
                direction.Normalize();

                // Add two shadow nodes
                for (int i = 0; i < SHADOW_NODE_COUNT; i++)
                {
                    Vector2 shadowPos = lastPos + direction * ((i + 1) * 50); // 50 pixels spacing
                    _shadowNodes.Add(new ShadowTrackNode(shadowPos));
                }
            }
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _pointTexture = new Texture2D(graphicsDevice, 1, 1);
            _pointTexture.SetData(new[] { Color.White });
            _preview = new TrackPreview(this, graphicsDevice);
        }

        public int GetHoveredPointIndex(Vector2 mousePosition)
        {
            const float HOVER_RADIUS = 10f;
            for (int i = 0; i < AllNodes.Count; i++)
            {
                if (Vector2.Distance(AllNodes[i].Position, mousePosition) <= HOVER_RADIUS)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SelectPoint(int index)
        {
            _selectedNodeIndex = index;
        }

        public void MoveSelectedPoint(Vector2 position)
        {
            if (_selectedNodeIndex >= 0 && _selectedNodeIndex < AllNodes.Count)
            {
                if (_selectedNodeIndex < _placedNodes.Count)
                {
                    _placedNodes[_selectedNodeIndex].Position = position;
                }
                else
                {
                    int shadowIndex = _selectedNodeIndex - _placedNodes.Count;
                    _shadowNodes[shadowIndex].Position = position;
                }
                UpdateShadowNodes();
                RecalculateArcLength();
            }
        }

        public void ReleaseSelectedPoint()
        {
            _selectedNodeIndex = -1;
        }

        public void AddPoint(Vector2 position)
        {
            _placedNodes.Add(new PlacedTrackNode(position));
            UpdateShadowNodes();
            RecalculateArcLength();
        }

        public void DeletePoint(int index)
        {
            if (index >= 0 && index < _placedNodes.Count)
            {
                _placedNodes.RemoveAt(index);
                UpdateShadowNodes();
                RecalculateArcLength();
            }
        }

        public Vector2 GetPoint(float t)
        {
            var allPositions = AllNodes.Select(n => n.Position).ToList();
            
            // Ensure there are at least 4 control points for Catmull-Rom
            if (allPositions.Count < 4)
                throw new InvalidOperationException("At least 4 control points are required for Catmull-Rom splines.");

            // Determine which segment of the spline `t` is in
            int segment = (int)Math.Floor(t);
            float localT = t - segment;

            // Clamp segment index to valid range
            int p0 = Math.Clamp(segment - 1, 0, allPositions.Count - 1);
            int p1 = Math.Clamp(segment, 0, allPositions.Count - 1);
            int p2 = Math.Clamp(segment + 1, 0, allPositions.Count - 1);
            int p3 = Math.Clamp(segment + 2, 0, allPositions.Count - 1);

            // Perform Catmull-Rom interpolation
            return CatmullRom(allPositions[p0], allPositions[p1], allPositions[p2], allPositions[p3], localT);
        }

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
            if (AllNodes.Count < 4)
                return;

            // Draw the main track segments
            const int segments = 100;
            float traversableRatio = (_placedNodes.Count - 1f) / (AllNodes.Count - 1f);
            int traversableSegments = (int)(segments * traversableRatio);

            // Draw traversable track
            for (int i = 0; i < traversableSegments; i++)
            {
                float t1 = i / (float)segments;
                float t2 = (i + 1) / (float)segments;

                Vector2 point1 = GetPoint(t1 * (AllNodes.Count - 1));
                Vector2 point2 = GetPoint(t2 * (AllNodes.Count - 1));

                DrawLine(spriteBatch, point1, point2, Color.Black, 2);
            }

            // Draw shadow track with transparency
            for (int i = traversableSegments; i < segments; i++)
            {
                float t1 = i / (float)segments;
                float t2 = (i + 1) / (float)segments;

                Vector2 point1 = GetPoint(t1 * (AllNodes.Count - 1));
                Vector2 point2 = GetPoint(t2 * (AllNodes.Count - 1));

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

            // Draw all nodes
            for (int i = 0; i < AllNodes.Count; i++)
            {
                AllNodes[i].Draw(spriteBatch, _pointTexture, i == _selectedNodeIndex);
            }

            // Draw preview if available
            _preview?.Draw(spriteBatch);
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
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

        public void UpdatePreview(Vector2 mousePosition)
        {
            _preview.Update(mousePosition);
        }

        public bool IsHoveringEndpoint => _preview?.IsHoveringEndpoint ?? false;

        public Vector2 GetPointByDistance(float distance)
        {
            if (_totalArcLength <= 0)
            {
                RecalculateArcLength();
            }

            float maxDistance = _totalArcLength * ((_placedNodes.Count - 1f) / (AllNodes.Count - 1f));
            distance = Math.Min(distance, maxDistance);
            if (distance < 0) distance += maxDistance;

            float t = MapDistanceToT(distance, _totalArcLength);
            return GetPoint(t);
        }

        public void RecalculateArcLength()
        {
            _totalArcLength = ComputeArcLength(0f, AllNodes.Count - 1, 100);
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
                arcLength += Vector2.Distance(previousPoint, currentPoint);

                if (_enableDebugVisualization && i % 5 == 0)
                {
                    _debugPoints.Add(currentPoint);
                }

                previousPoint = currentPoint;
            }

            return arcLength;
        }

        private float MapDistanceToT(float distance, float totalLength)
        {
            if (totalLength <= 0)
            {
                return 0f;
            }

            float normalizedTarget = distance / totalLength;
            normalizedTarget = Math.Clamp(normalizedTarget, 0f, 1f);
            
            float tMin = 0f;
            float tMax = AllNodes.Count - 1;
            float tMid = 0f;
            float lastArcLength = 0f;

            for (int i = 0; i < 30; i++)
            {
                tMid = (tMin + tMax) / 2f;
                float arcLength = ComputeArcLength(0f, tMid);
                float normalizedArcLength = arcLength / totalLength;

                if (Math.Abs(normalizedArcLength - normalizedTarget) < 0.001f)
                {
                    break;
                }
                else if (Math.Abs(lastArcLength - arcLength) < 0.0001f && i > 5)
                {
                    break;
                }
                else if (normalizedArcLength < normalizedTarget)
                {
                    tMin = tMid;
                }
                else
                {
                    tMax = tMid;
                }
                
                lastArcLength = arcLength;
            }

            return tMid;
        }

        public void VisualizeEquallySpacedPoints(int count)
        {
            _debugPoints.Clear();
            float totalLength = _totalArcLength > 0 ? _totalArcLength : ComputeArcLength(0f, AllNodes.Count - 1);
            float stepSize = totalLength / count;
            
            for (int i = 0; i < count; i++)
            {
                float distance = i * stepSize;
                Vector2 point = GetPointByDistance(distance);
                _debugPoints.Add(point);
            }
        }
    }
}
