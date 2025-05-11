using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Physics.Components;
using SplineMiner.Core.Physics.Entities;
using System;

namespace SplineMiner.Game.Cart
{
    /// <summary>
    /// Represents the state and physics of the cart, including movement mechanics.
    /// </summary>
    public class CartModel : ICollidable, IMovementController
    {
        #region Constants
        // Track constraints
        private const float MIN_TRACK_DISTANCE = 30f;
        
        // Movement parameters
        private const float MIN_MOVEMENT_THRESHOLD = 0.1f;
        private const float POSITION_INTERPOLATION_FACTOR = 0.5f;
        private const float MAX_ROTATION_CHANGE = MathHelper.Pi / 6;
        private const float DEFAULT_SPEED = 300f;
        #endregion

        #region Properties
        // Public properties
        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }
        public float CurrentDistance
        {
            get => _t;
            set => _t = MathHelper.Clamp(value, MIN_TRACK_DISTANCE, _maxTrackDistance - MIN_TRACK_DISTANCE);
        }
        public Vector2 Size => _size;
        public CartWheelSystem WheelSystem => _wheelSystem;
        public float Speed
        {
            get => _speed;
            set => _speed = Math.Max(0, value); // Prevent negative speed
        }

        // ICollidable implementation
        public IBoundingBox BoundingBox => _boundingBox;
        public float Mass => _physicsEntity.Mass;
        #endregion

        #region Fields
        // Core state
        private readonly Vector2 _size;
        private readonly DynamicEntity _physicsEntity;
        private readonly CartBoundingBox _boundingBox;
        private readonly CartWheelSystem _wheelSystem;
        
        // Movement state
        private float _t = MIN_TRACK_DISTANCE;
        private float _previousT = MIN_TRACK_DISTANCE;
        private Vector2 _previousPosition;
        private Vector2 _targetPosition;
        private float _lastRotation;
        private float _rotationChange;
        private float _speed = DEFAULT_SPEED;
        private float _maxTrackDistance = float.MaxValue;
        #endregion

        /// <summary>
        /// Initializes a new instance of the CartModel with the specified size.
        /// </summary>
        /// <param name="size">The size of the cart in world units.</param>
        public CartModel(Vector2 size)
        {
            _size = size;
            Position = Vector2.Zero;
            Rotation = 0f;
            _previousPosition = Vector2.Zero;
            _lastRotation = 0f;

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

        /// <summary>
        /// Updates the cart's state based on the current track and input.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="track">The track the cart is moving on.</param>
        /// <param name="isMovingForward">Whether the forward input is active.</param>
        /// <param name="isMovingBackward">Whether the backward input is active.</param>
        public void Update(GameTime gameTime, ITrack track, bool isMovingForward, bool isMovingBackward)
        {
            if (track == null) return;
            
            // Store track information
            _maxTrackDistance = track.TotalArcLength;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update cart state in sequence
            UpdateDistance(deltaTime, isMovingForward, isMovingBackward);
            UpdatePosition(track);
            UpdateWheels(track);
            UpdateRotation(track);
            UpdatePhysicsEntity();
        }

        /// <summary>
        /// Updates the distance traveled along the track based on input.
        /// </summary>
        private void UpdateDistance(float deltaTime, bool isMovingForward, bool isMovingBackward)
        {
            _previousT = _t;
            
            if (isMovingForward)
            {
                _t += _speed * deltaTime;
            }
            else if (isMovingBackward)
            {
                _t -= _speed * deltaTime;
            }
            
            // Clamp distance to track bounds
            _t = MathHelper.Clamp(_t, MIN_TRACK_DISTANCE, _maxTrackDistance - MIN_TRACK_DISTANCE);
        }

        /// <summary>
        /// Updates the cart's position based on the current track distance.
        /// </summary>
        public void UpdatePosition(GameTime gameTime, ITrack track)
        {
            if (track == null) return;
            
            UpdatePosition(track);
        }

        /// <summary>
        /// Updates the cart's position based on the current track distance.
        /// </summary>
        private void UpdatePosition(ITrack track)
        {
            _previousPosition = Position;

            // Get target position from track
            _targetPosition = track.GetPointByDistance(_t);

            // Calculate movement vector
            Vector2 movement = _targetPosition - _previousPosition;
            float movementLength = movement.Length();

            // Only update position if movement is significant
            if (movementLength > MIN_MOVEMENT_THRESHOLD)
            {
                movement.Normalize();
                Position = _previousPosition + movement * (movementLength * POSITION_INTERPOLATION_FACTOR);
            }
            else
            {
                Position = _previousPosition;
            }
            
            // Update bounding box with new position
            _boundingBox.Update(Position, Rotation);
        }

        /// <summary>
        /// Updates the cart wheel positions based on the track.
        /// </summary>
        private void UpdateWheels(ITrack track)
        {
            _wheelSystem.UpdateWheelPositions(track, _t);
        }

        /// <summary>
        /// Updates the cart's rotation based on the track curvature.
        /// </summary>
        public void UpdateRotation(ITrack track)
        {
            if (track == null) return;
            
            float targetRotation = track.GetRotationAtDistance(_t);
            _rotationChange = MathHelper.WrapAngle(targetRotation - _lastRotation);

            if (Math.Abs(_rotationChange) > 0.01f)
            {
                if (Math.Abs(_rotationChange) > MAX_ROTATION_CHANGE)
                {
                    targetRotation = _lastRotation + Math.Sign(_rotationChange) * MAX_ROTATION_CHANGE;
                }

                Rotation = targetRotation;
                _lastRotation = Rotation;
            }
        }
        
        /// <summary>
        /// Updates the physics entity's position based on the cart's position.
        /// </summary>
        private void UpdatePhysicsEntity()
        {
            if (_physicsEntity.BoundingBox is SplineMiner.Core.Physics.Components.BoundingBox boundingBox)
            {
                Vector2 physicsPosition = Position - new Vector2(_size.X * 0.25f, _size.Y * 0.75f);
                boundingBox.UpdatePosition(physicsPosition);
                _physicsEntity.Velocity = Vector2.Zero;
            }
        }

        /// <summary>
        /// Handles collision response.
        /// </summary>
        public void OnCollision(CollisionInfo info)
        {
            // Let the physics entity handle basic collision response
            _physicsEntity.OnCollision(info);

            // Stop movement by reverting to previous position on track
            _t = _previousT;
        }
    }
} 