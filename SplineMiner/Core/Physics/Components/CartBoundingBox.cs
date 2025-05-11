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
        private Vector2 _size;
        private float _rotation;
        private Vector2 _origin;

        /// <summary>
        /// Initializes a new instance of the CartBoundingBox class.
        /// </summary>
        /// <param name="position">The position of the cart.</param>
        /// <param name="size">The size of the cart texture.</param>
        /// <param name="rotation">The rotation of the cart in radians.</param>
        public CartBoundingBox(Vector2 position, Vector2 size, float rotation)
        {
            _position = position;
            _size = size;
            _rotation = rotation;
            _origin = new Vector2(size.X / 2f, size.Y); // Match the sprite's origin point
        }

        /// <inheritdoc />
        public Vector2 Position => _position;

        /// <inheritdoc />
        public Vector2 Size => _size;

        /// <inheritdoc />
        public Vector2 Center => _position;

        /// <summary>
        /// Gets the corners of the rotated bounding box.
        /// </summary>
        public Vector2[] GetCorners()
        {
            // Calculate the corners relative to the origin (matching sprite drawing)
            Vector2[] corners = new Vector2[4];
            corners[0] = new Vector2(-_size.X / 2, -_size.Y); // Top-left
            corners[1] = new Vector2(_size.X / 2, -_size.Y);  // Top-right
            corners[2] = new Vector2(_size.X / 2, 0);         // Bottom-right
            corners[3] = new Vector2(-_size.X / 2, 0);        // Bottom-left

            // Rotate each corner around the origin
            float cos = (float)Math.Cos(_rotation);
            float sin = (float)Math.Sin(_rotation);
            for (int i = 0; i < 4; i++)
            {
                // Rotate around origin
                float x = corners[i].X * cos - corners[i].Y * sin;
                float y = corners[i].X * sin + corners[i].Y * cos;
                // Translate to world position
                corners[i] = new Vector2(x, y) + _position;
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

        /// <inheritdoc />
        public bool Intersects(IBoundingBox other)
        {
            // If the other box is also a CartBoundingBox, use SAT
            if (other is CartBoundingBox otherCart)
            {
                return CheckRotatedIntersection(otherCart);
            }

            // For non-rotated boxes, use AABB intersection
            return !(Right < other.Left || Left > other.Right ||
                    Bottom < other.Top || Top > other.Bottom);
        }

        private bool CheckRotatedIntersection(CartBoundingBox other)
        {
            Vector2[] corners1 = GetCorners();
            Vector2[] corners2 = other.GetCorners();

            // Get the axes to test (normals of each edge)
            Vector2[] axes = new Vector2[8];
            int axisCount = 0;

            // Add axes from first box
            for (int i = 0; i < 4; i++)
            {
                Vector2 edge = corners1[(i + 1) % 4] - corners1[i];
                axes[axisCount++] = new Vector2(-edge.Y, edge.X);
            }

            // Add axes from second box
            for (int i = 0; i < 4; i++)
            {
                Vector2 edge = corners2[(i + 1) % 4] - corners2[i];
                axes[axisCount++] = new Vector2(-edge.Y, edge.X);
            }

            // Normalize all axes
            for (int i = 0; i < axisCount; i++)
            {
                axes[i].Normalize();
            }

            // Check for separation on each axis
            for (int i = 0; i < axisCount; i++)
            {
                if (IsSeparatedOnAxis(corners1, corners2, axes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSeparatedOnAxis(Vector2[] corners1, Vector2[] corners2, Vector2 axis)
        {
            float min1 = float.MaxValue;
            float max1 = float.MinValue;
            float min2 = float.MaxValue;
            float max2 = float.MinValue;

            // Project corners of first box
            for (int i = 0; i < 4; i++)
            {
                float projection = Vector2.Dot(corners1[i], axis);
                min1 = Math.Min(min1, projection);
                max1 = Math.Max(max1, projection);
            }

            // Project corners of second box
            for (int i = 0; i < 4; i++)
            {
                float projection = Vector2.Dot(corners2[i], axis);
                min2 = Math.Min(min2, projection);
                max2 = Math.Max(max2, projection);
            }

            // Check for overlap
            return max1 < min2 || max2 < min1;
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