using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Core.Physics.Systems
{
    /// <summary>
    /// Handles physics calculations and updates for entities in the game world.
    /// </summary>
    public class PhysicsSystem
    {
        private readonly List<ICollidable> _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicsSystem"/> class.
        /// </summary>
        public PhysicsSystem()
        {
            _entities = [];
        }

        /// <summary>
        /// Adds an entity to the physics system.
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
        /// Updates the physics system, applying forces and updating positions.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach (var entity in _entities)
            {
                if (entity.BoundingBox is Components.BoundingBox boundingBox)
                {
                    Vector2 newPosition = boundingBox.Position;
                    boundingBox.UpdatePosition(newPosition);
                }
            }
        }
    }
}