using System;
using System.Collections.Generic;
using System.Text;
using UIParser.Nodes;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo
{
    public class UIImage : UIElement
    {
        public UIImage(DefineImageNode Node, UIElement Parent) : base(Parent)
        {
            m_Name = Node.Name;
            Image = FileManager.GetTexture(ulong.Parse(Node.AssetID, System.Globalization.NumberStyles.HexNumber));
            TextureUtils.ManualTextureMask(ref Image, new uint[] { 255, 0, 255 });
        }
    }
}
