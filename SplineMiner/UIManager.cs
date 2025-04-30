using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace SplineMiner
{
    /// <summary>
    /// Manages the game's user interface elements and interactions.
    /// </summary>
    /// <remarks>
    /// TODO: Implement proper UI layout system
    /// TODO: Add support for UI themes and styling
    /// TODO: Implement UI animation system
    /// TODO: Add support for localization
    /// TODO: Implement proper UI event system
    /// </remarks>
    public class UIManager
    {
        private readonly List<UIButton> _buttons;
        private readonly SpriteFont _font;
        private UITool _currentTool;
        private float _scrollOffset;
        private const int BUTTON_SIZE = 50;
        private const int BUTTON_SPACING = 10;
        private const int PANEL_PADDING = 10;
        private const int TOTAL_BUTTONS = 8;
        private Texture2D _redCircle;
        private Texture2D _greenCircle;
        private readonly SpriteFont _debugFont;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Dictionary<string, UIButton> _buttonMap;

        /// <summary>
        /// Initializes a new instance of the UIManager.
        /// </summary>
        /// <param name="debugFont">The font used for debug text.</param>
        /// <param name="graphicsDevice">The graphics device used for rendering.</param>
        /// <exception cref="ArgumentNullException">Thrown when debugFont or graphicsDevice is null.</exception>
        /// <remarks>
        /// TODO: Implement proper UI initialization system
        /// TODO: Add support for UI configuration loading
        /// </remarks>
        public UIManager(SpriteFont debugFont, GraphicsDevice graphicsDevice)
        {
            _font = debugFont;
            _buttons = [];
            _currentTool = UITool.None;
            _scrollOffset = 0;
            _debugFont = debugFont;
            _graphicsDevice = graphicsDevice;
            _buttonMap = new();

            // Create colored circle textures
            CreateCircleTextures(graphicsDevice);
            InitializeButtons();
        }

        private void CreateCircleTextures(GraphicsDevice graphicsDevice)
        {
            int size = BUTTON_SIZE / 2;
            _redCircle = CreateCircleTexture(graphicsDevice, size, Color.Red);
            _greenCircle = CreateCircleTexture(graphicsDevice, size, Color.Green);
        }

        private static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color)
        {
            int diameter = radius * 2;
            Texture2D texture = new(graphicsDevice, diameter, diameter);
            Color[] data = new Color[diameter * diameter];

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    int index = y * diameter + x;
                    Vector2 pos = new(x - radius, y - radius);
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
                Rectangle bounds = new(buttonX, buttonY, BUTTON_SIZE, BUTTON_SIZE);
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
                    case 2:
                        // New tool for destroying grid cells
                        texture = _redCircle;
                        text = "Destroy";
                        tool = UITool.Destroy;
                        break;
                }

                UIButton button = new UIButton(bounds, text, tool, _font, texture);
                _buttons.Add(button);
                _buttonMap[text] = button;
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

        /// <summary>
        /// Updates the UI state and handles input.
        /// </summary>
        /// <param name="inputManager">The input manager for handling user input.</param>
        /// <remarks>
        /// TODO: Implement proper UI update priority system
        /// TODO: Add support for UI focus management
        /// </remarks>
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

        /// <summary>
        /// Draws the UI elements using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <remarks>
        /// TODO: Implement proper UI rendering layers
        /// TODO: Add support for UI effects
        /// TODO: Implement UI batching optimization
        /// </remarks>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Gets or sets the current UI tool.
        /// </summary>
        /// <remarks>
        /// TODO: Implement proper tool switching system
        /// TODO: Add support for tool combinations
        /// </remarks>
        public UITool CurrentTool
        {
            get => _currentTool;
            set => _currentTool = value;
        }
    }
} 