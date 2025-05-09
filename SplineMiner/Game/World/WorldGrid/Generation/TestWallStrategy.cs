using System;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Game.World.WorldGrid.Generation
{
    /// <summary>
    /// Generation strategy that creates a test wall for collision testing
    /// </summary>
    public class TestWallStrategy : IWorldGenerationStrategy
    {
        private const float CELL_SIZE = 20f;  // Constant cell size
        private readonly IDebugService _debugService;
        private bool[,] _wallMap;  // Cache for wall positions

        public TestWallStrategy(IDebugService debugService = null)
        {
            _debugService = debugService;
            _debugService?.GetLogger("TestWall")?.Log("TestWall", "TestWallStrategy initialized");
        }

        public string Name => "Test Wall";
        public string Description => "Generates a simple wall for collision testing";

        public bool ShouldBeActive(
            int x, int y,
            float worldX, float worldY,
            int width, int height,
            float worldWidth, float worldHeight,
            Random random,
            GenerationParameters parameters)
        {
            // Initialize wall map if first cell or different size
            if (_wallMap == null || _wallMap.GetLength(0) != width || _wallMap.GetLength(1) != height)
            {
                GenerateWall(width, height);
            }

            // Return true if this cell should be a wall
            return _wallMap[x, y];
        }

        private void GenerateWall(int width, int height)
        {
            _wallMap = new bool[width, height];

            // For a 5x5 grid, we want all cells to be walls
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _wallMap[x, y] = true;
                }
            }
        }
    }
}