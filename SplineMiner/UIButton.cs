using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SplineMiner
{
    public class UIButton(Rectangle bounds, string text, UITool tool, SpriteFont font)
    {
        private bool _isHovered;
        private bool _isSelected;
        private Color _normalColor = Color.Gray;
        private Color _hoverColor = Color.LightGray;
        private Color _selectedColor = Color.White;
        private Color _textColor = Color.Black;

        public UITool Tool => tool;
        public bool IsSelected => _isSelected;

        public void Select()
        {
            _isSelected = true;
        }

        public void Update(Vector2 mousePosition, bool isMouseClicked)
        {
            _isHovered = bounds.Contains(mousePosition);
            if (_isHovered && isMouseClicked)
            {
                _isSelected = true;
            }
        }

        public void Deselect()
        {
            _isSelected = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color buttonColor = _isSelected ? _selectedColor : (_isHovered ? _hoverColor : _normalColor);
            
            // Draw button background
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            spriteBatch.Draw(pixel, bounds, buttonColor);

            // Draw button text
            Vector2 textSize = font.MeasureString(text);
            Vector2 textPosition = new Vector2(
                bounds.X + (bounds.Width - textSize.X) / 2,
                bounds.Y + (bounds.Height - textSize.Y) / 2
            );
            spriteBatch.DrawString(font, text, textPosition, _textColor);
        }
    }
} 