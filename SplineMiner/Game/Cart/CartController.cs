using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Physics.Components;
using SplineMiner.Core.Physics.Entities;
using SplineMiner.Core.Utils;
using SplineMiner.Game.Debug;
using SplineMiner.Game.Debug.Visualizers;

namespace SplineMiner.Game.Cart
{
    /// <summary>
    /// Controls the player's cart movement and physics along the track.
    /// </summary>
    /// <remarks>
    /// TODO: Add support for cart upgrades and customization through abstraction
    /// TODO: Implement proper collision detection and response
    /// </remarks>
    public class CartController : ICart, ICameraObserver, ICollidable
    {
        private float _t = 0f;
        private float _previousT;

        private readonly CartMovementController _movementController;
        private readonly CartWheelSystem _wheelSystem;
        private readonly IInputService _inputService;
        private readonly CartDebugManager _debugManager;

        private readonly DynamicEntity _physicsEntity;

        public Texture2D Texture { get; set; }
        public float CurrentDistance => _t;
        public Vector2 Position => _movementController.Position;
        public float Rotation => _movementController.Rotation;
        public CartWheelSystem WheelSystem => _wheelSystem;

        // ICollidable implementation
        public IBoundingBox BoundingBox => _physicsEntity.BoundingBox;
        public float Mass => _physicsEntity.Mass;
        public void OnCollision(CollisionInfo info)
        {
            // First, let the physics entity handle basic collision response
            _physicsEntity.OnCollision(info);

            // Stop movement in both directions by resetting the distance parameter
            _t = _previousT;
        }

        /// <summary>
        /// Initializes a new instance of the CartController.
        /// </summary>
        /// <param name="inputService">The input service for handling player controls.</param>
        /// <param name="graphicsDevice">The graphics device used for rendering.</param>
        /// <exception cref="ArgumentNullException">Thrown when inputService is null.</exception>
        public CartController(IInputService inputService, GraphicsDevice graphicsDevice)
        {
            _inputService = inputService;
            _movementController = new CartMovementController();
            _wheelSystem = new CartWheelSystem();
            _debugManager = new CartDebugManager();

            // Initialize with appropriate mass, bounce, and friction values
            _physicsEntity = new DynamicEntity(
                position: Vector2.Zero,
                size: new Vector2(16, 16), // Smaller box size
                mass: 1.0f,
                bounceFactor: 0.5f,
                frictionCoefficient: 0.1f
            );

            // Initialize debug visualizers
            CartWheelVectorVisualizer wheelVectorVisualizer = new(graphicsDevice);
            _debugManager.AddVisualizer(wheelVectorVisualizer);

            CartBoundingBoxVisualizer boundingBoxVisualizer = new(graphicsDevice);
            _debugManager.AddVisualizer(boundingBoxVisualizer);
        }

        /// <summary>
        /// Updates the cart's position and physics based on input and track state.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="track">The current track the cart is on.</param>
        /// <remarks>
        /// TODO: Add support for track switching
        /// </remarks>
        public void Update(GameTime gameTime, ITrack track)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Store the previous t value before updating
            _previousT = _t;

            // Normal user-controlled movement
            if (_inputService.Forward())
            {
                _t += _movementController.Speed * deltaTime;
            }
            else if (_inputService.Backward())
            {
                _t -= _movementController.Speed * deltaTime;
            }

            _movementController.CurrentDistance = _t;
            _movementController.UpdatePosition(gameTime, track);
            _wheelSystem.UpdateWheelPositions(track, _t);
            _movementController.UpdateRotation(track);

            // Update physics entity position to match cart position, accounting for the origin point
            if (_physicsEntity.BoundingBox is SplineMiner.Core.Physics.Components.BoundingBox boundingBox)
            {
                // Calculate the position that centers the bounding box horizontally and places it higher on the cart
                Vector2 boundingBoxPosition = _movementController.Position - new Vector2(8, 40); // Center horizontally (half of 16), place higher up
                boundingBox.UpdatePosition(boundingBoxPosition);
                // Reset velocity to prevent drift
                _physicsEntity.Velocity = Vector2.Zero;
            }
        }

        /// <summary>
        /// Draws the cart using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <remarks>
        /// TODO: Add support for cart animations
        /// TODO: Implement proper cart rotation based on track curvature
        /// TODO: Add support for cart effects (sparks, smoke)
        /// </remarks>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new(Texture.Width / 2f, Texture.Height);

            spriteBatch.Draw(
                Texture,
                Position,
                null,
                Color.Red,
                _movementController.Rotation,
                origin,
                1.0f,
                SpriteEffects.None,
                0f
            );

            // Draw debug visualization
            _debugManager.Draw(spriteBatch, this);
        }
    }

    // Core interfaces
    public class CartMovementController : IMovementController
    {
        private const float MIN_MOVEMENT_THRESHOLD = 0.1f;
        private const float POSITION_INTERPOLATION_FACTOR = 0.5f;
        private const float MAX_ROTATION_CHANGE = MathHelper.Pi / 6;

        private Vector2 _position;
        private Vector2 _previousPosition;
        private Vector2 _targetPosition;
        private float _rotation;
        private float _lastRotation;
        private float _rotationChange;
        private float _t = 0f;
        private float _speed = 300;

        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }
        public float CurrentDistance
        {
            get => _t;
            set => _t = value;
        }

        public void UpdatePosition(GameTime gameTime, ITrack track)
        {
            _previousPosition = _position;

            // Update target position
            _targetPosition = track.GetPointByDistance(_t);

            // Calculate movement vector
            Vector2 movement = _targetPosition - _previousPosition;
            float movementLength = movement.Length();

            // Only update position if movement is significant
            if (movementLength > MIN_MOVEMENT_THRESHOLD)
            {
                movement.Normalize();
                _position = _previousPosition + movement * (movementLength * POSITION_INTERPOLATION_FACTOR);
            }
            else
            {
                _position = _previousPosition;
            }
        }

        public void UpdateRotation(ITrack track)
        {
            float targetRotation = track.GetRotationAtDistance(_t);
            _rotationChange = MathHelper.WrapAngle(targetRotation - _lastRotation);

            if (Math.Abs(_rotationChange) > 0.01f)
            {
                if (Math.Abs(_rotationChange) > MAX_ROTATION_CHANGE)
                {
                    targetRotation = _lastRotation + Math.Sign(_rotationChange) * MAX_ROTATION_CHANGE;
                }

                _rotation = targetRotation;
                _lastRotation = _rotation;
            }
        }
    }
}
