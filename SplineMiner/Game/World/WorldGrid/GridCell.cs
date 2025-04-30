using Microsoft.Xna.Framework;

namespace SplineMiner.Game.World.WorldGrid
{
    /// <summary>
    /// Represents a single destructible cell in the world grid
    /// </summary>
    public class GridCell
    {
        public Vector2 Position { get; set; }
        public float Size { get; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public Color Color { get; set; }

        public GridCell(Vector2 position, float size, bool isActive = true)
        {
            Position = position;
            Size = size;
            IsActive = isActive;
            IsSelected = false;
            Color = Color.DarkGray;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)(Position.X - Size / 2),
                (int)(Position.Y - Size / 2),
                (int)Size,
                (int)Size);
        }

        public bool Contains(Vector2 point)
        {
            return GetBounds().Contains(point);
        }
    }
}