using System.Diagnostics;
using System.Collections.Generic;
using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Files;
using Files.Manager;

namespace Gonzo.Dialogs
{
    public class UIDialog : UIElement
    {
        private UIButton m_CloseButton;
        private UIImage m_CloseBtnBack;

        /// <summary>
        /// UIDialogs can have elements that need to be registered for rendering and updates.
        /// </summary>
        public Dictionary<string, UIElement> RegistrableUIElements = new Dictionary<string, UIElement>();

        private bool m_HasExitBtn = false;

        /// <summary>
        /// Constructs a new UIDialog instance.
        /// </summary>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Pos">A Vector2 instance specifying the position of this dialog.</param>
        /// <param name="IsTall">Will this dialog use a tall background template?</param>
        /// <param name="IsDraggable">Is this dialog draggable?</param>
        /// <param name="HasExitButton">Does this dialog have an exit button?</param>
        public UIDialog(UIScreen Screen, Vector2 Pos, bool IsTall, bool IsDraggable, bool HasExitButton) : base(Screen)
        {
            Position = Pos;
            m_IsDraggable = IsDraggable;
            m_HasExitBtn = HasExitButton;

            Texture2D Tex = (IsTall != false) ? 
                FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_backgroundtemplatetall, true) : 
                FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_backgroundtemplate, true);

            Texture2D CloseBtnBackground = (IsTall != false) ? 
                FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtnbackgroundtall) : 
                FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtnbackground);

            Image = new UIImage(Tex, Screen, null);

            if (IsTall != false)
                Image.Slicer = new NineSlicer(new Vector2(0, 0), Tex.Width, Tex.Height, 41, 41, 66, 40);
            else
                Image.Slicer = new NineSlicer(new Vector2(0, 0), Tex.Width, Tex.Height, 41, 41, 60, 40);

            Image.Position = new Vector2(Pos.X, Pos.Y);

            m_CloseBtnBack = new UIImage(CloseBtnBackground, Screen, null);
            m_CloseBtnBack.Position = Position + new Vector2(Image.Slicer.Width - m_CloseBtnBack.Texture.Width, 0);

            Texture2D CloseButtonTex = FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtn);
            m_CloseButton = new UIButton("CloseBtn", 
                Position + new Vector2(Image.Slicer.Width - (CloseButtonTex.Width / 2.5f), 9f), Screen, CloseButtonTex);
            m_CloseButton.OnButtonClicked += CloseButton_OnButtonClicked;
        }

        private void CloseButton_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            Visible = false;
        }

        public override void MouseEvents(InputHelper Helper)
        {
            //Left button has been pressed.
            if(Helper.IsNewPress(MouseButtons.LeftButton) == true && 
                Helper.IsOldPress(MouseButtons.LeftButton) == false)
            {
                if (IsMouseOver(Helper))
                {
                    m_DragOffset = Helper.MousePosition / 5.0f;
                    m_DoDrag = true;
                }
            }

            //Left button has been released.
            if (Helper.IsNewPress(MouseButtons.LeftButton) == false &&
                Helper.IsOldPress(MouseButtons.LeftButton) == true)
            {
                m_DoDrag = false;
            }

        }

        /// <summary>
        /// Sets the size of this UIDialog instance.
        /// </summary>
        /// <param name="Width">The width to set it to.</param>
        /// <param name="Height">The height to set it to.</param>
        public void SetSize(int Width, int Height)
        {
            m_Size.X = Width;
            m_Size.Y = Height;
            Image.SetSize(Width, Height);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            if (Visible)
            {
                if (m_IsDraggable)
                {
                    if (m_DoDrag)
                    {
                        Position = (Helper.MousePosition - m_DragOffset);

                        if (m_HasExitBtn)
                        {
                            Vector2 OffsetFromMouse = new Vector2(Image.Slicer.Width - m_CloseBtnBack.Texture.Width, 0);
                            m_CloseBtnBack.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;

                            OffsetFromMouse = new Vector2(Image.Slicer.Width - (m_CloseButton.Image.Texture.Width / 2.5f), 9f);
                            m_CloseButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                        }
                    }

                    MouseEvents(Helper);
                }

                m_CloseButton.Update(Helper, GTime);
            }
        }

        public override bool IsMouseOver(InputHelper Input)
        {
            if (Input.MousePosition.X > Position.X && Input.MousePosition.X <= (Position.X + (Image.Texture.Width)))
            {
                if (Input.MousePosition.Y > Position.Y && Input.MousePosition.Y <= (Position.Y + (Image.Texture.Height)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Centers this dialog around a UIElement (UIImage, UILabel or similar).
        /// </summary>
        /// <param name="Element">The element to center around.</param>
        /// <param name="OffsetX">Offset on the X-axis when centering.</param>
        /// <param name="OffsetY">Offset on the Y-axis when centering.</param>
        public void CenterAround(UIElement Element, int OffsetX, int OffsetY)
        {
            Vector2 TopLeft = Element.Position; 

            Position = new Vector2(OffsetX + TopLeft.X + ((Element.Size.X - m_Size.X) / 2), 
                OffsetY + TopLeft.Y + ((Element.Size.Y - m_Size.Y) / 2));

            if(m_HasExitBtn)
            {
                m_CloseBtnBack.Position = Position + new Vector2(Image.Slicer.Width - m_CloseBtnBack.Texture.Width, 0);
                m_CloseButton.Position = Position + new Vector2(Image.Slicer.Width - (m_CloseButton.Image.Texture.Width / 2.5f), 9f);
            }
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.10f;

            if (Visible)
            {
                Image.DrawTextureTo(SBatch, null, Image.Slicer.TLeft, Image.Position + Vector2.Zero, Depth);
                Image.DrawTextureTo(SBatch, Image.Slicer.TCenter_Scale, Image.Slicer.TCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, 0), Depth);
                Image.DrawTextureTo(SBatch, null, Image.Slicer.TRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, 0), Depth);

                Image.DrawTextureTo(SBatch, Image.Slicer.CLeft_Scale, Image.Slicer.CLeft, Image.Position + new Vector2(0, Image.Slicer.TopPadding), null);
                Image.DrawTextureTo(SBatch, Image.Slicer.CCenter_Scale, Image.Slicer.CCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, Image.Slicer.TopPadding), Depth);
                Image.DrawTextureTo(SBatch, Image.Slicer.CRight_Scale, Image.Slicer.CRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, Image.Slicer.TopPadding), Depth);

                int BottomY = Image.Slicer.Height - Image.Slicer.BottomPadding;
                Image.DrawTextureTo(SBatch, null, Image.Slicer.BLeft, Image.Position + new Vector2(0, BottomY), null);
                Image.DrawTextureTo(SBatch, Image.Slicer.BCenter_Scale, Image.Slicer.BCenter, Image.Position + new Vector2(Image.Slicer.LeftPadding, BottomY), Depth);
                Image.DrawTextureTo(SBatch, null, Image.Slicer.BRight, Image.Position + new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, BottomY), Depth);

                if(m_DoDrag)
                {
                    if(Debugger.IsAttached) //Are we in debug mode?
                        SBatch.DrawString(m_Font, "Position: " + Position.X.ToString() + ", " + Position.Y.ToString(), 
                            new Vector2(0, 0), Color.Wheat);
                }

                if (m_HasExitBtn)
                {
                    m_CloseBtnBack.Draw(SBatch, null, Depth);
                    m_CloseButton.Draw(SBatch, Depth);
                }
            }
        }
    }
}
