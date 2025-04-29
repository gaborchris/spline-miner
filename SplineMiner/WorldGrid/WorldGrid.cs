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
        
        // Performance tracking
        private int _totalCells = 0;
        private int _visibleCells = 0;
        
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        
        // Statistics properties for debugging
        public int TotalCells => _totalCells;
        public int VisibleCells => _visibleCells;

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
            
            _totalCells = _cells.Count;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Get the visible area in world space
            Matrix inverseView = Matrix.Invert(CameraManager.Instance.Transform);
            Vector2 topLeft = Vector2.Transform(Vector2.Zero, inverseView);
            Vector2 bottomRight = Vector2.Transform(new Vector2(
                CameraManager.Instance.Viewport.Width,
                CameraManager.Instance.Viewport.Height
            ), inverseView);
            
            // Add padding to ensure we draw cells just off-screen
            float padding = CellSize * 2;
            Rectangle viewBounds = new Rectangle(
                (int)(topLeft.X - padding),
                (int)(topLeft.Y - padding),
                (int)(bottomRight.X - topLeft.X + padding * 2),
                (int)(bottomRight.Y - topLeft.Y + padding * 2)
            );
            
            // Reset visible cell counter
            _visibleCells = 0;
            
            // Only render cells that are within the view
            foreach (var cell in _cells)
            {
                if (cell.IsActive)
                {
                    var bounds = cell.GetBounds();
                    if (viewBounds.Intersects(bounds))
                    {
                        spriteBatch.Draw(
                            _cellTexture,
                            bounds,
                            cell.Color);
                        
                        _visibleCells++;
                    }
                }
            }
        }

        public GridCell GetCellAtPosition(Vector2 worldPosition)
        {
            // Calculate grid coordinates from world position
            float worldWidth = Width * CellSize;
            float worldHeight = Height * CellSize;
            float startX = -worldWidth / 2;
            float startY = -worldHeight / 2;
            
            int gridX = (int)((worldPosition.X - startX) / CellSize);
            int gridY = (int)((worldPosition.Y - startY) / CellSize);
            
            // Check if within grid bounds
            if (gridX < 0 || gridX >= Width || gridY < 0 || gridY >= Height)
            {
                return null;
            }
            
            // Find the cell at the calculated index
            int index = gridY * Width + gridX;
            if (index >= 0 && index < _cells.Count)
            {
                var cell = _cells[index];
                return cell.IsActive ? cell : null;
            }
            
            return null;
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