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

        public UISlider(AddSliderNode Node, UIScreen Screen) : base(Screen)
        {
            m_Name = Node.Name;
            m_ID = Node.ID;

            Image = m_Screen.GetImage(Node.Image);

            Position = new Vector2(Node.SliderPosition.Numbers[0], Node.SliderPosition.Numbers[1]);

            if (Node.MinimumValue != null)
                m_Minimumvalue = (int)Node.MinimumValue;
            if (Node.MaximumValue != null)
                m_MaximumValue = (int)Node.MaximumValue;

            m_Size = new Vector2(Node.Size.Numbers[0], Node.Size.Numbers[1]);
            m_Orientation = (SliderOrientation)Node.Orientation;
        }
    }
}
