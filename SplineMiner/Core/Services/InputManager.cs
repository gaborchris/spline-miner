using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SplineMiner.Core.Services
{
    /// <summary>
    /// Manages and processes all user input for the game.
    /// </summary>
    /// <remarks>
    /// TODO: Implement input remapping system
    /// TODO: Implement input buffering
    /// TODO: Add support for input macros
    /// TODO: Implement proper input state management
    /// </remarks>
    public class InputManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public Vector2 MousePosition => new Vector2(_currentMouseState.X, _currentMouseState.Y);
        public Vector2 PreviousMousePosition => new Vector2(_previousMouseState.X, _previousMouseState.Y);

        /// <summary>
        /// Initializes a new instance of the InputManager.
        /// </summary>
        /// <remarks>
        /// TODO: Add support for input configuration loading
        /// TODO: Implement input device detection
        /// </remarks>
        public InputManager()
        {
            // ... existing code ...
        }

        /// <summary>
        /// Updates the current input state.
        /// </summary>
        /// <remarks>
        /// TODO: Add support for input polling rate configuration
        /// TODO: Implement input event system
        /// </remarks>
        public void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Checks if a key was just pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was just pressed, false otherwise.</returns>
        /// <remarks>
        /// TODO: Add support for key combinations
        /// TODO: Implement key press duration tracking
        /// </remarks>
        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if the forward movement key is pressed.
        /// </summary>
        /// <returns>True if forward movement is requested, false otherwise.</returns>
        /// <remarks>
        /// TODO: Add support for analog input
        /// TODO: Implement input smoothing
        /// </remarks>
        public bool Forward()
        {
            return IsKeyHeld(Keys.D) || IsKeyHeld(Keys.Right);
        }

        /// <summary>
        /// Checks if the backward movement key is pressed.
        /// </summary>
        /// <returns>True if backward movement is requested, false otherwise.</returns>
        /// <remarks>
        /// TODO: Add support for analog input
        /// TODO: Implement input smoothing
        /// </remarks>
        public bool Backward()
        {
            return IsKeyHeld(Keys.A) || IsKeyHeld(Keys.Left);
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

        public bool IsRightMousePressed()
        {
            return _currentMouseState.RightButton == ButtonState.Pressed &&
                   _previousMouseState.RightButton == ButtonState.Released;
        }

        public bool IsRightMouseReleased()
        {
            return _currentMouseState.RightButton == ButtonState.Released &&
                   _previousMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsRightMouseHeld()
        {
            return _currentMouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsMouseWheelScrolled()
        {
            return _currentMouseState.ScrollWheelValue != _previousMouseState.ScrollWheelValue;
        }

        public float GetMouseWheelDelta()
        {
            return (_currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue) / 120f;
        }
    }
}
