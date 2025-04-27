using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SplineMiner
{
    public class CartController
    {
        // 
        private Vector2 WorldPosition2D;
        public Texture2D Texture { get; set; }
        private int _currentTrackIndex = 0;
        InputManager _inputManager;
        private float _t = 0f;
        private float _speed = 300; // Pixels per second

        // CartController is only meant to exist on a track
        // There should be an entirely separate controler for when a player hops out the cart
        public CartController(InputManager inputManger)
        {
            _inputManager = inputManger;
            WorldPosition2D = new Vector2(0, 0); 
        }

        public void Update(GameTime gameTime, Track track)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Get the current and next points on the track
            // There are multiple tracks. Whenever the level is clicked on world, the function is updated

            if (_inputManager.Forward())
            {
                _t += _speed * deltaTime;
            }
            else if (_inputManager.Backward())
            {
                _t -= _speed * deltaTime;
            }
            //WorldPosition2D = track.GetPoint(_t);
            WorldPosition2D = track.GetPointByDistance(_t);


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, WorldPosition2D, null, Color.Red, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
