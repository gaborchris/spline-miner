using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.UI
{
    /// <summary>
    /// A panel that displays and allows editing of world parameters.
    /// </summary>
    public class WorldParameterPanel
    {
        private readonly SpriteFont _font;
        private readonly GraphicsDevice _graphicsDevice;
        private bool _useLargeTrack;
        private Rectangle _bounds;
        private const int PANEL_WIDTH = 200;
        private const int PANEL_HEIGHT = 100;
        private const int PADDING = 10;

        public bool UseLargeTrack
        {
            get => _useLargeTrack;
            set
            {
                if (_useLargeTrack != value)
                {
                    _useLargeTrack = value;
                    OnTrackSizeToggle?.Invoke(_useLargeTrack);
                }
            }
        }

        public event System.Action<bool> OnTrackSizeToggle;

        public WorldParameterPanel(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            _graphicsDevice = graphicsDevice;
            _font = font;
            _bounds = new Rectangle(
                _graphicsDevice.Viewport.Width - PANEL_WIDTH - PADDING,
                PADDING,
                PANEL_WIDTH,
                PANEL_HEIGHT
            );
        }

        public void Update(IInputService inputService)
        {
            Vector2 mousePosition = inputService.MousePosition;
            if (_bounds.Contains(mousePosition) && inputService.IsLeftMousePressed())
            {
                UseLargeTrack = !UseLargeTrack;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string trackSizeText = $"Track Size: {(_useLargeTrack ? "Large" : "Small")}";
            Vector2 textSize = _font.MeasureString(trackSizeText);
            Vector2 textPosition = new Vector2(
                _bounds.X + (_bounds.Width - textSize.X) / 2,
                _bounds.Y + (_bounds.Height - textSize.Y) / 2
            );

            spriteBatch.DrawString(_font, trackSizeText, textPosition, Color.White);
        }
    }
} 