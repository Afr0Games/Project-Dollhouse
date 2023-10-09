/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using UI.Elements;
using UI.Textbox;
using Microsoft.Xna.Framework.Graphics;
using log4net;
using ResolutionBuddy;

namespace UI.Dialogs
{
    public delegate void OnLoginInitializedDelegate(string Username, string Password);

    public class LoginDialog : UIDialog, IDisposable
    {
        private UILabel m_LblTitle, m_LblUsername, m_LblPassword;
        private TextBox m_TxtUsername, m_TxtPassword;
        private UIButton m_BtnLogin, m_BtnExit;

        private CaretSeparatedText m_Cst;

        /// <summary>
        /// Called when the user clicked the login button to start the login.
        /// </summary>
        public event OnLoginInitializedDelegate OnLogin;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LoginDialog(UIScreen Screen, Vector2 Pos) : base(Screen, Pos, false, true, false, 0.800f)
        {
            m_Font = m_Screen.Font11px;

            m_Cst = StringManager.StrTable(209);

            this.ZIndex = (int)DrawOrderEnum.UI;

            Vector2 RelativePosition = new Vector2(45, 0);
            m_LblTitle = new UILabel(m_Cst[1], 1, Pos + RelativePosition, m_Font.MeasureString(m_Cst[1]),
                m_Screen.StandardTxtColor, 11, m_Screen, this, UIParser.Nodes.TextAlignment.Center_Center);
            RelativePosition = new Vector2(20, 50);
            m_LblTitle.ZIndex = this.ZIndex + 1;
            Children.Add(m_LblTitle);

            m_LblUsername = new UILabel(m_Cst[4], 2, Pos + RelativePosition, m_Font.MeasureString(m_Cst[4]), 
                m_Screen.StandardTxtColor, 9, m_Screen, this, UIParser.Nodes.TextAlignment.Center_Center);
            m_LblUsername.ZIndex = this.ZIndex + 1;
            Children.Add(m_LblUsername);

            RelativePosition = new Vector2(20, 110);
            m_LblPassword = new UILabel(m_Cst[5], 3, Pos + RelativePosition, m_Font.MeasureString(m_Cst[4]), 
                m_Screen.StandardTxtColor, 9, m_Screen, this, UIParser.Nodes.TextAlignment.Center_Center);
            //m_LblPassword.DrawOrder = (int)DrawOrderEnum.UI;
            m_LblPassword.ZIndex = this.ZIndex + 1;
            Children.Add(m_LblPassword);

            RelativePosition = new Vector2(20, 85);
            m_TxtUsername = new TextBox(new Rectangle((int)(Pos.X + RelativePosition.X), 
                (int)(Pos.Y + RelativePosition.Y), 230, 25), 64, "", m_Screen.Manager.Graphics, 9, 
                Color.Wheat, Color.White, 30, m_Screen, true, true, this);
            m_TxtUsername.Name = "TxtUsername";
            RelativePosition = new Vector2(20, 145);
            m_TxtUsername.ZIndex = this.ZIndex + 1;
            Children.Add(m_TxtUsername);

            m_TxtPassword = new TextBox(new Rectangle((int)(Pos.X + RelativePosition.X), 
                (int)(Pos.Y + RelativePosition.Y), 230, 25), 64, "", m_Screen.Manager.Graphics, 9, Color.Wheat, 
                Color.White, 30, m_Screen, true, true, this);
            m_TxtPassword.Name = "TxPassword";
            m_TxtPassword.ZIndex = this.ZIndex + 1;
            Children.Add(m_TxtPassword);

            KeyboardInput.Initialize(Screen.Manager, 500f, 20);

            RelativePosition = new Vector2(120, 175);
            m_BtnLogin = new UIButton("BtnLogin", Pos + RelativePosition, m_Screen, null, m_Cst[2], 9, true, this);
            m_BtnLogin.OnButtonClicked += BtnLogin_OnButtonClicked;
            m_BtnLogin.ZIndex = this.ZIndex + 1;
            Children.Add(m_BtnLogin);

            RelativePosition = new Vector2(200, 175);
            m_BtnExit = new UIButton("BtnExit", Pos + RelativePosition, m_Screen, null, m_Cst[3], 9, true, this);
            m_BtnExit.ZIndex = this.ZIndex + 1;
            Children.Add(m_BtnExit);

            SetSize((int)(50 + m_Font.MeasureString(m_Cst[1]).X + 40), m_DefaultSize.Y + m_BtnExit.Size.Y + 10);

            this.DrawOrder = (int)DrawOrderEnum.UI;
        }

        /// <summary>
        /// The user clicked on the login button to log in.
        /// </summary>
        private void BtnLogin_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            OnLogin?.Invoke(m_TxtUsername.Text.String, m_TxtPassword.Text.String);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            if(Visible)
            {
                KeyboardInput.Update();

                if (m_TxtUsername.IsMouseOver(Helper) || m_TxtPassword.IsMouseOver(Helper))
                    m_DoDrag = false;

                if(m_TxtUsername.IsMouseOver(Helper))
                {
                    if (Helper.IsNewPress(MouseButtons.LeftButton))
                        m_TxtPassword.HasFocus = false;
                }
                if (m_TxtPassword.IsMouseOver(Helper))
                {
                    if (Helper.IsNewPress(MouseButtons.LeftButton))
                        m_TxtUsername.HasFocus = false;
                }
            }
        }

        public override void Draw(SpriteBatch SBatch)
        {
            base.Draw(SBatch);
        }

        ~LoginDialog()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this LoginDialog instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this LoginDialog instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                /*if (m_TxtUsername != null)
                    m_TxtUsername.Dispose();
                if (m_TxtPassword != null)
                    m_TxtPassword.Dispose();*/
                if (m_BtnExit != null)
                    m_BtnExit.Dispose();
                if (m_BtnLogin != null)
                    m_BtnLogin.Dispose();

                // Prevent the finalizer from calling ~LoginDialog, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("LoginDialog not explicitly disposed!");
        }
    }
}
