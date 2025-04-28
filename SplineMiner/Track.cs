using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using SplineMiner.Core;

namespace SplineMiner
{
    // Spline calculation component
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

    // Caching component
    public class TrackCache : ICache<int, Vector2>, ICache<int, float>
    {
        private const int CACHE_SIZE = 1000;
        private readonly Dictionary<int, Vector2> _positionCache = new();
        private readonly Dictionary<int, float> _parameterCache = new();

        public bool TryGetValue(int key, out Vector2 value) => _positionCache.TryGetValue(key, out value);
        public bool TryGetValue(int key, out float value) => _parameterCache.TryGetValue(key, out value);

        public void SetValue(int key, Vector2 value)
        {
            if (_positionCache.Count >= CACHE_SIZE)
                _positionCache.Clear();
            _positionCache[key] = value;
        }

        public void SetValue(int key, float value)
        {
            if (_parameterCache.Count >= CACHE_SIZE)
                _parameterCache.Clear();
            _parameterCache[key] = value;
        }

        public void Clear()
        {
            _positionCache.Clear();
            _parameterCache.Clear();
        }
    }

    // Track visualization component
    public class TrackRenderer : ITrackRenderer
    {
        private readonly Texture2D _pointTexture;
        private readonly List<Vector2> _debugPoints = [];
        private readonly bool _enableDebugVisualization = true;

        public TrackRenderer(Texture2D pointTexture)
        {
            _pointTexture = pointTexture;
        }

        public void Draw(SpriteBatch spriteBatch, ITrack track)
        {
            // Implementation of track drawing
        }

        public void DrawDebugInfo(SpriteBatch spriteBatch, float distance, Texture2D debugTexture)
        {
            // Implementation of debug visualization
        }
    }

    // Main Track class
    public class Track : ITrack
    {
        private readonly List<PlacedTrackNode> _placedNodes;
        private readonly List<ShadowTrackNode> _shadowNodes;
        private int _selectedNodeIndex = -1;
        private Texture2D _pointTexture;
        private readonly List<Vector2> _debugPoints = [];
        private readonly bool _enableDebugVisualization = true;
        private float _totalArcLength = 0f;
        private TrackPreview _preview;
        private const int MIN_SHADOW_NODES = 3; // Minimum needed for Catmull-Rom
        private readonly UIManager _uiManager;
        private float _t = 0f;
        
        private readonly SplineCalculator _splineCalculator;
        private readonly TrackCache _cache;
        private readonly TrackRenderer _renderer;

        public IReadOnlyList<PlacedTrackNode> PlacedNodes => _placedNodes.AsReadOnly();
        public IReadOnlyList<ShadowTrackNode> ShadowNodes => _shadowNodes.AsReadOnly();
        public float TotalArcLength => _totalArcLength;

        public Track(List<Vector2> initialPoints, UIManager uiManager)
        {
            _placedNodes = initialPoints.Select(p => new PlacedTrackNode(p)).ToList();
            _shadowNodes = [];
            _uiManager = uiManager;
            _splineCalculator = new SplineCalculator(_placedNodes);
            _cache = new TrackCache();
            _renderer = new TrackRenderer(_pointTexture);
            UpdateShadowNodes();
            RecalculateArcLength();
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

        public int GetHoveredPointIndex(Vector2 screenPosition)
        {
            const float HOVER_RADIUS = 10f;
            
            // Convert screen position to world position using camera transform
            Vector2 worldPosition = CameraManager.Instance.ScreenToWorld(screenPosition);
            
            // Get viewport bounds in world space for culling
            Vector2 viewportTopLeft = CameraManager.Instance.ScreenToWorld(Vector2.Zero);
            Vector2 viewportBottomRight = CameraManager.Instance.ScreenToWorld(new Vector2(
                CameraManager.Instance.Viewport.Width,
                CameraManager.Instance.Viewport.Height
            ));
            
            // Check placed nodes first
            for (int i = 0; i < _placedNodes.Count; i++)
            {
                Vector2 nodePosition = _placedNodes[i].Position;
                
                // Cull nodes outside viewport for better performance
                if (nodePosition.X < viewportTopLeft.X - HOVER_RADIUS ||
                    nodePosition.X > viewportBottomRight.X + HOVER_RADIUS ||
                    nodePosition.Y < viewportTopLeft.Y - HOVER_RADIUS ||
                    nodePosition.Y > viewportBottomRight.Y + HOVER_RADIUS)
                {
                    continue;
                }
                
                if (Vector2.Distance(nodePosition, worldPosition) <= HOVER_RADIUS)
                {
                    return i;
                }
            }
            
            // Then check shadow nodes
            for (int i = 0; i < _shadowNodes.Count; i++)
            {
                Vector2 nodePosition = _shadowNodes[i].Position;
                
                // Cull nodes outside viewport for better performance
                if (nodePosition.X < viewportTopLeft.X - HOVER_RADIUS ||
                    nodePosition.X > viewportBottomRight.X + HOVER_RADIUS ||
                    nodePosition.Y < viewportTopLeft.Y - HOVER_RADIUS ||
                    nodePosition.Y > viewportBottomRight.Y + HOVER_RADIUS)
                {
                    continue;
                }
                
                if (Vector2.Distance(nodePosition, worldPosition) <= HOVER_RADIUS)
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

        public void MoveSelectedPoint(Vector2 screenPosition)
        {
            if (_selectedNodeIndex >= _placedNodes.Count && _selectedNodeIndex < _placedNodes.Count + _shadowNodes.Count)
            {
                int shadowIndex = _selectedNodeIndex - _placedNodes.Count;
                // Convert screen position to world position
                Vector2 worldPosition = CameraManager.Instance.ScreenToWorld(screenPosition);
                _shadowNodes[shadowIndex].Position = worldPosition;
                RecalculateArcLength();
            }
        }

        public void ReleaseSelectedPoint()
        {
            _selectedNodeIndex = -1;
        }

        /// <summary>
        /// Adds a new point to the track. When clicking on a shadow node, all shadow nodes up to 
        /// and including the clicked one are promoted to placed nodes. This ensures the spline preview
        /// that the user sees is exactly what gets placed when they click.
        /// </summary>
        /// <param name="position">The position where the point should be added</param>
        public void PlaceNode(Vector2 position)
        {
            // Check if we're clicking on a shadow node
            int hoveredIndex = GetHoveredPointIndex(position);
            if (hoveredIndex >= _placedNodes.Count && hoveredIndex < _placedNodes.Count + _shadowNodes.Count)
            {
                // Convert all shadow nodes up to and including the clicked one to placed nodes
                int shadowIndex = hoveredIndex - _placedNodes.Count;
                for (int i = 0; i <= shadowIndex; i++)
                {
                    _placedNodes.Add(new PlacedTrackNode(_shadowNodes[i].Position));
                }
                
                // Only update shadow nodes and recalculate if we actually placed nodes
                UpdateShadowNodes();
                RecalculateArcLength();
            }
            // If we didn't click on a shadow node, do nothing
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

        public Vector2 GetPointByDistance(float distance)
        {
            if (_totalArcLength <= 0)
            {
                RecalculateArcLength();
            }

            distance = Math.Min(distance, _totalArcLength);
            if (distance < 0) distance += _totalArcLength;

            int cacheKey = (int)(distance * 10);
            if (_cache.TryGetValue(cacheKey, out float cachedT))
                return _splineCalculator.GetPoint(cachedT);

            float t = _splineCalculator.GetParameterForDistance(distance, _totalArcLength);
            _cache.SetValue(cacheKey, t);
            return _splineCalculator.GetPoint(t);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_placedNodes.Count < 4)
                return;

            // Always draw the main track spline
            const int segments = 100;
            Vector2[] placedPoints = _placedNodes.Select(n => n.Position).ToArray();
            DrawingHelpers.DrawSplineCurve(spriteBatch, _pointTexture, placedPoints, segments, Color.Black, 2);

            switch (_uiManager.CurrentTool)
            {
                case UITool.Track:
                    // Draw shadow nodes and their connecting curve
                    if (_shadowNodes.Count >= MIN_SHADOW_NODES)
                    {
                        // Create combined array for shadow curve
                        var combinedPoints = new List<Vector2>();
                        combinedPoints.Add(_placedNodes[^1].Position);
                        combinedPoints.AddRange(_shadowNodes.Select(n => n.Position));

                        if (combinedPoints.Count >= 4)
                        {
                            DrawingHelpers.DrawSplineCurve(spriteBatch, _pointTexture, combinedPoints.ToArray(), segments, Color.Gray * 0.5f, 2);
                        }

                        // Draw shadow nodes as circles
                        foreach (var (node, index) in _shadowNodes.Select((n, i) => (n, i)))
                        {
                            node.Draw(spriteBatch, _pointTexture, index + _placedNodes.Count == _selectedNodeIndex);
                        }
                    }
                    break;

                case UITool.DeleteTrack:
                    // Draw placed nodes as circles when delete tool is selected
                    foreach (var node in _placedNodes)
                    {
                        node.Draw(spriteBatch, _pointTexture, false);
                    }
                    break;

                default:
                    // Draw nothing extra for other tools
                    break;
            }

            // Draw debug points if enabled
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
        }

        public void UpdatePreview(Vector2 mousePosition)
        {
            _preview.Update(mousePosition);
        }

        public bool IsHoveringEndpoint => _preview?.IsHoveringEndpoint ?? false;

        public float GetRotationAtDistance(float distance)
        {
            Vector2 currentPoint = GetPointByDistance(distance);
            Vector2 nextPoint = GetPointByDistance(distance + 10.0f);
            Vector2 prevPoint = GetPointByDistance(distance - 10.0f);
            
            Vector2 direction = (nextPoint - prevPoint);
            direction.Normalize();
            
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public void DrawDebugInfo(SpriteBatch spriteBatch, float distance, Texture2D debugTexture)
        {
            _renderer.DrawDebugInfo(spriteBatch, distance, debugTexture);
        }

        public void RecalculateArcLength()
        {
            _totalArcLength = _splineCalculator.ComputeArcLength(0f, _placedNodes.Count - 1, 100);
        }

        public void VisualizeEquallySpacedPoints(int count)
        {
            _debugPoints.Clear();
            
            // Get the current position's distance along the track
            float currentDistance = _t;
            float stepSize = _totalArcLength / count;
            
            // Start from current position and go forward
            for (int i = 0; i < count; i++)
            {
                float distance = currentDistance + (i * stepSize);
                // Wrap around if we exceed the total length
                if (distance > _totalArcLength)
                {
                    distance -= _totalArcLength;
                }
                Vector2 point = GetPointByDistance(distance);
                _debugPoints.Add(point);
            }
        }

        public void UpdateCurrentPosition(float distance)
        {
            _t = distance;
        }

        // Helper method to get a point slightly ahead for better rotation calculation
        private Vector2 GetPointAhead(float distance, float lookAhead = 1.0f)
        {
            float nextDistance = distance + lookAhead;
            if (nextDistance > _totalArcLength)
            {
                nextDistance -= _totalArcLength;
            }
            return GetPointByDistance(nextDistance);
        }
    }
}
