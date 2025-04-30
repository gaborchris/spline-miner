using Microsoft.Xna.Framework;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Controls the movement and rotation of a cart along a track. This interface manages
    /// the cart's position, speed, and orientation based on track geometry.
    /// </summary>
    public interface IMovementController
    {
        /// <summary>
        /// Updates the cart's position based on game time and track information.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="track">The track the cart is moving along.</param>
        void UpdatePosition(GameTime gameTime, ITrack track);

        /// <summary>
        /// Updates the cart's rotation to match the track's orientation at the current position.
        /// </summary>
        /// <param name="track">The track the cart is moving along.</param>
        void UpdateRotation(ITrack track);

        /// <summary>
        /// Gets the current position of the cart in world coordinates.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the current rotation of the cart in radians.
        /// </summary>
        float Rotation { get; }

        /// <summary>
        /// Gets or sets the current speed of the cart in pixels per second.
        /// </summary>
        float Speed { get; set; }

        /// <summary>
        /// Gets or sets the current distance along the track that the cart has traveled.
        /// </summary>
        float CurrentDistance { get; set; }
    }
} 