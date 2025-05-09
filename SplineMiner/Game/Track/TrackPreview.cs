using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplineMiner.Game.Track
{
    public class TrackPreview
    {
        private SplineTrack _parentTrack;
        private Vector2? _previewPoint;
        private const float PREVIEW_ALPHA = 0.5f;
        private const int PREVIEW_SEGMENTS = 20;
        private Texture2D _pointTexture;
        private const float ENDPOINT_RADIUS = 5f;
        private bool _isHoveringEndpoint;

        public bool IsHoveringEndpoint => _isHoveringEndpoint;

        public TrackPreview(SplineTrack parentTrack, GraphicsDevice graphicsDevice)
        {
            _parentTrack = parentTrack;
            _previewPoint = null;
            _isHoveringEndpoint = false;

            // Create point texture
            _pointTexture = new Texture2D(graphicsDevice, 1, 1);
            _pointTexture.SetData(new[] { Color.White });
        }

        public void Update(Vector2 mousePosition)
        {
            _previewPoint = mousePosition;

            // Check if hovering over endpoint
            if (_parentTrack.PlacedNodes.Count > 0)
            {
                Vector2 lastPoint = _parentTrack.PlacedNodes[^1].Position;
                _isHoveringEndpoint = Vector2.Distance(mousePosition, lastPoint) <= ENDPOINT_RADIUS;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_parentTrack.PlacedNodes.Count == 0 || !_previewPoint.HasValue)
                return;

            // Get the last point of the existing track
            Vector2 lastPoint = _parentTrack.PlacedNodes[^1].Position;

            // Draw preview line segments
            Vector2 start = lastPoint;
            Vector2 end = _previewPoint.Value;
            Vector2 direction = end - start;
            float length = direction.Length();
            direction.Normalize();

            for (int i = 0; i < PREVIEW_SEGMENTS; i++)
            {
                float t1 = i / (float)PREVIEW_SEGMENTS;
                float t2 = (i + 1) / (float)PREVIEW_SEGMENTS;

                Vector2 point1 = start + direction * (length * t1);
                Vector2 point2 = start + direction * (length * t2);

                DrawLine(spriteBatch, point1, point2, Color.White * PREVIEW_ALPHA, 2);
            }

            // Draw endpoint indicator
            if (_isHoveringEndpoint)
            {
                DrawCircle(spriteBatch, lastPoint, ENDPOINT_RADIUS, Color.Yellow * PREVIEW_ALPHA);
            }
            else
            {
                DrawCircle(spriteBatch, lastPoint, ENDPOINT_RADIUS, Color.White * PREVIEW_ALPHA);
            }
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
    }
}