using GameEngine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowKill.Shaders
{
    //WILL PROBABLY NEED TO CHANGE TO SOMETHING LESS UNMANAGEABLE
    //ideally a radius, strength, color etc should be defined NOT A TEXTURE
    public interface ILightSource
    {
        Texture2D GetLightSourceTexture(GameTime gameTime);

        Rectangle? GetLightSourceRectangle(GameTime gameTime);

        FRectangle GetRelativeDestRectangle(GameTime gameTime);

        Color GetLightColor(GameTime gameTime);
    }
}
