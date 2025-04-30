using Microsoft.Xna.Framework;
using System.Collections.Generic;
using SplineMiner.Core.Interfaces;

namespace SplineMiner
{
    public class TrackCache : ICache<int, Vector2>, ICache<int, float>
    {
        private const int CACHE_SIZE = 1000;
        private readonly Dictionary<int, Vector2> _positionCache = new();
        private readonly Dictionary<int, float> _parameterCache = new();

        public bool TryGetValue(int key, out Vector2 value) => _positionCache.TryGetValue(key, out value);
        public bool TryGetValue(int key, out float value) => _parameterCache.TryGetValue(key, out value);

        public void SetValue(int key, Vector2 value)
        {
            if (_positionCache.Count >= CACHE_SIZE)
                _positionCache.Clear();
            _positionCache[key] = value;
        }

        public void SetValue(int key, float value)
        {
            if (_parameterCache.Count >= CACHE_SIZE)
                _parameterCache.Clear();
            _parameterCache[key] = value;
        }

        public void Clear()
        {
            _positionCache.Clear();
            _parameterCache.Clear();
        }
    }
} 