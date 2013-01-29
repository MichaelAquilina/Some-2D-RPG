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
    /// for the Game to proporly be able to render the pallette.
    /// </summary>
    public interface IGroundPallette : ILoadable
    {
        //TODO: GROUND PALLETTE HAS BECOME REDUNDANT DUE TO LAYER DEPTH DEPENDENCIES
        //IT SHOULD NOT EXPOSE METHODS FOR DRAWING GROUND TEXTURES BUT RATHER RETURNING RECTANGLE FRAMES DEPNDING ON
        //THE TILE TYPE. LAYER DEPTH, COLOR, ETC SHOULD BE HANDLED BY THE DRAWVIEWPORT METHOD IN GAMEWORLD.CS
        Texture2D SourceTexture { get; }

        Rectangle GetTileSourceRectangle(byte TileType);

        Color GetTileColor(byte TileType);

        int TileCount { get; }
    }
}
