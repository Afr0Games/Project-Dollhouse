using System;
using System.Collections.Generic;
using System.Text;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo
{
    public class UILabel : UIElement
    {
        public UILabel(AddTextNode Node, UIElement Parent) : base(Parent)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;
            m_Position = new Vector2(Node.TextPosition.Numbers[0], Node.TextPosition.Numbers[1]);
            m_Size = new Vector2(Node.Size.Numbers[0], Node.Size.Numbers[1]);
        }
    }
}
