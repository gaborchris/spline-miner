using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplineMiner
{
    public interface ITrackNode
    {
        Vector2 Position { get; set; }
        void Draw(SpriteBatch spriteBatch, Texture2D texture, bool isSelected);
    }

    public class PlacedTrackNode : ITrackNode
    {
        public Vector2 Position { get; set; }
        private const float NODE_RADIUS = 10f;
        private readonly Color _normalColor = Color.Blue;
        private readonly Color _selectedColor = Color.Red;

        public PlacedTrackNode(Vector2 position)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, bool isSelected)
        {
            Color color = isSelected ? _selectedColor : _normalColor;
            DrawCircle(spriteBatch, texture, Position, NODE_RADIUS, color);
        }

        private void DrawCircle(SpriteBatch spriteBatch, Texture2D texture, Vector2 center, float radius, Color color)
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
    }

    public class ShadowTrackNode : ITrackNode
    {
        public Vector2 Position { get; set; }
        private const float NODE_RADIUS = 10f;
        private readonly Color _normalColor = Color.Gray * 0.5f;
        private readonly Color _selectedColor = Color.Red * 0.5f;

        public ShadowTrackNode(Vector2 position)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, bool isSelected)
        {
            Color color = isSelected ? _selectedColor : _normalColor;
            DrawCircle(spriteBatch, texture, Position, NODE_RADIUS, color);
        }

        private void DrawCircle(SpriteBatch spriteBatch, Texture2D texture, Vector2 center, float radius, Color color)
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
    }
} 