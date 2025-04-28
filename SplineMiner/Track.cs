using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SplineMiner
{
    public class Track
    {
        private readonly List<PlacedTrackNode> _placedNodes;
        private readonly List<ShadowTrackNode> _shadowNodes;
        private int _selectedNodeIndex = -1;
        private Texture2D _pointTexture;
        private readonly List<Vector2> _debugPoints = [];
        private readonly bool _enableDebugVisualization = false;
        private float _totalArcLength = 0f;
        private TrackPreview _preview;
        private const int MIN_SHADOW_NODES = 3; // Minimum needed for Catmull-Rom
        private readonly UIManager _uiManager;

        public IReadOnlyList<PlacedTrackNode> PlacedNodes => _placedNodes.AsReadOnly();
        public IReadOnlyList<ShadowTrackNode> ShadowNodes => _shadowNodes.AsReadOnly();
        public float TotalArcLength => _totalArcLength;

        public Track(List<Vector2> initialPoints, UIManager uiManager)
        {
            _placedNodes = initialPoints.Select(p => new PlacedTrackNode(p)).ToList();
            _shadowNodes = [];
            _uiManager = uiManager;
            UpdateShadowNodes();
        }

        private void UpdateShadowNodes()
        {
            _shadowNodes.Clear();
            if (_placedNodes.Count >= 2)
            {
                // Get direction from last two placed nodes
                Vector2 lastPos = _placedNodes[^1].Position;
                Vector2 direction = lastPos - _placedNodes[^2].Position;
                direction.Normalize();

                // Add enough shadow nodes for proper Catmull-Rom interpolation
                for (int i = 0; i < MIN_SHADOW_NODES; i++)
                {
                    Vector2 shadowPos = lastPos + direction * ((i + 1) * 50);
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
            
            // Check placed nodes first
            for (int i = 0; i < _placedNodes.Count; i++)
            {
                if (Vector2.Distance(_placedNodes[i].Position, mousePosition) <= HOVER_RADIUS)
                {
                    return i;
                }
            }
            
            // Then check shadow nodes
            for (int i = 0; i < _shadowNodes.Count; i++)
            {
                if (Vector2.Distance(_shadowNodes[i].Position, mousePosition) <= HOVER_RADIUS)
                {
                    return i + _placedNodes.Count; // Offset by placed nodes count
                }
            }
            
            return -1;
        }

        public void SelectPoint(int index)
        {
            if (index >= _placedNodes.Count) // Only allow selecting shadow nodes
            {
                _selectedNodeIndex = index;
            }
        }

        public void MoveSelectedPoint(Vector2 position)
        {
            if (_selectedNodeIndex >= _placedNodes.Count && _selectedNodeIndex < _placedNodes.Count + _shadowNodes.Count)
            {
                int shadowIndex = _selectedNodeIndex - _placedNodes.Count;
                _shadowNodes[shadowIndex].Position = position;
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

        private Vector2 GetPlacedTrackPoint(float t)
        {
            if (_placedNodes.Count < 4)
                throw new InvalidOperationException("At least 4 control points are required for Catmull-Rom splines.");

            int segment = (int)Math.Floor(t);
            float localT = t - segment;

            // Get points for Catmull-Rom calculation
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_placedNodes.Count < 4)
                return;

            // Draw placed track
            const int segments = 100;
            Vector2[] placedPoints = _placedNodes.Select(n => n.Position).ToArray();
            DrawingHelpers.DrawSplineCurve(spriteBatch, _pointTexture, placedPoints, segments, Color.Black, 2);

            // Draw shadow track and nodes only when track tool is selected
            if (_uiManager.CurrentTool == UITool.Track)
            {
                // Draw preview if available
                _preview?.Draw(spriteBatch);

                // Draw shadow nodes and their connecting curve only if we have enough nodes
                if (_shadowNodes.Count >= MIN_SHADOW_NODES)
                {
                    // Create combined array for shadow curve
                    var combinedPoints = new List<Vector2>();
                    // Add last few placed nodes for proper curve connection
                    combinedPoints.AddRange(_placedNodes.Skip(Math.Max(0, _placedNodes.Count - 3))
                                                    .Select(n => n.Position));
                    combinedPoints.AddRange(_shadowNodes.Select(n => n.Position));

                    if (combinedPoints.Count >= 4)
                    {
                        DrawingHelpers.DrawSplineCurve(spriteBatch, _pointTexture, combinedPoints.ToArray(), segments, Color.Gray * 0.5f, 2);
                    }

                    // Draw shadow nodes
                    foreach (var (node, index) in _shadowNodes.Select((n, i) => (n, i)))
                    {
                        node.Draw(spriteBatch, _pointTexture, index + _placedNodes.Count == _selectedNodeIndex);
                    }
                }
            }

            // Draw debug points
            if (_enableDebugVisualization)
            {
                foreach (var point in _debugPoints)
                {
                    DrawingHelpers.DrawCircle(spriteBatch, _pointTexture, point, 3, Color.Yellow);
                }

                if (_debugPoints.Count > 200)
                {
                    _debugPoints.RemoveRange(0, 100);
                }
            }

            // Draw placed nodes
            foreach (var node in _placedNodes)
            {
                node.Draw(spriteBatch, _pointTexture, false);
            }
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

            distance = Math.Min(distance, _totalArcLength);
            if (distance < 0) distance += _totalArcLength;

            float t = MapDistanceToT(distance, _totalArcLength);
            return GetPlacedTrackPoint(t);
        }

        public void RecalculateArcLength()
        {
            _totalArcLength = ComputeArcLength(0f, _placedNodes.Count - 1, 100);
        }

        private float ComputeArcLength(float tStart, float tEnd, int steps = 40)
        {
            float arcLength = 0f;
            float dt = (tEnd - tStart) / steps;

            Vector2 previousPoint = GetPlacedTrackPoint(tStart);
            for (int i = 1; i <= steps; i++)
            {
                float t = tStart + i * dt;
                Vector2 currentPoint = GetPlacedTrackPoint(t);
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
            if (totalLength <= 0) return 0f;

            float normalizedTarget = distance / totalLength;
            normalizedTarget = Math.Clamp(normalizedTarget, 0f, 1f);
            
            float tMin = 0f;
            float tMax = _placedNodes.Count - 1;
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
            float stepSize = _totalArcLength / count;
            
            for (int i = 0; i < count; i++)
            {
                float distance = i * stepSize;
                Vector2 point = GetPointByDistance(distance);
                _debugPoints.Add(point);
            }
        }

    }
}
