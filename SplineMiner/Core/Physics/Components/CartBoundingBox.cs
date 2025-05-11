using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using System;

namespace SplineMiner.Core.Physics.Components
{
    /// <summary>
    /// A specialized bounding box implementation for the cart that matches its visual shape.
    /// </summary>
    public class CartBoundingBox : IBoundingBox
    {
        private Vector2 _position;
        private Vector2 _scaledSize;
        private float _rotation;
        private Vector2 _offset;

        /// <summary>
        /// Initializes a new instance of the CartBoundingBox class.
        /// </summary>
        /// <param name="position">The position of the cart.</param>
        /// <param name="size">The size of the cart texture.</param>
        /// <param name="rotation">The rotation of the cart in radians.</param>
        /// <param name="scale">Scale factor for the bounding box size (default: 0.5).</param>
        public CartBoundingBox(Vector2 position, Vector2 size, float rotation, float scale = 0.7f)
        {
            _scaledSize = size * scale;
            // Calculate offset as 50% of the scale difference
            float scaleDiff = (1 - scale) * 0.5f;
            _offset = new Vector2(0, -size.Y * scaleDiff);
            _position = position;
            _rotation = rotation;
        }

        /// <inheritdoc />
        public Vector2 Position => _position;

        /// <inheritdoc />
        public Vector2 Size => _scaledSize;

        /// <inheritdoc />
        public Vector2 Center => _position;

        /// <inheritdoc />
        public Vector2[] GetCorners()
        {
            Vector2[] corners = new Vector2[4];
            corners[0] = new Vector2(-_scaledSize.X / 2, -_scaledSize.Y); // Top-left
            corners[1] = new Vector2(_scaledSize.X / 2, -_scaledSize.Y);  // Top-right
            corners[2] = new Vector2(_scaledSize.X / 2, 0);              // Bottom-right
            corners[3] = new Vector2(-_scaledSize.X / 2, 0);             // Bottom-left

            float cos = (float)Math.Cos(_rotation);
            float sin = (float)Math.Sin(_rotation);

            // Rotate the offset
            Vector2 rotatedOffset = new Vector2(
                _offset.X * cos - _offset.Y * sin,
                _offset.X * sin + _offset.Y * cos
            );

            for (int i = 0; i < 4; i++)
            {
                float x = corners[i].X * cos - corners[i].Y * sin;
                float y = corners[i].X * sin + corners[i].Y * cos;
                corners[i] = new Vector2(x, y) + _position + rotatedOffset;
            }

            return corners;
        }

        /// <inheritdoc />
        public float Left
        {
            get
            {
                var corners = GetCorners();
                return Math.Min(Math.Min(corners[0].X, corners[1].X), Math.Min(corners[2].X, corners[3].X));
            }
        }

        /// <inheritdoc />
        public float Right
        {
            get
            {
                var corners = GetCorners();
                return Math.Max(Math.Max(corners[0].X, corners[1].X), Math.Max(corners[2].X, corners[3].X));
            }
        }

        /// <inheritdoc />
        public float Top
        {
            get
            {
                var corners = GetCorners();
                return Math.Min(Math.Min(corners[0].Y, corners[1].Y), Math.Min(corners[2].Y, corners[3].Y));
            }
        }

        /// <inheritdoc />
        public float Bottom
        {
            get
            {
                var corners = GetCorners();
                return Math.Max(Math.Max(corners[0].Y, corners[1].Y), Math.Max(corners[2].Y, corners[3].Y));
            }
        }

        /// <summary>
        /// Updates the position and rotation of the bounding box.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        /// <param name="newRotation">The new rotation in radians.</param>
        public void Update(Vector2 newPosition, float newRotation)
        {
            _position = newPosition;
            _rotation = newRotation;
        }
    }
} 