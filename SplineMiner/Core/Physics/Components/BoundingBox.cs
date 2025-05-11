using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Core.Physics.Components
{
    /// <summary>
    /// Represents a rectangular bounding box used for collision detection.
    /// </summary>
    public class BoundingBox : IBoundingBox
    {
        private Vector2 _position;
        private Vector2 _size;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        /// <param name="position">The position of the bounding box.</param>
        /// <param name="size">The size of the bounding box.</param>
        public BoundingBox(Vector2 position, Vector2 size)
        {
            _position = position;
            _size = size;
        }

        /// <inheritdoc />
        public Vector2 Position => _position;

        /// <inheritdoc />
        public Vector2 Size => _size;

        /// <inheritdoc />
        public Vector2 Center => _position + (_size * 0.5f);

        /// <inheritdoc />
        public float Left => _position.X;

        /// <inheritdoc />
        public float Right => _position.X + _size.X;

        /// <inheritdoc />
        public float Top => _position.Y;

        /// <inheritdoc />
        public float Bottom => _position.Y + _size.Y;

        /// <inheritdoc />
        public Vector2[] GetCorners()
        {
            return [
                new Vector2(Left, Top),     // Top-left
                new Vector2(Right, Top),    // Top-right
                new Vector2(Right, Bottom), // Bottom-right
                new Vector2(Left, Bottom)   // Bottom-left
            ];
        }

        /// <inheritdoc />

        /// <summary>
        /// Updates the position of the bounding box.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        public void UpdatePosition(Vector2 newPosition)
        {
            _position = newPosition;
        }

        /// <summary>
        /// Updates the size of the bounding box.
        /// </summary>
        /// <param name="newSize">The new size.</param>
        public void UpdateSize(Vector2 newSize)
        {
            _size = newSize;
        }
    }
}