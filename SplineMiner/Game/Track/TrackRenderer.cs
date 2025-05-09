using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SplineMiner.Core.Interfaces;

namespace SplineMiner.Game.Track
{
    // TODO: this needs to own the logic for drawing the track, not the track itself
    public class TrackRenderer : ITrackRenderer
    {
        private readonly Texture2D _pointTexture;
        private readonly List<Vector2> _debugPoints = [];

        public TrackRenderer(Texture2D pointTexture)
        {
            _pointTexture = pointTexture;
        }

        public void Draw(SpriteBatch spriteBatch, ITrack track)
        {
            // Implementation of track drawing
        }
    }
}