using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Game.Debug.Interfaces
{
    /// <summary>
    /// Interface for visualizing debug information about any object that implements IBoundingBox.
    /// </summary>
    public interface IDebugVisualizer
    {
        /// <summary>
        /// Draws debug visualization for the given bounding box.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <param name="boundingBox">The bounding box to visualize.</param>
        void Draw(SpriteBatch spriteBatch, IBoundingBox boundingBox);
    }
} 