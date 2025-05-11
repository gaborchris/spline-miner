using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Game.Cart;

namespace SplineMiner.Game.Debug.Interfaces
{
    /// <summary>
    /// Interface for visualizing debug information about a cart.
    /// </summary>
    public interface ICartDebugVisualizer
    {
        /// <summary>
        /// Draws debug visualization for the cart.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <param name="cart">The cart model to visualize.</param>
        void Draw(SpriteBatch spriteBatch, CartModel cart);
    }
} 