using System;
using Microsoft.Xna.Framework;

namespace SplineMiner.WorldGrid.Generation
{
    /// <summary>
    /// Implements a maze generation algorithm using recursive division
    /// </summary>
    public class MazeGenerationStrategy : IWorldGenerationStrategy
    {
        public string Name => "Recursive Maze";
        
        public string Description => "Generates a maze using recursive division, creating corridors and walls";
        
        public bool ShouldBeActive(
            int x, int y, 
            float worldX, float worldY,
            int width, int height,
            float worldWidth, float worldHeight,
            Random random,
            GenerationParameters parameters)
        {
            // Border cells are always solid
            if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                return true;
                
            // Initialize a 2D grid for the maze
            bool[,] maze = new bool[width, height];
            
            // Set border cells
            for (int i = 0; i < width; i++)
            {
                maze[i, 0] = true;
                maze[i, height - 1] = true;
            }
            
            for (int i = 0; i < height; i++)
            {
                maze[0, i] = true;
                maze[width - 1, i] = true;
            }
            
            // Generate the maze using recursive division
            DivideArea(maze, 1, 1, width - 2, height - 2, random);
            
            // Return the final state for this specific cell
            return maze[x, y];
        }
        
        private void DivideArea(bool[,] maze, int x, int y, int width, int height, Random random)
        {
            // Stop recursion if the area is too small
            if (width < 2 || height < 2)
                return;
                
            // Decide whether to divide horizontally or vertically
            bool divideHorizontally = width < height || (width == height && random.NextDouble() < 0.5);
            
            if (divideHorizontally)
            {
                // Horizontal division
                int divideY = y + random.Next(height);
                int passageX = x + random.Next(width);
                
                // Create horizontal wall
                for (int i = x; i < x + width; i++)
                {
                    if (i != passageX)
                        maze[i, divideY] = true;
                }
                
                // Recursively divide the two new areas
                DivideArea(maze, x, y, width, divideY - y, random);
                DivideArea(maze, x, divideY + 1, width, y + height - divideY - 1, random);
            }
            else
            {
                // Vertical division
                int divideX = x + random.Next(width);
                int passageY = y + random.Next(height);
                
                // Create vertical wall
                for (int i = y; i < y + height; i++)
                {
                    if (i != passageY)
                        maze[divideX, i] = true;
                }
                
                // Recursively divide the two new areas
                DivideArea(maze, x, y, divideX - x, height, random);
                DivideArea(maze, divideX + 1, y, x + width - divideX - 1, height, random);
            }
        }
    }
} 