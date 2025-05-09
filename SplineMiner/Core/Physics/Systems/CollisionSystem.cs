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
            bool noCollision = entityBox.Right < blockBox.Left ||
                             entityBox.Left > blockBox.Right ||
                             entityBox.Bottom < blockBox.Top ||
                             entityBox.Top > blockBox.Bottom;

            return !noCollision;
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

        private Vector2 CalculateCollisionPoint(ICollidable entity, Rectangle blockBox)
        {
            // Calculate the point of collision based on the direction of movement
            float x = entity.BoundingBox.Right;
            float y = entity.BoundingBox.Bottom;
            return new Vector2(x, y);
        }
    }
}