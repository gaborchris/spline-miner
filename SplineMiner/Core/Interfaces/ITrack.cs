using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SplineMiner.Core.Interfaces
{
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