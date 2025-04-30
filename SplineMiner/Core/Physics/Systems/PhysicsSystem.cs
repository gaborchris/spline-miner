using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Physics.Components;

namespace SplineMiner.Core.Physics.Systems
{
    /// <summary>
    /// Handles physics calculations and updates for entities in the game world.
    /// </summary>
    public class PhysicsSystem
    {
        private readonly List<ICollidable> _entities;
        private readonly Vector2 _gravity;
        private readonly float _airResistance;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicsSystem"/> class.
        /// </summary>
        /// <param name="gravity">The gravity force to apply.</param>
        /// <param name="airResistance">The air resistance coefficient.</param>
        public PhysicsSystem(Vector2 gravity, float airResistance)
        {
            _entities = new List<ICollidable>();
            _gravity = gravity;
            _airResistance = airResistance;
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
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var entity in _entities)
            {
                // Apply gravity
                entity.Velocity += _gravity * deltaTime;

                // Apply air resistance
                Vector2 airResistanceForce = -entity.Velocity * _airResistance;
                entity.Velocity += airResistanceForce * deltaTime;

                // Update position based on velocity
                if (entity.BoundingBox is Components.BoundingBox boundingBox)
                {
                    Vector2 newPosition = boundingBox.Position + entity.Velocity * deltaTime;
                    boundingBox.UpdatePosition(newPosition);
                }
            }
        }
    }
} 