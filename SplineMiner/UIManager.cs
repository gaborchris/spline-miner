using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SplineMiner
{
    public class UIManager
    {
        private List<UIButton> _buttons;
        private SpriteFont _font;
        private UITool _currentTool;
        private Rectangle _uiPanel;
        private float _scrollOffset;
        private const float SCROLL_SPEED = 10f;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_SPACING = 10;
        private const int PANEL_WIDTH = 150;
        private const int PANEL_PADDING = 10;

        public UITool CurrentTool => _currentTool;

        public UIManager(SpriteFont font)
        {
            _font = font;
            _buttons = new List<UIButton>();
            _uiPanel = new Rectangle(PANEL_PADDING, PANEL_PADDING, PANEL_WIDTH, 0);
            _currentTool = UITool.None;
            _scrollOffset = 0;

            InitializeButtons();
        }

        private void InitializeButtons()
        {
            int buttonY = PANEL_PADDING;
            int buttonWidth = PANEL_WIDTH - (2 * PANEL_PADDING);

            // Create buttons for each tool
            AddButton(new Rectangle(PANEL_PADDING, buttonY, buttonWidth, BUTTON_HEIGHT), 
                     "Place Track", UITool.PlaceTrack);
            buttonY += BUTTON_HEIGHT + BUTTON_SPACING;

            AddButton(new Rectangle(PANEL_PADDING, buttonY, buttonWidth, BUTTON_HEIGHT), 
                     "Edit Track", UITool.EditTrack);
            buttonY += BUTTON_HEIGHT + BUTTON_SPACING;

            AddButton(new Rectangle(PANEL_PADDING, buttonY, buttonWidth, BUTTON_HEIGHT), 
                     "Delete Track", UITool.DeleteTrack);

            // Update panel height based on buttons
            _uiPanel.Height = buttonY + BUTTON_HEIGHT + PANEL_PADDING;
        }

        private void AddButton(Rectangle bounds, string text, UITool tool)
        {
            _buttons.Add(new UIButton(bounds, text, tool, _font));
        }

        public void Update(InputManager inputManager)
        {
            // Handle mouse wheel scrolling
            if (inputManager.IsMouseWheelScrolled())
            {
                _scrollOffset += inputManager.GetMouseWheelDelta() * SCROLL_SPEED;
                _scrollOffset = MathHelper.Clamp(_scrollOffset, 0, _uiPanel.Height - _uiPanel.Y);
            }

            // Update buttons
            bool isMouseClicked = inputManager.IsLeftMousePressed();
            Vector2 mousePosition = inputManager.MousePosition;

            foreach (var button in _buttons)
            {
                button.Update(mousePosition, isMouseClicked);
                if (button.IsSelected)
                {
                    // Deselect all other buttons
                    foreach (var otherButton in _buttons)
                    {
                        if (otherButton != button)
                        {
                            otherButton.Deselect();
                        }
                    }
                    _currentTool = button.Tool;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw UI panel background
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            spriteBatch.Draw(pixel, _uiPanel, Color.Black * 0.5f);

            // Draw buttons
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
        }
    }
} 