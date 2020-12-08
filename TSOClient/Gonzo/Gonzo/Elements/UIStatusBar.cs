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
using Gonzo.Dialogs;
using log4net;

namespace Gonzo.Elements
{
    public enum LoginProcess
    {
        /// <summary>
        /// Authorizing. Prompting for name and password...
        /// </summary>
        Authorizing = 4,

        /// <summary>
        /// Attempting EA.COM authorization...
        /// </summary>
        Attempting = 5,

        /// <summary>
        /// EA.COM authorization successful.
        /// </summary>
        Successful = 6,

        /// <summary>
        /// Initial server connection. Authorizing user...
        /// </summary>
        Initial = 7,

        /// <summary>
        /// Server connected. User authorized.
        /// </summary>
        Connected = 8
    }

    /// <summary>
    /// A statusbar, used by LoginProgressDialog for displaying the status of the login process.
    /// </summary>
    public class UIStatusBar : UIElement, IDisposable
    {
        private UIImage m_ProgressBarBack, m_ProgressBarFront;
        private UILabel m_LblCurrentStatus;
        private CaretSeparatedText m_CSTCurrentStatus;
        private int m_ProgressInPercentage = 0;
        public int TotalProgressInPercentage = 100; //This should never need to change...

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Consstructs a new UIProgressBar instance.
        /// </summary>
        /// <param name="Screen">A UIScreen instance.</param>
        /// <param name="Width">The width of this UIProgressBar instance.</param>
        /// <param name="Parent">A UIElement that acts as a parent (optional).</param>
        public UIStatusBar(UIScreen Screen, Vector2 ProgressBarPosition, int Width, UIElement Parent = null) :
            base(Screen, Parent)
        {
            m_Font = m_Screen.Font9px;
            m_CSTCurrentStatus = StringManager.StrTable(210);

            if (Parent != null)
            {
                //Would a text edit ever be attached to anything but a UIDialog instance? Probably not.
                UIDialog Dialog = (UIDialog)Parent;
                Dialog.OnDragged += Dialog_OnDragged;
            }

            m_ProgressBarBack = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_progressbarback),
                Screen, null, 0.800f);
            m_ProgressBarFront = new UIImage(FileManager.Instance.GetTexture((ulong)FileIDs.UIFileIDs.dialog_progressbarfront),
                Screen, null, 0.800f);

            string InitialValue = m_ProgressInPercentage.ToString() + " %";

            m_LblCurrentStatus = new UILabel(m_CSTCurrentStatus[4], 1, Position + new Vector2(Width / 2, 0),
                m_Font.MeasureString(InitialValue), Color.Wheat, 9, m_Screen);

            Position = ProgressBarPosition;

            m_Size.X = Width;
            m_Size.Y = m_ProgressBarBack.Texture.Height;
        }

        /// <summary>
        /// Login progressed, so update the text displayed.
        /// </summary>
        /// <param name="CurrentProcess">What stage of the login progress are we in?</param>
        public void UpdateStatus(LoginProcess CurrentProcess)
        {
            m_LblCurrentStatus.Caption = m_CSTCurrentStatus[(int)CurrentProcess];
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
            m_LblCurrentStatus.Position = Position;
            string Caption = m_LblCurrentStatus.Caption;

            float HalfX = m_Size.X / 2;
            float HalfY = m_Size.Y / 2;
            Vector2 NewPos = new Vector2(HalfX - (m_Font.MeasureString(Caption).X / 2),
                HalfY - (m_Font.MeasureString(Caption).Y / 2));
            m_LblCurrentStatus.Position += NewPos;

            base.Update(Helper, GTime);
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.9f; //Progressbars are always drawn on top

            //Texture is 45px / 3 = 15px wide
            m_ProgressBarBack.Draw(SBatch, new Rectangle((int)Position.X, (int)Position.Y, 15, (int)m_Size.Y),
                new Rectangle(0, 0, 15, (int)m_Size.Y), Depth + 0.1f);

            m_ProgressBarBack.Draw(SBatch, new Rectangle((int)(Position.X + 15), (int)Position.Y, (int)(Size.X - 30),
                (int)m_Size.Y), new Rectangle(15, 0, 15, (int)m_Size.Y), Depth + 0.1f);

            m_ProgressBarBack.Draw(SBatch, new Rectangle((int)((Position.X + Size.X) - 15), (int)Position.Y, 15,
                (int)m_Size.Y), new Rectangle(30, 0, 15, (int)m_Size.Y), Depth + 0.1f);

            m_LblCurrentStatus.Draw(SBatch, LayerDepth + 0.3f);

            base.Draw(SBatch, LayerDepth);
        }

        ~UIStatusBar()
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
                m_Logger.Error("UIStatusBar not explicitly disposed!");
        }
    }
}
