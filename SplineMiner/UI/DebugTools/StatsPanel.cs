using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SplineMiner.UI.DebugTools
{
    /// <summary>
    /// A panel that displays game statistics and performance metrics
    /// </summary>
    public class StatsPanel
    {
        private readonly SpriteFont _font;
        private Texture2D _pixelTexture;
        private Rectangle _panelBounds;
        private bool _isVisible = true;
        private const int PADDING = 10;

        // Performance tracking
        private readonly Stopwatch _frameTimer = new Stopwatch();
        private float _fps;
        private float _actualFps;
        private float _frameTimeMs;
        private float _frameCounter;
        private float _timeSinceLastUpdate;
        private const float UPDATE_INTERVAL = 0.25f;

        // References to game objects for stats
        private Game.World.WorldGrid.WorldGrid _worldGrid;

        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }

        public StatsPanel(SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _font = font;

            // Create pixel texture for drawing rectangles
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            // Set up panel bounds (left side of screen)
            _panelBounds = new Rectangle(
                PADDING,
                graphicsDevice.Viewport.Height - 250,
                250,
                240
            );

            _frameTimer.Start();
        }

        public void SetWorldGrid(Game.World.WorldGrid.WorldGrid worldGrid)
        {
            _worldGrid = worldGrid;
        }

        public void Update(GameTime gameTime)
        {
            _timeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter++;

            if (_timeSinceLastUpdate >= UPDATE_INTERVAL)
            {
                _fps = _frameCounter / _timeSinceLastUpdate;
                _frameCounter = 0;
                _timeSinceLastUpdate = 0;

                // Calculate actual FPS based on frame time
                _actualFps = 1000.0f / Math.Max(_frameTimeMs, 0.001f);
            }

            // Calculate frame time
            _frameTimeMs = (float)_frameTimer.Elapsed.TotalMilliseconds;
            _frameTimer.Restart();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible) return;

            // Draw panel background
            spriteBatch.Draw(_pixelTexture, _panelBounds, Color.Black * 0.7f);

            // Prepare stats lines
            var statLines = new List<string>
            {
                "Statistics",
                "----------",
                $"FPS: {_fps:F1}",
                $"Actual FPS: {_actualFps:F1}",
                $"Frame Time: {_frameTimeMs:F1} ms"
            };

            // Add grid stats if available
            if (_worldGrid != null)
            {
                statLines.Add(string.Empty);
                statLines.Add("World Grid");
                statLines.Add("----------");
                statLines.Add($"Grid Size: {_worldGrid.Width}x{_worldGrid.Height}");
                statLines.Add($"Cell Size: {_worldGrid.CellSize:F1}");
                statLines.Add($"Total Cells: {_worldGrid.TotalCells}");
                statLines.Add($"Visible Cells: {_worldGrid.VisibleCells}");

                float visiblePercentage = _worldGrid.TotalCells > 0
                    ? 100.0f * _worldGrid.VisibleCells / _worldGrid.TotalCells
                    : 0;
                statLines.Add($"Visible: {visiblePercentage:F1}%");
            }

            // Draw all stat lines
            if (_font != null)
            {
                float y = _panelBounds.Y + PADDING;
                foreach (string line in statLines)
                {
                    spriteBatch.DrawString(_font, line, new Vector2(_panelBounds.X + PADDING, y), Color.White);
                    y += _font.LineSpacing;
                }
            }
        }
    }
}