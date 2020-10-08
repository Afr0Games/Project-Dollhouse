using System;
using Microsoft.Xna.Framework.Graphics;
using Files.IFF;
using Microsoft.Xna.Framework;
using MonoGame.Forms.Controls;

namespace Iffinator
{
    public class FloorRenderer : MonoGameControl
    {
        private SPR2 m_Sprite;
        private SPR2Frame m_Frame; //Floors only have 1 frame.
        private SpriteBatch m_SBatch;

        public void AddSprite(SPR2 Sprite)
        {
            m_Sprite = Sprite;
            m_Frame = (SPR2Frame)m_Sprite.GetFrame(0); //Floors only have 1 frame.
        }

        protected override void Initialize()
        {
            m_SBatch = new SpriteBatch(this.GraphicsDevice);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw()
        {
            base.Draw();

            m_SBatch.Begin(SpriteSortMode.BackToFront);
            GraphicsDevice.Clear(Color.White);

            if (m_Sprite != null)
            {
                int X = (GraphicsDevice.Viewport.Width - m_Frame.Width) / 2;
                int Y = (GraphicsDevice.Viewport.Height - m_Frame.Height) / 2;
                m_SBatch.Draw(m_Frame.GetTexture(), new Rectangle(X, Y, m_Frame.Width, m_Frame.Height), Color.White);
            }

            m_SBatch.End();
        }
    }
}