using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SplineMiner
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private InputManager _inputManager;
        private CartController _player;
        private SplineTrack _track;
        private int _hoveredPointIndex = -1;
        private SpriteFont _debugFont;
        private bool _showDebugInfo = true;
        private UIManager _uiManager;
        private SpriteFont _uiFont;
        
        // FPS tracking
        private float _fps;
        private float _frameCounter;
        private float _timeSinceLastUpdate;
        private const float UPDATE_INTERVAL = 0.25f; // Update FPS display every quarter second

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Enable fixed time step for more consistent updates
            IsFixedTimeStep = true;
            _graphics.SynchronizeWithVerticalRetrace = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f); // Target 60 FPS
        }

        protected override void Initialize()
        {
            // Initialize the player at the start of the track
            _inputManager = new InputManager();
            _player = new CartController(_inputManager);

            // Initialize camera
            CameraManager.Instance.Initialize(GraphicsDevice.Viewport);
            CameraManager.Instance.SetTarget(_player);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load player texture (placeholder: a white rectangle)
            var w = 64;
            var h = (int)(w * 0.67);
            Texture2D minecartTexture = new Texture2D(GraphicsDevice, w, h);
            Color[] data = new Color[w * h];

            // Fill the texture with a solid color (e.g., gray)
            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Gray;

            minecartTexture.SetData(data);

            // Use this texture for your player or minecart
            _player.Texture = minecartTexture;
            _player.LoadDebugTexture(GraphicsDevice);

            // Create a simple debug font - since we don't have a real font loaded
            try 
            {
                _debugFont = Content.Load<SpriteFont>("debug_font");
                _uiFont = Content.Load<SpriteFont>("debug_font");
            }
            catch 
            {
                Debug.WriteLine("Fonts not found. Debug text and UI will not be rendered.");
            }

            // Initialize UI manager
            _uiManager = new UIManager(_uiFont, GraphicsDevice);
            
            // Initialize the track with test data
            _track = new SplineTrack(TestData.TestTrack.GetTestTrackNodes(), _uiManager);
            
            // Load track textures
            _track.LoadContent(GraphicsDevice);

            // Precalculate arc length for the track
            _track.RecalculateArcLength();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update FPS counter
            _timeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter++;
            
            if (_timeSinceLastUpdate >= UPDATE_INTERVAL)
            {
                _fps = _frameCounter / _timeSinceLastUpdate;
                _frameCounter = 0;
                _timeSinceLastUpdate = 0;
            }

            // Update input
            _inputManager.Update();
            
            // Update UI
            _uiManager.Update(_inputManager);
            
            // Update camera
            CameraManager.Instance.Update(gameTime);
            
            // Handle mouse interaction with control points based on current tool
            HandleMouseInteraction();

            // Update player movement along the track
            _player.Update(gameTime, _track);

            // Toggle debug info with F1
            if (_inputManager.IsKeyPressed(Keys.F1))
            {
                _showDebugInfo = !_showDebugInfo;
            }

            base.Update(gameTime);
        }
        
        private void HandleMouseInteraction()
        {
            // Get the mouse position
            Vector2 mousePosition = _inputManager.MousePosition;
            
            switch (_uiManager.CurrentTool)
            {
                case UITool.Track:
                    
                    // Handle right-click drag for editing shadow nodes
                    if (_inputManager.IsRightMousePressed())
                    {
                        int pointIndex = _track.GetHoveredPointIndex(mousePosition);
                        if (pointIndex != -1)
                        {
                            _track.SelectPoint(pointIndex);
                        }
                    }
                    else if (_inputManager.IsRightMouseHeld())
                    {
                        _track.MoveSelectedPoint(mousePosition);
                    }
                    else if (_inputManager.IsRightMouseReleased())
                    {
                        _track.ReleaseSelectedPoint();
                    }
                    // Handle left-click for placing new points
                    else if (_inputManager.IsLeftMousePressed())
                    {
                        if (!_track.IsHoveringEndpoint)
                        {
                            _track.PlaceNode(mousePosition);
                        }
                    }
                    break;

                case UITool.DeleteTrack:
                    if (_inputManager.IsLeftMousePressed())
                    {
                        int pointIndex = _track.GetHoveredPointIndex(mousePosition);
                        if (pointIndex != -1)
                        {
                            _track.DeletePoint(pointIndex);
                        }
                    }
                    break;
            }
            
            // Update hovered point for visual feedback
            _hoveredPointIndex = _track.GetHoveredPointIndex(mousePosition);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: CameraManager.Instance.Transform);

            // Draw the track
            _track.Draw(_spriteBatch);

            // Draw debug info for the track
            if (_showDebugInfo)
            {
                _track.DrawDebugInfo(_spriteBatch, _player.CurrentDistance, _player.DebugTexture);
            }

            // Draw the player
            _player.Draw(_spriteBatch);

            // Draw UI (UI should not be affected by camera)
            _spriteBatch.End();
            _spriteBatch.Begin();
            _uiManager.Draw(_spriteBatch);

            // Draw debug info
            if (_showDebugInfo)
            {
                // Create a simple debug background
                Texture2D debugBg = new Texture2D(GraphicsDevice, 1, 1);
                debugBg.SetData(new[] { Color.Black * 0.7f });
                
                string[] debugInfo = {
                    "Controls:",
                    "T: Test cart movement",
                    "V: Visualize equally spaced points",
                    "F1: Toggle debug info",
                    "Left/Right: Move cart",
                    $"Current Tool: {_uiManager.CurrentTool}",
                    $"FPS: {_fps:F5}"
                };

                // Calculate starting Y position at bottom of screen
                float yPos = GraphicsDevice.Viewport.Height - (debugInfo.Length * 25) - 10;
                
                foreach (string line in debugInfo)
                {
                    // Simple text background since we don't have a font
                    _spriteBatch.Draw(debugBg, new Rectangle(10, (int)yPos, 250, 20), Color.White);
                    
                    // If we do have the font, use it, otherwise just advance position
                    if (_debugFont != null)
                    {
                        _spriteBatch.DrawString(_debugFont, line, new Vector2(15, yPos), Color.White);
                    }
                    
                    yPos += 25;
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
