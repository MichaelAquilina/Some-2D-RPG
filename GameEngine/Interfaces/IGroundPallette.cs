using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Drawing;
using GameEngine.GameObjects;

namespace GameEngine.Interfaces
{
    /// <summary>
    /// Ground Pallette interface that specifies the required properties and methods that would allow
    /// for the Game to proporly be able to render the pallette. Provides an interface that allows GameWorld.cs
    /// to proporly decide what tile to draw from a SpriteSheet containing various tiles. The current format
    /// allows a single ground pallette to extract tiles from multiple sprite sheets.
    /// </summary>
    public interface IGroundPallette : ILoadable
    {
        Texture2D GetTileSourceTexture(byte TileType);

        Rectangle GetTileSourceRectangle(byte TileType);

        Color GetTileColor(byte TileType);

        int TileCount { get; }
    }
}
