using System;
using Microsoft.Xna.Framework.Graphics;

namespace Files.IFF
{
    public enum SPRType //Type safety...
    {
        SPR = 0x0,
        SPR2 = 0x01
    }

    /// <summary>
    /// Interface with common functionality shared between SPR# and SPR2
    /// </summary>
    public interface iSprite
    {
        SPRType GetSPRType();
        iSpriteFrame GetFrame(int ID);
    }

    /// <summary>
    /// Interface with common functionality shared between SPRFrame and SPR2Frame
    /// </summary>
    public interface iSpriteFrame
    {
        ushort XLocation { get; set; }
        ushort YLocation { get; set; }
        Texture2D GetTexture();
    }
}
