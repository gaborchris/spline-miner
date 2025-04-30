using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Represents a cart that can move along a track. This interface defines the core functionality
    /// for a cart's movement and rendering, including position tracking and distance-based movement.
    /// </summary>
    public interface ICart
    {
        /// <summary>
        /// Updates the cart's state based on game time and track information.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="track">The track the cart is moving along.</param>
        void Update(GameTime gameTime, ITrack track);

        /// <summary>
        /// Draws the cart using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Gets the current position of the cart in world coordinates.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the current distance along the track that the cart has traveled.
        /// </summary>
        float CurrentDistance { get; }
    }
} 