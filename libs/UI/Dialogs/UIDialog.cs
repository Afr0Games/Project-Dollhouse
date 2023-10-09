using System.Diagnostics;
using System.Collections.Generic;
using UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Files;
using Files.Manager;
using System.Reflection;

namespace UI.Dialogs
{
    public delegate void OnDraggedEventHandler(Vector2 MousePosition, Vector2 DragOffset);

    public class UIDialog : UIElement
    {
        private UIButton m_CloseButton;
        private UIImage m_CloseBtnBack;

        protected Vector2 m_DefaultSize;

        /// <summary>
        /// Is this dialog see-through?
        /// </summary>
        protected float m_Opacity = 255.0f;

        /// <summary>
        /// Event called when this UIDialog is being dragged.
        /// </summary>
        public event OnDraggedEventHandler OnDragged;

        /// <summary>
        /// UIDialogs can have elements that need to be registered for rendering and updates.
        /// </summary>
        public Dictionary<string, UIElement> RegistrableUIElements = new Dictionary<string, UIElement>();

        private bool m_HasExitBtn = false;

        /// <summary>
        /// Constructs a new UIDialog instance.
        /// NOTE: If the dialog has an exit button, IsTall NEEDS to be set to false,
        /// otherwise it won't draw properly!!
        /// </summary>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Pos">A Vector2 instance specifying the position of this dialog.</param>
        /// <param name="IsTall">Will this dialog use a tall background template?</param>
        /// <param name="IsDraggable">Is this dialog draggable?</param>
        /// <param name="HasExitButton">Does this dialog have an exit button?</param>
        /// <param name="Opacity">The opacity of this UIDialog. Defaults to 255.</param>
        public UIDialog(UIScreen Screen, Vector2 Pos, bool IsTall, bool IsDraggable, bool HasExitButton,
            float Opacity = 255) : base(Screen)
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

            m_Opacity = Opacity;
            Image = new UIImage(Tex, new Vector2(Pos.X, Pos.Y), Screen, null, m_Opacity);

            //Set the default size of the dialog.
            m_DefaultSize = new Vector2(Tex.Width, Tex.Height);
            SetSize(m_DefaultSize.X, m_DefaultSize.Y);

            if (IsTall != false)
                Image.Slicer = new NineSlicer(new Vector2(0, 0), Tex.Width, Tex.Height, 41, 41, 66, 40);
            else
                Image.Slicer = new NineSlicer(new Vector2(0, 0), Tex.Width, Tex.Height, 41, 41, 60, 40);

            Texture2D CloseButtonTex = FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtn);

            if (m_HasExitBtn)
            {
                //This needs to start drawing at 1 on the Y axis to render correctly.
                m_CloseBtnBack = new UIImage(CloseBtnBackground,
                    new Vector2(Image.Slicer.Width - CloseBtnBackground.Width, 1), Screen, this, m_Opacity);
                Children.Add(m_CloseBtnBack);

                m_CloseButton = new UIButton("CloseBtn",
                    new Vector2(Image.Slicer.Width - (CloseButtonTex.Width / 2.5f), 9f), Screen, CloseButtonTex, "", 9,
                    true, this);
                m_CloseButton.OnButtonClicked += CloseButton_OnButtonClicked;
                m_CloseButton.ZIndex = this.ZIndex + 1;
                Children.Add(m_CloseButton);
            }
        }

        private void CloseButton_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            ToggleVisibility(false);
        }

        /// <summary>
        /// Processes mouse events for this UIDialog instance.
        /// </summary>
        /// <param name="Helper">An InputHelper instance.</param>
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
        public void SetSize(float Width, float Height)
        {
            m_Size.X = Width;
            m_Size.Y = Height;
            Image.SetSize(Width, Height);
        }

        /// <summary>
        /// Toggles on or off visibility for this UIDialog instance.
        /// </summary>
        /// <param name="Visible">Should this dialog be visible or not?</param>
        public void ToggleVisibility(bool Visible)
        {
            this.Visible = Visible;

            foreach (UIElement Child in Children)
                Child.Visible = Visible;
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            if (Visible)
            {
                foreach (var Child in Children.OrderBy(C => C.ZIndex))
                    Child.Update(Helper, GTime);

                if (m_IsDraggable)
                {
                    if (m_DoDrag)
                    {
                        OnDragged(Helper.MousePosition, m_DragOffset);

                        Position = (Helper.MousePosition - m_DragOffset);

                        if (m_HasExitBtn)
                        {
                            //This needs to start drawing at 1 on the Y axis to render correctly.
                            Vector2 OffsetFromMouse = new Vector2(Image.Slicer.Width - m_CloseBtnBack.Texture.Width, 1);
                            m_CloseBtnBack.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;

                            OffsetFromMouse = new Vector2(Image.Slicer.Width - (m_CloseButton.Image.Texture.Width / 2.5f), 9f);
                            m_CloseButton.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                        }
                    }

                    MouseEvents(Helper);
                }
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

        private bool m_AdjustDepth = true;

        public override void Draw(SpriteBatch SBatch)
        {
            if (Visible)
            {
                if (m_DoDrag)
                {
                    if (Debugger.IsAttached) //Are we in debug mode?
                        SBatch.DrawString(m_Font, "Position: " + Position.X.ToString() + ", " + Position.Y.ToString(),
                            new Vector2(0, 0), Color.Wheat);
                }

                Image.DrawTextureTo(SBatch, null, Image.Slicer.TLeft, Image.Position + Vector2.Zero);
                Image.DrawTextureTo(SBatch, Image.Slicer.TCenter_Scale, Image.Slicer.TCenter, Image.Position +
                    new Vector2(Image.Slicer.LeftPadding, 0));
                if (!m_HasExitBtn)
                    Image.DrawTextureTo(SBatch, null, Image.Slicer.TRight, Image.Position +
                        new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, 0));

                Image.DrawTextureTo(SBatch, Image.Slicer.CLeft_Scale, Image.Slicer.CLeft, Image.Position +
                    new Vector2(0, Image.Slicer.TopPadding));
                Image.DrawTextureTo(SBatch, Image.Slicer.CCenter_Scale, Image.Slicer.CCenter, Image.Position +
                    new Vector2(Image.Slicer.LeftPadding, Image.Slicer.TopPadding));
                Image.DrawTextureTo(SBatch, Image.Slicer.CRight_Scale, Image.Slicer.CRight, Image.Position +
                    new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, Image.Slicer.TopPadding));

                int BottomY = Image.Slicer.Height - Image.Slicer.BottomPadding;
                Image.DrawTextureTo(SBatch, null, Image.Slicer.BLeft, Image.Position +
                    new Vector2(0, BottomY));
                Image.DrawTextureTo(SBatch, Image.Slicer.BCenter_Scale, Image.Slicer.BCenter, Image.Position +
                    new Vector2(Image.Slicer.LeftPadding, BottomY));
                Image.DrawTextureTo(SBatch, null, Image.Slicer.BRight, Image.Position +
                    new Vector2(Image.Slicer.Width - Image.Slicer.RightPadding, BottomY));

                /*if (m_HasExitBtn)
                    m_CloseBtnBack.Draw(SBatch);*/

                foreach (var Child in Children.OrderBy(C => C.ZIndex))
                {
                    if (Child is UIElement UIElementChild)
                        UIElementChild.Draw(SBatch);
                }
            }
        }
    }
}
