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

        private UIProgressBar m_ProgressBar;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LoginProgressDialog(UIScreen Screen, Vector2 Pos) : base(Screen, Pos, false, true, false)
        {
            m_Font = m_Screen.Font11px;

            //cityselprotocolstrings.cst
            m_Cst = StringManager.StrTable(210);

            Vector2 RelativePosition = new Vector2(60, 0);
            m_LblTitle = new UILabel(m_Cst[1], 1, Pos + RelativePosition, m_Font.MeasureString(m_Cst[1]),
                m_Screen.StandardTxtColor, 11, m_Screen, UIParser.Nodes.TextAlignment.Center_Center);

            RelativePosition = new Vector2(20, 85);
            m_ProgressBar = new UIProgressBar(m_Screen, Pos + RelativePosition, 250);

            SetSize((int)((m_Font.MeasureString(m_Cst[1]).X + 40) * Resolution.getVirtualAspectRatio()),
                (int)(175 * Resolution.getVirtualAspectRatio()));
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            base.Update(Helper, GTime);

            if (Visible)
            {
                if (m_DoDrag)
                {
                    Vector2 OffsetFromMouse = new Vector2(60, 0);
                    m_LblTitle.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;

                    OffsetFromMouse = new Vector2(20, 85);
                    m_ProgressBar.Position = (Helper.MousePosition + OffsetFromMouse) - m_DragOffset;
                }
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

            m_ProgressBar.Draw(SBatch, Depth + 0.1f);

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
