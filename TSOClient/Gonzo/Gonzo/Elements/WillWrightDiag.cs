using System;
using System.Collections.Generic;
using System.Text;
using Files;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gonzo.Elements
{
    public class WillWrightDiag : UIDialog
    {
        private UIImage m_WillWrightImg;

        public WillWrightDiag(UIImage Img, UIScreen Screen, Vector2 Position) : base(Screen, Position, true, true, true)
        {
            //Texture2D WillWrightImg = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.creditscreen_will);
            m_WillWrightImg = Img; //new UIImage(WillWrightImg, Screen, null);
            Image.SetSize(m_WillWrightImg.Texture.Width + 40, m_WillWrightImg.Texture.Height + 20);
            CenterAround(m_WillWrightImg, 50, -50);
        }

        public override void Update(InputHelper Helper)
        {
            if (IsDrawn)
            {
                CenterAround(m_WillWrightImg, 50, -50);

                if (m_DoDrag)
                    m_WillWrightImg.Position = (Helper.MousePosition - m_DragOffset);
            }

            base.Update(Helper);
        }

        public override void Draw(SpriteBatch SBatch)
        {
            base.Draw(SBatch);

            if(IsDrawn)
                SBatch.Draw(m_WillWrightImg.Texture, Position, Color.White);
        }
    }
}
