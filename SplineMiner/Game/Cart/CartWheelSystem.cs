using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Game.Cart

{
    // Wheel system implementation
    public class CartWheelSystem : IWheelSystem
    {
        private const float WHEEL_DISTANCE = 30f;
        private Vector2 _frontWheelPosition;
        private Vector2 _backWheelPosition;

        public Vector2 FrontWheelPosition => _frontWheelPosition;
        public Vector2 BackWheelPosition => _backWheelPosition;

        public void UpdateWheelPositions(ITrack track, float currentDistance)
        {
            float frontDistance = currentDistance + WHEEL_DISTANCE;
            float backDistance = currentDistance - WHEEL_DISTANCE;

            if (frontDistance > track.TotalArcLength)
                frontDistance -= track.TotalArcLength;
            if (backDistance < 0)
                backDistance += track.TotalArcLength;

            _frontWheelPosition = track.GetPointByDistance(frontDistance);
            _backWheelPosition = track.GetPointByDistance(backDistance);
        }
    }
}
