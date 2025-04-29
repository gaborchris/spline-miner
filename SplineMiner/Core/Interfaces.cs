using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SplineMiner.Core
{
    /// <summary>
    /// Represents a cart that can move along a track. This interface defines the core functionality
    /// for a cart's movement and rendering, including position tracking and distance-based movement.
    /// </summary>
    public interface ICart
    {
        /// <summary>
        /// Updates the cart's state based on game time and track information.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="track">The track the cart is moving along.</param>
        void Update(GameTime gameTime, ITrack track);

        /// <summary>
        /// Draws the cart using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Gets the current position of the cart in world coordinates.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the current distance along the track that the cart has traveled.
        /// </summary>
        float CurrentDistance { get; }
    }

    /// <summary>
    /// Controls the movement and rotation of a cart along a track. This interface manages
    /// the cart's position, speed, and orientation based on track geometry.
    /// </summary>
    public interface IMovementController
    {
        /// <summary>
        /// Updates the cart's position based on game time and track information.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="track">The track the cart is moving along.</param>
        void UpdatePosition(GameTime gameTime, ITrack track);

        /// <summary>
        /// Updates the cart's rotation to match the track's orientation at the current position.
        /// </summary>
        /// <param name="track">The track the cart is moving along.</param>
        void UpdateRotation(ITrack track);

        /// <summary>
        /// Gets the current position of the cart in world coordinates.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Gets the current rotation of the cart in radians.
        /// </summary>
        float Rotation { get; }

        /// <summary>
        /// Gets or sets the current speed of the cart in pixels per second.
        /// </summary>
        float Speed { get; set; }

        /// <summary>
        /// Gets or sets the current distance along the track that the cart has traveled.
        /// </summary>
        float CurrentDistance { get; set; }
    }

    /// <summary>
    /// Manages the wheel positions of a cart. This interface handles the calculation
    /// and tracking of front and back wheel positions based on the cart's current position.
    /// </summary>
    public interface IWheelSystem
    {
        /// <summary>
        /// Updates the wheel positions based on the current track and cart position.
        /// </summary>
        /// <param name="track">The track the cart is moving along.</param>
        /// <param name="currentDistance">The current distance along the track.</param>
        void UpdateWheelPositions(ITrack track, float currentDistance);

        /// <summary>
        /// Gets the position of the front wheel in world coordinates.
        /// </summary>
        Vector2 FrontWheelPosition { get; }

        /// <summary>
        /// Gets the position of the back wheel in world coordinates.
        /// </summary>
        Vector2 BackWheelPosition { get; }
    }

    /// <summary>
    /// Provides visualization and analysis tools for debugging cart movement.
    /// This interface enables testing and visualization of cart movement patterns.
    /// </summary>
    public interface IDebugVisualizer
    {
        /// <summary>
        /// Draws debug information about the cart's movement and position.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="debugTexture">The texture to use for debug visualization.</param>
        void DrawDebugInfo(SpriteBatch spriteBatch, Texture2D debugTexture);

        /// <summary>
        /// Starts a movement test to analyze cart movement patterns.
        /// </summary>
        void StartMovementTest();

        /// <summary>
        /// Analyzes the smoothness of the cart's movement during a test.
        /// </summary>
        void AnalyzeMovementSmoothness();

        /// <summary>
        /// Gets whether a movement test is currently in progress.
        /// </summary>
        bool IsTestingMovement { get; }

        /// <summary> 
        /// Updates the debug visualizer.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>    
        void Update(GameTime gameTime);
    }

    /// <summary>
    /// Calculates spline-based track geometry. This interface provides methods for
    /// computing points along a spline and calculating arc lengths.
    /// </summary>
    public interface ISplineCalculator
    {
        /// <summary>
        /// Gets a point on the spline at the specified parameter value.
        /// </summary>
        /// <param name="t">The parameter value along the spline (0 to n-1).</param>
        /// <returns>The point on the spline at parameter t.</returns>
        Vector2 GetPoint(float t);

        /// <summary>
        /// Gets the parameter value corresponding to a specific distance along the spline.
        /// </summary>
        /// <param name="distance">The distance along the spline.</param>
        /// <param name="totalLength">The total length of the spline.</param>
        /// <returns>The parameter value corresponding to the distance.</returns>
        float GetParameterForDistance(float distance, float totalLength);

        /// <summary>
        /// Computes the arc length of a segment of the spline.
        /// </summary>
        /// <param name="tStart">The starting parameter value.</param>
        /// <param name="tEnd">The ending parameter value.</param>
        /// <param name="baseSteps">The number of steps to use for the calculation.</param>
        /// <returns>The arc length of the specified segment.</returns>
        float ComputeArcLength(float tStart, float tEnd, int baseSteps = 40);
    }

    /// <summary>
    /// Provides a generic caching mechanism for key-value pairs.
    /// This interface is used to optimize performance by caching frequently accessed values.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key.</typeparam>
    /// <typeparam name="TValue">The type of the cached value.</typeparam>
    public interface ICache<TKey, TValue>
    {
        /// <summary>
        /// Attempts to get a value from the cache.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="value">The value if found.</param>
        /// <returns>True if the value was found in the cache.</returns>
        bool TryGetValue(TKey key, out TValue value);

        /// <summary>
        /// Sets a value in the cache.
        /// </summary>
        /// <param name="key">The key to store the value under.</param>
        /// <param name="value">The value to store.</param>
        void SetValue(TKey key, TValue value);

        /// <summary>
        /// Clears all values from the cache.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Handles the rendering of a track. This interface manages the visual representation
    /// of the track and its debug information.
    /// </summary>
    public interface ITrackRenderer
    {
        /// <summary>
        /// Draws the track using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="track">The track to draw.</param>
        void Draw(SpriteBatch spriteBatch, ITrack track);

        /// <summary>
        /// Draws debug information about the track.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="distance">The current distance along the track.</param>
        /// <param name="debugTexture">The texture to use for debug visualization.</param>
        void DrawDebugInfo(SpriteBatch spriteBatch, float distance, Texture2D debugTexture);
    }

    /// <summary>
    /// Represents a track that a cart can move along. This interface defines the core
    /// functionality for track geometry, including point lookup and rotation calculation.
    /// </summary>
    public interface ITrack
    {
        /// <summary>
        /// Gets a point on the track at the specified distance.
        /// </summary>
        /// <param name="distance">The distance along the track.</param>
        /// <returns>The point on the track at the specified distance.</returns>
        Vector2 GetPointByDistance(float distance);

        /// <summary>
        /// Gets the rotation of the track at the specified distance.
        /// </summary>
        /// <param name="distance">The distance along the track.</param>
        /// <returns>The rotation in radians at the specified distance.</returns>
        float GetRotationAtDistance(float distance);

        /// <summary>
        /// Gets the total length of the track.
        /// </summary>
        float TotalArcLength { get; }

        /// <summary>
        /// Draws the track using the provided sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Draws debug information about the track.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to use for drawing.</param>
        /// <param name="distance">The current distance along the track.</param>
        /// <param name="debugTexture">The texture to use for debug visualization.</param>
        void DrawDebugInfo(SpriteBatch spriteBatch, float distance, Texture2D debugTexture);

        /// <summary>
        /// Gets the list of placed track nodes.
        /// </summary>
        IReadOnlyList<PlacedTrackNode> PlacedNodes { get; }

        /// <summary>
        /// Gets the list of shadow track nodes.
        /// </summary>
        IReadOnlyList<ShadowTrackNode> ShadowNodes { get; }
    }
} 