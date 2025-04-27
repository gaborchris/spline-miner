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

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
