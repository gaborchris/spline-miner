using Microsoft.Xna.Framework;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Calculates spline-based track geometry. This interface provides methods for
    /// computing points along a spline and calculating arc lengths.
    /// </summary>
    public interface ISplineCalculator
    {
        /// <summary>
        /// Gets a point on the spline at the specified parameter value.
        /// </summary>
        /// <param name="t">The parameter value along the spline (0 to n-1).</param>
        /// <returns>The point on the spline at parameter t.</returns>
        Vector2 GetPoint(float t);

        /// <summary>
        /// Gets the parameter value corresponding to a specific distance along the spline.
        /// </summary>
        /// <param name="distance">The distance along the spline.</param>
        /// <param name="totalLength">The total length of the spline.</param>
        /// <returns>The parameter value corresponding to the distance.</returns>
        float GetParameterForDistance(float distance, float totalLength);

        /// <summary>
        /// Computes the arc length of a segment of the spline.
        /// </summary>
        /// <param name="tStart">The starting parameter value.</param>
        /// <param name="tEnd">The ending parameter value.</param>
        /// <param name="baseSteps">The number of steps to use for the calculation.</param>
        /// <returns>The arc length of the specified segment.</returns>
        float ComputeArcLength(float tStart, float tEnd, int baseSteps = 40);
    }
} 