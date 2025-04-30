using Microsoft.Xna.Framework;
using SplineMiner.Core.Physics.Components;

namespace SplineMiner.Core.Physics.Entities
{
    /// <summary>
    /// Represents an entity that can move and be affected by physics forces.
    /// </summary>
    public class DynamicEntity : PhysicalEntity
    {
        private readonly float _bounceFactor;
        private readonly float _frictionCoefficient;

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
            float mass,
            float bounceFactor = 0.2f,
            float frictionCoefficient = 0.1f)
            : base(position, size, mass)
        {
            _bounceFactor = bounceFactor;
            _frictionCoefficient = frictionCoefficient;
        }

        /// <inheritdoc />
        public override void OnCollision(CollisionInfo info)
        {
            // Apply bounce
            CollisionResponse.ApplyBounce(this, info.Normal, _bounceFactor);

            // Apply friction
            CollisionResponse.ApplyFriction(this, _frictionCoefficient);

            // Update position to resolve collision
            Vector2 newPosition = CollisionResponse.ResolveCollision(
                this,
                info.Block,
                info.Penetration,
                info.Normal
            );

            UpdatePosition(newPosition);
        }
    }
} 