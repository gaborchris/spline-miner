using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SplineMiner.WorldGrid;
using SplineMiner.WorldGrid.Generation;
using System;
using System.Collections.Generic;

namespace SplineMiner.UI
{
    /// <summary>
    /// A panel that provides interactive controls for adjusting WorldGrid parameters
    /// </summary>
    public class WorldParameterPanel
    {
        // World grid reference
        private readonly WorldGrid.WorldGrid _worldGrid;
        private readonly InputManager _inputManager;
        
        // UI elements
        private readonly SpriteFont _font;
        private Texture2D _pixelTexture;
        private List<Slider> _sliders = new List<Slider>();
        private Button _regenerateButton;
        private Button _trackSizeToggleButton;
        private Dropdown _strategyDropdown;
        
        // Panel properties
        private Rectangle _panelBounds;
        private bool _isVisible = true;
        private const int PADDING = 10;
        private const int SLIDER_HEIGHT = 20;
        private const int SLIDER_SPACING = 45; // Increased spacing between sliders
        private const int BUTTON_HEIGHT = 30;
        private const int BUTTON_SPACING = 15;
        private const int SECTION_SPACING = 30; // Space between sections
        private const int DROPDOWN_HEIGHT = 30;
        
        // World parameters with default values
        private float _caveProbability = 0.45f;
        private int _gridWidth = 500;
        private int _gridHeight = 200;
        private float _cellSize = 20f;
        private bool _useLargeTrack = false;
        
        // Events for external handlers
        public event Action OnGridRegenerate;
        public event Action<bool> OnTrackSizeToggle;
        
        public bool IsVisible
        {
            get => _isVisible;
            set => _isVisible = value;
        }
        
        public float CaveProbability => _caveProbability;
        public int GridWidth => _gridWidth;
        public int GridHeight => _gridHeight;
        public float CellSize => _cellSize;
        public bool UseLargeTrack
        {
            get => _useLargeTrack;
            set 
            { 
                _useLargeTrack = value;
                if (_trackSizeToggleButton != null)
                {
                    _trackSizeToggleButton.Label = _useLargeTrack ? "Use Small Track" : "Use Large Track";
                }
            }
        }

        public WorldParameterPanel(WorldGrid.WorldGrid worldGrid, InputManager inputManager, SpriteFont font, GraphicsDevice graphicsDevice)
        {
            _worldGrid = worldGrid;
            _inputManager = inputManager;
            _font = font;
            
            // Create the pixel texture for drawing rectangles
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
            
            // Set up the panel bounds (right side of the screen)
            _panelBounds = new Rectangle(
                graphicsDevice.Viewport.Width - 250 - PADDING,
                PADDING,
                250,
                graphicsDevice.Viewport.Height - PADDING * 2
            );
            
            // Initialize UI elements
            InitializeControls();
        }
        
        private void InitializeControls()
        {
            if (_font == null) return;
            
            int y = _panelBounds.Y + PADDING;
            
            // Title space
            string title = "World Parameters";
            Vector2 titleSize = _font.MeasureString(title);
            y += (int)titleSize.Y + 15;
            
            // Generation Strategy section
            y = AddSectionHeader("Generation Strategy", y);
            
            // Strategy dropdown - get strategy names from world grid
            List<string> strategyNames = new List<string>();
            foreach (var strategy in _worldGrid.AvailableStrategies)
            {
                strategyNames.Add(strategy.Name);
            }
            
            // Extra space after section header
            y += 30;
            
            // Create strategy dropdown
            _strategyDropdown = new Dropdown(
                new Rectangle(_panelBounds.X + PADDING, y, _panelBounds.Width - PADDING * 2, DROPDOWN_HEIGHT),
                strategyNames,
                "Generation Strategy",
                OnStrategySelected
            );
            y += DROPDOWN_HEIGHT + BUTTON_SPACING;
            
            // Grid Parameters section header
            y = AddSectionHeader("Grid Parameters", y);
            
            // Extra space after section header before first slider
            y += 60; // Make room for slider label above slider
            
            // Cave probability slider (0.0 to 1.0)
            _sliders.Add(new Slider(
                new Rectangle(_panelBounds.X + PADDING, y, _panelBounds.Width - PADDING * 2, SLIDER_HEIGHT),
                "Cave Density",
                0f, 1f, _caveProbability,
                (value) => _caveProbability = value
            ));
            y += SLIDER_SPACING;
            
            // Grid width slider (50 to 1000)
            _sliders.Add(new Slider(
                new Rectangle(_panelBounds.X + PADDING, y, _panelBounds.Width - PADDING * 2, SLIDER_HEIGHT),
                "Grid Width",
                50, 1000, _gridWidth,
                (value) => _gridWidth = (int)value
            ));
            y += SLIDER_SPACING;
            
            // Grid height slider (50 to 500)
            _sliders.Add(new Slider(
                new Rectangle(_panelBounds.X + PADDING, y, _panelBounds.Width - PADDING * 2, SLIDER_HEIGHT),
                "Grid Height",
                50, 500, _gridHeight,
                (value) => _gridHeight = (int)value
            ));
            y += SLIDER_SPACING;
            
            // Cell size slider (5 to 50)
            _sliders.Add(new Slider(
                new Rectangle(_panelBounds.X + PADDING, y, _panelBounds.Width - PADDING * 2, SLIDER_HEIGHT),
                "Cell Size",
                5f, 50f, _cellSize,
                (value) => _cellSize = value
            ));
            y += SLIDER_SPACING;
            
            // Regenerate button
            _regenerateButton = new Button(
                new Rectangle(_panelBounds.X + PADDING, y, _panelBounds.Width - PADDING * 2, BUTTON_HEIGHT),
                "Regenerate World"
            );
            y += BUTTON_HEIGHT + BUTTON_SPACING + SECTION_SPACING;
            
            // Track Controls section
            // y = AddSectionHeader("Track Controls", y);
            
            // Extra space after section header
            // y += 20;
            
            // Track size toggle button
            _trackSizeToggleButton = new Button(
                new Rectangle(_panelBounds.X + PADDING, y, _panelBounds.Width - PADDING * 2, BUTTON_HEIGHT),
                _useLargeTrack ? "Use Small Track" : "Use Large Track"
            );
        }
        
        private void OnStrategySelected(int index)
        {
            if (_worldGrid != null)
            {
                _worldGrid.SetGenerationStrategy(index);
            }
        }
        
        private int AddSectionHeader(string title, int yPosition)
        {
            // Just add space for the header - actual drawing happens in Draw method
            return yPosition;
        }
        
        public void CheckForKeyboardShortcuts()
        {
            // Handle F3 key for grid regeneration
            if (_inputManager.IsKeyPressed(Keys.F3))
            {
                RegenerateWorld();
            }
            
            // Handle F2 key for track size toggle
            if (_inputManager.IsKeyPressed(Keys.F2))
            {
                ToggleTrackSize();
            }
        }
        
        public void Update()
        {
            if (!_isVisible) return;
            
            // Handle keyboard shortcuts even if the panel is visible
            CheckForKeyboardShortcuts();
            
            // Update all sliders
            bool isMousePressed = _inputManager.IsLeftMousePressed();
            bool isMouseHeld = _inputManager.IsLeftMouseHeld();
            Vector2 mousePosition = _inputManager.MousePosition;
            
            // Update strategy dropdown
            _strategyDropdown.Update(mousePosition, isMousePressed, isMouseHeld);
            
            foreach (var slider in _sliders)
            {
                slider.Update(mousePosition, isMousePressed, isMouseHeld);
            }
            
            // Update regenerate button
            _regenerateButton.Update(mousePosition, isMousePressed);
            
            // If regenerate button was clicked, create a new world with current parameters
            if (_regenerateButton.WasClicked)
            {
                RegenerateWorld();
                _regenerateButton.Reset();
            }
            
            // Update track size toggle button
            _trackSizeToggleButton.Update(mousePosition, isMousePressed);
            
            // If track size toggle button was clicked, toggle track size
            if (_trackSizeToggleButton.WasClicked)
            {
                ToggleTrackSize();
                _trackSizeToggleButton.Reset();
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isVisible || _font == null) return;
            
            // Draw panel background
            spriteBatch.Draw(_pixelTexture, _panelBounds, Color.Black * 0.7f);
            
            // Draw panel title
            string title = "World Parameters";
            Vector2 titleSize = _font.MeasureString(title);
            float y = _panelBounds.Y + PADDING;
            
            spriteBatch.DrawString(_font, title, 
                new Vector2(_panelBounds.X + (_panelBounds.Width - titleSize.X) / 2, y),
                Color.White);
            
            y += titleSize.Y + 15;
            
            // Draw section headers
            y = DrawSectionHeader(spriteBatch, "Generation Strategy", y);
            
            // Spacing for dropdown
            float gridParamsY = y + DROPDOWN_HEIGHT + BUTTON_SPACING + SECTION_SPACING;
            
            // Draw grid parameters section
            // gridParamsY = DrawSectionHeader(spriteBatch, "Grid Parameters", gridParamsY);
            
            // Calculate position for next section based on slider count
            float trackSectionY = gridParamsY +  SLIDER_SPACING * _sliders.Count + BUTTON_HEIGHT + BUTTON_SPACING + SECTION_SPACING;
            
            // Draw Track Controls section
            trackSectionY = DrawSectionHeader(spriteBatch, "Track Controls", trackSectionY);
            
            // Draw dropdown button (but not the dropdown list yet)
            _strategyDropdown.DrawButton(spriteBatch, _pixelTexture, _font);
            
            // Draw all sliders
            foreach (var slider in _sliders)
            {
                slider.Draw(spriteBatch, _pixelTexture, _font);
            }
            
            // Draw buttons
            _regenerateButton.Draw(spriteBatch, _pixelTexture, _font);
            _trackSizeToggleButton.Draw(spriteBatch, _pixelTexture, _font);
            
            // Draw dropdown list LAST so it appears on top of everything else
            _strategyDropdown.DrawDropdownList(spriteBatch, _pixelTexture, _font);
        }
        
        private float DrawSectionHeader(SpriteBatch spriteBatch, string headerText, float yPosition)
        {
            Vector2 textSize = _font.MeasureString(headerText);
            spriteBatch.DrawString(_font, headerText, 
                new Vector2(_panelBounds.X + PADDING, yPosition),
                Color.Yellow);
            
            // Draw a line under the header
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle(_panelBounds.X + PADDING, (int)(yPosition + textSize.Y + 2), 
                    _panelBounds.Width - PADDING * 2, 1), 
                Color.Yellow);
            
            return yPosition + textSize.Y + 10;
        }
        
        public void RegenerateWorld()
        {
            // Create a new world grid with current parameters
            try
            {
                // We'll reuse the same world grid object but update its parameters
                _worldGrid.UpdateParameters(_gridWidth, _gridHeight, _cellSize, _caveProbability);
                _worldGrid.GenerateGrid();
                
                // Notify any listeners
                OnGridRegenerate?.Invoke();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error regenerating world: {ex.Message}");
            }
        }
        
        public void ToggleTrackSize()
        {
            _useLargeTrack = !_useLargeTrack;
            _trackSizeToggleButton.Label = _useLargeTrack ? "Use Small Track" : "Use Large Track";
            
            // Notify any listeners
            OnTrackSizeToggle?.Invoke(_useLargeTrack);
        }
        
        #region UI Control Classes
        
        private class Slider
        {
            public Rectangle Bounds { get; }
            public string Label { get; }
            public float MinValue { get; }
            public float MaxValue { get; }
            public float Value { get; private set; }
            
            private Rectangle _trackBounds;
            private Rectangle _thumbBounds;
            private readonly Action<float> _onValueChanged;
            private bool _isDragging = false;
            
            public Slider(Rectangle bounds, string label, float minValue, float maxValue, float initialValue, Action<float> onValueChanged)
            {
                Bounds = bounds;
                Label = label;
                MinValue = minValue;
                MaxValue = maxValue;
                Value = initialValue;
                _onValueChanged = onValueChanged;
                
                // Calculate track and thumb bounds
                UpdateBounds();
            }
            
            private void UpdateBounds()
            {
                // Track is the full width of the slider
                _trackBounds = new Rectangle(
                    Bounds.X,
                    Bounds.Y + Bounds.Height / 2 - 2,
                    Bounds.Width,
                    4
                );
                
                // Thumb position based on current value
                float valuePercent = (Value - MinValue) / (MaxValue - MinValue);
                int thumbX = (int)(Bounds.X + valuePercent * Bounds.Width - 5);
                _thumbBounds = new Rectangle(
                    thumbX,
                    Bounds.Y,
                    10,
                    Bounds.Height
                );
            }
            
            public void Update(Vector2 mousePosition, bool isMousePressed, bool isMouseHeld)
            {
                // Start dragging when the thumb is clicked
                if (isMousePressed && _thumbBounds.Contains(mousePosition))
                {
                    _isDragging = true;
                }
                
                // Stop dragging when mouse is released
                if (!isMouseHeld)
                {
                    _isDragging = false;
                }
                
                // Update value while dragging
                if (_isDragging)
                {
                    float percent = MathHelper.Clamp((mousePosition.X - Bounds.X) / Bounds.Width, 0, 1);
                    Value = MinValue + percent * (MaxValue - MinValue);
                    UpdateBounds();
                    _onValueChanged?.Invoke(Value);
                }
            }
            
            public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
            {
                // Draw label (now with more space and better positioning)
                if (font != null)
                {
                    Vector2 labelSize = font.MeasureString(Label);
                    spriteBatch.DrawString(font, Label, 
                        new Vector2(Bounds.X, Bounds.Y - labelSize.Y - 2), // Added extra spacing
                        Color.White);
                    
                    // Draw value
                    string valueText = Value.ToString("F2");
                    Vector2 valueSize = font.MeasureString(valueText);
                    spriteBatch.DrawString(font, valueText, 
                        new Vector2(Bounds.X + Bounds.Width - valueSize.X, Bounds.Y - valueSize.Y - 2), // Added extra spacing
                        Color.Yellow);
                }
                
                // Draw track
                spriteBatch.Draw(pixelTexture, _trackBounds, Color.Gray);
                
                // Draw thumb
                spriteBatch.Draw(pixelTexture, _thumbBounds, Color.White);
            }
        }
        
        private class Button
        {
            public Rectangle Bounds { get; }
            public string Label { get; set; }
            public bool WasClicked { get; private set; }
            private bool _isHovered;
            
            public Button(Rectangle bounds, string label)
            {
                Bounds = bounds;
                Label = label;
            }
            
            public void Update(Vector2 mousePosition, bool isMousePressed)
            {
                _isHovered = Bounds.Contains(mousePosition);
                WasClicked = _isHovered && isMousePressed;
            }
            
            public void Reset()
            {
                WasClicked = false;
            }
            
            public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
            {
                // Draw button background
                Color bgColor = _isHovered ? Color.Gray : Color.DarkGray;
                spriteBatch.Draw(pixelTexture, Bounds, bgColor);
                
                // Draw button border
                int borderThickness = 2;
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderThickness), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - borderThickness, Bounds.Width, borderThickness), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y, borderThickness, Bounds.Height), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(Bounds.X + Bounds.Width - borderThickness, Bounds.Y, borderThickness, Bounds.Height), 
                    Color.White);
                
                // Draw label
                if (font != null)
                {
                    Vector2 labelSize = font.MeasureString(Label);
                    spriteBatch.DrawString(font, Label, 
                        new Vector2(Bounds.X + (Bounds.Width - labelSize.X) / 2, Bounds.Y + (Bounds.Height - labelSize.Y) / 2),
                        Color.White);
                }
            }
        }
        
        private class Dropdown
        {
            public Rectangle Bounds { get; }
            public string Label { get; }
            private readonly List<string> _items;
            private readonly Action<int> _onItemSelected;
            
            private int _selectedIndex = 0;
            private bool _isOpen = false;
            private Rectangle _dropdownListBounds;
            private const int ITEM_HEIGHT = 25;
            
            public Dropdown(Rectangle bounds, List<string> items, string label, Action<int> onItemSelected)
            {
                Bounds = bounds;
                _items = items;
                Label = label;
                _onItemSelected = onItemSelected;
                
                // Calculate dropdown list bounds
                _dropdownListBounds = new Rectangle(
                    bounds.X,
                    bounds.Y + bounds.Height,
                    bounds.Width,
                    ITEM_HEIGHT * items.Count
                );
            }
            
            public void Update(Vector2 mousePosition, bool isMousePressed, bool isMouseHeld)
            {
                // Toggle dropdown open/closed when clicked
                if (isMousePressed && Bounds.Contains(mousePosition))
                {
                    _isOpen = !_isOpen;
                }
                
                // Process item selection if dropdown is open
                if (_isOpen && isMousePressed)
                {
                    for (int i = 0; i < _items.Count; i++)
                    {
                        Rectangle itemBounds = new Rectangle(
                            _dropdownListBounds.X,
                            _dropdownListBounds.Y + i * ITEM_HEIGHT,
                            _dropdownListBounds.Width,
                            ITEM_HEIGHT
                        );
                        
                        if (itemBounds.Contains(mousePosition))
                        {
                            _selectedIndex = i;
                            _isOpen = false;
                            
                            // Notify of selection change
                            _onItemSelected?.Invoke(_selectedIndex);
                            break;
                        }
                    }
                }
                
                // Close dropdown if clicked outside
                if (isMousePressed && _isOpen && !_dropdownListBounds.Contains(mousePosition) && !Bounds.Contains(mousePosition))
                {
                    _isOpen = false;
                }
            }
            
            public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
            {
                // Draw the button portion only
                DrawButton(spriteBatch, pixelTexture, font);
            }
            
            public void DrawButton(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
            {
                // Draw dropdown button
                Color bgColor = _isOpen ? Color.Gray : Color.DarkGray;
                spriteBatch.Draw(pixelTexture, Bounds, bgColor);
                
                // Draw button border
                int borderThickness = 2;
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, borderThickness), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - borderThickness, Bounds.Width, borderThickness), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(Bounds.X, Bounds.Y, borderThickness, Bounds.Height), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(Bounds.X + Bounds.Width - borderThickness, Bounds.Y, borderThickness, Bounds.Height), 
                    Color.White);
                
                // Draw selected item
                if (font != null && _selectedIndex >= 0 && _selectedIndex < _items.Count)
                {
                    string selectedText = _items[_selectedIndex];
                    Vector2 textSize = font.MeasureString(selectedText);
                    spriteBatch.DrawString(font, selectedText, 
                        new Vector2(Bounds.X + 10, Bounds.Y + (Bounds.Height - textSize.Y) / 2),
                        Color.White);
                    
                    // Draw dropdown arrow
                    string arrow = _isOpen ? "<" : ">";
                    Vector2 arrowSize = font.MeasureString(arrow);
                    spriteBatch.DrawString(font, arrow, 
                        new Vector2(Bounds.X + Bounds.Width - arrowSize.X - 10, Bounds.Y + (Bounds.Height - arrowSize.Y) / 2),
                        Color.White);
                }
            }
            
            public void DrawDropdownList(SpriteBatch spriteBatch, Texture2D pixelTexture, SpriteFont font)
            {
                if (!_isOpen) return;
                
                int borderThickness = 2;
                
                // Draw list background
                spriteBatch.Draw(pixelTexture, _dropdownListBounds, Color.DarkGray * 0.9f);
                
                // Draw list border
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(_dropdownListBounds.X, _dropdownListBounds.Y, _dropdownListBounds.Width, borderThickness), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(_dropdownListBounds.X, _dropdownListBounds.Y + _dropdownListBounds.Height - borderThickness, _dropdownListBounds.Width, borderThickness), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(_dropdownListBounds.X, _dropdownListBounds.Y, borderThickness, _dropdownListBounds.Height), 
                    Color.White);
                spriteBatch.Draw(pixelTexture, 
                    new Rectangle(_dropdownListBounds.X + _dropdownListBounds.Width - borderThickness, _dropdownListBounds.Y, borderThickness, _dropdownListBounds.Height), 
                    Color.White);
                
                // Draw list items
                if (font != null)
                {
                    for (int i = 0; i < _items.Count; i++)
                    {
                        Rectangle itemBounds = new Rectangle(
                            _dropdownListBounds.X,
                            _dropdownListBounds.Y + i * ITEM_HEIGHT,
                            _dropdownListBounds.Width,
                            ITEM_HEIGHT
                        );
                        
                        // Draw item background (highlight if selected)
                        Color itemBgColor = (i == _selectedIndex) ? Color.SlateGray : Color.Transparent;
                        spriteBatch.Draw(pixelTexture, itemBounds, itemBgColor);
                        
                        // Draw item text
                        spriteBatch.DrawString(font, _items[i], 
                            new Vector2(itemBounds.X + 10, itemBounds.Y + (itemBounds.Height - font.LineSpacing) / 2),
                            Color.White);
                    }
                }
            }
            
            public bool IsOpen => _isOpen;
        }
        
        #endregion
    }
} 