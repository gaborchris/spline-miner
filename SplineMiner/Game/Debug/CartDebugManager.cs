using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Game.Cart;
using SplineMiner.Game.Debug.Interfaces;

namespace SplineMiner.Game.Debug
{
    /// <summary>
    /// Manages debug visualization for carts.
    /// </summary>
    public class CartDebugManager
    {
        private readonly List<ICartDebugVisualizer> _visualizers = [];
        private bool _isEnabled = true;

        /// <summary>
        /// Gets or sets whether debug visualization is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        /// <summary>
        /// Adds a visualizer to the debug manager.
        /// </summary>
        /// <param name="visualizer">The visualizer to add.</param>
        public void AddVisualizer(ICartDebugVisualizer visualizer)
        {
            if (!_visualizers.Contains(visualizer))
            {
                _visualizers.Add(visualizer);
            }
        }

        /// <summary>
        /// Removes a visualizer from the debug manager.
        /// </summary>
        /// <param name="visualizer">The visualizer to remove.</param>
        public void RemoveVisualizer(ICartDebugVisualizer visualizer)
        {
            _visualizers.Remove(visualizer);
        }

        /// <summary>
        /// Draws debug visualization for a cart using all registered visualizers.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <param name="cart">The cart to visualize.</param>
        public void Draw(SpriteBatch spriteBatch, CartController cart)
        {
            if (!_isEnabled) return;

            foreach (var visualizer in _visualizers)
            {
                visualizer.Draw(spriteBatch, cart);
            }
        }
    }
} 