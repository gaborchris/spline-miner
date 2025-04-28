using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SplineMiner.Core
{
    public interface ICart
    {
        void Update(GameTime gameTime, ITrack track);
        void Draw(SpriteBatch spriteBatch);
        Vector2 Position { get; }
        float CurrentDistance { get; }
    }

    public interface IMovementController
    {
        void UpdatePosition(GameTime gameTime, ITrack track);
        void UpdateRotation(ITrack track);
        Vector2 Position { get; }
        float Rotation { get; }
        float Speed { get; set; }
        float CurrentDistance { get; set; }
    }

    public interface IWheelSystem
    {
        void UpdateWheelPositions(ITrack track, float currentDistance);
        Vector2 FrontWheelPosition { get; }
        Vector2 BackWheelPosition { get; }
    }

    public interface IDebugVisualizer
    {
        void DrawDebugInfo(SpriteBatch spriteBatch, Texture2D debugTexture);
        void StartMovementTest();
        void AnalyzeMovementSmoothness();
        bool IsTestingMovement { get; }
    }

    public interface ISplineCalculator
    {
        Vector2 GetPoint(float t);
        float GetParameterForDistance(float distance, float totalLength);
        float ComputeArcLength(float tStart, float tEnd, int baseSteps = 40);
    }

    public interface ICache<TKey, TValue>
    {
        bool TryGetValue(TKey key, out TValue value);
        void SetValue(TKey key, TValue value);
        void Clear();
    }

    public interface ITrackRenderer
    {
        void Draw(SpriteBatch spriteBatch, ITrack track);
        void DrawDebugInfo(SpriteBatch spriteBatch, float distance, Texture2D debugTexture);
    }

    public interface ITrack
    {
        Vector2 GetPointByDistance(float distance);
        float GetRotationAtDistance(float distance);
        float TotalArcLength { get; }
        void Draw(SpriteBatch spriteBatch);
        void DrawDebugInfo(SpriteBatch spriteBatch, float distance, Texture2D debugTexture);
        IReadOnlyList<PlacedTrackNode> PlacedNodes { get; }
        IReadOnlyList<ShadowTrackNode> ShadowNodes { get; }
    }
} 