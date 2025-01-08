/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;

namespace Terrain
{
    /// <summary>
    /// A camera for the <see cref="CityRenderer"/>.
    /// </summary>
    public class CityCamera : ICamera
    {
        private const float CITYCAMERAPOSX = 176f;
        private const float CITYCAMERAPOSY = -452f;
        private const float CITYCAMERAPOSZ = 138f;

        private const float CITYCAMERAYAW = -0.3f;
        private const float CITYCAMERAPITCH = -0.3f;
        private const float CITYCAMERAROLL = -0.0f;

        Vector3 m_Position, m_Target;
        Matrix m_WorldMatrix, m_ViewMatrix, m_ProjectionMatrix;
        float m_Pitch, m_Yaw, m_Roll, m_Zoom;

        /// <summary>
        /// Constructs a new CityCamera instance.
        /// </summary>
        /// <param name="DynamicAspectRatio">The game's dynamic aspect ratio.</param>
        public CityCamera(float DynamicAspectRatio)
        {
            m_Position = new Vector3(CITYCAMERAPOSX, CITYCAMERAPOSY, CITYCAMERAPOSZ);

            //Create the rotation matrix using yaw, pitch, and roll
            Matrix CameraRotation = Matrix.CreateFromYawPitchRoll(CITYCAMERAYAW, CITYCAMERAPITCH, CITYCAMERAROLL);

            //Forward vector in the direction the camera is facing
            Vector3 CameraForward = Vector3.Forward;

            //Transform the forward vector based on the camera's rotation
            Vector3 TransformedForward = Vector3.Transform(CameraForward, CameraRotation);

            m_Target = m_Position + TransformedForward;

            m_WorldMatrix = Matrix.Identity;
            m_ViewMatrix = Matrix.CreateLookAt(m_Position, m_Target, Vector3.Up);
            m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), DynamicAspectRatio, 0.1f, 4096f);
        }

        /// <summary>
        /// This CityCamera's pitch.
        /// </summary>
        public float Pitch { get { return m_Pitch; } }

        /// <summary>
        /// This CityCamera's yaw.
        /// </summary>
        public float Yaw { get { return m_Yaw; } }

        /// <summary>
        /// This CityCamera's roll.
        /// </summary>
        public float Roll { get { return m_Roll; } }

        /// <summary>
        /// This CityCamera's zoom.
        /// </summary>
        public float Zoom { get { return m_Zoom; } }

        /// <summary>
        /// This CityCamera's world matrix.
        /// </summary>
        public Matrix WorldMatrix { get { return m_WorldMatrix; } }

        /// <summary>
        /// This CityCamera's view matrix.
        /// </summary>
        public Matrix ViewMatrix { get { return m_ViewMatrix; } set { m_ViewMatrix = value; } }

        /// <summary>
        /// This CityCamera's projection matrix.
        /// </summary>
        public Matrix ProjectionMatrix { get { return m_ProjectionMatrix; } }

        /// <summary>
        /// Gets this CityCamera's target vector.
        /// </summary>
        public Vector3 Target { get { return m_Target; } }

        /// <summary>
        /// Gets or sets this CityCamera's position.
        /// </summary>
        public Vector3 Position { get { return m_Position; } set { m_Position = value; } }
    }
}
