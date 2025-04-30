using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using SplineMiner.Game.Items.Tools;

namespace SplineMiner.Game.World.WorldGrid
{
    /// <summary>
    /// Handles mouse interactions with the world grid
    /// </summary>
    public class GridInteractionManager
    {
        private readonly IInputService _inputService;
        private readonly WorldGrid _worldGrid;
        private Vector2 _lastMousePosition;
        private bool _isDragging;

        public GridInteractionManager(IInputService inputService, WorldGrid worldGrid)
        {
            _inputService = inputService;
            _worldGrid = worldGrid;
            _lastMousePosition = Vector2.Zero;
            _isDragging = false;
        }

        public void Update(UITool currentTool)
        {
            Vector2 mousePosition = _inputService.MousePosition;

            if (_inputService.IsLeftMousePressed() || _inputService.IsLeftMouseHeld())
            {
                Vector2 screenPos = _inputService.MousePosition;
                // Handle grid interaction based on current tool
                switch (currentTool)
                {
                    case UITool.Destroy:
                        HandleDestroyTool(screenPos);
                        break;
                }
            }

            _lastMousePosition = mousePosition;
        }

        private void HandleDestroyTool(Vector2 screenPos)
        {
            if (_inputService.IsLeftMousePressed() || _inputService.IsLeftMouseHeld())
            {
                // Delete cell at position
                _worldGrid.DeleteCell(screenPos);
            }
        }
    }
}