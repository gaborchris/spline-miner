namespace SplineMiner.Core.Enums
{
    /// <summary>
    /// Defines the different types of blocks that can exist in the world.
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// A solid block that cannot be destroyed.
        /// </summary>
        Solid,

        /// <summary>
        /// A destructible block that can be destroyed.
        /// </summary>
        Destructible,

        /// <summary>
        /// A track block that can be used for the cart to move on.
        /// </summary>
        Track,

        /// <summary>
        /// A special block that triggers events or effects.
        /// </summary>
        Special
    }
}