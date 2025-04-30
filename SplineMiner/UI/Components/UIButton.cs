using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SplineMiner.Game.Items.Tools;

namespace SplineMiner.UI.Components
{
    public class UIButton(Rectangle bounds, string text, UITool tool, SpriteFont font, Texture2D texture = null)
    {
        private bool _isHovered;
        private bool _isSelected;
        private Color _normalColor = Color.Gray * 0.7f;
        private Color _hoverColor = Color.LightGray * 0.7f;
        private Color _selectedColor = Color.White * 0.7f;
        private Color _textColor = Color.Black;

        public UITool Tool => tool;
        public bool IsSelected => _isSelected;


        // Note: this does not unselect other buttons
        // This means than other logic can check for tool selection without
        // this button overriding the selection
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

        public void Select()
        {
            _isSelected = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color buttonColor = _isSelected ? _selectedColor : _isHovered ? _hoverColor : _normalColor;

            // Draw button background
            Texture2D pixel = new(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            spriteBatch.Draw(pixel, bounds, buttonColor);

            // Draw texture if available
            if (texture != null)
            {
                Rectangle textureBounds = new(
                    bounds.X + bounds.Width / 4,
                    bounds.Y + bounds.Height / 4,
                    bounds.Width / 2,
                    bounds.Height / 2
                );
                spriteBatch.Draw(texture, textureBounds, Color.White);
            }
            else
            {
                // Draw button text if no texture
                Vector2 textSize = font.MeasureString(text);
                Vector2 textPosition = new(
                    bounds.X + (bounds.Width - textSize.X) / 2,
                    bounds.Y + (bounds.Height - textSize.Y) / 2
                );
                spriteBatch.DrawString(font, text, textPosition, _textColor);
            }
        }
    }
}