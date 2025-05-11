using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Game.Cart;
using SplineMiner.Game.Debug.Interfaces;
using SplineMiner.Game.Debug.Visualizers;

namespace SplineMiner.Game.Debug
{
    /// <summary>
    /// Manages debug visualization for carts.
    /// </summary>
    public class CartDebugManager
    {
        private readonly List<ICartDebugVisualizer> _cartVisualizers = [];
        private readonly List<IDebugVisualizer> _boundingBoxVisualizers = [];
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
        /// Adds a cart-specific visualizer to the debug manager.
        /// </summary>
        /// <param name="visualizer">The visualizer to add.</param>
        public void AddCartVisualizer(ICartDebugVisualizer visualizer)
        {
            if (!_cartVisualizers.Contains(visualizer))
            {
                _cartVisualizers.Add(visualizer);
            }
        }

        /// <summary>
        /// Adds a bounding box visualizer to the debug manager.
        /// </summary>
        /// <param name="visualizer">The visualizer to add.</param>
        public void AddBoundingBoxVisualizer(IDebugVisualizer visualizer)
        {
            if (!_boundingBoxVisualizers.Contains(visualizer))
            {
                _boundingBoxVisualizers.Add(visualizer);
            }
        }

        /// <summary>
        /// Removes a cart-specific visualizer from the debug manager.
        /// </summary>
        /// <param name="visualizer">The visualizer to remove.</param>
        public void RemoveCartVisualizer(ICartDebugVisualizer visualizer)
        {
            _cartVisualizers.Remove(visualizer);
        }

        /// <summary>
        /// Removes a bounding box visualizer from the debug manager.
        /// </summary>
        /// <param name="visualizer">The visualizer to remove.</param>
        public void RemoveBoundingBoxVisualizer(IDebugVisualizer visualizer)
        {
            _boundingBoxVisualizers.Remove(visualizer);
        }

        /// <summary>
        /// Draws debug visualization for a cart using all registered visualizers.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <param name="cart">The cart model to visualize.</param>
        public void Draw(SpriteBatch spriteBatch, CartModel cart)
        {
            if (!_isEnabled) return;

            // Draw cart-specific visualizations
            foreach (var visualizer in _cartVisualizers)
            {
                visualizer.Draw(spriteBatch, cart);
            }

            // Draw bounding box visualizations
            foreach (var visualizer in _boundingBoxVisualizers)
            {
                visualizer.Draw(spriteBatch, cart.BoundingBox);
            }
        }
    }
} 