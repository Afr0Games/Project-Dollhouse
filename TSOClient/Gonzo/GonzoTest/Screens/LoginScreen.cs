using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gonzo;
using Gonzo.Dialogs;
using Gonzo.Elements;
using Files;
using Files.Manager;

namespace GonzoTest
{
    public class LoginScreen : UIScreen
    {
        private UIImage m_BackgroundImg;
        private LoginDialog m_LoginDiag;
        private LoginProgressDialog m_LoginProgressDiag;

        public LoginScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "LoginScreen",
            SBatch, new Vector2(0, 0),
            new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight))
        {

            m_BackgroundImg = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.setup, false), this);
            m_LoginDiag = new LoginDialog(this, new Vector2((GlobalSettings.Default.ScreenWidth / 2) - 150, 
                ((GlobalSettings.Default.ScreenHeight / 2) - 150)));
            m_LoginProgressDiag = new LoginProgressDialog(this, new Vector2(
                (GlobalSettings.Default.ScreenWidth - 350), (GlobalSettings.Default.ScreenHeight - 350)));

            foreach (KeyValuePair<string, UIElement> KVP in m_LoginDiag.RegistrableUIElements)
                m_PResult.Elements.Add(KVP.Key, KVP.Value);
            m_PResult.Elements.Add("LoginDialog", m_LoginDiag);

            foreach (KeyValuePair<string, UIElement> KVP in m_LoginProgressDiag.RegistrableUIElements)
                m_PResult.Elements.Add(KVP.Key, KVP.Value);
            m_PResult.Elements.Add("LoginProgressDialog", m_LoginProgressDiag);
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            m_LoginDiag.Update(Input, GTime);
        }

        public override void Draw()
        {
            m_SBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, 
                RasterizerState.CullCounterClockwise, null, Resolution.getTransformationMatrix());

            base.Draw();

            m_BackgroundImg.Draw(m_SBatch, null, 0.0f, new Vector2(Resolution.getVirtualAspectRatio(), Resolution.getVirtualAspectRatio()));

            m_SBatch.End();

            foreach (UIElement Element in m_PResult.Elements.Values)
            {
                if (Element.NeedsClipping)
                {
                    RasterizerState RasterState = new RasterizerState();
                    RasterState.ScissorTestEnable = true;
                    RasterState.CullMode = CullMode.CullCounterClockwiseFace;

                    m_SBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null,
                       RasterState, null, Resolution.getTransformationMatrix());

                    Element.Draw(m_SBatch, 0.5f);

                    m_SBatch.End();
                }
            }
        }
    }
}
