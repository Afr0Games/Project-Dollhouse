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
using log4net;
using Microsoft.Xna.Framework.Graphics;
using ResolutionBuddy;

namespace UI.Dialogs
{
    public class LoginProgressDialog : UIDialog, IDisposable
    {
        private UILabel m_LblTitle;
        private CaretSeparatedText m_Cst;

        private UILabel m_LblProgress, m_LblCurrentTask;
        private UIProgressBar m_ProgressBar;
        private UIStatusBar m_StatusBar;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private CaretSeparatedText m_CSTCurrentStatus;

        public LoginProgressDialog(UIScreen Screen, Vector2 Pos) : base(Screen, Pos, false, false, false, 0.800f)
        {
            m_Font = m_Screen.Font11px;

            m_CSTCurrentStatus = StringManager.StrTable(210);

            //cityselprotocolstrings.cst
            m_Cst = StringManager.StrTable(210);
            float Width = (m_Font.MeasureString(m_Cst[1]).X + 100);

            Vector2 RelativePosition = new Vector2(60, 0);
            m_LblTitle = new UILabel(m_Cst[1], 1, Pos + RelativePosition, m_Font.MeasureString(m_Cst[1]),
                m_Screen.StandardTxtColor, 11, m_Screen, this, UIParser.Nodes.TextAlignment.Center_Center);
            m_LblTitle.ZIndex = this.ZIndex + 1;
            Children.Add(m_LblTitle);

            RelativePosition = new Vector2(20, 40);
            m_LblProgress = new UILabel(m_Cst[2], 1, Pos + RelativePosition,
                new Vector2(300, 20), Color.Wheat, 9, m_Screen, this, UIParser.Nodes.TextAlignment.Left_Center);
            m_LblProgress.ZIndex = this.ZIndex + 1;
            Children.Add(m_LblProgress);

            RelativePosition = new Vector2(20, 70);
            m_ProgressBar = new UIProgressBar(m_Screen, Pos + RelativePosition, 300, this);
            m_ProgressBar.ZIndex = this.ZIndex + 1;
            Children.Add(m_ProgressBar);

            RelativePosition = new Vector2(20, 100);
            m_LblCurrentTask = new UILabel(m_Cst[3], 1, Pos + RelativePosition,
                new Vector2(300, 20), Color.Wheat, 9, m_Screen, this, UIParser.Nodes.TextAlignment.Left_Center);
            m_LblCurrentTask.ZIndex = this.ZIndex + 1;
            Children.Add(m_LblCurrentTask);

            RelativePosition = new Vector2(20, 130);
            m_StatusBar = new UIStatusBar(m_Screen, Pos + RelativePosition, 300, this);
            m_StatusBar.ZIndex = this.ZIndex + 1;
            Children.Add(m_StatusBar);

            SetSize((Width < m_StatusBar.Size.X) ? (m_StatusBar.Size.X + (RelativePosition.X * 2)) : Width, 175);

            this.DrawOrder = (int)DrawOrderEnum.UI;
        }

        /// <summary>
        /// Updates this LoginProgressDialog with the status of 
        /// the current login process being performed.
        /// </summary>
        /// <param name="CurrentProcess">The current status.</param>
        public async Task UpdateStatus(LoginProcess CurrentProcess)
        {
            await m_StatusBar.UpdateStatus(CurrentProcess);

            switch (CurrentProcess)
            {
                case LoginProcess.Unavailable:
                    m_ProgressBar.SetProgressInPercentage(0);
                    break;
                case LoginProcess.Authorizing:
                    m_ProgressBar.SetProgressInPercentage(0);
                    break;
                case LoginProcess.Attempting:
                    m_ProgressBar.SetProgressInPercentage(25);
                    break;
                case LoginProcess.Initial:
                    m_ProgressBar.SetProgressInPercentage(50);
                    break;
                case LoginProcess.Loading:
                    m_ProgressBar.SetProgressInPercentage(75);
                    break;
                case LoginProcess.DoneLoading:
                    m_ProgressBar.SetProgressInPercentage(100);
                    break;
            }
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            m_LblTitle.Update(Helper, GTime);
            m_LblProgress.Update(Helper, GTime);
            m_LblCurrentTask.Update(Helper, GTime);

            if (Visible)
            {
                m_ProgressBar.Update(Helper, GTime);
                m_StatusBar.Update(Helper, GTime);
            }
        }

        public override void Draw(SpriteBatch SBatch)
        {
            base.Draw(SBatch);
        }

        ~LoginProgressDialog()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this LoginProgressDialog instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this LoginProgressDialog instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_ProgressBar != null)
                    m_ProgressBar.Dispose();

                // Prevent the finalizer from calling ~LoginProgressDialog, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("LoginProgressDialog not explicitly disposed!");
        }
    }
}
