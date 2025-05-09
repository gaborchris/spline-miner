using Microsoft.Xna.Framework;
using SplineMiner.Core.Enums;

namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for blocks that can exist in the world.
    /// </summary>
    public interface IWorldBlock
    {
        /// <summary>
        /// Gets the bounding box of the block.
        /// </summary>
        Rectangle BoundingBox { get; }

        /// <summary>
        /// Gets whether the block is destructible.
        /// </summary>
        bool IsDestructible { get; }

        /// <summary>
        /// Gets the type of the block.
        /// </summary>
        BlockType BlockType { get; }

        /// <summary>
        /// Gets the position of the block.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the size of the block.
        /// </summary>
        Vector2 Size { get; }

        /// <summary>
        /// Destroys the block.
        /// </summary>
        void Destroy();
    }
}