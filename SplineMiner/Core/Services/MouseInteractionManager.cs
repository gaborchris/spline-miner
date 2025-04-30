using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;
using SplineMiner.Game.Track;
using SplineMiner.Game.Items.Tools;

namespace SplineMiner.Core.Services
{
    public class MouseInteractionManager
    {
        private readonly IInputService _inputService;
        private readonly SplineTrack _track;
        private Vector2 _lastMousePosition;
        private bool _isDragging;

        public MouseInteractionManager(IInputService inputService, SplineTrack track)
        {
            _inputService = inputService;
            _track = track;
            _lastMousePosition = Vector2.Zero;
            _isDragging = false;
        }

        public void Update(UITool currentTool)
        {
            Vector2 mousePosition = _inputService.MousePosition;

            switch (currentTool)
            {
                case UITool.Track:
                    HandleTrackTool(mousePosition);
                    break;
                case UITool.Select:
                    HandleSelectTool(mousePosition);
                    break;
                case UITool.DeleteTrack:
                    HandleDeleteTool(mousePosition);
                    break;
            }

            _lastMousePosition = mousePosition;
        }

        private void HandleTrackTool(Vector2 mousePosition)
        {
            if (_inputService.IsRightMousePressed())
            {
                int pointIndex = _track.GetHoveredPointIndex(mousePosition);
                if (pointIndex != -1)
                {
                    _track.SelectPoint(pointIndex);
                }
            }
            else if (_inputService.IsRightMouseHeld())
            {
                _track.MoveSelectedPoint(mousePosition);
            }
            else if (_inputService.IsRightMouseReleased())
            {
                _track.ReleaseSelectedPoint();
            }
            else if (_inputService.IsLeftMousePressed())
            {
                if (!_track.IsHoveringEndpoint)
                {
                    _track.PlaceNode(mousePosition);
                }
            }
        }

        private void HandleSelectTool(Vector2 mousePosition)
        {
            if (_inputService.IsRightMousePressed())
            {
                // Start selection
                _isDragging = true;
            }
            else if (_inputService.IsRightMouseHeld())
            {
                // Continue selection
                if (_isDragging)
                {
                    // Update selection area
                }
            }
            else if (_inputService.IsRightMouseReleased())
            {
                // End selection
                _isDragging = false;
            }
            else if (_inputService.IsLeftMousePressed())
            {
                // Select single point
                var pointIndex = _track.GetHoveredPointIndex(mousePosition);
                if (pointIndex != -1)
                {
                    // Handle point selection
                    _track.SelectPoint(pointIndex);
                }
            }
        }

        private void HandleDeleteTool(Vector2 mousePosition)
        {
            if (_inputService.IsLeftMousePressed())
            {
                var pointIndex = _track.GetHoveredPointIndex(mousePosition);
                if (pointIndex != -1)
                {
                    // Delete the point
                    _track.DeletePoint(pointIndex);
                }
            }
        }
    }
}