using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for input handling services.
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// Gets the current mouse position.
        /// </summary>
        Vector2 MousePosition { get; }

        /// <summary>
        /// Updates the input state.
        /// </summary>
        void Update();

        /// <summary>
        /// Checks if a key was just pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was just pressed, false otherwise.</returns>
        bool IsKeyPressed(Keys key);

        /// <summary>
        /// Checks if a key is being held down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is being held, false otherwise.</returns>
        bool IsKeyHeld(Keys key);

        /// <summary>
        /// Checks if the left mouse button is currently pressed.
        /// </summary>
        /// <returns>True if the left mouse button is pressed, false otherwise.</returns>
        bool IsLeftMousePressed();

        /// <summary>
        /// Checks if the left mouse button is being held down.
        /// </summary>
        /// <returns>True if the left mouse button is being held, false otherwise.</returns>
        bool IsLeftMouseHeld();

        /// <summary>
        /// Checks if the right mouse button is currently pressed.
        /// </summary>
        /// <returns>True if the right mouse button is pressed, false otherwise.</returns>
        bool IsRightMousePressed();

        /// <summary>
        /// Checks if the right mouse button is being held down.
        /// </summary>
        /// <returns>True if the right mouse button is being held, false otherwise.</returns>
        bool IsRightMouseHeld();

        /// <summary>
        /// Checks if the right mouse button was just released.
        /// </summary>
        /// <returns>True if the right mouse button was just released, false otherwise.</returns>
        bool IsRightMouseReleased();

        /// <summary>
        /// Checks if the mouse wheel was scrolled.
        /// </summary>
        /// <returns>True if the mouse wheel was scrolled, false otherwise.</returns>
        bool IsMouseWheelScrolled();

        /// <summary>
        /// Gets the mouse wheel scroll delta.
        /// </summary>
        /// <returns>The mouse wheel scroll delta.</returns>
        float GetMouseWheelDelta();
    }
} 