using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Utils;
using SplineMiner.Core.Physics.Components;
using SplineMiner.Core.Physics.Entities;

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

        private readonly IMovementController _movementController;
        private readonly IWheelSystem _wheelSystem;
        private readonly IDebugVisualizer _debugVisualizer;
        private readonly IInputService _inputService;
        private Texture2D _texture;
        private Texture2D _debugTexture;
        private bool _showDebugInfo = true;
        private bool _isTestRunning = false;
        private const float TEST_SPEED = 200f; // Speed in pixels per second

        private readonly DynamicEntity _physicsEntity;

        public Texture2D Texture { get; set; }
        public Texture2D DebugTexture => _debugTexture;
        public float CurrentDistance => _t;
        public Vector2 Position => _movementController.Position;

        // ICollidable implementation
        public IBoundingBox BoundingBox => _physicsEntity.BoundingBox;
        public Vector2 Velocity 
        {
            get => _physicsEntity.Velocity;
            set => _physicsEntity.Velocity = value;
        }
        public float Mass => _physicsEntity.Mass;
        public void OnCollision(CollisionInfo info) => _physicsEntity.OnCollision(info);

        /// <summary>
        /// Initializes a new instance of the CartController.
        /// </summary>
        /// <param name="inputService">The input service for handling player controls.</param>
        /// <exception cref="ArgumentNullException">Thrown when inputService is null.</exception>
        public CartController(IInputService inputService)
        {
            _inputService = inputService;
            _movementController = new CartMovementController();
            _wheelSystem = new CartWheelSystem();
            _debugVisualizer = new CartDebugVisualizer(_movementController, _wheelSystem);

            // Initialize with appropriate mass, bounce, and friction values
            _physicsEntity = new DynamicEntity(
                position: Vector2.Zero,
                size: new Vector2(32, 32), // Cart size
                mass: 1.0f,
                bounceFactor: 0.5f,
                frictionCoefficient: 0.1f
            );
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

            if (_debugVisualizer.IsTestingMovement)
            {
                _t += TEST_SPEED * deltaTime;
            }
            else
            {
                // Normal user-controlled movement
                if (_inputService.Forward())
                {
                    _t += _movementController.Speed * deltaTime;
                }
                else if (_inputService.Backward())
                {
                    _t -= _movementController.Speed * deltaTime;
                }
            }

            _movementController.CurrentDistance = _t;
            _movementController.UpdatePosition(gameTime, track);
            _wheelSystem.UpdateWheelPositions(track, _t);
            _movementController.UpdateRotation(track);

            if (_inputService.IsKeyPressed(Keys.T))
            {
                Debug.WriteLine("[CartController] T key pressed, starting movement test");
                _debugVisualizer.StartMovementTest();
            }
            _debugVisualizer.Update(gameTime);
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
            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height);

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

            if (_showDebugInfo)
            {
                _debugVisualizer.DrawDebugInfo(spriteBatch, DebugTexture);
            }
        }

        /// <summary>
        /// Loads debug textures for visualization purposes.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device used for rendering.</param>
        /// <remarks>
        /// TODO: Remove debug textures in release builds
        /// TODO: Implement proper debug visualization system
        /// </remarks>
        public void LoadDebugTexture(GraphicsDevice graphicsDevice)
        {
            _debugTexture = new Texture2D(graphicsDevice, 1, 1);
            _debugTexture.SetData(new[] { Color.White });
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

    // Wheel system implementation
    public class CartWheelSystem : IWheelSystem
    {
        private const float WHEEL_DISTANCE = 30f;
        private Vector2 _frontWheelPosition;
        private Vector2 _backWheelPosition;

        public Vector2 FrontWheelPosition => _frontWheelPosition;
        public Vector2 BackWheelPosition => _backWheelPosition;

        public void UpdateWheelPositions(ITrack track, float currentDistance)
        {
            float frontDistance = currentDistance + WHEEL_DISTANCE;
            float backDistance = currentDistance - WHEEL_DISTANCE;

            if (frontDistance > track.TotalArcLength)
                frontDistance -= track.TotalArcLength;
            if (backDistance < 0)
                backDistance += track.TotalArcLength;

            _frontWheelPosition = track.GetPointByDistance(frontDistance);
            _backWheelPosition = track.GetPointByDistance(backDistance);
        }
    }

    // Debug visualizer implementation
    public class CartDebugVisualizer : IDebugVisualizer
    {
        private readonly IMovementController _movementController;
        private readonly IWheelSystem _wheelSystem;
        private readonly List<Vector2> _positionHistory = new();
        private bool _isTestingMovement = false;
        private float _testTimer = 0f;
        private const float TEST_DURATION = 5.0f;

        public bool IsTestingMovement => _isTestingMovement;

        public CartDebugVisualizer(IMovementController movementController, IWheelSystem wheelSystem)
        {
            _movementController = movementController;
            _wheelSystem = wheelSystem;
        }

        public void DrawDebugInfo(SpriteBatch spriteBatch, Texture2D debugTexture)
        {
            // Draw wheel positions
            DrawingHelpers.DrawCircle(spriteBatch, debugTexture, _wheelSystem.FrontWheelPosition, 3, Color.Green);
            DrawingHelpers.DrawCircle(spriteBatch, debugTexture, _wheelSystem.BackWheelPosition, 3, Color.Blue);

            // Draw line between wheels
            DrawingHelpers.DrawLine(spriteBatch, debugTexture, _wheelSystem.FrontWheelPosition, _wheelSystem.BackWheelPosition, Color.Yellow, 1);

            // Draw cart's position point
            DrawingHelpers.DrawCircle(spriteBatch, debugTexture, _movementController.Position, 3, Color.White);

            // Draw rotation change indicator
            const float INDICATOR_LENGTH = 20f;
            Vector2 indicatorEnd = _movementController.Position + new Vector2(
                (float)Math.Cos(_movementController.Rotation) * INDICATOR_LENGTH,
                (float)Math.Sin(_movementController.Rotation) * INDICATOR_LENGTH
            );
            DrawingHelpers.DrawLine(spriteBatch, debugTexture, _movementController.Position, indicatorEnd, Color.Purple, 2);

            // Draw normal and tangent vectors
            const float VECTOR_LENGTH = 30f;
            float normalAngle = _movementController.Rotation + MathHelper.PiOver2;
            float tangentAngle = _movementController.Rotation;

            // Draw normal vector (perpendicular to track)
            Vector2 normalEnd = _movementController.Position + new Vector2(
                (float)Math.Cos(normalAngle) * VECTOR_LENGTH,
                (float)Math.Sin(normalAngle) * VECTOR_LENGTH
            );
            DrawingHelpers.DrawLine(spriteBatch, debugTexture, _movementController.Position, normalEnd, Color.Red, 2);

            // Draw tangent vector (along track)
            Vector2 tangentEnd = _movementController.Position + new Vector2(
                (float)Math.Cos(tangentAngle) * VECTOR_LENGTH,
                (float)Math.Sin(tangentAngle) * VECTOR_LENGTH
            );
            DrawingHelpers.DrawLine(spriteBatch, debugTexture, _movementController.Position, tangentEnd, Color.Green, 2);

            // Draw position history if testing
            if (_isTestingMovement)
            {
                for (int i = 1; i < _positionHistory.Count; i++)
                {
                    DrawingHelpers.DrawLine(spriteBatch, debugTexture, _positionHistory[i - 1], _positionHistory[i], Color.Orange, 1);
                }
            }
        }

        public void StartMovementTest()
        {
            Debug.WriteLine("[CartDebugVisualizer] Starting movement test");
            _isTestingMovement = true;
            _positionHistory.Clear();
            _testTimer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            if (_isTestingMovement)
            {
                _positionHistory.Add(_movementController.Position);
                _testTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_testTimer >= TEST_DURATION)
                {
                    AnalyzeMovementSmoothness();
                    _isTestingMovement = false;
                }
            }
        }

        public void AnalyzeMovementSmoothness()
        {
            if (_positionHistory.Count < 3)
            {
                Debug.WriteLine("[CartDebugVisualizer] Not enough positions for analysis");
                return;
            }

            // Calculate velocity and acceleration magnitudes
            List<float> velocities = new();
            List<float> accelerations = new();

            for (int i = 1; i < _positionHistory.Count; i++)
            {
                float velocity = Vector2.Distance(_positionHistory[i - 1], _positionHistory[i]);
                velocities.Add(velocity);
            }

            for (int i = 1; i < velocities.Count; i++)
            {
                float acceleration = Math.Abs(velocities[i] - velocities[i - 1]);
                accelerations.Add(acceleration);
            }

            float avgVelocity = velocities.Count > 0 ? velocities.Sum() / velocities.Count : 0;
            float avgAcceleration = accelerations.Count > 0 ? accelerations.Sum() / accelerations.Count : 0;
            float maxAcceleration = accelerations.Count > 0 ? accelerations.Max() : 0;

            Debug.WriteLine($"[CartDebugVisualizer] Movement analysis complete:");
            Debug.WriteLine($"- Average velocity: {avgVelocity:F2} pixels per frame");
            Debug.WriteLine($"- Average acceleration: {avgAcceleration:F2}");
            Debug.WriteLine($"- Maximum acceleration: {maxAcceleration:F2}");
        }
    }
}
