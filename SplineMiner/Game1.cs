using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Physics.Systems;
using SplineMiner.Core.Services;
using SplineMiner.Game.Cart;
using SplineMiner.Game.Track;
using SplineMiner.Game.World.WorldGrid;
using SplineMiner.Presets;

namespace SplineMiner
{
    /// <summary>
    /// Main game class that initializes and manages the core game loop and components.
    /// </summary>
    /// <remarks>
    /// TODO: Consider splitting this into smaller, more focused components:
    /// - SceneManager for handling different game states
    /// - ResourceManager for content loading
    /// - GameStateManager for managing game state transitions
    /// </remarks>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Service container for dependency injection
        private readonly IServiceContainer _services;

        private CartController _player;
        private SplineTrack _track;
        private MouseInteractionManager _mouseInteractionManager;

        private TrackPresetId _currentTrackPreset = TrackPresetId.Small;
        private readonly WorldPresetId _currentWorldPreset = WorldPresetId.Test;

        // World grid components
        private Game.World.WorldGrid.WorldGrid _worldGrid;
        private GridInteractionManager _gridInteractionManager;

        private PhysicsSystem _physicsSystem;
        private CollisionSystem _collisionSystem;

        /// <summary>
        /// Initializes a new instance of the Game1 class.
        /// </summary>
        /// <remarks>
        /// TODO: Consider implementing a proper dependency injection system
        /// TODO: Move initialization logic to separate initialization classes
        /// </remarks>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _services = new ServiceContainer(); // Initialize service container
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Enable fixed time step for more consistent updates
            IsFixedTimeStep = true;
            _graphics.SynchronizeWithVerticalRetrace = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f); // Target 60 FPS
        }

        /// <summary>
        /// Initializes the game and its components.
        /// </summary>
        /// <remarks>
        /// TODO: Consider implementing a proper dependency injection system
        /// TODO: Move initialization logic to separate initialization classes
        /// </remarks>
        protected override void Initialize()
        {
            try
            {
                // Create and register input service
                var inputManager = new InputManager();
                _services.RegisterSingleton<IInputService>(inputManager);

                // Initialize debug manager first
                var debugManager = new DebugManager(null); // Font will be set in LoadContent
                debugManager.IsDebugEnabled = true; // Explicitly enable debug mode
                _services.RegisterSingleton<IDebugService>(debugManager);

                // Verify debug service is working
                var logger = debugManager.GetLogger("GameInit");
                logger?.Log("GameInit", "Game initialization started");

                // Initialize the player at the start of the track
                _player = new CartController(inputManager, GraphicsDevice);

                // Initialize camera
                CameraManager.Instance.Initialize(GraphicsDevice.Viewport);
                CameraManager.Instance.SetTarget(_player);

                // Initialize world grid using presets
                _worldGrid = GamePresets.CreateWorldGrid(
                    _currentWorldPreset,
                    debugManager,
                    GraphicsDevice
                );

                // Initialize physics systems
                _physicsSystem = new PhysicsSystem(
                );
                _collisionSystem = new CollisionSystem(debugManager);

                // Verify collision system logger
                var collisionLogger = debugManager.GetLogger("CollisionSystem");
                collisionLogger?.Log("CollisionSystem", "Collision system initialized");

                // Add cart to physics system
                _physicsSystem.AddEntity(_player);
                _collisionSystem.AddEntity(_player);

                base.Initialize();
            }
            catch (Exception ex)
            {
                // Handle initialization errors
                Debug.WriteLine($"Error during initialization: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads game content and initializes game components.
        /// </summary>
        /// <remarks>
        /// TODO: Consider implementing a proper content management system
        /// TODO: Move content loading to a separate ContentManager class
        /// TODO: Implement proper error handling for missing content
        /// </remarks>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load player texture (placeholder: a white rectangle)
            var w = 64;
            var h = (int)(w * 0.67);
            Texture2D minecartTexture = new Texture2D(GraphicsDevice, w, h);
            Color[] data = new Color[w * h];

            // Fill the texture with a solid color (e.g., gray)
            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Gray;

            minecartTexture.SetData(data);

            // Use this texture for your player or minecart
            _player.Texture = minecartTexture;

            // Load fonts
            SpriteFont debugFont = null;
            debugFont = Content.Load<SpriteFont>("debug_font");

            // Initialize managers
            var uiManager = new UIManager(debugFont, GraphicsDevice);
            var debugManager = _services.GetService<IDebugService>() as DebugManager;
            if (debugManager != null)
            {
                debugManager.SetDebugFont(debugFont);
                debugManager.IsDebugEnabled = true; // Ensure debug is enabled

                // Verify debug is working after font setup
                var logger = debugManager.GetLogger("GameLoad");
                logger?.Log("GameLoad", "Game content loaded and debug system initialized");
            }

            // Register additional services
            _services.RegisterSingleton<IUIService>(uiManager);

            // Initialize the track with test data
            InitializeTrack();

            // Load track textures
            _track.LoadContent(GraphicsDevice);

            // Precalculate arc length for the track
            _track.RecalculateArcLength();

            // Initialize world grid
            _worldGrid.Initialize(GraphicsDevice);
            _gridInteractionManager = new GridInteractionManager(_services.GetService<IInputService>(), _worldGrid);

            // Initialize debug panels with grid reference
            debugManager?.Initialize(GraphicsDevice, _worldGrid, _services.GetService<IInputService>());

            // Set up world parameter panel event handlers
            SetupWorldParameterEvents();

            // Create and configure collision logger
            var collisionLogger = debugManager?.CreateLogger("CollisionSystem");
            if (collisionLogger != null)
            {
                collisionLogger.IsEnabled = true;
                collisionLogger.LogInterval = 0.1f; // Log every 100ms
            }
        }

        private void SetupWorldParameterEvents()
        {
            // Get reference to the world parameter panel from debug manager
            var debugManager = _services.GetService<IDebugService>();
            var parameterPanel = debugManager.GetWorldParameterPanel();
            if (parameterPanel != null)
            {
                // Set initial track size
                parameterPanel.UseLargeTrack = _currentTrackPreset == TrackPresetId.Large;

                // Subscribe to track size toggle event
                parameterPanel.OnTrackSizeToggle += (useLargeTrack) =>
                {
                    _currentTrackPreset = useLargeTrack ? TrackPresetId.Large : TrackPresetId.Small;
                    InitializeTrack();
                    _track.LoadContent(GraphicsDevice);
                    _track.RecalculateArcLength();
                };
            }
        }

        private void InitializeTrack()
        {
            // Get track nodes from presets
            var trackNodes = GamePresets.GetTrack(_currentTrackPreset);

            _track = new SplineTrack(trackNodes, _services.GetService<IUIService>());
            _mouseInteractionManager = new MouseInteractionManager(_services.GetService<IInputService>(), _track);
        }

        /// <summary>
        /// Updates game state and components.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <remarks>
        /// TODO: Consider implementing a proper update priority system
        /// TODO: Add frame rate independent updates
        /// TODO: Implement proper game state management
        /// </remarks>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Get input service from container
            var inputService = _services.GetService<IInputService>();
            var uiManager = _services.GetService<IUIService>();

            // Update input
            inputService.Update();

            // Update UI
            uiManager.Update(inputService);

            // Update camera
            CameraManager.Instance.Update(gameTime);

            // Update mouse interactions
            _mouseInteractionManager.Update(uiManager.CurrentTool);

            // Update grid interactions
            _gridInteractionManager.Update(uiManager.CurrentTool);

            // Update player movement along the track
            _player.Update(gameTime, _track);

            // Update debug manager
            var debugManager = _services.GetService<IDebugService>();
            debugManager.Update(gameTime);

            // Update player stats in debug panel
            if (debugManager is DebugManager debugManagerImpl)
            {
                debugManagerImpl.UpdatePlayerStats(
                    position: _player.Position,
                    currentDistance: _player.CurrentDistance
                );
            }

            // Toggle debug info with F1
            if (inputService.IsKeyPressed(Keys.F1))
            {
                debugManager.IsDebugEnabled = !debugManager.IsDebugEnabled;
            }

            // Get nearby blocks for collision checking
            var nearbyBlocks = _worldGrid.GetNearbyBlocks(
                _player.Position,
                radius: 128f // Adjust based on cart size and velocity
            );
            // Update physics
            _physicsSystem.Update(gameTime);

            // Clear previous blocks and add new ones for collision checking
            int count = 0;
            _collisionSystem.ClearBlocks();
            foreach (var block in nearbyBlocks)
            {
                _collisionSystem.AddBlock(block);
                count++;
            }
            //Debug.WriteLine($"Nearby: {count}");

            _collisionSystem.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game world and UI components.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <remarks>
        /// TODO: Implement proper rendering layers
        /// TODO: Add sprite batching optimization
        /// TODO: Consider implementing a proper scene graph
        /// </remarks>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw game world (affected by camera)
            _spriteBatch.Begin(transformMatrix: CameraManager.Instance.Transform);
            _worldGrid.Draw(_spriteBatch); // Draw grid first (below other elements)
            _track.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);
            _spriteBatch.End();

            var uiManager = _services.GetService<IUIService>();
            var debugManager = _services.GetService<IDebugService>();
            // Draw UI (not affected by camera)
            _spriteBatch.Begin();
            uiManager.Draw(_spriteBatch);
            debugManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
