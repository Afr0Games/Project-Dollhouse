/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the CityRenderer.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cityrenderer
{
    /// <summary>
    /// A camera used for rendering.
    /// Should not be accessed directly, only by using a specific camera controller.
    /// </summary>
    public class Camera
    {
        public GraphicsDevice Device;
        private Matrix m_Projection = new Matrix(), m_View = new Matrix();
        public bool ProjectionDirty = true, ViewDirty = true;

        /// <summary>
        /// Constructs a new camera.
        /// </summary>
        /// <param name="Devc">A graphics device.</param>
        public Camera(GraphicsDevice Devc)
        {
            Device = Devc;
        }

        public Matrix Projection
        {
            get { return m_Projection; }
            set
            {
                m_Projection = value;
                ProjectionDirty = true;
            }
        }

        public Matrix View
        {
            get { return m_View; }
            set
            {
                m_View = value;
                ViewDirty = true;
            }
        }

        private Vector2 m_ProjectionOrigin = Vector2.Zero;

        public Vector2 ProjectionOrigin
        {
            get { return m_ProjectionOrigin; }
            set
            {
                m_ProjectionOrigin = value;
                ProjectionDirty = true;
            }
        }

        private Vector3 m_Translation;

        /// <summary>
        /// Gets or sets this camera's translation on the X axis.
        /// </summary>
        public float TranslationX
        {
            get { return m_Translation.X; }
            set
            {
                m_Translation.X = value;
                ViewDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets this camera's translation on the Y axis.
        /// </summary>
        public float TranslationY
        {
            get { return m_Translation.Y; }
            set
            {
                m_Translation.Y = value;
                ViewDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets this camera's translation on the Z axis.
        /// </summary>
        public float TranslationZ
        {
            get { return m_Translation.Z; }
            set
            {
                m_Translation.Z = value;
                ViewDirty = true;
            }
        }

        private Vector3 m_Rotation;

        public float RotationX
        {
            get { return m_Rotation.X; }
            set
            {
                m_Rotation.X = value;
                ViewDirty = true;
            }
        }

        public float RotationY
        {
            get { return m_Rotation.Y; }
            set
            {
                m_Rotation.Y = value;
                ViewDirty = true;
            }
        }

        public float RotationZ
        {
            get { return m_Rotation.Z; }
            set
            {
                m_Rotation.Z = value;
                ViewDirty = true;
            }
        }

        private Vector3 m_Target = new Vector3();

        /// <summary>
        /// The target that this camera is currently looking at.
        /// </summary>
        public Vector3 Target
        {
            get { return m_Target; }
            set
            {
                m_Target = value;
                ViewDirty = true;
            }
        }

        private Vector3 m_UpVector = new Vector3();

        /// <summary>
        /// This camera's up vector.
        /// </summary>
        public Vector3 UpVector
        {
            get { return m_UpVector; }
            set
            {
                m_UpVector = value;
                ViewDirty = true;
            }

        }

        private float m_NearPlane = 0.0f, m_FarPlane = 0.0f;

        /// <summary>
        /// The near value for this camera's depth of field.
        /// </summary>
        public float NearPlane
        {
            get { return m_NearPlane; }
            set
            {
                m_NearPlane = value;
                ProjectionDirty = true;
            }
        }

        /// <summary>
        /// The far value for this camera's depth of field.
        /// </summary>
        public float FarPlane
        {
            get { return m_FarPlane; }
            set
            {
                m_FarPlane = value;
                ProjectionDirty = true;
            }
        }

        public float Zoom = 1.0f;

        private float m_ZoomProgress = 0.0f;

        public float ZoomProgress
        {
            get { return m_ZoomProgress; }
            set
            {
                m_ZoomProgress = value;
                ProjectionDirty = true;
            }
        }

        private float m_FarZoomScale = 5.10f, m_NearZoomScale = 144.0f;

        /// <summary>
        /// The far value for this camera's zoom scale.
        /// </summary>
        public float FarZoomScale
        {
            get { return m_FarZoomScale; }
            set
            {
                m_FarZoomScale = value;
                ProjectionDirty = true;
            }
        }

        /// <summary>
        /// The near value for this camera's zoom scale.
        /// </summary>
        public float NearZoomScale
        {
            get { return m_NearZoomScale; }
            set
            {
                m_NearZoomScale = value;
                ProjectionDirty = true;
            }
        }

        private float m_AspectRatioMultiplier = 1.0f;
        public float AspectRatioMultiplier
        {
            get { return m_AspectRatioMultiplier; }
            set
            {
                m_AspectRatioMultiplier = value;
                ProjectionDirty = true;
            }
        }

        public Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);
    }
}
