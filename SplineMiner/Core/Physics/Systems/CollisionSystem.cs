using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Physics.Components;

namespace SplineMiner.Core.Physics.Systems
{
    /// <summary>
    /// Handles collision detection and resolution between entities and world blocks.
    /// </summary>
    public class CollisionSystem
    {
        private readonly List<ICollidable> _entities;
        private readonly List<IWorldBlock> _blocks;
        private readonly IDebugLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionSystem"/> class.
        /// </summary>
        public CollisionSystem(IDebugService debugService = null)
        {
            _entities = new List<ICollidable>();
            _blocks = new List<IWorldBlock>();
            _logger = debugService?.CreateLogger("CollisionSystem");

            if (_logger != null)
            {
                _logger.IsEnabled = true;
                _logger.LogInterval = 0.1f; // Log every 100ms
                _logger.Log("CollisionSystem", "CollisionSystem constructor: Logger initialized");
            }
            else
            {
                Debug.WriteLine("CollisionSystem constructor: Logger is null");
            }
        }

        /// <summary>
        /// Adds an entity to the collision system.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void AddEntity(ICollidable entity)
        {
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
            }
        }

        /// <summary>
        /// Adds a world block to the collision system.
        /// </summary>
        /// <param name="block">The block to add.</param>
        public void AddBlock(IWorldBlock block)
        {
            _blocks.Add(block);
        }

        /// <summary>
        /// Clears all blocks from the collision system.
        /// </summary>
        public void ClearBlocks()
        {
            _blocks.Clear();
        }

        /// <summary>
        /// Updates the collision system, checking for and resolving collisions.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities)
            {
                foreach (var block in _blocks)
                {
                    if (CheckCollision(entity.BoundingBox, block.BoundingBox))
                    {
                        ResolveCollision(entity, block);
                    }
                }
            }
        }

        private static bool CheckCollision(IBoundingBox entityBox, Rectangle blockBox)
        {
            // Get corners of both boxes
            Vector2[] blockCorners = GetRectangleCorners(blockBox);
            Vector2[] entityCorners = entityBox.GetCorners();

            // Get all axes to test (normals of each edge)
            Vector2[] axes = GetAllAxes(entityCorners, blockCorners);

            // If there's separation on any axis, the boxes don't intersect
            return !IsSeparatedOnAnyAxis(entityCorners, blockCorners, axes);
        }

        private static Vector2[] GetRectangleCorners(Rectangle rect)
        {
            return new Vector2[]
            {
                new Vector2(rect.Left, rect.Top),     // Top-left
                new Vector2(rect.Right, rect.Top),    // Top-right
                new Vector2(rect.Right, rect.Bottom), // Bottom-right
                new Vector2(rect.Left, rect.Bottom)   // Bottom-left
            };
        }

        private static Vector2[] GetAllAxes(Vector2[] corners1, Vector2[] corners2)
        {
            Vector2[] axes = new Vector2[8];
            int axisCount = 0;

            // Get axes from both boxes
            axisCount = AddBoxAxes(corners1, axes, axisCount);
            axisCount = AddBoxAxes(corners2, axes, axisCount);

            // Normalize all axes
            for (int i = 0; i < axisCount; i++)
            {
                axes[i].Normalize();
            }

            return axes;
        }

        private static int AddBoxAxes(Vector2[] corners, Vector2[] axes, int startIndex)
        {
            for (int i = 0; i < 4; i++)
            {
                // Get edge vector
                Vector2 edge = corners[(i + 1) % 4] - corners[i];
                // Get normal vector (perpendicular to edge)
                axes[startIndex + i] = new Vector2(-edge.Y, edge.X);
            }
            return startIndex + 4;
        }

        private static bool IsSeparatedOnAnyAxis(Vector2[] corners1, Vector2[] corners2, Vector2[] axes)
        {
            foreach (var axis in axes)
            {
                if (IsSeparatedOnAxis(corners1, corners2, axis))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSeparatedOnAxis(Vector2[] corners1, Vector2[] corners2, Vector2 axis)
        {
            // Project both boxes onto the axis
            ProjectBox(corners1, axis, out float min1, out float max1);
            ProjectBox(corners2, axis, out float min2, out float max2);

            // Check if projections overlap
            return max1 < min2 || max2 < min1;
        }

        private static void ProjectBox(Vector2[] corners, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            foreach (var corner in corners)
            {
                float projection = Vector2.Dot(corner, axis);
                min = Math.Min(min, projection);
                max = Math.Max(max, projection);
            }
        }

        private void ResolveCollision(ICollidable entity, IWorldBlock block)
        {
            // Calculate penetration depth and normal
            CalculateCollisionInfo(entity.BoundingBox, block.BoundingBox, out float penetration, out Vector2 normal);

            // Create collision info
            var collisionInfo = new CollisionInfo(
                entity,
                block,
                CalculateCollisionPoint(entity, block.BoundingBox),
                normal,
                penetration
            );

            // Notify the entity of the collision
            entity.OnCollision(collisionInfo);

            // If the block is also collidable, notify it
            if (block is ICollidable collidableBlock)
            {
                collidableBlock.OnCollision(collisionInfo);
            }

            // Resolve the collision
            Vector2 newPosition = CollisionResponse.ResolveCollision(
                entity);

            // Update entity position through its bounding box
            if (entity.BoundingBox is Components.BoundingBox boundingBox)
            {
                boundingBox.UpdatePosition(newPosition);
            }
        }

        private static void CalculateCollisionInfo(IBoundingBox entityBox, Rectangle blockBox, out float penetration, out Vector2 normal)
        {
            // Calculate overlap on each axis
            float xOverlap = Math.Min(entityBox.Right - blockBox.Left, blockBox.Right - entityBox.Left);
            float yOverlap = Math.Min(entityBox.Bottom - blockBox.Top, blockBox.Bottom - entityBox.Top);

            // Determine the axis of least penetration
            if (xOverlap < yOverlap)
            {
                penetration = xOverlap;
                normal = new Vector2(entityBox.Center.X < blockBox.Center.X ? -1 : 1, 0);
            }
            else
            {
                penetration = yOverlap;
                normal = new Vector2(0, entityBox.Center.Y < blockBox.Center.Y ? -1 : 1);
            }
        }

        private static Vector2 CalculateCollisionPoint(ICollidable entity, Rectangle blockBox)
        {
            // Calculate the point of collision based on the direction of movement
            float x = entity.BoundingBox.Right;
            float y = entity.BoundingBox.Bottom;
            return new Vector2(x, y);
        }
    }
}