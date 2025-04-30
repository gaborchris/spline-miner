using Microsoft.Xna.Framework;
public interface IDebugLogger
{
    bool IsEnabled { get; set; }
    float LogInterval { get; set; }
    void Log(string category, string message);
    void Update(GameTime gameTime);
}
