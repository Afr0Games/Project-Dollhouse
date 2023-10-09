/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using UIParser.Nodes;
using UI.Dialogs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI.Elements
{
    /// <summary>
    /// A text label for the UI.
    /// Mostly used for displaying static text.
    /// </summary>
    public class UILabel : UIElement, IBasicDrawable
    {
        private Color m_TextColor;
        private TextAlignment m_Alignment;

        private bool m_TextHasBeenUpdated = false;

        private string m_Caption = "";
        public string Caption
        {
            get { return m_Caption; }
            set 
            {
                m_Caption = value;

                Vector2 StringDimensions = m_Font.MeasureString(Caption);
                m_TextTex = new RenderTarget2D(GraphicsDevice, 
                    ((int)StringDimensions.X != 0) ? (int)StringDimensions.X : 1, 
                    ((int)StringDimensions.Y != 0) ? (int)StringDimensions.Y : 1);

                m_TextHasBeenUpdated = true;
            }
        }

        //For rendering static text.
        private RenderTarget2D m_TextTex;

        public UILabel(AddTextNode Node, ParseResult Result, UIScreen Screen) : base(Screen)
        {
            Name = Node.Name;
            m_ID = Node.ID;
            Position = new Vector2(Node.TextPosition.X, Node.TextPosition.Y) + Screen.Position;

            DrawOrder = (int)DrawOrderEnum.UI;

            if (Node.Size != null)
                m_Size = new Vector2(((Vector2)Node.Size).X, ((Vector2)Node.Size).Y);
            else
                m_Size = (Vector2)Result.State.Size;

            if (Node.Color != null)
            {
                m_TextColor = (Color)Node.Color;
                m_TextColor.A = 255; //Ignore opacity, The Sims Online doesn't support transparent text.
            }
            else
            {
                m_TextColor = (Color)Result.State.Color;
                m_TextColor.A = 255; //Ignore opacity, The Sims Online doesn't support transparent text.
            }

            if (Node.Alignment != 0)
                m_Alignment = Node.Alignment;
            else
                m_Alignment = (TextAlignment)Result.State.Alignment;

            int Font = 0;
            if (Node.Font != null)
            {
                if (Node.Font != 0)
                    Font = (int)Node.Font;
            }
            else
                Font = Result.State.Font;

            switch (Font)
            {
                    case 7:
                        m_Font = Screen.Font9px; //TODO: Fixme.
                        break;
                    case 9:
                    m_Font = Screen.Font9px;
                        break;
                    case 10:
                        m_Font = Screen.Font10px;
                        break;
                    case 11:
                        m_Font = Screen.Font11px;
                        break;
                    case 12:
                        m_Font = Screen.Font12px;
                        break;
                    case 14:
                        m_Font = Screen.Font14px;
                        break;
                    case 16:
                        m_Font = Screen.Font16px;
                        break;
            }

            if (Node.Text != "")
                Caption = Result.Strings[Node.Text];
            else
            {
                if (Result.State.Caption != "")
                    //Sometimes labels will not have pre-defined text, as they will hold text
                    //generated in-game.
                    Caption = Result.Strings[Result.State.Caption];
            }

            AlignText();
        }

        public UILabel(string StrCaption, int ID, Vector2 TextPosition, Vector2 Size, Color Clr, int Font,
            UIScreen Screen, UIElement Parent = null, TextAlignment Alignment = TextAlignment.Center_Center) : base(Screen)
        {
            Name = "Lbl" + Caption;
            m_ID = ID;
            Position = new Vector2(TextPosition.X, TextPosition.Y) + Screen.Position;

            if (Parent != null)
            {
                m_Parent = Parent;

                //Would a UILabel ever be attached to anything but a UIDialog instance? Probably not.
                UIDialog Dialog = (UIDialog)Parent;
                Dialog.OnDragged += Dialog_OnDragged;

                //DrawOrder = Parent.DrawOrder;
            }

            m_Size = new Vector2(Size.X, Size.Y);

            m_TextColor = new Color();
            m_TextColor.A = 255; //Ignore opacity, The Sims Online doesn't support transparent text.
            m_TextColor.R = Clr.R;
            m_TextColor.G = Clr.G;
            m_TextColor.B = Clr.B;

            m_Alignment = Alignment;

            switch (Font)
            {
                case 7:
                    m_Font = Screen.Font9px; //TODO: Fixme.
                    break;
                case 9:
                    m_Font = Screen.Font9px;
                    break;
                case 10:
                    m_Font = Screen.Font10px;
                    break;
                case 11:
                    m_Font = Screen.Font11px;
                    break;
                case 12:
                    m_Font = Screen.Font12px;
                    break;
                case 14:
                    m_Font = Screen.Font14px;
                    break;
                case 16:
                    m_Font = Screen.Font16px;
                    break;
            }

            Caption = StrCaption;

            AlignText();
        }

        /// <summary>
        /// The position of this UILabel on the X coordinate scale. Used for tweening.
        /// </summary>
        public float XPosition
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        /// <summary>
        /// The position of this UILabel on the Y coordinate scale. Used for tweening.
        /// </summary>
        public float YPosition
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        private void AlignText()
        {
            Vector2 Measurement = m_Font.MeasureString(Caption);
            Vector2 LocalCopy = Position;

            switch (m_Alignment)
            {
                case TextAlignment.Left_Top:
                    //Is there a need to modify position at all here??
                    break;
                case TextAlignment.Left_Center:
                    LocalCopy.Y += (m_Size.Y / 2) - (Measurement.Y / 2);
                    Position = LocalCopy;
                    break;
                case TextAlignment.Center_Top:
                    LocalCopy.X += (m_Size.X / 2) - (Measurement.X / 2);
                    Position = LocalCopy;
                    break;
                case TextAlignment.Center_Center:
                    LocalCopy.X += (m_Size.X / 2) - (Measurement.X / 2);
                    LocalCopy.Y += (m_Size.Y / 2) - (Measurement.Y / 2);
                    Position = LocalCopy;
                    break;
                case TextAlignment.Right_Top:
                    LocalCopy.X += m_Size.X - Measurement.X;
                    Position = LocalCopy;
                    break;
                case TextAlignment.Right_Center:
                    LocalCopy.X += m_Size.X - Measurement.X;
                    LocalCopy.Y += (m_Size.Y / 2) - (Measurement.Y / 2);
                    Position = LocalCopy;
                    break;
            }
        }

        /// <summary>
        /// This UIButton instance is attached to a dialog, and the dialog is being dragged.
        /// </summary>
        /// <param name="MousePosition">The mouse position.</param>
        /// <param name="DragOffset">The dialog's drag offset.</param>
        private void Dialog_OnDragged(Vector2 MousePosition, Vector2 DragOffset)
        {
            Vector2 RelativePosition = Position - m_Parent.Position;

            Position = (MousePosition + RelativePosition) - DragOffset;
        }

        private void PreRenderText()
        {
            SpriteBatch SBatch = new SpriteBatch(GraphicsDevice);

            GraphicsDevice.SetRenderTarget(m_TextTex);

            SBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, 
                null, null);
            GraphicsDevice.Clear(Color.Transparent);
            SBatch.DrawString(m_Font, Caption, Vector2.Zero, m_TextColor);
            SBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            //Because labels are infrequently updated (if at all), this is OK.
            if (m_TextHasBeenUpdated)
            {
                PreRenderText();

                m_TextHasBeenUpdated = false;
            }
        }

        public override void Draw(SpriteBatch SBatch)
        {
            if (Caption != "")
            {
                if (Visible)
                {
                    SBatch.Draw(m_TextTex, Position, null, Color.White, 0.0f, new Vector2(0.0f, 0.0f), 1.0f, 
                        SpriteEffects.None, 0);
                }
            }
        }
    }
}
