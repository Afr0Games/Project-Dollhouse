using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo
{
    public class TextureUtils
    {
        public static Texture2D CreateRectangle(GraphicsDevice Graphics, int Width, int Height, Color C)
        {
            Texture2D RectTexture = new Texture2D(Graphics, Width, Height);
            RectTexture.SetData<Color>(new Color[] { C });
            return RectTexture;
        }
    }
}
