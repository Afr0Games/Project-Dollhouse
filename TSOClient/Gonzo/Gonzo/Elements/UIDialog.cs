using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Files;
using Files.Manager;
using Microsoft.Xna.Framework.Input;

namespace Gonzo.Elements
{
    public class UIDialog : UIElement
    {
        public UIButton CloseButton, OkCheckButton;
        private UIImage m_CloseBtnBack;

        private bool m_IsDraggable;
        private bool m_DoDrag = false;
        private Vector2 m_DragOffset;

        private bool m_HasExitBtn = false;

        public UIDialog(UIScreen Screen, Vector2 Pos, bool IsTall, bool IsDraggable, bool HasExitButton) : base(Screen)
        {
            m_IsDraggable = IsDraggable;
            m_HasExitBtn = HasExitButton;

            //TODO: Find a way to NOT hardcode these references.
            Texture2D Tex = (IsTall != false) ? 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_backgroundtemplatetall) : 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_backgroundtemplate);

            Texture2D CloseBtnBackground = (IsTall != false) ? 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtnbackgroundtall) : 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtnbackground);

            Image = new UIImage(Tex, Screen, null);

            if (IsTall != false)
                Image.Slicer = new NineSlicer(Position, Tex.Width, Tex.Height, 41, 41, 66, 40);
            else
                Image.Slicer = new NineSlicer(Position, Tex.Width, Tex.Height, 41, 41, 60, 40);

            Image.Position = new Vector2(Pos.X, Pos.Y);
            m_Elements.Add("Background", Image);

            //Not entirely sure why this needs to have a parent, but it works. :)
            m_CloseBtnBack = new UIImage(CloseBtnBackground, Screen, this);
            m_CloseBtnBack.Position = new Vector2(((CloseBtnBackground.Width) * 2), 0);
            m_Elements.Add("CloseBtnBackground", m_CloseBtnBack);

            Texture2D CloseButtonTex = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtn);
            CloseButton = new UIButton("CloseBtn", CloseButtonTex,
                new Vector2((Tex.Width - (CloseButtonTex.Width / 4)), 12), Screen, this);
            m_Elements.Add("CloseBtn", CloseButton);
        }

        public override void MouseEvents(InputHelper Helper)
        {
            switch(Helper.CurrentMouseState.LeftButton)
            {
                case ButtonState.Pressed:
                    if (IsMouseOver(Helper))
                    {
                        m_DragOffset = Helper.CurrentMouseState.Position.ToVector2();
                        m_DoDrag = true;
                    }
                    break;
                case ButtonState.Released:
                    m_DoDrag = false;
                    break;
            }
        }

        public override void Update(InputHelper Helper)
        {
            if (m_IsDraggable)
            {
                if (m_DoDrag)
                {
                    Position = (Helper.MousePosition - m_DragOffset);

                    if (m_HasExitBtn)
                    {
                        Vector2 OffsetFromMouse = new Vector2(((m_CloseBtnBack.Texture.Width) * 2), 0);
                        m_CloseBtnBack.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;

                        OffsetFromMouse = new Vector2((CloseButton.Image.Texture.Width) * 2.89f, 9f);
                        CloseButton.Position = (Helper.MousePosition + (OffsetFromMouse))  - m_DragOffset;
                    }
                }

                this.MouseEvents(Helper);
            }

            CloseButton.Update(Helper);
        }

        public override bool IsMouseOver(InputHelper Input)
        {
            if (Input.MousePosition.X > Position.X && Input.MousePosition.X <= (Position.X + (Image.Texture.Width) /** 3*/))
            {
                if (Input.MousePosition.Y > Position.Y && Input.MousePosition.Y <= (Position.Y + (Image.Texture.Height) /** 3*/))
                    return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch SBatch)
        {
            Image.Draw(SBatch, null, Image.Slicer.TLeft);
            Image.Draw(SBatch, Image.Slicer.TCenter_Scale, Image.Slicer.TCenter);
            Image.Draw(SBatch, null, Image.Slicer.TRight);

            Image.Draw(SBatch, Image.Slicer.CLeft_Scale, Image.Slicer.CLeft);
            Image.Draw(SBatch, Image.Slicer.CCenter_Scale, Image.Slicer.CCenter);
            Image.Draw(SBatch, Image.Slicer.CRight_Scale, Image.Slicer.CRight);

            Image.Draw(SBatch, null, Image.Slicer.BLeft);
            Image.Draw(SBatch, Image.Slicer.BCenter_Scale, Image.Slicer.BCenter);
            Image.Draw(SBatch, null, Image.Slicer.BRight);

            CloseButton.DrawBorder(SBatch, new Rectangle((int)CloseButton.Position.X, (int)CloseButton.Position.Y,
                CloseButton.Image.Texture.Width / 3, CloseButton.Image.Texture.Height), 5, Color.Red);

            if (m_HasExitBtn)
            {
                m_CloseBtnBack.Draw(SBatch, null, null);
                CloseButton.Draw(SBatch);
            }
        }
    }
}
