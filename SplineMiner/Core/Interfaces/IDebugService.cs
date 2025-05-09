using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.UI.DebugTools;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for debug services.
    /// </summary>
    public interface IDebugService
    {
        /// <summary>
        /// Gets whether debug functionality is enabled.
        /// </summary>
        bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Updates the debug state.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Draws debug information.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Gets the world parameter panel for external configuration.
        /// </summary>
        /// <returns>The control panel for world parameters.</returns>
        ControlPanel GetWorldParameterPanel();
        IDebugLogger CreateLogger(string loggerName);
        IDebugLogger GetLogger(string loggerName);


    }
}