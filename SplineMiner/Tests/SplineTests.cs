using Microsoft.Xna.Framework;
using SplineMiner.Core.Utils;
using Xunit;

namespace SplineMiner.Tests
{
    public class SplineTests
    {
        [Fact]
        public void CatmullRom_ShouldInterpolateBetweenMiddlePoints()
        {
            // Arrange
            Vector2 p0 = new Vector2(0, 0);
            Vector2 p1 = new Vector2(0, 0);
            Vector2 p2 = new Vector2(100, 0);
            Vector2 p3 = new Vector2(100, 0);

            // Act
            Vector2 midpoint = SplineUtils.CatmullRom(p0, p1, p2, p3, 0.5f);

            // Assert
            Assert.Equal(50, midpoint.X, 0.1f); // Allow small floating point error
            Assert.Equal(0, midpoint.Y, 0.1f);
        }

        [Fact]
        public void CatmullRom_ShouldPassThroughControlPoints()
        {
            // Arrange
            Vector2 p0 = new Vector2(0, 0);
            Vector2 p1 = new Vector2(100, 0);
            Vector2 p2 = new Vector2(100, 100);
            Vector2 p3 = new Vector2(0, 100);

            // Act & Assert
            // Should pass through p1 when t = 0
            Vector2 startPoint = SplineUtils.CatmullRom(p0, p1, p2, p3, 0f);
            Assert.Equal(p1, startPoint);

            // Should pass through p2 when t = 1
            Vector2 endPoint = SplineUtils.CatmullRom(p0, p1, p2, p3, 1f);
            Assert.Equal(p2, endPoint);
        }

        [Fact]
        public void CatmullRom_ShouldHandleZeroLengthSegments()
        {
            // Arrange
            Vector2 p = new Vector2(100, 100);

            // Act
            Vector2 result = SplineUtils.CatmullRom(p, p, p, p, 0.5f);

            // Assert
            Assert.Equal(p, result);
        }
    }
}