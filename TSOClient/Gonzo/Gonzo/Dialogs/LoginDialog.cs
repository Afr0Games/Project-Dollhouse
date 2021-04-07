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
using Gonzo.Elements;
using MonoGame_Textbox;
using Microsoft.Xna.Framework.Graphics;
using log4net;

namespace Gonzo.Dialogs
{
    public delegate void OnLoginInitializedDelegate(string Username, string Password);

    public class LoginDialog : UIDialog, IDisposable
    {
        private UILabel m_LblTitle, m_LblUsername, m_LblPassword;
        private MonoGame_Textbox.Cursor m_Cursor;
        private TextBox m_TxtUsername, m_TxtPassword;
        private UIButton m_BtnLogin, m_BtnExit;
        private MessageBox m_MsgBox;

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

            Vector2 RelativePosition = new Vector2(45, 0);
            m_LblTitle = new UILabel(m_Cst[1], 1, Pos + RelativePosition, m_Font.MeasureString(m_Cst[1]),
                m_Screen.StandardTxtColor, 11, m_Screen, this, UIParser.Nodes.TextAlignment.Center_Center);
            RelativePosition = new Vector2(20, 50);
            m_LblUsername = new UILabel(m_Cst[4], 2, Pos + RelativePosition, m_Font.MeasureString(m_Cst[4]), 
                m_Screen.StandardTxtColor, 9, m_Screen, this, UIParser.Nodes.TextAlignment.Center_Center);
            RelativePosition = new Vector2(20, 110);
            m_LblPassword = new UILabel(m_Cst[5], 3, Pos + RelativePosition, m_Font.MeasureString(m_Cst[4]), 
                m_Screen.StandardTxtColor, 9, m_Screen, this, UIParser.Nodes.TextAlignment.Center_Center);

            RelativePosition = new Vector2(20, 85);
            m_TxtUsername = new TextBox(new Rectangle((int)(Pos.X + RelativePosition.X), (int)(Pos.Y + RelativePosition.Y), 
                230, 25), 64, "", m_Screen.Manager.Graphics, 9, Color.Wheat, Color.White, 30, m_Screen, true, this);
            RelativePosition = new Vector2(20, 145);
            m_TxtPassword = new TextBox(new Rectangle((int)(Pos.X + RelativePosition.X), (int)(Pos.Y + RelativePosition.Y),
                230, 25), 64, "", m_Screen.Manager.Graphics, 9, Color.Wheat, Color.White, 30, m_Screen, true, this);

            KeyboardInput.Initialize(Screen.Manager, 500f, 20);

            RelativePosition = new Vector2(120, 175);
            m_BtnLogin = new UIButton("BtnLogin", Pos + RelativePosition, m_Screen, null, m_Cst[2], 9, true, this);
            m_BtnLogin.OnButtonClicked += BtnLogin_OnButtonClicked;

            RelativePosition = new Vector2(200, 175);
            m_BtnExit = new UIButton("BtnExit", Pos + RelativePosition, m_Screen, null, m_Cst[3], 9, true, this);

            SetSize((int)(50 + m_Font.MeasureString(m_Cst[1]).X + 40), m_DefaultSize.Y + m_BtnExit.Size.Y + 10);

            m_MsgBox = new MessageBox(m_Screen, new Vector2(150, 150), "This is a message!", "Message");
            m_MsgBox.Visible = false;
        }

        /// <summary>
        /// The user clicked on the login button to log in.
        /// </summary>
        private void BtnLogin_OnButtonClicked(object Sender, ButtonClickEventArgs E)
        {
            if(m_TxtUsername.Text.String != "" && m_TxtPassword.Text.String != "")
                OnLogin?.Invoke(m_TxtUsername.Text.String, m_TxtPassword.Text.String);
            else
            {
                //TODO: Show messagebox.
            }
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

                m_TxtUsername.Update(Helper, GTime);
                m_TxtPassword.Update(Helper, GTime);

                m_BtnLogin.Update(Helper, GTime);
                m_BtnExit.Update(Helper, GTime);
            }
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.10f;

            m_LblTitle.Draw(SBatch, Depth + 0.1f);
            m_LblUsername.Draw(SBatch, Depth + 0.1f);
            m_LblPassword.Draw(SBatch, Depth + 0.1f);

            m_TxtUsername.Draw(SBatch, Depth + 0.1f);
            m_TxtPassword.Draw(SBatch, Depth + 0.1f);

            m_BtnLogin.Draw(SBatch, Depth + 0.1f);
            m_BtnExit.Draw(SBatch, Depth + 0.1f);

            m_MsgBox.Draw(SBatch, Depth + 0.1f);

            base.Draw(SBatch, LayerDepth);
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
