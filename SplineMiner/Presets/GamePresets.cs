using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;
using SplineMiner.Game.World.WorldGrid;
using SplineMiner.Game.World.WorldGrid.Generation;

namespace SplineMiner.Presets
{
    /// <summary>
    /// Enums for different preset types
    /// </summary>
    public enum TrackPresetId
    {
        CollisionTest,
        Small,
        Large
    }

    public enum WorldPresetId
    {
        Default,
        Test,
        Dense
    }

    public enum BlockPresetId
    {
        CollisionTestWall,
        Tunnel,
        OreVein
    }

    /// <summary>
    /// Provides preset configurations for various game elements like tracks, world generation, and blocks.
    /// </summary>
    public static class GamePresets
    {
        private static readonly Dictionary<TrackPresetId, List<Vector2>> _trackPresets;
        private static readonly Dictionary<WorldPresetId, WorldPresetConfig> _worldPresets;
        private static readonly Dictionary<BlockPresetId, List<Vector2>> _blockPresets;

        static GamePresets()
        {
            // Initialize track presets
            _trackPresets = new Dictionary<TrackPresetId, List<Vector2>>
            {
                { TrackPresetId.CollisionTest, GetCollisionTestTrack() },
                { TrackPresetId.Small, GetSmallTrack() },
                { TrackPresetId.Large, GetLargeTrack() }
            };

            // Initialize world presets
            _worldPresets = new Dictionary<WorldPresetId, WorldPresetConfig>
            {
                { WorldPresetId.Default, new WorldPresetConfig(500, 200, 20f, 0.3f, 0.1f, 3, 10) },
                { WorldPresetId.Test, new WorldPresetConfig(5, 5, 20f, 0.1f, 0.05f, 5, 15, new Vector2(300, 250)) },  // Test wall centered at (300, 250)
                { WorldPresetId.Dense, new WorldPresetConfig(500, 200, 20f, 0.5f, 0.2f, 2, 8) }
            };

            // Initialize block presets
            _blockPresets = new Dictionary<BlockPresetId, List<Vector2>>
            {
                { BlockPresetId.CollisionTestWall, GetCollisionTestWall() },
                { BlockPresetId.Tunnel, GetTunnelPattern() },
                { BlockPresetId.OreVein, GetOreVeinPattern() }
            };
        }

        /// <summary>
        /// Gets a track configuration by preset ID
        /// </summary>
        public static List<Vector2> GetTrack(TrackPresetId presetId)
        {
            return _trackPresets[presetId];
        }

        /// <summary>
        /// Gets a world configuration by preset ID
        /// </summary>
        public static WorldPresetConfig GetWorld(WorldPresetId presetId)
        {
            return _worldPresets[presetId];
        }

        /// </summary>
        public static List<Vector2> GetBlockPattern(BlockPresetId presetId)
        {
            return _blockPresets[presetId];
        }

        /// <summary>
        /// Creates and initializes a WorldGrid instance using the specified preset
        /// </summary>
        /// <param name="presetId">The world preset to use</param>
        /// <param name="debugManager">The debug manager instance</param>
        /// <param name="graphicsDevice">The graphics device for initializing textures</param>
        /// <returns>A fully initialized WorldGrid instance</returns>
        public static WorldGrid CreateWorldGrid(WorldPresetId presetId, IDebugService debugManager, GraphicsDevice graphicsDevice)
        {
            var config = GetWorld(presetId);

            // Create strategies based on preset
            var strategies = presetId switch
            {
                WorldPresetId.Test => new IWorldGenerationStrategy[] { new TestWallStrategy(debugManager) },
                WorldPresetId.Dense => new IWorldGenerationStrategy[]
                {
                    new CellularAutomataStrategy(),
                    new DrunkardWalkStrategy()
                },
                _ => new IWorldGenerationStrategy[]  // Default case
                {
                    new CenterCaveStrategy(),
                    new CellularAutomataStrategy(),
                    new DrunkardWalkStrategy(),
                    new MazeGenerationStrategy()
                }
            };

            var worldGrid = new WorldGrid(
                config.GridWidth,
                config.GridHeight,
                config.CellSize,
                strategies,
                debugManager,
                config.WorldOrigin
            );

            // Update the grid parameters
            worldGrid.UpdateParameters(
                config.GridWidth,
                config.GridHeight,
                config.CellSize,
                config.CaveProbability,
                config.WorldOrigin
            );

            // Initialize the grid with graphics device
            worldGrid.Initialize(graphicsDevice);

            // Generate the grid
            worldGrid.GenerateGrid();

            return worldGrid;
        }

        #region Track Preset Definitions
        private static List<Vector2> GetCollisionTestTrack()
        {
            return new List<Vector2>
            {
                new(100, 300),
                new(200, 300),
                new(300, 300),
                new(400, 300),
                new(500, 300),
                new(600, 300),
                new(700, 300),
                new(800, 300),
                new(900, 300),
                new(1000, 300)
            };
        }

        private static List<Vector2> GetSmallTrack()
        {
            return new List<Vector2>
            {
                new(100, 300),
                new(200, 250),
                new(300, 300),
                new(400, 350),
                new(500, 300),
                new(600, 250),
                new(700, 300)
            };
        }

        private static List<Vector2> GetLargeTrack()
        {
            return new List<Vector2>
            {
                new(100, 300),
                new(200, 250),
                new(300, 200),
                new(400, 250),
                new(500, 300),
                new(600, 350),
                new(700, 400),
                new(800, 350),
                new(900, 300),
                new(1000, 250),
                new(1100, 200),
                new(1200, 250),
                new(1300, 300),
                new(1400, 350),
                new(1500, 300)
            };
        }
        #endregion

        #region Block Preset Definitions
        private static List<Vector2> GetCollisionTestWall()
        {
            var wall = new List<Vector2>();
            float centerX = 300;  // Center X position (moved 100 units right from 200)
            float centerY = 250;  // Center Y position
            float wallWidth = 100;  // Total width of the wall (5 cells * 20 units)
            float wallHeight = 100; // Total height of the wall (5 cells * 20 units)

            // Calculate start position to center the wall
            float startX = centerX - wallWidth / 2;
            float startY = centerY - wallHeight / 2;

            // Create a 5x5 wall
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    wall.Add(new Vector2(startX + i * 20, startY + j * 20));
                }
            }

            return wall;
        }

        private static List<Vector2> GetTunnelPattern()
        {
            var tunnel = new List<Vector2>();
            float startX = 100;
            float y = 300;

            // Create tunnel walls
            for (int i = 0; i < 20; i++)
            {
                // Top wall
                tunnel.Add(new Vector2(startX + i * 20, y - 40));
                // Bottom wall
                tunnel.Add(new Vector2(startX + i * 20, y + 40));
            }

            return tunnel;
        }

        private static List<Vector2> GetOreVeinPattern()
        {
            var vein = new List<Vector2>();
            float startX = 300;
            float startY = 200;

            // Create a branching ore vein
            for (int i = 0; i < 5; i++)
            {
                vein.Add(new Vector2(startX + i * 20, startY + i * 10));
                vein.Add(new Vector2(startX + i * 20, startY - i * 10));
            }

            return vein;
        }
        #endregion
    }

    /// <summary>
    /// Configuration class for world generation parameters
    /// </summary>
    public class WorldPresetConfig
    {
        public int GridWidth { get; }
        public int GridHeight { get; }
        public float CellSize { get; }
        public float CaveProbability { get; }
        public float OreProbability { get; }
        public int MinCaveSize { get; }
        public int MaxCaveSize { get; }
        public Vector2 WorldOrigin { get; }  // Position of the world grid's origin

        /// <summary>
        /// Creates a new WorldPresetConfig with the specified parameters
        /// </summary>
        public WorldPresetConfig(
            int gridWidth,
            int gridHeight,
            float cellSize,
            float caveProbability,
            float oreProbability,
            int minCaveSize,
            int maxCaveSize,
            Vector2? worldOrigin = null)  // Optional parameter with default null
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            CellSize = cellSize;
            CaveProbability = caveProbability;
            OreProbability = oreProbability;
            MinCaveSize = minCaveSize;
            MaxCaveSize = maxCaveSize;
            WorldOrigin = worldOrigin ?? Vector2.Zero;  // Default to (0,0) if not specified
        }
    }
}