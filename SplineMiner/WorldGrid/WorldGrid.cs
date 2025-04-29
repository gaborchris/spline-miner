using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace SplineMiner.WorldGrid
{
    /// <summary>
    /// Manages a collection of grid cells and handles procedural generation
    /// </summary>
    public class WorldGrid
    {
        private readonly List<GridCell> _cells = new List<GridCell>();
        private readonly Random _random;
        private Texture2D _cellTexture;
        
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }

        public WorldGrid(int width, int height, float cellSize, int seed = 0)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            _random = seed == 0 ? new Random() : new Random(seed);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            // Create a simple white texture for rendering cells
            _cellTexture = new Texture2D(graphicsDevice, 1, 1);
            _cellTexture.SetData(new[] { Color.White });
            
            // Generate the grid
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            _cells.Clear();
            
            // Calculate world bounds
            float worldWidth = Width * CellSize;
            float worldHeight = Height * CellSize;
            float startX = -worldWidth / 2;
            float startY = -worldHeight / 2;
            
            // Parameters for cave generation
            float caveProbability = 0.45f; // Probability of a cell being a cave (empty)
            
            // Generate cells
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float posX = startX + x * CellSize + CellSize / 2;
                    float posY = startY + y * CellSize + CellSize / 2;
                    
                    // Simple procedural generation - more caves toward the center
                    float distanceToCenter = Vector2.Distance(
                        new Vector2(posX, posY), 
                        Vector2.Zero);
                    
                    float maxDistance = MathF.Sqrt(worldWidth * worldWidth + worldHeight * worldHeight) / 2;
                    float normalizedDistance = distanceToCenter / maxDistance;
                    
                    // Invert so center has more caves
                    float caveFactor = 1.0f - normalizedDistance;
                    bool isActive = _random.NextDouble() > (caveProbability + caveFactor * 0.3f);
                    
                    _cells.Add(new GridCell(new Vector2(posX, posY), CellSize, isActive));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var cell in _cells)
            {
                if (cell.IsActive)
                {
                    var bounds = cell.GetBounds();
                    spriteBatch.Draw(
                        _cellTexture,
                        bounds,
                        cell.Color);
                }
            }
        }

        public GridCell GetCellAtPosition(Vector2 worldPosition)
        {
            return _cells.Find(cell => cell.Contains(worldPosition));
        }

        public void DestroyCell(Vector2 worldPosition)
        {
            var cell = GetCellAtPosition(worldPosition);
            if (cell != null)
            {
                cell.IsActive = false;
            }
        }
    }
} 