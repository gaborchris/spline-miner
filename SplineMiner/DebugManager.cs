using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SplineMiner.UI;
using SplineMiner.WorldGrid;
using System;
using System.Collections.Generic;

namespace SplineMiner
{
    public class DebugManager
    {
        private readonly SpriteFont _debugFont;
        private bool _showDebugInfo = true;
        
        // UI panels
        private StatsPanel _statsPanel;
        private WorldParameterPanel _worldParameterPanel;
        
        public bool ShowDebugInfo
        {
            get => _showDebugInfo;
            set
            {
                _showDebugInfo = value;
                if (_statsPanel != null) _statsPanel.IsVisible = value;
                if (_worldParameterPanel != null) _worldParameterPanel.IsVisible = value;
            }
        }
        
        /// <summary>
        /// Gets the WorldParameterPanel for external configuration
        /// </summary>
        public WorldParameterPanel GetWorldParameterPanel()
        {
            return _worldParameterPanel;
        }

        public DebugManager(SpriteFont debugFont)
        {
            _debugFont = debugFont;
        }
        
        public void Initialize(GraphicsDevice graphicsDevice, WorldGrid.WorldGrid worldGrid, InputManager inputManager)
        {
            // Initialize panels
            _statsPanel = new StatsPanel(_debugFont, graphicsDevice);
            _worldParameterPanel = new WorldParameterPanel(worldGrid, inputManager, _debugFont, graphicsDevice);
            
            // Set references
            _statsPanel.SetWorldGrid(worldGrid);
            
            // Set initial visibility
            _statsPanel.IsVisible = _showDebugInfo;
            _worldParameterPanel.IsVisible = _showDebugInfo;
        }

        public void Update(GameTime gameTime)
        {
            if (!_showDebugInfo) return;
            
            // Update panels
            _statsPanel?.Update(gameTime);
            _worldParameterPanel?.Update();
            
            // Toggle between panels with F4
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                TogglePanels();
            }
        }
        
        private void TogglePanels()
        {
            // If both are visible, hide world parameter panel
            if (_statsPanel.IsVisible && _worldParameterPanel.IsVisible)
            {
                _worldParameterPanel.IsVisible = false;
            }
            // If only stats panel is visible, hide stats and show parameters
            else if (_statsPanel.IsVisible)
            {
                _statsPanel.IsVisible = false;
                _worldParameterPanel.IsVisible = true;
            }
            // If only parameters panel is visible, show both
            else if (_worldParameterPanel.IsVisible)
            {
                _statsPanel.IsVisible = true;
            }
            // If both are hidden, show stats panel
            else
            {
                _statsPanel.IsVisible = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, UITool currentTool)
        {
            if (!_showDebugInfo) return;
            
            // Draw panels
            _statsPanel?.Draw(spriteBatch);
            _worldParameterPanel?.Draw(spriteBatch);
            
        }
    }
} 