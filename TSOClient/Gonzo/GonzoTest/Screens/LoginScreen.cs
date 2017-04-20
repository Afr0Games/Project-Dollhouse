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

        public LoginScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "LoginScreen",
            SBatch, new Vector2(0, 0),
            new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight))
        {
            m_BackgroundImg = new UIImage(FileManager.GetTexture((ulong)FileIDs.UIFileIDs.setup, false), this);
            m_LoginDiag = new LoginDialog(this, new Vector2(Position.X / 2, Position.Y / 2));

            foreach (KeyValuePair<string, UIElement> KVP in m_LoginDiag.RegistrableUIElements)
                m_PResult.Elements.Add(KVP.Key, KVP.Value);
            m_PResult.Elements.Add("LoginDialog", m_LoginDiag);
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            m_LoginDiag.Update(Input, GTime);
        }

        public override void Draw()
        {
            base.Draw();

            m_BackgroundImg.Draw(m_SBatch, null, 0.0f);
        }
    }
}
