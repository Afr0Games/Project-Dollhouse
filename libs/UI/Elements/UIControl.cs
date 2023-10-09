/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Elements
{
    /// <summary>
    /// A control is most often used to represent images, but can represent anything.
    /// </summary>
    public class UIControl : UIElement, IBasicDrawable
    {
        /// <summary>
        /// The size of a thumbnail for an image-browsing control.
        /// </summary>
        protected Vector2 m_ThumbSize;

        /// <summary>
        /// The thumbnail margins for an image-browsing control.
        /// </summary>
        protected Vector2 m_ThumbMargins;

        /// <summary>
        /// The size of an image inside a thumbnail in an image-browsing control.
        /// </summary>
        protected Vector2 m_ThumbImageSize;

        /// <summary>
        /// Offsets for images inside thumbnails in an image-browsing control.
        /// </summary>
        protected Vector2 m_ThumbImageOffsets;

        /// <summary>
        /// The thumbnail button image for an image-browsing control.
        /// </summary>
        protected string m_ThumbButtonImage;

        protected string m_LeftArrowImage, m_RightArrowImage;

        /// <summary>
        /// Copy constructor for the UIControl class.
        /// This constructor will deep copy another UIControl instance.
        /// </summary>
        /// <param name="Ctrl">A UIControl instance to copy.</param>
        /// <param name="Screen">A UIScreen instance that this UIControl belongs to.</param>
        public UIControl(UIControl Ctrl, UIScreen Screen) : base(Screen)
        {
            Image = Ctrl.Image;
            Position = Ctrl.Position;
            m_Size = Ctrl.Size;
            m_LeftArrowImage = Ctrl.m_LeftArrowImage;
            m_RightArrowImage = Ctrl.m_RightArrowImage;
            m_ThumbSize = Ctrl.m_ThumbSize;
            m_ThumbMargins = Ctrl.m_ThumbMargins;
            m_ThumbImageSize = Ctrl.m_ThumbImageSize;
            m_ThumbImageOffsets = Ctrl.m_ThumbImageOffsets;
            m_ThumbButtonImage = Ctrl.m_ThumbButtonImage;
        }

        public UIControl(SetControlPropsNode Node, UIScreen Screen, UIParser.ParserState State) : base(Screen)
        {
            Name = Node.Control;

            if (!State.InSharedPropertiesGroup)
            {
                if (Node.Image != "")
                    Image = m_Screen.GetImage(Node.Image, false);
            }
            else
            {
                if(State.Image != "")
                    Image = m_Screen.GetImage(State.Image, true);

                if (State.LeftArrowImage != "")
                    m_LeftArrowImage = State.LeftArrowImage;

                if (State.RightArrowImage != "")
                    m_RightArrowImage = State.RightArrowImage;
            }

            if (Node.PositionAssignment != null)
                Position = (Vector2)Node.PositionAssignment;

            if(Node.Size != null)
                m_Size = (Vector2)Node.Size;

            if(Node.ThumbSize != null)
                m_ThumbSize = (Vector2)Node.ThumbSize;

            if (Node.ThumbMargins != null)
                m_ThumbMargins = new Vector2(Node.ThumbMargins.Numbers[0], Node.ThumbMargins.Numbers[1]);

            if (Node.ThumbImageSize != null)
                m_ThumbImageSize = Node.ThumbImageSize;

            if (Node.ThumbImageOffsets != null)
                m_ThumbImageOffsets = new Vector2(Node.ThumbImageOffsets.Numbers[0], Node.ThumbImageOffsets.Numbers[1]);

            if (Node.ThumbButtonImage != null)
                m_ThumbButtonImage = Node.ThumbButtonImage;
        }

        public override void Draw(SpriteBatch SBatch)
        {
            if (Image != null)
                Image.Draw(SBatch);
        }
    }
}
