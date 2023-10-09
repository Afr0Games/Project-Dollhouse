/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;

namespace UI.Elements
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
            Name = Node.Name;
            m_ID = Node.ID;
            Position = Node.SliderPosition;

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
                m_Size = (Vector2)Node.Size;
                m_Orientation = (SliderOrientation)Node.Orientation;
            }
            else
            {
                m_Size = (Vector2)State.Size;
                m_Orientation = (SliderOrientation)State.Orientation;
            }
        }
    }
}
