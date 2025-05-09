using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;
using SplineMiner.Core.Services;
using SplineMiner.Game.World.WorldGrid.Generation;

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
        private readonly IDebugService _debugService;

        // Performance tracking
        private int _totalCells = 0;
        private int _visibleCells = 0;

        // Grid parameters
        private int _width;
        private int _height;
        private float _cellSize;
        private float _caveProbability = 0.45f;
        private Vector2 _worldOrigin;  // Store the world origin position

        // Generation strategy
        private IWorldGenerationStrategy _generationStrategy;
        private GenerationParameters _generationParameters;

        // Current available strategies
        private readonly List<IWorldGenerationStrategy> _availableStrategies;

        // Add spatial partitioning
        private Dictionary<Point, List<GridCell>> _spatialGrid;
        private const int PARTITION_SIZE = 64; // Size of each partition

        private Point GetPartitionKey(Vector2 position)
        {
            return new Point(
                (int)Math.Floor(position.X / PARTITION_SIZE),
                (int)Math.Floor(position.Y / PARTITION_SIZE)
            );
        }

        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;
        public float CaveProbability => _caveProbability;
        public Vector2 WorldOrigin => _worldOrigin;

        // Statistics properties for debugging
        public int TotalCells => _totalCells;
        public int VisibleCells => _visibleCells;

        // Generation strategy properties
        public IWorldGenerationStrategy GenerationStrategy => _generationStrategy;
        public IReadOnlyList<IWorldGenerationStrategy> AvailableStrategies => _availableStrategies.AsReadOnly();

        public WorldGrid(
            int width,
            int height,
            float cellSize,
            IEnumerable<IWorldGenerationStrategy> strategies,
            IDebugService debugService = null,
            Vector2? worldOrigin = null)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _debugService = debugService;
            _random = new Random();
            _generationParameters = new GenerationParameters(_caveProbability, _random.Next());
            _worldOrigin = worldOrigin ?? Vector2.Zero;

            // Set available strategies from constructor parameter
            _availableStrategies = new List<IWorldGenerationStrategy>(strategies);
            if (_availableStrategies.Count == 0)
            {
                throw new ArgumentException("At least one generation strategy must be provided", nameof(strategies));
            }
            _generationStrategy = _availableStrategies[0];
        }

        /// <summary>
        /// Updates the grid parameters without regenerating the grid
        /// </summary>
        public void UpdateParameters(int width, int height, float cellSize, float caveProbability, Vector2? worldOrigin = null)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _caveProbability = caveProbability;
            if (worldOrigin.HasValue)
            {
                _worldOrigin = worldOrigin.Value;
            }

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

            // Initialize spatial partitioning
            _spatialGrid = new Dictionary<Point, List<GridCell>>();

            // Generate the grid
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            _cells.Clear();
            _spatialGrid.Clear();

            // Calculate world bounds
            float worldWidth = _width * _cellSize;
            float worldHeight = _height * _cellSize;

            // Calculate start position based on world origin
            float startX = _worldOrigin.X - worldWidth / 2;
            float startY = _worldOrigin.Y - worldHeight / 2;

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

                    var cell = new GridCell(new Vector2(posX, posY), _cellSize, isActive, _debugService);
                    _cells.Add(cell);

                    // Add cells to spatial partitioning
                    var key = GetPartitionKey(cell.Position);
                    if (!_spatialGrid.ContainsKey(key))
                    {
                        _spatialGrid[key] = new List<GridCell>();
                    }
                    _spatialGrid[key].Add(cell);
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

            // Calculate start position based on world origin
            float startX = _worldOrigin.X - worldWidth / 2;
            float startY = _worldOrigin.Y - worldHeight / 2;

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

        // Get only nearby blocks for collision checking
        public IEnumerable<IWorldBlock> GetNearbyBlocks(Vector2 position, float radius)
        {
            var centerPartition = GetPartitionKey(position);
            var radiusInPartitions = (int)Math.Ceiling(radius / PARTITION_SIZE);

            for (int y = -radiusInPartitions; y <= radiusInPartitions; y++)
            {
                for (int x = -radiusInPartitions; x <= radiusInPartitions; x++)
                {
                    var key = new Point(centerPartition.X + x, centerPartition.Y + y);
                    if (_spatialGrid.TryGetValue(key, out var cells))
                    {
                        foreach (var cell in cells)
                        {
                            if (cell.IsActive && Vector2.Distance(position, cell.Position) <= radius)
                            {
                                yield return cell;
                            }
                        }
                    }
                }
            }
        }
    }
}