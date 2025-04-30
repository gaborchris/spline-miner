using Microsoft.Xna.Framework;
using SplineMiner.Core.Services;
using SplineMiner.Game.Cart;
using SplineMiner.Game.Items.Tools;

namespace SplineMiner.Game.World.WorldGrid
{
    /// <summary>
    /// Handles mouse interactions with the world grid
    /// </summary>
    public class GridInteractionManager
    {
        private readonly InputManager _inputManager;
        private readonly WorldGrid _worldGrid;

        public GridInteractionManager(InputManager inputManager, WorldGrid worldGrid)
        {
            _inputManager = inputManager;
            _worldGrid = worldGrid;
        }

        public void Update(UITool currentTool)
        {
            // Only allow interaction with grid when using the appropriate tool
            if (currentTool != UITool.Destroy) return;

            // Check for both initial click and held-down mouse state
            if (_inputManager.IsLeftMousePressed() || _inputManager.IsLeftMouseHeld())
            {
                // Convert screen position to world position
                Vector2 screenPos = _inputManager.MousePosition;
                Vector2 worldPos = CameraManager.Instance.ScreenToWorld(screenPos);

                // Try to destroy a cell at the clicked position
                _worldGrid.DestroyCell(worldPos);
            }
        }
    }
}