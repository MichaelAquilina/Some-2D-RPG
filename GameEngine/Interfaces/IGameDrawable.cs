using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Interfaces
{
    public interface IGameDrawable
    {
        float X { get; }
        float Y { get; }
        float Width { get; }
        float Height { get; }

        bool Visible { get; }           //Value Specifying if the Drawable Object should currently be visible

        Color DrawColor { get; }        //Color mask with which to draw the SpriteBatch. Should be White by Default

        Vector2 Origin { get; }         //Relative Origin. 0,0 Being Top Left Corner and 1,1 Being Bottom Right Corner

        Texture2D GetTexture(GameTime GameTime);
        Rectangle GetSourceRectangle(GameTime GameTime);
    }
}
