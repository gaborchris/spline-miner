using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Core.Physics.Components
{
    /// <summary>
    /// Contains information about a collision between two objects.
    /// </summary>
    public class CollisionInfo
    {
        /// <summary>
        /// Gets the entity involved in the collision.
        /// </summary>
        public ICollidable Entity { get; }

        /// <summary>
        /// Gets the world block involved in the collision.
        /// </summary>
        public IWorldBlock Block { get; }

        /// <summary>
        /// Gets the point of collision.
        /// </summary>
        public Vector2 CollisionPoint { get; }

        /// <summary>
        /// Gets the normal vector of the collision surface.
        /// </summary>
        public Vector2 Normal { get; }

        /// <summary>
        /// Gets the penetration depth of the collision.
        /// </summary>
        public float Penetration { get; }

        /// <summary>
        /// Initializes a new instance of the CollisionInfo class.
        /// </summary>
        /// <param name="entity">The entity involved in the collision.</param>
        /// <param name="block">The world block involved in the collision.</param>
        /// <param name="collisionPoint">The point of collision.</param>
        /// <param name="normal">The normal vector of the collision surface.</param>
        /// <param name="penetration">The penetration depth of the collision.</param>
        public CollisionInfo(ICollidable entity, IWorldBlock block, Vector2 collisionPoint, Vector2 normal, float penetration)
        {
            Entity = entity;
            Block = block;
            CollisionPoint = collisionPoint;
            Normal = normal;
            Penetration = penetration;
        }
    }
}