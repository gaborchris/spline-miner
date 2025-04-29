using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using SplineMiner.WorldGrid;
using System;
using System.Collections.Generic;

namespace SplineMiner
{
    public class DebugManager
    {
        private readonly SpriteFont _debugFont;
        private bool _showDebugInfo = true;
        private float _fps;
        private float _frameCounter;
        private float _timeSinceLastUpdate;
        private const float UPDATE_INTERVAL = 0.25f;
        
        private readonly Stopwatch _frameTimer = new Stopwatch();
        private float _actualFps;
        private float _frameTimeMs;
        
        // Reference to the world grid for statistics
        private WorldGrid.WorldGrid _worldGrid;

        public bool ShowDebugInfo
        {
            get => _showDebugInfo;
            set => _showDebugInfo = value;
        }

        public DebugManager(SpriteFont debugFont)
        {
            _debugFont = debugFont;
            _frameTimer.Start();
        }
        
        public void SetWorldGrid(WorldGrid.WorldGrid worldGrid)
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
                _actualFps = 1000.0f / _frameTimeMs;
            }
            
            // Calculate frame time
            _frameTimeMs = (float)_frameTimer.Elapsed.TotalMilliseconds;
            _frameTimer.Restart();
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, UITool currentTool)
        {
            if (!_showDebugInfo) return;

            // Create a simple debug background
            Texture2D debugBg = new Texture2D(graphicsDevice, 1, 1);
            debugBg.SetData(new[] { Color.Black * 0.7f });
            
            // Basic debug info
            var debugLines = new List<string>
            {
                "Controls:",
                "T: Test cart movement",
                "V: Visualize equally spaced points",
                "F1: Toggle debug info",
                "F3: Regenerate grid",
                "Left/Right: Move cart",
                $"Current Tool: {currentTool}",
                $"Game FPS: {_fps:F1}",
                $"Actual FPS: {_actualFps:F1}",
                $"Frame Time: {_frameTimeMs:F1} ms"
            };
            
            // Add grid stats if available
            if (_worldGrid != null)
            {
                debugLines.Add($"Grid Size: {_worldGrid.Width}x{_worldGrid.Height}");
                debugLines.Add($"Total Cells: {_worldGrid.TotalCells}");
                debugLines.Add($"Visible Cells: {_worldGrid.VisibleCells}");
                
                float visiblePercentage = _worldGrid.TotalCells > 0 
                    ? (100.0f * _worldGrid.VisibleCells / _worldGrid.TotalCells) 
                    : 0;
                debugLines.Add($"Visible: {visiblePercentage:F1}%");
            }

            // Calculate starting Y position at bottom of screen
            float yPos = graphicsDevice.Viewport.Height - (debugLines.Count * 25) - 10;
            
            foreach (string line in debugLines)
            {
                // Calculate width based on text length
                float width = _debugFont?.MeasureString(line).X ?? 250;
                width = Math.Max(width + 10, 250); // Minimum width of 250
                
                spriteBatch.Draw(debugBg, new Rectangle(10, (int)yPos, (int)width, 20), Color.White);
                
                if (_debugFont != null)
                {
                    spriteBatch.DrawString(_debugFont, line, new Vector2(15, yPos), Color.White);
                }
                
                yPos += 25;
            }
        }
    }
} 