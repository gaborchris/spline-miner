using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;
using SplineMiner.Game.Items.Tools;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for UI services.
    /// </summary>
    public interface IUIService
    {
        /// <summary>
        /// Gets or sets the current UI tool.
        /// </summary>
        UITool CurrentTool { get; set; }

        /// <summary>
        /// Updates the UI state.
        /// </summary>
        /// <param name="inputService">The input service for handling user input.</param>
        void Update(IInputService inputService);

        /// <summary>
        /// Draws the UI elements.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        void Draw(SpriteBatch spriteBatch);
    }
} 