using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for camera services.
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// Gets the camera's transformation matrix.
        /// </summary>
        Matrix Transform { get; }

        /// <summary>
        /// Gets the camera's position.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the camera's zoom level.
        /// </summary>
        float Zoom { get; }

        /// <summary>
        /// Gets the camera's viewport.
        /// </summary>
        Viewport Viewport { get; }

        /// <summary>
        /// Initializes the camera with the specified viewport.
        /// </summary>
        /// <param name="viewport">The game viewport.</param>
        void Initialize(Viewport viewport);

        /// <summary>
        /// Sets the camera's target to follow.
        /// </summary>
        /// <param name="target">The target to follow.</param>
        void SetTarget(ICameraObserver target);

        /// <summary>
        /// Updates the camera's position and transformation matrix.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Converts a screen position to world coordinates.
        /// </summary>
        /// <param name="screenPosition">The screen position to convert.</param>
        /// <returns>The world position.</returns>
        Vector2 ScreenToWorld(Vector2 screenPosition);
    }
}