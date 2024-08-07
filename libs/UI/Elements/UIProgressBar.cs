﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
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
using UI.Dialogs;
using log4net;

namespace UI.Elements
{
    /// <summary>
    /// A progressbar, used by LoginProgressDialog.
    /// </summary>
    public class UIProgressBar : UIElement, IDisposable, IBasicDrawable
    {
        private UIImage m_ProgressBarBack, m_ProgressBarFront;
        private UILabel m_LblProgressInPercentage;
        private int m_ProgressInPercentage = 0;
        public int TotalProgressInPercentage = 100; //This should never need to change...

        private SemaphoreSlim m_ProgressSemaphore = new SemaphoreSlim(1, 1);

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

            if (Parent != null)
            {
                //Would a text edit ever be attached to anything but a UIDialog instance? Probably not.
                UIDialog Dialog = (UIDialog)Parent;
                Dialog.OnDragged += Dialog_OnDragged;
            }

            m_ProgressBarBack = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_progressbarback), 
                new Vector2(0, 0), Screen, null, 0.800f);
            m_ProgressBarFront = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_progressbarfront), 
                new Vector2(0, 0), Screen, null, 0.800f);

            string InitialValue = m_ProgressInPercentage.ToString() + " %";

            m_LblProgressInPercentage = new UILabel(InitialValue, 1, Position + new Vector2(Width / 2, 0), 
                m_Font.MeasureString(InitialValue), Color.Black, 11, m_Screen);
            m_LblProgressInPercentage.ZIndex = this.ZIndex + 1;
            Children.Add(m_LblProgressInPercentage);

            Position = ProgressBarPosition;
            m_ProgressBarBack.Position = ProgressBarPosition;
            m_ProgressBarFront.Position = ProgressBarPosition;

            m_Size.X = Width;
            m_Size.Y = m_ProgressBarBack.Texture.Height;
        }

        /// <summary>
        /// Sets the total progress, in percentage, of this UIProgressBar instance.
        /// </summary>
        /// <param name="ProgressInPercentage">The number of percents that this bar has progressed.</param>
        public async Task SetProgressInPercentage(int ProgressInPercentage)
        {
            await m_ProgressSemaphore.WaitAsync();
                m_ProgressInPercentage = ProgressInPercentage;
                m_LblProgressInPercentage.Caption = m_ProgressInPercentage.ToString() + " %";
            m_ProgressSemaphore.Release();
        }

        /// <summary>
        /// This UIProgressBar instance is attached to a dialog, and the dialog is being dragged.
        /// </summary>
        /// <param name="MousePosition">The mouse position.</param>
        /// <param name="DragOffset">The dialog's drag offset.</param>
        private void Dialog_OnDragged(Vector2 MousePosition, Vector2 DragOffset)
        {
            Vector2 RelativePosition = Position - m_Parent.Position;

            Position = (MousePosition + RelativePosition) - DragOffset;
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            m_LblProgressInPercentage.Position = Position;
            string Caption = m_LblProgressInPercentage.Caption;

            float HalfX = m_Size.X / 2;
            float HalfY = m_Size.Y / 2;
            Vector2 NewPos = new Vector2(HalfX - (m_Font.MeasureString(Caption).X / 2), 
                HalfY - (m_Font.MeasureString(Caption).Y / 2));
            m_LblProgressInPercentage.Position += NewPos; 

            base.Update(Helper, GTime);
        }

        public override void Draw(SpriteBatch SBatch)
        {
            //Texture is 45px / 3 = 15px wide
            m_ProgressBarBack.Draw(SBatch, new Rectangle((int)Position.X, (int)Position.Y, 15, (int)m_Size.Y),
                new Rectangle(0, 0, 15, (int)m_Size.Y));

            m_ProgressBarBack.Draw(SBatch, new Rectangle((int)(Position.X + 15), (int)Position.Y, (int)(Size.X - 30),
                (int)m_Size.Y), new Rectangle(15, 0, 15, (int)m_Size.Y));

            m_ProgressBarBack.Draw(SBatch, new Rectangle((int)((Position.X + Size.X) - 15), (int)Position.Y, 15,
                (int)m_Size.Y), new Rectangle(30, 0, 15, (int)m_Size.Y));

            //Calculate the width of the progress bar front based on the progress percentage
            int progressWidth = (int)((m_ProgressInPercentage / (float)m_ProgressInPercentage) * (Size.X - 30));

            if (progressWidth > 0)
            {
                m_ProgressBarFront.Draw(SBatch, new Rectangle((int)Position.X, (int)Position.Y, 15, (int)m_Size.Y),
                    new Rectangle(0, 0, 15, (int)m_Size.Y));

                m_ProgressBarFront.Draw(SBatch, new Rectangle((int)(Position.X + 15), (int)Position.Y, progressWidth,
                    (int)m_Size.Y), new Rectangle(15, 0, 15, (int)m_Size.Y));

                m_ProgressBarFront.Draw(SBatch, new Rectangle((int)(Position.X + 15 + progressWidth), (int)Position.Y, 15,
                    (int)m_Size.Y), new Rectangle(30, 0, 15, (int)m_Size.Y));
            }

            base.Draw(SBatch);
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
