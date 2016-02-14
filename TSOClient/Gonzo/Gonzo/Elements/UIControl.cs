using System;
using System.Collections.Generic;
using System.Text;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    /// <summary>
    /// A control is most often used to represent images, but can represent anything.
    /// </summary>
    public class UIControl : UIElement
    {
        public UIControl(SetControlPropsNode Node, UIScreen Screen) : base(Screen)
        {
            m_Name = Node.Control;

            if(Node.Image != "")
                Image = m_Screen.GetImage(Node.Image);

            if (Node.PositionAssignment != null)
            {
                Position = new Vector2(Node.PositionAssignment[0], Node.PositionAssignment[1]);

                if(Image != null)
                    Image.Position = new Vector2(Position.X, Position.Y);
            }

            if(Node.Size != null)
                m_Size = new Vector2(Node.Size.Numbers[0], Node.Size.Numbers[1]);
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.0f;

            if (Image != null)
                Image.Draw(SBatch, null, Depth);
        }
    }
}
