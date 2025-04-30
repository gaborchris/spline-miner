namespace SplineMiner.Core.Interfaces
{
    /// <summary>
    /// Provides a generic caching mechanism for key-value pairs.
    /// This interface is used to optimize performance by caching frequently accessed values.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key.</typeparam>
    /// <typeparam name="TValue">The type of the cached value.</typeparam>
    public interface ICache<TKey, TValue>
    {
        /// <summary>
        /// Attempts to get a value from the cache.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if the value was found in the cache.</returns>
        bool TryGetValue(TKey key, out TValue value);

        /// <summary>
        /// Sets a value in the cache.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The value to store.</param>
        void SetValue(TKey key, TValue value);

        /// <summary>
        /// Clears all values from the cache.
        /// </summary>
        void Clear();
    }
} 