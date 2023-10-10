/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Shared library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;

namespace Shared
{
    /// <summary>
    /// A controller that sets up a camera to view UI scenes.
    /// </summary>
    public class UICameraController : CameraController
    {
        public UICameraController(Camera Cam)
        {
            m_Camera = Cam;
            m_Camera.Position = new Vector3(0.0f, -2.0f, 17.0f);
            m_Camera.Zoom = 0.7f;

            m_Camera.NearPlane = 1.0f;
            m_Camera.FarPlane = 800.0f;

            m_Camera.UpVector = Vector3.Down;
            m_Camera.Target = Vector3.Zero;

            if (Cam.Device.Viewport.Width == 800)
                m_Camera.ProjectionOrigin = new Vector2(145, 80);
            else
                m_Camera.ProjectionOrigin = new Vector2(175, 100);
        }

        /// <summary>
        /// Initializes this UICameraController by calculating the projection and view.
        /// </summary>
        public void Initialize()
        {
            CalculateProjection();
            CalculateView();
        }

        protected override void CalculateProjection()
        {
            float Aspect = m_Camera.Device.Viewport.AspectRatio * 1.0f;

            float RatioX = m_Camera.ProjectionOrigin.X / m_Camera.Device.Viewport.Width;
            float RatioY = m_Camera.ProjectionOrigin.Y / m_Camera.Device.Viewport.Height;

            float ProjectionX = 0.0f - (1.0f * RatioX);
            float ProjectionY = (1.0f * RatioY);

            m_Camera.Projection = Matrix.CreatePerspectiveOffCenter(ProjectionX, ProjectionX + 1.0f,
                ((ProjectionY - 1.0f) / Aspect), (ProjectionY) / Aspect, m_Camera.NearPlane, m_Camera.FarPlane);
            m_Camera.Projection = Matrix.CreateScale(m_Camera.Zoom, m_Camera.Zoom, 1.0f) * m_Camera.Projection;
        }

        protected override void CalculateView()
        {
            Matrix Translate = Matrix.CreateTranslation(m_Camera.TranslationX, m_Camera.TranslationY, 
                m_Camera.TranslationZ);
            Vector3 Position = Vector3.Transform(m_Camera.Position, Translate);
            Vector3 Target = Vector3.Transform(m_Camera.Target, Translate);

            m_Camera.View = Matrix.CreateLookAt(Position, Target, m_Camera.UpVector);
        }
    }
}
