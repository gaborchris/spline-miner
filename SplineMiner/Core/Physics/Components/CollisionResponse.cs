using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Core.Physics.Components
{
    /// <summary>
    /// Defines different strategies for handling collisions.
    /// </summary>
    public static class CollisionResponse
    {
        /// <summary>
        /// Resolves a collision by moving the entity out of the collision.
        /// </summary>
        /// <param name="entity">The entity to resolve collision for.</param>
        /// <returns>The new position for the entity.</returns>
        public static Vector2 ResolveCollision(ICollidable entity)
        {
            return entity.BoundingBox.Position;
        }

    }
}