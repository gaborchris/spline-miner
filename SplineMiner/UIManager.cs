using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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
        private const int BUTTON_SIZE = 50;
        private const int BUTTON_SPACING = 10;
        private const int PANEL_PADDING = 10;
        private const int TOTAL_BUTTONS = 8;
        private Texture2D _redCircle;
        private Texture2D _greenCircle;
        private Texture2D _blueCircle;

        public UITool CurrentTool => _currentTool;

        public UIManager(SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _font = font;
            _buttons = new List<UIButton>();
            _uiPanel = new Rectangle(PANEL_PADDING, PANEL_PADDING, 
                                   TOTAL_BUTTONS * (BUTTON_SIZE + BUTTON_SPACING) + PANEL_PADDING,
                                   BUTTON_SIZE + (2 * PANEL_PADDING));
            _currentTool = UITool.None;
            _scrollOffset = 0;

            // Create colored circle textures
            CreateCircleTextures(graphicsDevice);
            InitializeButtons();
        }

        private void CreateCircleTextures(GraphicsDevice graphicsDevice)
        {
            int size = BUTTON_SIZE / 2;
            _redCircle = CreateCircleTexture(graphicsDevice, size, Color.Red);
            _greenCircle = CreateCircleTexture(graphicsDevice, size, Color.Green);
            _blueCircle = CreateCircleTexture(graphicsDevice, size, Color.Blue);
        }

        private Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color)
        {
            int diameter = radius * 2;
            Texture2D texture = new Texture2D(graphicsDevice, diameter, diameter);
            Color[] data = new Color[diameter * diameter];

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    int index = y * diameter + x;
                    Vector2 pos = new Vector2(x - radius, y - radius);
                    if (pos.Length() <= radius)
                    {
                        data[index] = color;
                    }
                    else
                    {
                        data[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(data);
            return texture;
        }

        private void InitializeButtons()
        {
            int buttonX = PANEL_PADDING;
            int buttonY = PANEL_PADDING;

            // Create buttons in a single row
            for (int i = 0; i < TOTAL_BUTTONS; i++)
            {
                Rectangle bounds = new Rectangle(buttonX, buttonY, BUTTON_SIZE, BUTTON_SIZE);
                Texture2D? texture = null;
                string text = $"Tool {i + 1}";
                UITool tool = UITool.None;

                // Assign textures and tools to first two buttons
                switch (i)
                {
                    case 0:
                        texture = _greenCircle;
                        text = "Track";
                        tool = UITool.Track;
                        break;
                    case 1:
                        texture = _redCircle;
                        text = "Delete";
                        tool = UITool.DeleteTrack;
                        break;
                }

                _buttons.Add(new UIButton(bounds, text, tool, _font, texture));
                buttonX += BUTTON_SIZE + BUTTON_SPACING;
            }

            SetToolIndex(0);
        }

        private void SetToolIndex(int index)
        {
            DeselectAllButtons();
            _scrollOffset = index;
            _buttons[index].Select();
            _currentTool = _buttons[index].Tool;
        }

        private void DeselectAllButtons()
        {
            foreach (var button in _buttons)
            {
                    button.Deselect();
            }
        }

        public void Update(InputManager inputManager)
        {
            // Handle mouse wheel scrolling with precise increments
            if (inputManager.IsMouseWheelScrolled())
            {
                float delta = inputManager.GetMouseWheelDelta();
                float newOffset = Math.Clamp(_scrollOffset + delta, 0, TOTAL_BUTTONS - 1);
                if (newOffset != _scrollOffset)
                {
                    SetToolIndex((int)newOffset);
                }
            }

            // Update buttons
            bool isMouseClicked = inputManager.IsLeftMousePressed();
            Vector2 mousePosition = inputManager.MousePosition;
            for (int i = 0; i < _buttons.Count; i++) {
                _buttons[i].Update(mousePosition, isMouseClicked);
                if (_buttons[i].IsSelected)
                {
                    SetToolIndex(i);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
        }
    }
} 