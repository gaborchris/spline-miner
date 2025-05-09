using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Enums;
using SplineMiner.Core.Physics.Components;

namespace SplineMiner.Game.World.WorldGrid
{
    /// <summary>
    /// Represents a single destructible cell in the world grid
    /// </summary>
    public class GridCell : IWorldBlock, ICollidable
    {
        public Vector2 Position { get; set; }
        public float Width { get; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public Color Color { get; set; }
        private readonly IDebugLogger _logger;
        private Vector2 _velocity;
        private readonly float _mass = 1.0f;
        private readonly SplineMiner.Core.Physics.Components.BoundingBox _boundingBox;

        // IWorldBlock implementation
        public Rectangle BoundingBox => GetBounds();
        public bool IsDestructible => true;
        public BlockType BlockType => BlockType.Destructible;
        public Vector2 Size => new Vector2(Width, Width);

        // ICollidable implementation
        IBoundingBox ICollidable.BoundingBox => _boundingBox;
        public Vector2 Velocity 
        { 
            get => _velocity;
            set => _velocity = value;
        }
        public float Mass => _mass;

        public GridCell(Vector2 position, float width, bool isActive, IDebugService debugService)
        {
            Position = position;
            Width = width;
            IsActive = isActive;
            IsSelected = false;
            Color = Color.DarkGray;
            _logger = debugService?.CreateLogger("PlayerCollision");
            _velocity = Vector2.Zero;
            _boundingBox = new SplineMiner.Core.Physics.Components.BoundingBox(position, new Vector2(width, width));
            
            if (_logger != null)
            {
                _logger.Log("GridCell", 
                    $"Created GridCell at {position} with width {width}\n" +
                    $"BoundingBox: Left={_boundingBox.Left:F1}, Right={_boundingBox.Right:F1}, " +
                    $"Top={_boundingBox.Top:F1}, Bottom={_boundingBox.Bottom:F1}");
            }
        }

        public GridCell(Vector2 position, float width, IDebugService debugService = null)
        {
            Position = position;
            Width = width;
            IsActive = true;  // Set default value
            IsSelected = false;
            Color = Color.DarkGray;
            _logger = debugService?.CreateLogger("PlayerCollision");
            _velocity = Vector2.Zero;
            _boundingBox = new SplineMiner.Core.Physics.Components.BoundingBox(position, new Vector2(width, width));
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

        public void OnCollision(CollisionInfo info)
        {
            if (_logger != null)
            {
                _logger.Log("PlayerCollision", 
                    $"Block at {Position} collided with player at {info.Entity.BoundingBox.Position} " +
                    $"with velocity ({info.Entity.Velocity.X:F1}, {info.Entity.Velocity.Y:F1})");
            }
        }
    }
}