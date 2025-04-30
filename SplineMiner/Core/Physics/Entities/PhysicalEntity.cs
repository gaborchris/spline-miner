using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Physics.Components;

namespace SplineMiner.Core.Physics.Entities
{
    /// <summary>
    /// Base class for entities that participate in physics simulation.
    /// </summary>
    public abstract class PhysicalEntity : ICollidable
    {
        private readonly Components.BoundingBox _boundingBox;
        private Vector2 _velocity;
        private readonly float _mass;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalEntity"/> class.
        /// </summary>
        /// <param name="position">The initial position.</param>
        /// <param name="size">The size of the entity.</param>
        /// <param name="mass">The mass of the entity.</param>
        protected PhysicalEntity(Vector2 position, Vector2 size, float mass)
        {
            _boundingBox = new Components.BoundingBox(position, size);
            _mass = mass;
        }

        /// <inheritdoc />
        public IBoundingBox BoundingBox => _boundingBox;

        /// <inheritdoc />
        public Vector2 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        /// <inheritdoc />
        public float Mass => _mass;

        /// <inheritdoc />
        public abstract void OnCollision(CollisionInfo info);

        /// <summary>
        /// Updates the entity's position.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        protected void UpdatePosition(Vector2 newPosition)
        {
            _boundingBox.UpdatePosition(newPosition);
        }

        /// <summary>
        /// Updates the entity's size.
        /// </summary>
        /// <param name="newSize">The new size.</param>
        protected void UpdateSize(Vector2 newSize)
        {
            _boundingBox.UpdateSize(newSize);
        }
    }
} 