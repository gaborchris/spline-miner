using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Physics.Components;
using SplineMiner.Core.Physics.Entities;

namespace SplineMiner.Game.Cart
{
    /// <summary>
    /// Represents the state and physics of the cart.
    /// </summary>
    public class CartModel : ICollidable
    {
        private float _t = 0f;
        private float _previousT;
        private readonly Vector2 _size;
        private readonly DynamicEntity _physicsEntity;
        private readonly CartBoundingBox _boundingBox;
        private readonly CartWheelSystem _wheelSystem;

        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }
        public float CurrentDistance => _t;
        public Vector2 Size => _size;
        public CartWheelSystem WheelSystem => _wheelSystem;

        // ICollidable implementation
        public IBoundingBox BoundingBox => _boundingBox;
        public float Mass => _physicsEntity.Mass;

        public CartModel(Vector2 size)
        {
            _size = size;
            Position = Vector2.Zero;
            Rotation = 0f;

            // Initialize physics entity
            _physicsEntity = new DynamicEntity(
                position: Vector2.Zero,
                size: new Vector2(size.X * 0.5f, size.Y * 0.5f),
                mass: 1.0f
            );

            // Initialize bounding box
            _boundingBox = new CartBoundingBox(
                position: Vector2.Zero,
                size: size,
                rotation: 0f
            );

            _wheelSystem = new CartWheelSystem();
        }

        public void UpdatePosition(Vector2 newPosition, float newRotation)
        {
            Position = newPosition;
            Rotation = newRotation;
            _boundingBox.Update(newPosition, newRotation);

            // Update physics entity position
            if (_physicsEntity.BoundingBox is SplineMiner.Core.Physics.Components.BoundingBox boundingBox)
            {
                Vector2 physicsPosition = newPosition - new Vector2(_size.X * 0.25f, _size.Y * 0.75f);
                boundingBox.UpdatePosition(physicsPosition);
                _physicsEntity.Velocity = Vector2.Zero;
            }
        }

        public void UpdateDistance(float deltaTime, bool isMovingForward, bool isMovingBackward)
        {
            _previousT = _t;
            float speed = 300f; // Default speed

            if (isMovingForward)
            {
                _t += speed * deltaTime;
            }
            else if (isMovingBackward)
            {
                _t -= speed * deltaTime;
            }
        }

        public void OnCollision(CollisionInfo info)
        {
            // First, let the physics entity handle basic collision response
            _physicsEntity.OnCollision(info);

            // Stop movement in both directions by resetting the distance parameter
            _t = _previousT;
        }
    }
} 