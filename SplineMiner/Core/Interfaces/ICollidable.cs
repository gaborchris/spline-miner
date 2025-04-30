using Microsoft.Xna.Framework;
using SplineMiner.Core.Physics.Components;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for entities that can participate in collisions.
    /// </summary>
    public interface ICollidable
    {
        /// <summary>
        /// Gets the bounding box of the entity.
        /// </summary>
        IBoundingBox BoundingBox { get; }

        /// <summary>
        /// Gets or sets the velocity of the entity.
        /// </summary>
        Vector2 Velocity { get; set; }

        /// <summary>
        /// Gets the mass of the entity.
        /// </summary>
        float Mass { get; }

        /// <summary>
        /// Called when a collision occurs.
        /// </summary>
        /// <param name="info">Information about the collision.</param>
        void OnCollision(CollisionInfo info);
    }
} 