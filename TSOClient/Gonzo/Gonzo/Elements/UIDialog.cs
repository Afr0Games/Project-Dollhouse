using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Files;
using Files.Manager;

namespace Gonzo.Elements
{
    public class UIDialog : UIElement
    {
        public UIButton CloseButton, OkCheckButton;
        private UIImage m_CloseBtnBack;
        private bool m_IsDraggable;

        public UIDialog(UIScreen Screen, Vector2 Pos, bool IsTall, bool IsDraggable) : base(Screen)
        {
            Position = Pos;
            Position *= m_Screen.Scale;

            m_IsDraggable = IsDraggable;

            //TODO: Find a way to NOT hardcode these references.
            Texture2D Tex = (IsTall != false) ? 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_backgroundtemplatetall) : 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_backgroundtemplate);

            Texture2D CloseBtnBackground = (IsTall != false) ? 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtnbackgroundtall) : 
                FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtnbackground);

            Image = new UIImage(Tex, Screen, this);
            Image.Slicer = new NineSlicer(Position, Tex.Width, Tex.Height);
            Image.Position = new Vector2(Pos.X, Pos.Y);

            m_CloseBtnBack = new UIImage(CloseBtnBackground, Screen, this);
            m_CloseBtnBack.Position = new Vector2(((CloseBtnBackground.Width * m_Screen.Scale.X) * 2), 0);
            m_CloseBtnBack.Position *= m_Screen.Scale;

            Texture2D CloseButtonTex = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_closebtn);
            CloseButton = new UIButton("CloseBtn", CloseButtonTex,
                new Vector2((Tex.Width - (CloseButtonTex.Width / (4 * m_Screen.Scale.X))), 12), Screen, this);
        }

        public override void Update(InputHelper Helper)
        {
            CloseButton.Update(Helper);

            if(m_IsDraggable)
            {
                if (IsMouseOver(Helper))
                {
                    if (Helper.IsCurPress(MouseButtons.LeftButton) || Helper.IsOldPress(MouseButtons.LeftButton))
                    {
                        //Position = Helper.MousePosition - Position;
                        Image.Position = Helper.MousePosition - Image.Position;
                        Image.Slicer.Calculate();

                        m_CloseBtnBack.Position = Helper.MousePosition - m_CloseBtnBack.Position;

                        CloseButton.Image.Position = new Vector2(Helper.MousePosition.X - CloseButton.Image.Position.X, Helper.MousePosition.Y - CloseButton.Image.Position.Y);
                    }
                }
            }
        }

        public override bool IsMouseOver(InputHelper Input)
        {
            if (Input.MousePosition.X > Position.X && Input.MousePosition.X <= (Position.X + ((Image.Texture.Width * m_Screen.Scale.X)) * 3))
            {
                if (Input.MousePosition.Y > Position.Y && Input.MousePosition.Y <= (Position.Y + ((Image.Texture.Height * m_Screen.Scale.Y)) * 3))
                    return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch SBatch)
        {
            Image.Draw(SBatch, Image.Slicer.TLeft);
            Image.Draw(SBatch, Image.Slicer.TCenter);
            Image.Draw(SBatch, Image.Slicer.TRight);

            Image.Draw(SBatch, Image.Slicer.CLeft);
            Image.Draw(SBatch, Image.Slicer.CCenter);
            Image.Draw(SBatch, Image.Slicer.CRight);

            Image.Draw(SBatch, Image.Slicer.BLeft);
            Image.Draw(SBatch, Image.Slicer.BCenter);
            Image.Draw(SBatch, Image.Slicer.BRight);

            m_CloseBtnBack.Draw(SBatch, null);
            CloseButton.Draw(SBatch);
        }
    }
}
