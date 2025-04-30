using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Provides visualization and analysis tools for debugging cart movement.
    /// This interface enables testing and visualization of cart movement patterns.
    /// </summary>
    public interface IDebugVisualizer
    {
        /// <summary>
        /// Draws debug information about the cart's movement and position.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="debugTexture">The texture to use for debug visualization.</param>
        void DrawDebugInfo(SpriteBatch spriteBatch, Texture2D debugTexture);

        /// <summary>
        /// Starts a movement test to analyze cart movement patterns.
        /// </summary>
        void StartMovementTest();

        /// <summary>
        /// Analyzes the smoothness of the cart's movement during a test.
        /// </summary>
        void AnalyzeMovementSmoothness();

        /// <summary>
        /// Gets whether a movement test is currently in progress.
        /// </summary>
        bool IsTestingMovement { get; }

        /// <summary> 
        /// Updates the debug visualizer.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>    
        void Update(GameTime gameTime);
    }
} 