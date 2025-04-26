using System.Collections.Generic;
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
                           // Interesting shape: Star-like pattern
                           new Vector2(400, 200), // Top point
                           new Vector2(450, 300), // Upper-right
                           new Vector2(550, 300), // Far-right
                           new Vector2(475, 350), // Lower-right
                           new Vector2(500, 450), // Bottom point
                           new Vector2(400, 400), // Lower-middle-left
                           new Vector2(300, 450), // Bottom-left
                           new Vector2(325, 350), // Lower-left
                           new Vector2(250, 300), // Far-left
                           new Vector2(350, 300), // Upper-left
                           new Vector2(400, 250), // Upper-middle
                           new Vector2(400, 200), // Back to Top point (closing the shape)
                       });

            // Initialize the player at the start of the track

            _inputManager = new InputManager();
            _player = new CartController(_inputManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load player texture (placeholder: a white rectangle)

            // Create a 32x32 placeholder texture
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

            // Update player movement along the track
            _inputManager.Update();
            _player.Update(gameTime, _track);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
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
