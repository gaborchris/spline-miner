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
        private Track _track;
        private int _hoveredPointIndex = -1;
        private SpriteFont _debugFont;
        private bool _showDebugInfo = true;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Initialize the track with predefined points
            _track = new Track(new List<Vector2>
                       {
                          // Simple stretched out semi-sinusoidal pattern
                          new(100, 300), // Start point
                          new(200, 250), // First peak
                          new(300, 350), // First trough
                          new(400, 250), // Second peak
                          new(500, 350), // Second trough
                          new(600, 250), // Third peak
                          new(700, 300), // End point
                       });

            // Initialize the player at the start of the track
            _inputManager = new InputManager();
            _player = new CartController(_inputManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load track textures
            _track.LoadContent(GraphicsDevice);

            // Precalculate arc length for the track
            _track.RecalculateArcLength();

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

            // Create a simple debug font - since we don't have a real font loaded
            try 
            {
                _debugFont = Content.Load<SpriteFont>("debug_font");
            }
            catch 
            {
                Debug.WriteLine("Debug font not found. Debug text will not be rendered.");
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update input
            _inputManager.Update();
            
            // Handle mouse interaction with control points
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
            
            // Check if we're dragging a point
            if (_inputManager.IsLeftMousePressed())
            {
                // Check if we're clicking on a control point
                int pointIndex = _track.GetHoveredPointIndex(mousePosition);
                Debug.WriteLine(pointIndex);
                if (pointIndex != -1)
                {
                    _track.SelectPoint(pointIndex);
                }
            }
            else if (_inputManager.IsLeftMouseHeld())
            {
                _track.MoveSelectedPoint(mousePosition);
            }
            else if (_inputManager.IsLeftMouseReleased())
            {
                // Stop dragging
                _track.ReleaseSelectedPoint();
            }
            
            // Update hovered point for visual feedback
            _hoveredPointIndex = _track.GetHoveredPointIndex(mousePosition);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Draw the track
            _track.Draw(_spriteBatch);

            // Draw the player
            _player.Draw(_spriteBatch);

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
                    "Left/Right: Move cart"
                };

                float yPos = 10;
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
