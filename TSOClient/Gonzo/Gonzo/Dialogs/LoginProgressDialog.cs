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
using log4net;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Dialogs
{
    public class LoginProgressDialog : UIDialog, IDisposable
    {
        private UILabel m_LblTitle;
        private CaretSeparatedText m_Cst;

        private UILabel m_LblProgress, m_LblCurrentTask;
        private UIProgressBar m_ProgressBar;
        private UIStatusBar m_StatusBar;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LoginProgressDialog(UIScreen Screen, Vector2 Pos) : base(Screen, Pos, false, false, false, 0.800f)
        {
            m_Font = m_Screen.Font11px;

            //cityselprotocolstrings.cst
            m_Cst = StringManager.StrTable(210);

            Vector2 RelativePosition = new Vector2(60, 0);
            m_LblTitle = new UILabel(m_Cst[1], 1, Pos + RelativePosition, m_Font.MeasureString(m_Cst[1]),
                m_Screen.StandardTxtColor, 11, m_Screen, this, UIParser.Nodes.TextAlignment.Center_Center);

            RelativePosition = new Vector2(20, 55);
            m_LblProgress = new UILabel(m_Cst[2], 1, Pos + RelativePosition,
                new Vector2(300, 20), Color.Wheat, 9, m_Screen, this, UIParser.Nodes.TextAlignment.Left_Center);

            RelativePosition = new Vector2(20, 85);
            m_ProgressBar = new UIProgressBar(m_Screen, Pos + RelativePosition, 300, this);
            RegistrableUIElements.Add("ProgressBar", m_ProgressBar);

            RelativePosition = new Vector2(20, 115);
            m_LblCurrentTask = new UILabel(m_Cst[3], 1, Pos + RelativePosition,
                new Vector2(300, 20), Color.Wheat, 9, m_Screen, this, UIParser.Nodes.TextAlignment.Left_Center);

            RelativePosition = new Vector2(20, 145);
            m_StatusBar = new UIStatusBar(m_Screen, Pos + RelativePosition, 300, this);
            RegistrableUIElements.Add("StatusBar", m_ProgressBar);

            SetSize((int)((m_Font.MeasureString(m_Cst[1]).X + 100) * m_Screen.Manager.Resolution.ScalingRatio), 
                (int)(175) * m_Screen.Manager.Resolution.ScalingRatio);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            if (Visible)
            {
                m_ProgressBar.Update(Helper, GTime);
                m_StatusBar.Update(Helper, GTime);
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

            m_LblProgress.Draw(SBatch, Depth + 0.1f);
            m_ProgressBar.Draw(SBatch, Depth + 0.1f);

            m_LblCurrentTask.Draw(SBatch, Depth + 0.1f);
            m_StatusBar.Draw(SBatch, Depth + 0.1f);

            base.Draw(SBatch, LayerDepth);
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
