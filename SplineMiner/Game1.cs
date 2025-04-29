using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SplineMiner.WorldGrid;
using SplineMiner.UI;

namespace SplineMiner
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private InputManager _inputManager;
        private CartController _player;
        private SplineTrack _track;
        private UIManager _uiManager;
        private DebugManager _debugManager;
        private MouseInteractionManager _mouseInteractionManager;
        private bool _useLargeTrack = false;
        
        // World grid components
        private WorldGrid.WorldGrid _worldGrid;
        private GridInteractionManager _gridInteractionManager;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Enable fixed time step for more consistent updates
            IsFixedTimeStep = true;
            _graphics.SynchronizeWithVerticalRetrace = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 60.0f); // Target 60 FPS
        }

        protected override void Initialize()
        {
            // Initialize the player at the start of the track
            _inputManager = new InputManager();
            _player = new CartController(_inputManager);

            // Initialize camera
            CameraManager.Instance.Initialize(GraphicsDevice.Viewport);
            CameraManager.Instance.SetTarget(_player);
            
            // Initialize world grid with a more reasonable size
            // (5000x500 was causing performance issues - 500x200 is more manageable)
            _worldGrid = new WorldGrid.WorldGrid(500, 200, 20);

            base.Initialize();
        }

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
            _player.LoadDebugTexture(GraphicsDevice);

            // Load fonts
            SpriteFont debugFont = null;
            try 
            {
                debugFont = Content.Load<SpriteFont>("debug_font");
            }
            catch 
            {
                Debug.WriteLine("Fonts not found. Debug text and UI will not be rendered.");
            }

            // Initialize managers
            _uiManager = new UIManager(debugFont, GraphicsDevice);
            _debugManager = new DebugManager(debugFont);
            
            // Initialize the track with test data
            InitializeTrack();
            
            // Load track textures
            _track.LoadContent(GraphicsDevice);

            // Precalculate arc length for the track
            _track.RecalculateArcLength();
            
            // Initialize world grid
            _worldGrid.Initialize(GraphicsDevice);
            _gridInteractionManager = new GridInteractionManager(_inputManager, _worldGrid);
            
            // Initialize debug panels with grid reference
            _debugManager.Initialize(GraphicsDevice, _worldGrid, _inputManager);
            
            // Set up world parameter panel event handlers
            SetupWorldParameterEvents();
        }
        
        private void SetupWorldParameterEvents()
        {
            // Get reference to the world parameter panel from debug manager
            var parameterPanel = _debugManager.GetWorldParameterPanel();
            if (parameterPanel != null)
            {
                // Set initial track size
                parameterPanel.UseLargeTrack = _useLargeTrack;
                
                // Subscribe to track size toggle event
                parameterPanel.OnTrackSizeToggle += (useLargeTrack) =>
                {
                    _useLargeTrack = useLargeTrack;
                    InitializeTrack();
                    _track.LoadContent(GraphicsDevice);
                    _track.RecalculateArcLength();
                };
            }
        }

        private void InitializeTrack()
        {
            var trackNodes = _useLargeTrack 
                ? TestData.TestTrack.GetLargeTrackNodes() 
                : TestData.TestTrack.GetSmallTrackNodes();
            
            _track = new SplineTrack(trackNodes, _uiManager);
            _mouseInteractionManager = new MouseInteractionManager(_inputManager, _track);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update input
            _inputManager.Update();
            
            // Update UI
            _uiManager.Update(_inputManager);
            
            // Update camera
            CameraManager.Instance.Update(gameTime);
            
            // Update mouse interactions
            _mouseInteractionManager.Update(_uiManager.CurrentTool);
            
            // Update grid interactions
            _gridInteractionManager.Update(_uiManager.CurrentTool);

            // Update player movement along the track
            _player.Update(gameTime, _track);

            // Update debug manager
            _debugManager.Update(gameTime);

            // Toggle debug info with F1
            if (_inputManager.IsKeyPressed(Keys.F1))
            {
                _debugManager.ShowDebugInfo = !_debugManager.ShowDebugInfo;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw game world (affected by camera)
            _spriteBatch.Begin(transformMatrix: CameraManager.Instance.Transform);
            _worldGrid.Draw(_spriteBatch); // Draw grid first (below other elements)
            _track.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);
            _spriteBatch.End();

            // Draw UI (not affected by camera)
            _spriteBatch.Begin();
            _uiManager.Draw(_spriteBatch);
            _debugManager.Draw(_spriteBatch, GraphicsDevice, _uiManager.CurrentTool);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
