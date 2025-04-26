using Microsoft.Xna.Framework.Input;

namespace SplineMiner
{
    public class InputManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
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
    }
}
