using Microsoft.Xna.Framework;

namespace SplineMiner.Core.Utils
{
    public static class SplineUtils
    {
        /// <summary>
        /// Calculates a point on a Catmull-Rom spline.
        /// </summary>
        /// <param name="p0">First control point</param>
        /// <param name="p1">Second control point</param>
        /// <param name="p2">Third control point</param>
        /// <param name="p3">Fourth control point</param>
        /// <param name="t">Parameter value (0 to 1)</param>
        /// <returns>Interpolated point on the spline</returns>
        public static Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                2 * p1 +
                (-p0 + p2) * t +
                (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                (-p0 + 3 * p1 - 3 * p2 + p3) * t3
            );
        }
    }
}