using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Physics.Components;
using SplineMiner.Core.Physics.Entities;
using SplineMiner.Core.Utils;
using SplineMiner.Game.Debug;
using SplineMiner.Game.Debug.Visualizers;

namespace SplineMiner.Game.Cart
{
    /// <summary>
    /// Controls the player's cart movement and physics along the track.
    /// </summary>
    /// <remarks>
    /// TODO: Add support for cart upgrades and customization through abstraction
    /// TODO: Implement proper collision detection and response
    /// </remarks>
    public class CartController : ICart, ICameraObserver, ICollidable
    {
        private readonly CartModel _model;
        private readonly CartView _view;
        private readonly IInputService _inputService;
        private readonly CartMovementController _movementController;

        public Texture2D Texture 
        { 
            get => _view.Texture;
            set => _view.SetTexture(value);
        }

        // ICart implementation
        public float CurrentDistance => _model.CurrentDistance;
        public Vector2 Position => _model.Position;
        public float Rotation => _model.Rotation;
        public CartWheelSystem WheelSystem => _model.WheelSystem;

        // ICollidable implementation
        public IBoundingBox BoundingBox => _model.BoundingBox;
        public float Mass => _model.Mass;

        /// <summary>
        /// Initializes a new instance of the CartController.
        /// </summary>
        /// <param name="inputService">The input service for handling player controls.</param>
        /// <param name="graphicsDevice">The graphics device used for rendering.</param>
        /// <param name="cartSize">The size of the cart in world space units.</param>
        /// <exception cref="ArgumentNullException">Thrown when inputService is null.</exception>
        public CartController(IInputService inputService, GraphicsDevice graphicsDevice, Vector2 cartSize)
        {
            _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            _model = new CartModel(cartSize);
            _view = new CartView(_model, graphicsDevice);
            _movementController = new CartMovementController();
        }

        /// <summary>
        /// Updates the cart's position and physics based on input and track state.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="track">The current track the cart is on.</param>
        /// <remarks>
        /// TODO: Add support for track switching
        /// </remarks>
        public void Update(GameTime gameTime, ITrack track)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update distance based on input
            _model.UpdateDistance(
                deltaTime,
                _inputService.Forward(),
                _inputService.Backward()
            );

            // Update movement controller
            _movementController.CurrentDistance = _model.CurrentDistance;
            _movementController.UpdatePosition(gameTime, track);
            _model.WheelSystem.UpdateWheelPositions(track, _model.CurrentDistance);
            _movementController.UpdateRotation(track);

            // Update model position and rotation
            _model.UpdatePosition(_movementController.Position, _movementController.Rotation);
        }

        /// <summary>
        /// Draws the cart using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <remarks>
        /// TODO: Add support for cart animations
        /// TODO: Implement proper cart rotation based on track curvature
        /// TODO: Add support for cart effects (sparks, smoke)
        /// </remarks>
        public void Draw(SpriteBatch spriteBatch)
        {
            _view.Draw(spriteBatch);
        }

        public void OnCollision(CollisionInfo info)
        {
            _model.OnCollision(info);
        }
    }
}
