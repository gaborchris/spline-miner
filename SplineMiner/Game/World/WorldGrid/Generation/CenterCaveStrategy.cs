using System;
using Microsoft.Xna.Framework;

namespace SplineMiner.Game.World.WorldGrid.Generation
{
    /// <summary>
    /// A simple strategy that creates more caves toward the center of the world, with a gradient outward
    /// </summary>
    public class CenterCaveStrategy : IWorldGenerationStrategy
    {
        public string Name => "Center Cave";

        public string Description => "Creates more caves toward the center, with solid walls at the edges.";

        public bool ShouldBeActive(
            int x, int y,
            float worldX, float worldY,
            int width, int height,
            float worldWidth, float worldHeight,
            Random random,
            GenerationParameters parameters)
        {
            // Calculate distance to the center of the world
            Vector2 center = new Vector2(0, 0);
            Vector2 position = new Vector2(worldX, worldY);
            float distanceToCenter = Vector2.Distance(position, center);

            // Calculate maximum possible distance
            float maxDistance = MathF.Sqrt(worldWidth * worldWidth + worldHeight * worldHeight) / 2;
            float normalizedDistance = distanceToCenter / maxDistance;

            // Invert so that center has more caves
            float caveFactor = 1.0f - normalizedDistance;

            // Higher probability of caverns in the center, less at the edges
            return random.NextDouble() > parameters.CaveProbability + caveFactor * 0.3f;
        }
    }
}