using System;
using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Game.Cart
{
    // Core interfaces
    public class CartMovementController : IMovementController
    {
        private const float MIN_MOVEMENT_THRESHOLD = 0.1f;
        private const float POSITION_INTERPOLATION_FACTOR = 0.5f;
        private const float MAX_ROTATION_CHANGE = MathHelper.Pi / 6;

        private Vector2 _position;
        private Vector2 _previousPosition;
        private Vector2 _targetPosition;
        private float _rotation;
        private float _lastRotation;
        private float _rotationChange;
        private float _t = 0f;
        private float _speed = 300;

        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }
        public float CurrentDistance
        {
            get => _t;
            set => _t = value;
        }

        public void UpdatePosition(GameTime gameTime, ITrack track)
        {
            _previousPosition = _position;

            // Update target position
            _targetPosition = track.GetPointByDistance(_t);

            // Calculate movement vector
            Vector2 movement = _targetPosition - _previousPosition;
            float movementLength = movement.Length();

            // Only update position if movement is significant
            if (movementLength > MIN_MOVEMENT_THRESHOLD)
            {
                movement.Normalize();
                _position = _previousPosition + movement * (movementLength * POSITION_INTERPOLATION_FACTOR);
            }
            else
            {
                _position = _previousPosition;
            }
        }

        public void UpdateRotation(ITrack track)
        {
            float targetRotation = track.GetRotationAtDistance(_t);
            _rotationChange = MathHelper.WrapAngle(targetRotation - _lastRotation);

            if (Math.Abs(_rotationChange) > 0.01f)
            {
                if (Math.Abs(_rotationChange) > MAX_ROTATION_CHANGE)
                {
                    targetRotation = _lastRotation + Math.Sign(_rotationChange) * MAX_ROTATION_CHANGE;
                }

                _rotation = targetRotation;
                _lastRotation = _rotation;
            }
        }
    }
}
