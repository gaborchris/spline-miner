using Microsoft.Xna.Framework;
using SplineMiner.Core.Physics.Components;

namespace SplineMiner.Core.Physics.Entities
{
    /// <summary>
    /// Represents an entity that can move and be affected by physics forces.
    /// </summary>
    public class DynamicEntity : PhysicalEntity
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEntity"/> class.
        /// </summary>
        /// <param name="position">The initial position.</param>
        /// <param name="size">The size of the entity.</param>
        /// <param name="mass">The mass of the entity.</param>
        /// <param name="bounceFactor">The bounce factor (0-1).</param>
        /// <param name="frictionCoefficient">The friction coefficient.</param>
        public DynamicEntity(
            Vector2 position,
            Vector2 size,
            float mass)
            : base(position, size, mass)
        {
        }

        /// <inheritdoc />
        public override void OnCollision(CollisionInfo info)
        {
            // Update position to resolve collision
            Vector2 newPosition = CollisionResponse.ResolveCollision(
                this);

            UpdatePosition(newPosition);
        }
    }
}