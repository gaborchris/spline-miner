using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Interface for managing game state.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Gets or sets the current game state.
        /// </summary>
        GameState CurrentState { get; set; }

        /// <summary>
        /// Initializes the game state.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Loads content for the game state.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Updates the game state.
        /// </summary>
        /// <param name="gameTime">The game's timing values.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Draws the game state.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        void Draw(SpriteBatch spriteBatch);
    }

    /// <summary>
    /// Enumeration of possible game states.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}