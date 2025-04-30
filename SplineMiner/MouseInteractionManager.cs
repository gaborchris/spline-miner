using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SplineMiner.Game.Track;

namespace SplineMiner
{
    public class MouseInteractionManager
    {
        private readonly InputManager _inputManager;
        private readonly SplineTrack _track;
        private int _hoveredPointIndex = -1;

        public int HoveredPointIndex => _hoveredPointIndex;

        public MouseInteractionManager(InputManager inputManager, SplineTrack track)
        {
            _inputManager = inputManager;
            _track = track;
        }

        public void Update(UITool currentTool)
        {
            Vector2 mousePosition = _inputManager.MousePosition;
            
            switch (currentTool)
            {
                case UITool.Track:
                    HandleTrackTool(mousePosition);
                    break;

                case UITool.DeleteTrack:
                    HandleDeleteTool(mousePosition);
                    break;
            }
            
            // Update hovered point for visual feedback
            _hoveredPointIndex = _track.GetHoveredPointIndex(mousePosition);
        }

        private void HandleTrackTool(Vector2 mousePosition)
        {
            // Handle right-click drag for editing shadow nodes
            if (_inputManager.IsRightMousePressed())
            {
                int pointIndex = _track.GetHoveredPointIndex(mousePosition);
                if (pointIndex != -1)
                {
                    _track.SelectPoint(pointIndex);
                }
            }
            else if (_inputManager.IsRightMouseHeld())
            {
                _track.MoveSelectedPoint(mousePosition);
            }
            else if (_inputManager.IsRightMouseReleased())
            {
                _track.ReleaseSelectedPoint();
            }
            // Handle left-click for placing new points
            else if (_inputManager.IsLeftMousePressed())
            {
                if (!_track.IsHoveringEndpoint)
                {
                    _track.PlaceNode(mousePosition);
                }
            }
        }

        private void HandleDeleteTool(Vector2 mousePosition)
        {
            if (_inputManager.IsLeftMousePressed())
            {
                int pointIndex = _track.GetHoveredPointIndex(mousePosition);
                if (pointIndex != -1)
                {
                    _track.DeletePoint(pointIndex);
                }
            }
        }
    }
} 