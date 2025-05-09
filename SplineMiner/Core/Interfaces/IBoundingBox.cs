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
        /// Checks if this bounding box intersects with another.
        /// </summary>
        /// <param name="other">The other bounding box to check against.</param>
        /// <returns>True if the boxes intersect, false otherwise.</returns>
        bool Intersects(IBoundingBox other);
    }
}