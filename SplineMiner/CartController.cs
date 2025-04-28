using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace SplineMiner
{
    public class CartController : ICameraObserver
    {
        // 
        private Vector2 WorldPosition2D;
        public Texture2D Texture { get; set; }
        public Texture2D DebugTexture { get; private set; }
        public float CurrentDistance => _t;
        private int _currentTrackIndex = 0;
        InputManager _inputManager;
        private float _t = 0f;
        private float _speed = 300; // Pixels per second
        private float _rotation = 0f;
        private float _rotationSmoothness = 0.1f; // Adjust this to control rotation smoothness
        private bool _showDebugInfo = true;
        
        // For movement smoothness testing
        private bool _isTestingMovement = false;
        private List<Vector2> _positionHistory = new List<Vector2>();
        private float _testTimer = 0f;
        private const float TEST_DURATION = 5.0f; // 5 seconds of test

        // CartController is only meant to exist on a track
        // There should be an entirely separate controler for when a player hops out the cart
        public CartController(InputManager inputManger)
        {
            _inputManager = inputManger;
            WorldPosition2D = new Vector2(0, 0); 
        }

        public void StartMovementTest()
        {
            _isTestingMovement = true;
            _positionHistory.Clear();
            _testTimer = 0f;
            // _t = 0f; // Start from beginning of track
            Debug.WriteLine("Starting cart movement test for 5 seconds");
        }

        private void UpdateMovementTest(GameTime gameTime)
        {
            if (!_isTestingMovement) return;
            
            _testTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Store position every frame during test
            _positionHistory.Add(WorldPosition2D);
            
            // Force movement at constant speed for testing
            _t += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // End test after duration
            if (_testTimer >= TEST_DURATION)
            {
                _isTestingMovement = false;
                AnalyzeMovementSmoothnessTest();
            }
        }

        private void AnalyzeMovementSmoothnessTest()
        {
            if (_positionHistory.Count < 3)
            {
                Debug.WriteLine("Not enough position data for analysis");
                return;
            }

            // Calculate velocity and acceleration magnitudes
            List<float> velocities = new List<float>();
            List<float> accelerations = new List<float>();
            
            // First calculate velocities between consecutive positions
            for (int i = 1; i < _positionHistory.Count; i++)
            {
                Vector2 p1 = _positionHistory[i - 1];
                Vector2 p2 = _positionHistory[i];
                float velocity = Vector2.Distance(p1, p2);
                velocities.Add(velocity);
            }
            
            // Then calculate accelerations (changes in velocity)
            for (int i = 1; i < velocities.Count; i++)
            {
                float v1 = velocities[i - 1];
                float v2 = velocities[i];
                float acceleration = System.Math.Abs(v2 - v1);
                accelerations.Add(acceleration);
            }
            
            // Calculate statistics
            float avgVelocity = velocities.Count > 0 ? velocities.Sum() / velocities.Count : 0;
            float avgAcceleration = accelerations.Count > 0 ? accelerations.Sum() / accelerations.Count : 0;
            float maxAcceleration = accelerations.Count > 0 ? accelerations.Max() : 0;
            
            // Find abrupt changes in acceleration (jerk)
            int jumpCount = 0;
            float jerkThreshold = avgAcceleration * 5.0f; // 5x average is suspicious
            for (int i = 0; i < accelerations.Count; i++)
            {
                if (accelerations[i] > jerkThreshold)
                {
                    jumpCount++;
                    Debug.WriteLine($"Potential jump detected at position {i+2} with acceleration {accelerations[i]}");
                }
            }
            
            Debug.WriteLine($"Movement analysis complete:");
            Debug.WriteLine($"- Average velocity: {avgVelocity:F2} pixels per frame");
            Debug.WriteLine($"- Average acceleration: {avgAcceleration:F2}");
            Debug.WriteLine($"- Maximum acceleration: {maxAcceleration:F2}");
            Debug.WriteLine($"- Number of potential jumps: {jumpCount}");
            
            if (jumpCount > 0)
            {
                Debug.WriteLine("WARNING: Movement is not smooth. Check the spline interpolation and arc length calculations.");
            }
            else
            {
                Debug.WriteLine("Movement appears to be smooth.");
            }
        }

        public void Update(GameTime gameTime, Track track)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle test movement if we're testing
            if (_isTestingMovement)
            {
                UpdateMovementTest(gameTime);
                WorldPosition2D = track.GetPointByDistance(_t);
                // Update rotation during test
                float testRotation = track.GetRotationAtDistance(_t);
                _rotation = MathHelper.Lerp(_rotation, testRotation, _rotationSmoothness);
                track.UpdateCurrentPosition(_t);
                return;
            }

            // Store previous position for interpolation
            Vector2 previousPosition = WorldPosition2D;

            // Normal movement control
            if (_inputManager.Forward())
            {
                float newT = _t + _speed * deltaTime;
                // Only move forward if we haven't reached the traversable limit
                if (newT <= track.TotalArcLength)
                {
                    _t = newT;
                }
            }
            else if (_inputManager.Backward())
            {
                _t -= _speed * deltaTime;
            }
            
            // Get new position
            Vector2 newPosition = track.GetPointByDistance(_t);
            
            // Smooth position transition
            float smoothFactor = Math.Min(1.0f, deltaTime * 10.0f);
            WorldPosition2D = Vector2.Lerp(previousPosition, newPosition, smoothFactor);
            
            // Update rotation
            float currentRotation = track.GetRotationAtDistance(_t);
            _rotation = MathHelper.Lerp(_rotation, currentRotation, _rotationSmoothness);
            
            track.UpdateCurrentPosition(_t);
            
            // Toggle movement test with T key
            if (_inputManager.IsKeyPressed(Keys.T))
            {
                StartMovementTest();
            }
            
            // Visualize equally spaced points with V key
            if (_inputManager.IsKeyPressed(Keys.V))
            {
                track.VisualizeEquallySpacedPoints(20);
            }
        }

        public void LoadDebugTexture(GraphicsDevice graphicsDevice)
        {
            DebugTexture = new Texture2D(graphicsDevice, 1, 1);
            DebugTexture.SetData(new[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Calculate the origin at the bottom center of the texture
            Vector2 origin = new Vector2(Texture.Width / 2f, Texture.Height);
            
            // Draw cart with rotation
            spriteBatch.Draw(
                Texture,
                WorldPosition2D,
                null,
                Color.Red,
                _rotation,
                origin,
                1.0f,
                SpriteEffects.None,
                0f
            );
            
            if (_showDebugInfo && DebugTexture != null)
            {
                // Draw cart's forward direction
                const float DIRECTION_LENGTH = 30f;
                Vector2 directionEnd = WorldPosition2D + new Vector2(
                    (float)Math.Cos(_rotation) * DIRECTION_LENGTH,
                    (float)Math.Sin(_rotation) * DIRECTION_LENGTH
                );
                DrawingHelpers.DrawLine(spriteBatch, DebugTexture, WorldPosition2D, directionEnd, Color.Blue, 2);
                
                // Draw cart's position point
                DrawingHelpers.DrawCircle(spriteBatch, DebugTexture, WorldPosition2D, 3, Color.White);
            }
            
            // Draw path history during testing
            if (_isTestingMovement)
            {
                foreach (Vector2 pos in _positionHistory)
                {
                    spriteBatch.Draw(
                        Texture,
                        pos,
                        null,
                        Color.Yellow * 0.3f,
                        _rotation,
                        origin,
                        0.5f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }

        public Vector2 Position => WorldPosition2D;
    }
}
