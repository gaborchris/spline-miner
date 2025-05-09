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

    }
}