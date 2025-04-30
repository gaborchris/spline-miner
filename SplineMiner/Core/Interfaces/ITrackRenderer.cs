using Microsoft.Xna.Framework.Graphics;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Handles the rendering of a track. This interface manages the visual representation
    /// of the track and its debug information.
    /// </summary>
    public interface ITrackRenderer
    {
        /// <summary>
        /// Draws the track using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="track">The track to draw.</param>
        void Draw(SpriteBatch spriteBatch, ITrack track);

        /// <summary>
        /// Draws debug information about the track.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="distance">The current distance along the track.</param>
        /// <param name="debugTexture">The texture to use for debug visualization.</param>
        void DrawDebugInfo(SpriteBatch spriteBatch, float distance, Texture2D debugTexture);
    }
} 