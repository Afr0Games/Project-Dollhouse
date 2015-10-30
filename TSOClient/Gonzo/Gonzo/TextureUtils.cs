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
        private static uint[] GetBuffer(int size)
        {
            var newBuffer = new uint[size];
            return newBuffer;
        }

        /// <summary>
        /// Manually replaces a specified color in a texture with transparent black,
        /// thereby masking it.
        /// </summary>
        /// <param name="Texture">The texture on which to apply the mask.</param>
        /// <param name="ColorFrom">The color to mask away.</param>
        public static void ManualTextureMask(ref Texture2D Texture, uint[] ColorsFrom)
        {
            var ColorTo = Color.Transparent.PackedValue;

            var size = Texture.Width * Texture.Height;
            uint[] buffer = GetBuffer(size);

            Texture.GetData(buffer, 0, size);

            var didChange = false;

            for (int i = 0; i < size; i++)
            {
                if (ColorsFrom.Contains(buffer[i]))
                {
                    didChange = true;
                    buffer[i] = ColorTo;
                }
            }

            if (didChange)
            {
                Texture.SetData(buffer, 0, size);
            }
        }
    }
}
