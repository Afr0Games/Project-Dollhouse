using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UI;
using UI.Dialogs;
using UI.Elements;
using Files;
using Files.Manager;
using Parlo;
using ResolutionBuddy;
using TSOProtocol;

namespace GonzoTest
{
    public class LoginScreen : UIScreen
    {
        private UIBackgroundImage m_BackgroundImg;
        private LoginDialog m_LoginDiag;
        private LoginProgressDialog m_LoginProgressDiag;
        private MessageBox m_ErrorMsgBox;

        public LoginScreen(ScreenManager Manager, SpriteBatch SBatch) : base(Manager, "LoginScreen",
            SBatch, new Vector2(0, 0),
            new Vector2(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight))
        {
            m_BackgroundImg = new UIBackgroundImage("Setup", 
                FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.setup, false), this);
            Children.Add(m_BackgroundImg);

            m_LoginDiag = new LoginDialog(this, new Vector2((Resolution.ScreenArea.Width / 2) - 150, 
                ((Resolution.ScreenArea.Height / 2) - 150)));
            Children.Add(m_LoginDiag);

            m_LoginProgressDiag = new LoginProgressDialog(this, new Vector2(
                (Resolution.ScreenArea.Width - 345), (Resolution.ScreenArea.Height - 180)));
            Children.Add(m_LoginProgressDiag);

            m_LoginDiag.OnLogin += LoginDiag_OnLogin;
            ClientNetworkManager.OnConnected += ClientNetworkManager_OnLogin;
            ClientNetworkManager.OnNetworkError += ClientNetworkManager_OnNetworkError;
            Children.Add(m_LoginDiag);

            m_ErrorMsgBox = new MessageBox(this, new Vector2((Resolution.ScreenArea.Width - 350) / 2, 
                (Resolution.ScreenArea.Height - 200) / 2), "", "Error", MsgBoxButtonEnum.Ok);
            m_ErrorMsgBox.ZIndex = this.ZIndex + 10; //Set it to 10 to make sure it's on top of everything else.
            Children.Add(m_ErrorMsgBox);
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
        private async Task ClientNetworkManager_OnLogin(LoginArgsContainer LoginArgs)
        {
            await m_LoginProgressDiag.UpdateStatus(LoginProcess.Initial);
        }

        private CaretSeparatedText m_CSTStatus = StringManager.StrTable(210);

        private void ClientNetworkManager_OnNetworkError(System.Net.Sockets.SocketException Exception)
        {
            Debug.WriteLine("Got network error: " + Exception.ErrorCode);
            switch(Exception.ErrorCode) //The original client seems to use Attempting for all of these.
            {
                case 10050: //WSAENETDOWN
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Attempting);
                    m_ErrorMsgBox.Message = m_CSTStatus[(int)LoginProcess.Unavailable];
                    m_ErrorMsgBox.Show();
                    break;
                case 10051: //WSAENETUNREACH
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Attempting);
                    m_ErrorMsgBox.Message = m_CSTStatus[(int)LoginProcess.Unavailable];
                    m_ErrorMsgBox.Show();
                    break;
                case 10061: //WSAECONNREFUSED
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Attempting);
                    m_ErrorMsgBox.Message = m_CSTStatus[(int)LoginProcess.Unavailable];
                    m_ErrorMsgBox.Show();
                    break;
                case 10064: //WSAEHOSTDOWN
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Attempting);
                    m_ErrorMsgBox.Message = m_CSTStatus[(int)LoginProcess.Unavailable];

                    m_ErrorMsgBox.Show();
                    break;
                case 10065: //WSAEHOSTUNREACH
                    m_LoginProgressDiag.UpdateStatus(LoginProcess.Attempting);
                    m_ErrorMsgBox.Message = m_CSTStatus[(int)LoginProcess.Unavailable];
                    m_ErrorMsgBox.Show();
                    break;
            }
        }

        public override void Update(InputHelper Input, GameTime GTime)
        {
            base.Update(Input, GTime);
        }

        public override void Draw()
        {
            m_SBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, 
                RasterizerState.CullCounterClockwise, null, Resolution.TransformationMatrix());

            base.Draw();

            m_SBatch.End();
        }
    }
}
