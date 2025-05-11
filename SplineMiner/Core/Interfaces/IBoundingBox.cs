using Microsoft.Xna.Framework;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for a bounding box used in collision detection.
    /// </summary>
    public interface IBoundingBox
    {
        /// <summary>
        /// Gets the position of the bounding box.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the size of the bounding box.
        /// </summary>
        Vector2 Size { get; }

        /// <summary>
        /// Gets the center of the bounding box.
        /// </summary>
        Vector2 Center { get; }

        /// <summary>
        /// Gets the left edge of the bounding box.
        /// </summary>
        float Left { get; }

        /// <summary>
        /// Gets the right edge of the bounding box.
        /// </summary>
        float Right { get; }

        /// <summary>
        /// Gets the top edge of the bounding box.
        /// </summary>
        float Top { get; }

        /// <summary>
        /// Gets the bottom edge of the bounding box.
        /// </summary>
        float Bottom { get; }

        /// <summary>
        /// Gets the corners of the bounding box.
        /// </summary>
        /// <returns>An array of 4 Vector2 points representing the corners of the box in clockwise order starting from top-left.</returns>
        Vector2[] GetCorners();

    }
}