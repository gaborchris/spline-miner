using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Enums;

namespace SplineMiner.Game.World.WorldGrid
{
    /// <summary>
    /// Represents a single destructible cell in the world grid
    /// </summary>
    public class GridCell : IWorldBlock
    {
        public Vector2 Position { get; set; }
        public float Width { get; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public Color Color { get; set; }
        private readonly IDebugLogger _logger;

        // IWorldBlock implementation
        public Rectangle BoundingBox => GetBounds();
        public bool IsDestructible => true;
        public BlockType BlockType => BlockType.Destructible;
        public Vector2 Size => new Vector2(Width, Width);

        public GridCell(Vector2 position, float width, bool isActive, IDebugService debugService)
        {
            Position = position;
            Width = width;
            IsActive = isActive;
            IsSelected = false;
            Color = Color.DarkGray;
            _logger = debugService?.GetLogger("Collision");
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)(Position.X - Width / 2),
                (int)(Position.Y - Width / 2),
                (int)Width,
                (int)Width);
        }

        public bool Contains(Vector2 point)
        {
            return GetBounds().Contains(point);
        }

        public void Destroy()
        {
            IsActive = false;
        }

        public void OnCollision(ICollidable entity)
        {
            _logger?.Log("Collision", $"Block at {Position} is colliding with entity at {entity.BoundingBox.Position}");
        }
    }
}