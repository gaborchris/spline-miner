using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace SplineMiner
{
    public class UIManager
    {
        private List<UIButton> _buttons;
        private SpriteFont _font;
        private UITool _currentTool;
        private Rectangle _uiPanel;
        private float _scrollOffset;
        private const float SCROLL_SPEED = 1f;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_SPACING = 10;
        private const int PANEL_WIDTH = 150;
        private const int PANEL_PADDING = 10;
        private const int TOTAL_BUTTONS = 8;
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

            // Create 10 buttons (3 real tools + 7 placeholders)
            AddButton(new Rectangle(PANEL_PADDING, buttonY, buttonWidth, BUTTON_HEIGHT), 
                     "Place Track", UITool.PlaceTrack);
            buttonY += BUTTON_HEIGHT + BUTTON_SPACING;

            AddButton(new Rectangle(PANEL_PADDING, buttonY, buttonWidth, BUTTON_HEIGHT), 
                     "Edit Track", UITool.EditTrack);
            buttonY += BUTTON_HEIGHT + BUTTON_SPACING;

            AddButton(new Rectangle(PANEL_PADDING, buttonY, buttonWidth, BUTTON_HEIGHT), 
                     "Delete Track", UITool.DeleteTrack);
            buttonY += BUTTON_HEIGHT + BUTTON_SPACING;

            // Add 7 placeholder buttons
            for (int i = 0; i < TOTAL_BUTTONS - 3; i++)
            {
                AddButton(new Rectangle(PANEL_PADDING, buttonY, buttonWidth, BUTTON_HEIGHT), 
                         $"Tool {i + 4}", UITool.None);
                buttonY += BUTTON_HEIGHT + BUTTON_SPACING;
            }

            // Update panel height based on visible buttons
            _uiPanel.Height = (BUTTON_HEIGHT + BUTTON_SPACING) * TOTAL_BUTTONS + PANEL_PADDING;
            SetToolIndex(0);
        }

        private void AddButton(Rectangle bounds, string text, UITool tool)
        {
            _buttons.Add(new UIButton(bounds, text, tool, _font));
        }

        private void SetToolIndex(int index) {
            _scrollOffset = index;
            _buttons[index].Select();
            _currentTool = _buttons[index].Tool;
        }

        public void Update(InputManager inputManager)
        {
            // Handle mouse wheel scrolling with precise increments
            if (inputManager.IsMouseWheelScrolled())
            {
                float delta = inputManager.GetMouseWheelDelta();
                float newOffset = Math.Clamp(_scrollOffset + delta, 0, TOTAL_BUTTONS);
                Debug.WriteLine($"Scroll delta: {delta}");
                Debug.WriteLine($"Scroll offset: {_scrollOffset}");
                
                // Only update if the offset actually changed
                if (newOffset != _scrollOffset)
                {
                    _scrollOffset = newOffset;
                    // Deselect all buttons
                    foreach (var button in _buttons)
                    {
                        button.Deselect();
                    }
                    // Select the button at the current offset
                    int selectedIndex = (int)_scrollOffset;
                    if (selectedIndex < _buttons.Count)
                    {
                        _buttons[selectedIndex].Select();
                        _currentTool = _buttons[selectedIndex].Tool;
                    }
                }
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

            for (int i = 0; i < _buttons.Count; i++)
            {
                var button = _buttons[i];
                button.Draw(spriteBatch);
            }
        }
    }
} 