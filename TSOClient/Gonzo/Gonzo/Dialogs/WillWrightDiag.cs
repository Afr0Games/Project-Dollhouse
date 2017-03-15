using Gonzo.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Gonzo.Dialogs
{
    public class WillWrightDiag : UIDialog
    {
        private UIImage m_WillWrightImg;

        public WillWrightDiag(UIImage Img, UIScreen Screen, Vector2 Position) : base(Screen, Position, true, true, true)
        {
            m_WillWrightImg = Img;
            m_WillWrightImg.Position = Position;
            Image.SetSize(m_WillWrightImg.Texture.Width + 50, m_WillWrightImg.Texture.Height + 55);
            CenterAround(m_WillWrightImg, -22, -42);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            if (Visible)
            {
                if (m_DoDrag)
                    m_WillWrightImg.Position = Position - new Vector2(-22, -42);
            }

            base.Update(Helper, GTime);
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.10f;

            if (Visible)
                SBatch.Draw(m_WillWrightImg.Texture, m_WillWrightImg.Position, null, null, new Vector2(0.0f, 0.0f), 0.0f, null, 
                    Color.White, SpriteEffects.None, Depth);

            base.Draw(SBatch, LayerDepth);
        }
    }
}
