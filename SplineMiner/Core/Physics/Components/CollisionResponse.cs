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
        /// <param name="block">The block being collided with.</param>
        /// <param name="penetration">The penetration depth.</param>
        /// <param name="normal">The collision normal.</param>
        /// <returns>The new position for the entity.</returns>
        public static Vector2 ResolveCollision(ICollidable entity, IWorldBlock block, float penetration, Vector2 normal)
        {
            // Calculate the minimum translation vector to resolve the collision
            Vector2 mtv = normal * penetration;
            
            // Apply the resolution based on the entity's velocity
            if (Vector2.Dot(entity.Velocity, normal) < 0)
            {
                return entity.BoundingBox.Position + mtv;
            }
            
            return entity.BoundingBox.Position;
        }

        /// <summary>
        /// Applies a bounce response to the collision.
        /// </summary>
        /// <param name="entity">The entity to apply bounce to.</param>
        /// <param name="normal">The collision normal.</param>
        /// <param name="bounceFactor">The bounce factor (0-1).</param>
        public static void ApplyBounce(ICollidable entity, Vector2 normal, float bounceFactor)
        {
            // Calculate the reflection vector
            Vector2 reflection = Vector2.Reflect(entity.Velocity, normal);
            
            // Apply the bounce factor
            entity.Velocity = reflection * bounceFactor;
        }

        /// <summary>
        /// Applies friction to the entity's velocity.
        /// </summary>
        /// <param name="entity">The entity to apply friction to.</param>
        /// <param name="frictionCoefficient">The friction coefficient.</param>
        public static void ApplyFriction(ICollidable entity, float frictionCoefficient)
        {
            // Apply friction in the opposite direction of velocity
            Vector2 friction = -entity.Velocity * frictionCoefficient;
            entity.Velocity += friction;
        }
    }
} 