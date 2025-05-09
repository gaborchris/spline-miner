using System;
using Microsoft.Xna.Framework;

namespace SplineMiner.Game.World.WorldGrid.Generation
{
    /// <summary>
    /// Interface for world generation strategies using the Strategy pattern.
    /// Each implementation provides a different algorithm for procedural world generation.
    /// </summary>
    public interface IWorldGenerationStrategy
    {
        /// <summary>
        /// Gets the name of the generation strategy for UI display
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a description of the generation algorithm for UI display
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Determines if a cell at the given position should be active (solid) or inactive (empty)
        /// </summary>
        /// <param name="x">Grid X position</param>
        /// <param name="y">Grid Y position</param>
        /// <param name="worldX">World X position</param>
        /// <param name="worldY">World Y position</param>
        /// <param name="width">Total grid width</param>
        /// <param name="height">Total grid height</param>
        /// <param name="worldWidth">Total world width</param>
        /// <param name="worldHeight">Total world height</param>
        /// <param name="random">Random number generator instance</param>
        /// <param name="parameters">Generation parameters like cave density</param>
        /// <returns>True if the cell should be active (solid), false if it should be inactive (empty)</returns>
        bool ShouldBeActive(
            int x, int y,
            float worldX, float worldY,
            int width, int height,
            float worldWidth, float worldHeight,
            Random random,
            GenerationParameters parameters);
    }

    /// <summary>
    /// Parameters for controlling procedural generation
    /// </summary>
    public class GenerationParameters
    {
        // Common parameters across generation strategies
        public float CaveProbability { get; set; } = 0.45f;
        public int Seed { get; set; } = 0;
        public float CellSize { get; set; } = 20f;

        // Parameters for specific strategies
        public int CellularAutomataIterations { get; set; } = 4;
        public float PerlinNoiseScale { get; set; } = 0.1f;
        public int DrunkWalkSteps { get; set; } = 1000;
        public float DrunkWalkTurnChance { get; set; } = 0.3f;

        // Constructor
        public GenerationParameters(float caveProbability = 0.45f, int seed = 0, float cellSize = 20f)
        {
            CaveProbability = caveProbability;
            Seed = seed;
            CellSize = cellSize;
        }
    }
}