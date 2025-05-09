using System;
using Microsoft.Xna.Framework;
using SplineMiner.Core.Interfaces;

public class TimedDebugLogger : IDebugLogger
{
    private readonly IDebugService _debugService;
    private float _timer;
    private bool _isEnabled;
    private float _logInterval = 1.0f;  // Default 1 second

    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    public float LogInterval
    {
        get => _logInterval;
        set => _logInterval = Math.Max(0.1f, value); // Minimum 100ms interval
    }

    public TimedDebugLogger(IDebugService debugService)
    {
        _debugService = debugService;
    }

    public void Log(string category, string message)
    {
        if (!_isEnabled || _timer > 0) return;

        _debugService.GetLogger("Default")?.Log(category, message);
        _timer = _logInterval;
    }

    public void Update(GameTime gameTime)
    {
        if (_timer > 0)
        {
            _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
