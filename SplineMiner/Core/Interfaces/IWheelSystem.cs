using Microsoft.Xna.Framework;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Manages the wheel positions of a cart. This interface handles the calculation
    /// and tracking of front and back wheel positions based on the cart's current position.
    /// </summary>
    public interface IWheelSystem
    {
        /// <summary>
        /// Updates the wheel positions based on the current track and cart position.
        /// </summary>
        /// <param name="track">The track the cart is moving along.</param>
        /// <param name="currentDistance">The current distance along the track.</param>
        void UpdateWheelPositions(ITrack track, float currentDistance);

        /// <summary>
        /// Gets the position of the front wheel in world coordinates.
        /// </summary>
        Vector2 FrontWheelPosition { get; }

        /// <summary>
        /// Gets the position of the back wheel in world coordinates.
        /// </summary>
        Vector2 BackWheelPosition { get; }
    }
}