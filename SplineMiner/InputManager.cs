using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SplineMiner
{
    public class InputManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public Vector2 MousePosition => new Vector2(_currentMouseState.X, _currentMouseState.Y);
        public Vector2 PreviousMousePosition => new Vector2(_previousMouseState.X, _previousMouseState.Y);

        public void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
        }

        public bool Forward()
        {
            return IsKeyHeld(Keys.D) || IsKeyHeld(Keys.Right);
        }

        public bool Backward()
        {
            return IsKeyHeld(Keys.A) || IsKeyHeld(Keys.Left);
        }

        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyHeld(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyReleased(Keys key)
        {
            return !_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key);
        }
        
        public bool IsLeftMousePressed()
        {
            return _currentMouseState.LeftButton == ButtonState.Pressed && 
                   _previousMouseState.LeftButton == ButtonState.Released;
        }
        
        public bool IsLeftMouseReleased()
        {
            return _currentMouseState.LeftButton == ButtonState.Released && 
                   _previousMouseState.LeftButton == ButtonState.Pressed;
        }
        
        public bool IsLeftMouseHeld()
        {
            return _currentMouseState.LeftButton == ButtonState.Pressed;
        }
    }
}
