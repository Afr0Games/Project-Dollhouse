using System;
using System.Collections.Generic;
using System.Text;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    public enum SliderOrientation
    {
        Horizontal = 0,
        Vertical = 1
    }

    public class UISlider : UIElement
    {
        //Values associated with the small and big end of this slider, respectively.
        private int m_Minimumvalue = 0, m_MaximumValue = 0;
        private SliderOrientation m_Orientation;

        public UISlider(AddSliderNode Node, ParserState State, UIScreen Screen) : base(Screen)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;
            Position = new Vector2(Node.SliderPosition.Numbers[0], Node.SliderPosition.Numbers[1]);

            if (!State.InSharedPropertiesGroup)
            {
                Image = m_Screen.GetImage(Node.Image);
                Image.Position = Position;
            }
            else
            {
                Image = m_Screen.GetImage(State.Image);
                Image.Position = Position;
            }

            if (Node.MinimumValue != null)
                m_Minimumvalue = (int)Node.MinimumValue;
            if (Node.MaximumValue != null)
                m_MaximumValue = (int)Node.MaximumValue;

            if (!State.InSharedPropertiesGroup)
            {
                m_Size = new Vector2(Node.Size.Numbers[0], Node.Size.Numbers[1]);
                m_Orientation = (SliderOrientation)Node.Orientation;
            }
            else
            {
                m_Size = State.Size;
                m_Orientation = (SliderOrientation)State.Orientation;
            }
        }
    }
}
