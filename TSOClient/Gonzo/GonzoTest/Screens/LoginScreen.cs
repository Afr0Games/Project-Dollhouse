using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gonzo;
using Gonzo.Dialogs;
using Gonzo.Elements;
using Files;
using Files.Manager;
using TSOProtocol;
using GonzoNet;
using ResolutionBuddy;

namespace GonzoTest
{
    public class LoginScreen : UIScreen
    {
        private UIBackgroundImage m_BackgroundImg;
        private LoginDialog m_LoginDiag;
        private LoginProgressDialog m_LoginProgressDiag;

        public LoginScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "LoginScreen",
            SBatch, new Vector2(0, 0),
            new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight))
        {
            m_BackgroundImg = new UIBackgroundImage("Setup", 
                FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.setup, false), this);
            RegisterElement(m_BackgroundImg);
            m_LoginDiag = new LoginDialog(this, new Vector2((Resolution.ScreenArea.Width / 2) - 150, 
                ((Resolution.ScreenArea.Height / 2) - 150)));
            RegisterElement(m_LoginDiag);

            m_LoginProgressDiag = new LoginProgressDialog(this, new Vector2(
                (Resolution.ScreenArea.Width - 350), (Resolution.ScreenArea.Height - 150)));
            RegisterElement(m_LoginProgressDiag);

            foreach (KeyValuePair<string, UIElement> KVP in m_LoginDiag.RegistrableUIElements)
                m_PResult.Elements.Add(KVP.Key, KVP.Value);
            m_PResult.Elements.Add("LoginDialog", m_LoginDiag);

            foreach (KeyValuePair<string, UIElement> KVP in m_LoginProgressDiag.RegistrableUIElements)
                m_PResult.Elements.Add(KVP.Key, KVP.Value);
            m_PResult.Elements.Add("LoginProgressDialog", m_LoginProgressDiag);

            m_LoginDiag.OnLogin += LoginDiag_OnLogin;
            ClientNetworkManager.OnConnected += ClientNetworkManager_OnLogin;
            ClientNetworkManager.OnNetworkError += ClientNetworkManager_OnNetworkError;
        }

        private void LoginDiag_OnLogin(string Username, string Password)
        {
            //TODO: Store the IP and port in an external file.
            ClientNetworkManager.Instance.Connect("127.0.0.1", 666, Username, Password);
        }

        /// <summary>
        /// The client successfully connected to the login server!
        /// </summary>
        /// <param name="LoginArgs">The arguments the user used for logging in.</param>
        private void ClientNetworkManager_OnLogin(LoginArgsContainer LoginArgs)
        {
            m_LoginProgressDiag.UpdateStatus(LoginProcess.Initial);
        }

        private void ClientNetworkManager_OnNetworkError(System.Net.Sockets.SocketException Exception)
        {
            Debug.WriteLine("Got network error: " + Exception.ErrorCode);
            switch(Exception.ErrorCode)
            {
                case 10050: //WSAENETDOWN
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Unavailable);
                    break;
                case 10051: //WSAENETUNREACH
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Unavailable);
                    break;
                case 10061: //WSAECONNREFUSED
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Unavailable);
                    break;
                case 10064: //WSAEHOSTDOWN
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Unavailable);
                    break;
                case 10065: //WSAEHOSTUNREACH
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Unavailable);
                    break;
            }
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);

            m_LoginDiag.Update(Input, GTime);
        }

        public override void Draw()
        {
            m_SBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, 
                RasterizerState.CullCounterClockwise, null, Resolution.TransformationMatrix());

            base.Draw();

            m_BackgroundImg.Draw(m_SBatch, null, 0.0f);

            m_SBatch.End();

            foreach (UIElement Element in m_PResult.Elements.Values)
            {
                if (Element.NeedsClipping)
                {
                    RasterizerState RasterState = new RasterizerState();
                    RasterState.ScissorTestEnable = true;
                    RasterState.CullMode = CullMode.CullCounterClockwiseFace;

                    m_SBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null,
                       RasterState, null, Resolution.TransformationMatrix());

                    Element.Draw(m_SBatch, 0.5f);

                    m_SBatch.End();
                }
            }
        }
    }
}
