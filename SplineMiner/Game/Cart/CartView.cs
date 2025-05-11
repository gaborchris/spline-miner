using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Game.Debug;
using SplineMiner.Game.Debug.Visualizers;

namespace SplineMiner.Game.Cart
{
    /// <summary>
    /// Handles the visual representation of the cart.
    /// </summary>
    public class CartView
    {
        private readonly CartModel _model;
        private readonly CartDebugManager _debugManager;
        private readonly CartWheelVectorVisualizer _wheelVectorVisualizer;
        private readonly BoundingBoxVisualizer _boundingBoxVisualizer;
        private Texture2D _texture;
        private float _textureScale;

        public Texture2D Texture => _texture;

        public CartView(CartModel model, GraphicsDevice graphicsDevice)
        {
            _model = model;
            _debugManager = new CartDebugManager();
            _wheelVectorVisualizer = new CartWheelVectorVisualizer(graphicsDevice);
            _boundingBoxVisualizer = new BoundingBoxVisualizer(graphicsDevice);

            // Add visualizers to debug manager
            _debugManager.AddCartVisualizer(_wheelVectorVisualizer);
            _debugManager.AddBoundingBoxVisualizer(_boundingBoxVisualizer);
        }

        /// <summary>
        /// Sets the cart's texture and calculates the appropriate scale to match the world space size.
        /// </summary>
        /// <param name="texture">The texture to use for the cart.</param>
        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
            if (texture != null)
            {
                // Calculate scale to make texture match world space size
                _textureScale = _model.Size.Y / texture.Height;
            }
        }

        /// <summary>
        /// Draws the cart using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture == null) return;

            Vector2 origin = new(_texture.Width / 2f, _texture.Height);

            spriteBatch.Draw(
                _texture,
                _model.Position,
                null,
                Color.White,
                _model.Rotation,
                origin,
                _textureScale,
                SpriteEffects.None,
                0f
            );

            // Draw debug visualization
            _debugManager.Draw(spriteBatch, _model);
        }
    }
} 