using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SplineMiner.Game.World.WorldGrid.Generation
{
    /// <summary>
    /// Generates caves using Cellular Automata, similar to Conway's Game of Life.
    /// First randomly fills cells, then applies CA rules for several iterations to form natural-looking caverns.
    /// </summary>
    public class CellularAutomataStrategy : IWorldGenerationStrategy
    {
        public string Name => "Cellular Automata";

        public string Description => "Creates natural-looking cave systems using cellular automata rules.";

        // Cache for storing the grid state during CA iterations
        private bool[,] _initialGrid;
        private bool[,] _tempGrid;

        public bool ShouldBeActive(
            int x, int y,
            float worldX, float worldY,
            int width, int height,
            float worldWidth, float worldHeight,
            Random random,
            GenerationParameters parameters)
        {
            // Initialize caches if first cell or different size
            if (_initialGrid == null || _initialGrid.GetLength(0) != width || _initialGrid.GetLength(1) != height)
            {
                InitializeGrids(width, height, random, parameters);
                RunCellularAutomata(width, height, parameters.CellularAutomataIterations);
            }

            // Return the final state after CA iterations
            return _tempGrid[x, y];
        }

        private void InitializeGrids(int width, int height, Random random, GenerationParameters parameters)
        {
            _initialGrid = new bool[width, height];
            _tempGrid = new bool[width, height];

            // Initialize with random cells based on cave probability
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Edge cells are always solid
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        _initialGrid[x, y] = true;
                    }
                    else
                    {
                        // Random fill based on cave probability
                        // Note: We invert the logic here because in CA, true means solid wall
                        _initialGrid[x, y] = random.NextDouble() > parameters.CaveProbability;
                    }

                    // Copy to temp grid
                    _tempGrid[x, y] = _initialGrid[x, y];
                }
            }
        }

        private void RunCellularAutomata(int width, int height, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                // Apply CA rule to each cell
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        int wallCount = CountWallNeighbors(x, y);

                        // Apply B678/S345678 rule (a modified Conway's rule for caves)
                        // If a cell has 5+ wall neighbors, it becomes a wall
                        // If a cell has <4 wall neighbors, it becomes open space
                        if (wallCount >= 5)
                        {
                            _tempGrid[x, y] = true; // Make it a wall
                        }
                        else if (wallCount <= 3)
                        {
                            _tempGrid[x, y] = false; // Make it open space
                        }
                        // Otherwise it remains unchanged
                    }
                }

                // Copy temp grid back to initial grid for next iteration
                if (i < iterations - 1)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            _initialGrid[x, y] = _tempGrid[x, y];
                        }
                    }
                }
            }
        }

        private int CountWallNeighbors(int x, int y)
        {
            int count = 0;

            // Check all 8 neighbors
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                for (int nx = x - 1; nx <= x + 1; nx++)
                {
                    // Skip the cell itself
                    if (nx == x && ny == y) continue;

                    // Count wall neighbors
                    if (_initialGrid[nx, ny])
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}