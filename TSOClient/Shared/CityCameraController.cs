/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Shared library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using Microsoft.Xna.Framework;

namespace Shared
{
    /// <summary>
    /// A controller that sets up the camera to view the city.
    /// </summary>
    public class CityCameraController : CameraController
    {
        private float m_ViewOffsetX = 0.0f, m_ViewOffsetY = 0.0f;

        private float m_ScreenWidth, m_ScreenHeight;

        public CityCameraController(Camera Cam)
        {
            m_Camera = Cam;
            m_Camera.RotationX = 45f;
            m_Camera.RotationY = 30.0f;
            m_Camera.TranslationX = -360.0f;
            m_Camera.TranslationY = -512.0f;

            m_Camera.FarZoomScale = 5.10f;
            m_Camera.NearZoomScale = 144.0f;

            CalculateProjection();
            CalculateView();
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
            m_Camera.View = Matrix.Identity;
            m_Camera.View *= Matrix.CreateRotationY((m_Camera.RotationX / 180.0f) * MathHelper.Pi);
            m_Camera.View *= Matrix.CreateRotationX((m_Camera.RotationY / 180.0f) * MathHelper.Pi);
            m_Camera.View *= Matrix.CreateTranslation(new Vector3(m_Camera.TranslationX, 0.0f, 
                m_Camera.TranslationY));
            m_Camera.View *= Matrix.CreateScale(1.0f, 0.5f + ((1.0f - m_Camera.ZoomProgress) / 2.0f), 1.0f);

            m_Camera.ViewDirty = false;
        }

        /// <summary>
        /// Calculates the projection for this controller's camera.
        /// </summary>
        protected override void CalculateProjection()
        {
            float FisoScale = (float)Math.Sqrt(0.5f * 0.5f * 2.0f) / m_Camera.FarZoomScale;
            float ZisoScale = (float)Math.Sqrt(0.5f * 0.5f * 2.0f) / m_Camera.NearZoomScale;

            float IsoScale = (float)((1.0f - m_Camera.ZoomProgress) * 
                FisoScale + (m_Camera.ZoomProgress) * ZisoScale);

            var hb = ((m_ScreenWidth) * IsoScale);
            var vb = ((m_ScreenHeight) * IsoScale) * m_Camera.AspectRatioMultiplier;

            m_Camera.Projection = Matrix.CreateOrthographicOffCenter(-hb + m_ViewOffsetX, hb + m_ViewOffsetX, 
                (-vb + m_ViewOffsetY), (vb + m_ViewOffsetY), 0.1f, 1000000);

            m_Camera.ProjectionDirty = false;
        }
    }
}
