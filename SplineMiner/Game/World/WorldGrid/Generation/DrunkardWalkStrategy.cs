using System;
using Microsoft.Xna.Framework;

namespace SplineMiner.Game.World.WorldGrid.Generation
{
    /// <summary>
    /// Generates tunnels using the Drunkard's Walk algorithm.
    /// Starts with a solid world and carves tunnels by randomly wandering.
    /// </summary>
    public class DrunkardWalkStrategy : IWorldGenerationStrategy
    {
        public string Name => "Drunkard's Walk";

        public string Description => "Creates winding tunnels by random walks through the grid.";

        // Cache for storing the tunnel map
        private bool[,] _tunnelMap;
        private readonly Vector2[] _directions = new Vector2[]
        {
            new Vector2(0, -1), // Up
            new Vector2(1, 0),  // Right
            new Vector2(0, 1),  // Down
            new Vector2(-1, 0)  // Left
        };

        public bool ShouldBeActive(
            int x, int y,
            float worldX, float worldY,
            int width, int height,
            float worldWidth, float worldHeight,
            Random random,
            GenerationParameters parameters)
        {
            // Initialize tunnel map if first cell or different size
            if (_tunnelMap == null || _tunnelMap.GetLength(0) != width || _tunnelMap.GetLength(1) != height)
            {
                GenerateTunnels(width, height, random, parameters);
            }

            // If a tunnel exists at this position, make the cell inactive (empty)
            return !_tunnelMap[x, y];
        }

        private void GenerateTunnels(int width, int height, Random random, GenerationParameters parameters)
        {
            // Create a map where true = tunnel, false = solid
            _tunnelMap = new bool[width, height];

            // Start in the center
            int x = width / 2;
            int y = height / 2;

            // Number of random steps to take
            int steps = parameters.DrunkWalkSteps;

            // Current direction index (0-3)
            int direction = random.Next(4);

            // Carve the initial position
            _tunnelMap[x, y] = true;

            // Perform the random walk
            for (int i = 0; i < steps; i++)
            {
                // Possibly change direction
                if (random.NextDouble() < parameters.DrunkWalkTurnChance)
                {
                    // Either turn left or right
                    if (random.NextDouble() < 0.5)
                    {
                        direction = (direction + 1) % 4; // Turn right
                    }
                    else
                    {
                        direction = (direction + 3) % 4; // Turn left
                    }
                }

                // Move in the current direction
                x += (int)_directions[direction].X;
                y += (int)_directions[direction].Y;

                // Clamp to grid bounds
                x = Math.Clamp(x, 1, width - 2);
                y = Math.Clamp(y, 1, height - 2);

                // Carve the tunnel at the new position
                _tunnelMap[x, y] = true;

                // Carve additional cells occasionally to widen tunnel
                if (random.NextDouble() < 0.2)
                {
                    // Randomly choose a neighbor to carve
                    int nx = x + random.Next(-1, 2);
                    int ny = y + random.Next(-1, 2);

                    // Ensure within bounds
                    if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1)
                    {
                        _tunnelMap[nx, ny] = true;
                    }
                }
            }

            // Add some additional tunnels from the center
            for (int i = 0; i < 3; i++)
            {
                // Start a new tunnel from the center or a random carved point
                if (random.NextDouble() < 0.5)
                {
                    x = width / 2;
                    y = height / 2;
                }
                else
                {
                    // Find a random existing tunnel point
                    bool found = false;
                    for (int attempts = 0; attempts < 100 && !found; attempts++)
                    {
                        int rx = random.Next(1, width - 1);
                        int ry = random.Next(1, height - 1);
                        if (_tunnelMap[rx, ry])
                        {
                            x = rx;
                            y = ry;
                            found = true;
                        }
                    }
                }

                // Choose a random direction
                direction = random.Next(4);

                // Carve a new tunnel
                int subSteps = parameters.DrunkWalkSteps / 3;
                for (int j = 0; j < subSteps; j++)
                {
                    // Possibly change direction
                    if (random.NextDouble() < parameters.DrunkWalkTurnChance)
                    {
                        direction = random.Next(4); // Any direction
                    }

                    // Move in the current direction
                    x += (int)_directions[direction].X;
                    y += (int)_directions[direction].Y;

                    // Clamp to grid bounds
                    x = Math.Clamp(x, 1, width - 2);
                    y = Math.Clamp(y, 1, height - 2);

                    // Carve the tunnel
                    _tunnelMap[x, y] = true;
                }
            }
        }
    }
}