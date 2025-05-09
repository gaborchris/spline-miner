using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Core.Services
{
    /// <summary>
    /// Manages the game camera, including following targets and viewport transformations.
    /// </summary>
    /// <remarks>
    /// TODO: Implement camera smoothing and interpolation
    /// TODO: Add support for camera shake effects
    /// TODO: Implement camera zoom functionality
    /// TODO: Add support for multiple camera modes
    /// TODO: Implement camera constraints and boundaries
    /// </remarks>
    public class CameraManager
    {
        private static CameraManager _instance;
        private Vector2 _position;
        private readonly float _zoom = 0.8f;
        private Matrix _transform;
        private ICameraObserver _target;
        private readonly float _smoothSpeed = 0.1f;
        private Viewport _viewport;

        /// <summary>
        /// Gets the singleton instance of the CameraManager.
        /// </summary>
        /// <remarks>
        /// TODO: Consider implementing dependency injection instead of singleton
        /// TODO: Add support for multiple cameras
        /// </remarks>
        public static CameraManager Instance
        {
            get
            {
                _instance ??= new CameraManager();
                return _instance;
            }
        }

        /// <summary>
        /// Gets the camera's transformation matrix.
        /// </summary>
        /// <remarks>
        /// TODO: Add support for custom transformation matrices
        /// TODO: Implement proper matrix caching
        /// </remarks>
        public Matrix Transform => _transform;
        public Vector2 Position => _position;
        public float Zoom => _zoom;
        public Viewport Viewport => _viewport;

        private CameraManager()
        {
            _position = Vector2.Zero;
            UpdateTransform();
        }

        /// <summary>
        /// Initializes the camera with the specified viewport.
        /// </summary>
        /// <param name="viewport">The game viewport.</param>
        /// <remarks>
        /// TODO: Add support for viewport resizing
        /// TODO: Implement proper aspect ratio handling
        /// </remarks>
        public void Initialize(Viewport viewport)
        {
            _viewport = viewport;
        }

        /// <summary>
        /// Sets the camera's target to follow.
        /// </summary>
        /// <param name="target">The target to follow.</param>
        /// <remarks>
        /// TODO: Implement target switching with smooth transitions
        /// TODO: Add support for target offset
        /// </remarks>
        public void SetTarget(ICameraObserver target)
        {
            _target = target;
        }

        /// <summary>
        /// Updates the camera's position and transformation matrix.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <remarks>
        /// TODO: Implement proper camera interpolation
        /// TODO: Add support for camera effects
        /// </remarks>
        public void Update(GameTime gameTime)
        {
            if (_target != null)
            {
                // Smoothly follow the target
                Vector2 targetPosition = _target.Position;
                _position = Vector2.Lerp(_position, targetPosition, _smoothSpeed);
            }

            UpdateTransform();
        }

        private void UpdateTransform()
        {
            _transform = Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0)) *
                        Matrix.CreateScale(_zoom) *
                        Matrix.CreateTranslation(new Vector3(_viewport.Width / 2, _viewport.Height / 2, 0));
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(_transform));
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, _transform);
        }
    }
}