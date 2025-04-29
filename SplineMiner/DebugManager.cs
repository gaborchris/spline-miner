using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

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

        public bool ShowDebugInfo
        {
            get => _showDebugInfo;
            set => _showDebugInfo = value;
        }

        public DebugManager(SpriteFont debugFont)
        {
            _debugFont = debugFont;
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
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, UITool currentTool)
        {
            if (!_showDebugInfo) return;

            // Create a simple debug background
            Texture2D debugBg = new Texture2D(graphicsDevice, 1, 1);
            debugBg.SetData(new[] { Color.Black * 0.7f });
            
            string[] debugInfo = {
                "Controls:",
                "T: Test cart movement",
                "V: Visualize equally spaced points",
                "F1: Toggle debug info",
                "Left/Right: Move cart",
                $"Current Tool: {currentTool}",
                $"FPS: {_fps:F5}"
            };

            // Calculate starting Y position at bottom of screen
            float yPos = graphicsDevice.Viewport.Height - (debugInfo.Length * 25) - 10;
            
            foreach (string line in debugInfo)
            {
                spriteBatch.Draw(debugBg, new Rectangle(10, (int)yPos, 250, 20), Color.White);
                
                if (_debugFont != null)
                {
                    spriteBatch.DrawString(_debugFont, line, new Vector2(15, yPos), Color.White);
                }
                
                yPos += 25;
            }
        }
    }
} 