using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SplineMiner
{
    public class Player
    {
        public Vector2 Position { get; private set; }
        public Texture2D Texture { get; set; }
        private int _currentTrackIndex = 0;
        private float _speed = 100f; // Pixels per second

        public Player(Vector2 startPosition)
        {
            Position = startPosition;
        }

        public void Update(GameTime gameTime, Track track)
        {
            // TODO: Note this is just a placeholder for the player movement logic
            // In a real game, you would want to use input from the keyboard or gamepad
            // and apply physics for smoother movement
            if (track.Points.Count < 2) return;

            // Get the current and next points on the track
            Vector2 currentPoint = track.Points[_currentTrackIndex];
            Vector2 nextPoint = track.Points[_currentTrackIndex + 1];

            // Calculate movement direction
            Vector2 direction = Vector2.Normalize(nextPoint - currentPoint);

            // Move the player
            Position += direction * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Check if the player has reached the next point
            if (Vector2.Distance(Position, nextPoint) < 5f)
            {
                _currentTrackIndex++;
                Debug.WriteLine($"Current Track Index: {_currentTrackIndex}");
                if (_currentTrackIndex >= track.Points.Count - 1)
                {
                    _currentTrackIndex = 0; // Loop back to the start
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.Red, 0f, Vector2.Zero, new Vector2(10, 10), SpriteEffects.None, 0f);
        }
    }
}
