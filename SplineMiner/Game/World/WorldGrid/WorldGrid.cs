using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Services;
using SplineMiner.Game.World.WorldGrid.Generation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SplineMiner.Game.World.WorldGrid
{
    /// <summary>
    /// Manages a procedurally generated grid of cells that forms the game world.
    /// The world is generated once at startup and can be modified by deleting cells.
    /// Note: This is a read-only world after generation - cells can only be deleted, not added or moved.
    /// </summary>
    public class WorldGrid
    {
        private readonly List<GridCell> _cells = new List<GridCell>();
        private readonly Random _random;
        private Texture2D _cellTexture;

        // Performance tracking
        private int _totalCells = 0;
        private int _visibleCells = 0;

        // Grid parameters
        private int _width;
        private int _height;
        private float _cellSize;
        private float _caveProbability = 0.45f;

        // Generation strategy
        private IWorldGenerationStrategy _generationStrategy;
        private GenerationParameters _generationParameters;

        // Available generation strategies
        private static readonly List<IWorldGenerationStrategy> _availableStrategies = new List<IWorldGenerationStrategy>
        {
            new CenterCaveStrategy(),
            new CellularAutomataStrategy(),
            new DrunkardWalkStrategy(),
            new MazeGenerationStrategy()
        };

        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;
        public float CaveProbability => _caveProbability;

        // Statistics properties for debugging
        public int TotalCells => _totalCells;
        public int VisibleCells => _visibleCells;

        // Generation strategy properties
        public IWorldGenerationStrategy GenerationStrategy => _generationStrategy;
        public IReadOnlyList<IWorldGenerationStrategy> AvailableStrategies => _availableStrategies.AsReadOnly();

        public WorldGrid(int width, int height, float cellSize, int seed = 0)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _random = seed == 0 ? new Random() : new Random(seed);

            // Initialize with default generation parameters
            _generationParameters = new GenerationParameters(_caveProbability, seed);

            // Set default generation strategy
            _generationStrategy = _availableStrategies[0]; // Center cave strategy
        }

        /// <summary>
        /// Updates the grid parameters without regenerating the grid
        /// </summary>
        public void UpdateParameters(int width, int height, float cellSize, float caveProbability)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _caveProbability = caveProbability;

            // Update generation parameters
            _generationParameters.CaveProbability = caveProbability;
        }

        /// <summary>
        /// Sets the generation strategy to use for world generation
        /// </summary>
        public void SetGenerationStrategy(IWorldGenerationStrategy strategy)
        {
            _generationStrategy = strategy ?? _availableStrategies[0];
        }

        /// <summary>
        /// Sets the generation strategy by index from the available strategies list
        /// </summary>
        public void SetGenerationStrategy(int strategyIndex)
        {
            if (strategyIndex >= 0 && strategyIndex < _availableStrategies.Count)
            {
                _generationStrategy = _availableStrategies[strategyIndex];
            }
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
            float worldWidth = _width * _cellSize;
            float worldHeight = _height * _cellSize;
            float startX = -worldWidth / 2;
            float startY = -worldHeight / 2;

            // Update generation parameters with current settings
            _generationParameters.CaveProbability = _caveProbability;

            // Generate cells using the selected strategy
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float posX = startX + x * _cellSize + _cellSize / 2;
                    float posY = startY + y * _cellSize + _cellSize / 2;

                    // Use the strategy to determine if cell should be active
                    bool isActive = _generationStrategy.ShouldBeActive(
                        x, y,
                        posX, posY,
                        _width, _height,
                        worldWidth, worldHeight,
                        _random,
                        _generationParameters);

                    _cells.Add(new GridCell(new Vector2(posX, posY), _cellSize, isActive));
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
            float padding = _cellSize * 2;
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
            float worldWidth = _width * _cellSize;
            float worldHeight = _height * _cellSize;
            float startX = -worldWidth / 2;
            float startY = -worldHeight / 2;

            int gridX = (int)((worldPosition.X - startX) / _cellSize);
            int gridY = (int)((worldPosition.Y - startY) / _cellSize);

            // Check if within grid bounds
            if (gridX < 0 || gridX >= _width || gridY < 0 || gridY >= _height)
            {
                return null;
            }

            // Find the cell at the calculated index
            int index = gridY * _width + gridX;
            if (index >= 0 && index < _cells.Count)
            {
                var cell = _cells[index];
                return cell.IsActive ? cell : null;
            }

            return null;
        }

        public void DeleteCell(Vector2 screenPosition)
        {
            Vector2 worldPosition = CameraManager.Instance.ScreenToWorld(screenPosition);
            var cell = GetCellAtPosition(worldPosition);
            if (cell != null)
            {
                cell.IsActive = false;
            }
        }
    }
}