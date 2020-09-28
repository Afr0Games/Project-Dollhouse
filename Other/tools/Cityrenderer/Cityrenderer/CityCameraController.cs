/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the CityRenderer.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using Microsoft.Xna.Framework;

namespace Cityrenderer
{
    /// <summary>
    /// A controller that sets up the camera to view the city.
    /// </summary>
    public class CityCameraController : CameraController
    {
        private float m_ViewOffsetX = 0.0f, m_ViewOffsetY = 0.0f, m_TargetViewOffsetX, m_TargetViewOffsetY;
        private Vector2 m_MouseStart = new Vector2();
        private int m_ScreenWidth, m_ScreenHeight;
        private bool m_Zoomed = false;
        private bool m_MouseMoved = false;

        public CityCameraController(Camera Cam)
        {
            m_Camera = Cam;
            m_Camera.RotationX = 45f;
            m_Camera.RotationY = 30.0f;
            m_Camera.TranslationX = -360.0f;
            m_Camera.TranslationY = -262.0f;
            m_Camera.Zoom = 1.0f;
            m_Camera.Position = Vector3.Zero;

            m_Camera.FarZoomScale = 5.10f;
            m_Camera.NearZoomScale = /*144.0f*/30f;

            m_ScreenWidth = Cam.Device.Viewport.Width;
            m_ScreenHeight = Cam.Device.Viewport.Height;

            CalculateProjection();
            CalculateView();
        }

        /// <summary>
        /// Updates this CityCameraController.
        /// All calculations: FreeSO Project, 
        /// https://github.com/RHY3756547/FreeSO/blob/master/TSOClient/tso.client/Rendering/City/Terrain.cs
        /// <param name="Input">An InputHelper instance.</param>
        /// </summary>
        public void Update(InputHelper Input)
        {
            m_MouseMoved = Input.IsCurPress(MouseButtons.RightButton);

            if (Input.IsNewPress(MouseButtons.RightButton))
            {
                m_MouseStart = new Vector2(Input.CurrentMouseState.X, Input.CurrentMouseState.Y);
            }
            else if (Input.IsOldPress(MouseButtons.LeftButton))
            {
                if (!m_Zoomed)
                {
                    m_Zoomed = true;
                    double ResScale = 768.0 / m_ScreenHeight;
                    double IsoScale = (Math.Sqrt(0.5 * 0.5 * 2) / 5.10) * ResScale;
                    double HB = m_ScreenWidth * IsoScale;
                    double VB = m_ScreenHeight * IsoScale;

                    m_TargetViewOffsetX = (float)(-HB + Input.CurrentMouseState.X * IsoScale * 2);
                    m_TargetViewOffsetY = (float)(VB - Input.CurrentMouseState.Y * IsoScale * 2);
                }
            }

            FixedTimeUpdate(Input);

            m_ViewOffsetX = m_TargetViewOffsetX * m_Camera.ZoomProgress;
            m_ViewOffsetY = m_TargetViewOffsetY * m_Camera.ZoomProgress;
            CalculateProjection();
        }

        /// <summary>
        /// Updates this CityCameraController. All calculations:
        /// https://github.com/RHY3756547/FreeSO/blob/master/TSOClient/tso.client/Rendering/City/Terrain.cs
        /// </summary>
        /// <param name="Input">An InputHelper instance.</param>
        private void FixedTimeUpdate(InputHelper Input)
        {
            if (!m_Zoomed)
            {
                m_Camera.ZoomProgress += (0 - m_Camera.ZoomProgress) / 5.0f;

                //new...
                if (m_MouseMoved)
                {
                    m_TargetViewOffsetX += (Input.CurrentMouseState.X - m_MouseStart.X) / 1000;
                    m_TargetViewOffsetY -= (Input.CurrentMouseState.Y - m_MouseStart.Y) / 1000;
                }
            }
            else
            {
                m_Camera.ZoomProgress += (1.0f - m_Camera.ZoomProgress) / 5.0f;

                if (m_MouseMoved)
                {
                    m_TargetViewOffsetX += (Input.CurrentMouseState.X - m_MouseStart.X) / 1000;
                    m_TargetViewOffsetY -= (Input.CurrentMouseState.Y - m_MouseStart.Y) / 1000;
                }

                /*m_TargetViewOffsetX = Math.Max(-135, Math.Min(m_TargetViewOffsetX, 138));
                m_TargetViewOffsetY = Math.Max(-100, Math.Min(m_TargetViewOffsetY, 103));*/
            }

            if (m_MouseMoved)
            {
                m_TargetViewOffsetX += (Input.CurrentMouseState.X - m_MouseStart.X) / 1000; //move by fraction of distance between the mouse and where it started in both axis
                m_TargetViewOffsetY -= (Input.CurrentMouseState.Y - m_MouseStart.Y) / 1000;
            }
        }

        /// <summary>
        /// Gets or sets the position for this controller's camera.
        /// </summary>
        public Vector3 Position
        {
            get { return m_Camera.Position; }
            set { m_Camera.Position = value; }
        }

        /// <summary>
        /// Gets or sets the zoom for this controller's camera.
        /// </summary>
        public float Zoom
        {
            get { return m_Camera.Zoom; }
            set { m_Camera.Zoom = value; }
        }

        /// <summary>
        /// Which zoom progress is the camera currently at?
        /// </summary>
        public float ZoomProgress
        {
            get { return m_Camera.ZoomProgress; }
            set
            {
                m_Camera.ZoomProgress = value;
            }
        }

        /// <summary>
        /// Calculates the view for this controller's camera.
        /// </summary>
        protected override void CalculateView()
        {
            if (m_Camera.ViewDirty)
            {
                m_Camera.View = Matrix.Identity;
                m_Camera.View *= Matrix.CreateScale(1.0f, 0.5f + ((1.0f - m_Camera.ZoomProgress) / 2.0f), 1.0f);
                m_Camera.View *= Matrix.CreateRotationY((m_Camera.RotationX / 180.0f) * MathHelper.Pi);
                m_Camera.View *= Matrix.CreateRotationX((m_Camera.RotationY / 180.0f) * MathHelper.Pi);
                m_Camera.View *= Matrix.CreateTranslation(new Vector3(m_Camera.TranslationX, 0.0f,
                    m_Camera.TranslationY));

                m_Camera.ViewDirty = false;
            }
        }

        /// <summary>
        /// Calculates the projection for this controller's camera.
        /// </summary>
        protected override void CalculateProjection()
        {
            if (m_Camera.ProjectionDirty)
            {
                float ResScale = 768.0f / m_ScreenHeight;
                float FisoScale = (float)(Math.Sqrt(0.5f * 0.5f * 2.0f) / m_Camera.FarZoomScale) * ResScale;
                float ZisoScale = (float)Math.Sqrt(0.5f * 0.5f * 2.0f) / m_Camera.NearZoomScale;

                float IsoScale = ((1.0f - m_Camera.ZoomProgress) * FisoScale +
                    (m_Camera.ZoomProgress) * ZisoScale);

                float HB = ((m_ScreenWidth) * IsoScale);
                float VB = ((m_ScreenHeight) * IsoScale);

                m_Camera.Projection = Matrix.CreateOrthographicOffCenter(-HB + m_ViewOffsetX, HB + m_ViewOffsetX,
                    -VB + m_ViewOffsetY, VB + m_ViewOffsetY, 0.1f, 524);

                m_Camera.ProjectionDirty = false;
            }
        }
    }
}
