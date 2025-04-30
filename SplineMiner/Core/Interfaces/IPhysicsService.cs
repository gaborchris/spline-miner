using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for physics and collision handling services.
    /// </summary>
    public interface IPhysicsService
    {
        /// <summary>
        /// Handles collisions between an entity and nearby world blocks.
        /// </summary>
        /// <param name="entity">The entity to check collisions for.</param>
        /// <param name="nearbyBlocks">The blocks to check collisions against.</param>
        void HandleCollisions(ICollidable entity, IEnumerable<IWorldBlock> nearbyBlocks);

        /// <summary>
        /// Checks if two bounding boxes are colliding.
        /// </summary>
        /// <param name="a">The first bounding box.</param>
        /// <param name="b">The second bounding box.</param>
        /// <returns>True if the boxes are colliding, false otherwise.</returns>
        bool CheckCollision(IBoundingBox a, IBoundingBox b);

        /// <summary>
        /// Resolves a collision between an entity and a world block.
        /// </summary>
        /// <param name="entity">The entity involved in the collision.</param>
        /// <param name="block">The world block involved in the collision.</param>
        /// <returns>The new position for the entity after collision resolution.</returns>
        Vector2 ResolveCollision(ICollidable entity, IWorldBlock block);
    }
} 