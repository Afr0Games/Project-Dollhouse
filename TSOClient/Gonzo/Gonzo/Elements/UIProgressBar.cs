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
using Microsoft.Xna.Framework.Graphics;
using Files;
using Files.Manager;
using log4net;

namespace Gonzo.Elements
{
    /// <summary>
    /// A progressbar, used by LoginProgressDialog.
    /// </summary>
    public class UIProgressBar : UIElement, IDisposable
    {
        private Texture2D m_ProgressBarBack, m_ProgressBarFront;
        private UILabel m_LblProgressInPercentage;
        private int m_ProgressInPercentage = 0;
        public int TotalProgressInPercentage = 100; //This should never need to change...

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Consstructs a new UIProgressBar instance.
        /// </summary>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Width">The width of this UIProgressBar instance.</param>
        /// <param name="Parent">A UIElement that acts as a parent (optional).</param>
        public UIProgressBar(UIScreen Screen, Vector2 ProgressBarPosition, int Width, UIElement Parent = null) : 
            base(Screen, Parent)
        {
            m_Font = m_Screen.Font11px;

            m_ProgressBarBack = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_progressbarback);
            m_ProgressBarFront = FileManager.GetTexture((ulong)FileIDs.UIFileIDs.dialog_progressbarfront);

            string InitialValue = m_ProgressInPercentage.ToString() + " %";

            m_LblProgressInPercentage = new UILabel(InitialValue, 1, new Vector2(Width / 2, 0), 
                m_Font.MeasureString(InitialValue), Color.Black, 11, m_Screen); 

            Position = ProgressBarPosition;

            m_Size.X = Width;
            m_Size.Y = m_ProgressBarBack.Height;
        }

        /// <summary>
        /// Sets the total progress, in percentage, of this UIProgressBar instance.
        /// </summary>
        /// <param name="ProgressInPercentage">The number of percents that this bar has progressed.</param>
        public void SetProgressInPercentage(int ProgressInPercentage)
        {
            m_ProgressInPercentage = ProgressInPercentage;
            m_LblProgressInPercentage.Caption = m_ProgressInPercentage.ToString() + " %";
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            SBatch.Draw(m_ProgressBarBack, new Rectangle((int)Position.X, (int)Position.Y, (int)m_Size.X, (int)m_Size.Y), 
                Color.White);

            if (m_ProgressInPercentage > 0)
                SBatch.Draw(m_ProgressBarFront, new Rectangle((int)Position.X, (int)Position.Y, (int)m_Size.X, 
                    (int)(m_ProgressInPercentage / 100) * TotalProgressInPercentage), Color.White);

            m_LblProgressInPercentage.Draw(SBatch, LayerDepth);

            base.Draw(SBatch, LayerDepth);
        }

        ~UIProgressBar()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this UIProgressBar instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this UIProgressBar instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_ProgressBarBack != null)
                    m_ProgressBarBack.Dispose();
                if (m_ProgressBarFront != null)
                    m_ProgressBarFront.Dispose();

                // Prevent the finalizer from calling ~UIProgressBar, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("UIProgressBar not explicitly disposed!");
        }
    }
}
