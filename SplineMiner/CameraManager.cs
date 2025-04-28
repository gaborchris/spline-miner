using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SplineMiner
{
    public class CameraManager
    {
        private static CameraManager _instance;
        private Vector2 _position;
        private float _zoom = 1.0f;
        private Matrix _transform;
        private ICameraObserver _target;
        private float _smoothSpeed = 0.1f;
        private Viewport _viewport;

        public static CameraManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CameraManager();
                }
                return _instance;
            }
        }

        public Matrix Transform => _transform;
        public Vector2 Position => _position;
        public float Zoom => _zoom;

        private CameraManager()
        {
            _position = Vector2.Zero;
            UpdateTransform();
        }

        public void Initialize(Viewport viewport)
        {
            _viewport = viewport;
        }

        public void SetTarget(ICameraObserver target)
        {
            _target = target;
        }

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