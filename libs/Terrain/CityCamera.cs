/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
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

        public delegate bool TerrainHeightSampler(float x, float y, out float height);

        // Set by Quadtree after it has loaded the terrain heightmap.
        public TerrainHeightSampler SampleTerrainHeight { get; set; }

        /// <summary>
        /// Constructs a new CityCamera instance.
        /// </summary>
        /// <param name="DynamicAspectRatio">The game's dynamic aspect ratio.</param>
        public CityCamera(float DynamicAspectRatio)
        {
            m_Position = new Vector3(CITYCAMERAPOSX, CITYCAMERAPOSY, CITYCAMERAPOSZ);
            m_Yaw = CITYCAMERAYAW;
            m_Pitch = CITYCAMERAPITCH;
            m_Roll = CITYCAMERAROLL;

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
        /// Zooms towards the actual heightmap surface at the center of the view.
        /// No changes to projection FOV or tile/mesh selection.
        /// </summary>
        public void ZoomTowardsTerrain(float steps)
        {
            Matrix orientation = Matrix.CreateFromYawPitchRoll(m_Yaw, m_Pitch, m_Roll);
            Vector3 forward = Vector3.Normalize(Vector3.Transform(Vector3.Forward, orientation));
            Vector3 position = m_Position;
            if (forward.Z >= -0.001f) return;

            Vector3 focus;
            if (SampleTerrainHeight == null || !TryFindTerrainFocus(position, forward, out focus))
            {
                //Water/outside the city: use the sea-level plane as a fallback.
                float distanceToSea = -position.Z / forward.Z;

                if (distanceToSea <= 0f)
                    return;

                focus = position + forward * distanceToSea;
            }

            float distance = Vector3.Distance(position, focus);
            //Keep a useful distance from the terrain; clamp distance instead of
            //world Z so a mountaintop cannot pull the camera underground.
            float nextDistance = MathHelper.Clamp(distance * (float)Math.Pow(0.83, steps), 8f, 2000f);

            m_Position = focus + (position - focus) * (nextDistance / distance);
            RefreshView();
        }

        /// <summary>
        /// Returns the first ray/heightfield intersection. Matches the mesh's
        /// X/Y mapping and elevation / 8 convention in QuadtreeNode.
        /// </summary>
        private bool TryFindTerrainFocus(Vector3 origin, Vector3 direction, out Vector3 focus)
        {
            focus = Vector3.Zero;
            if (direction.Z >= 0f || origin.Z <= 0f)
                return false;

            //The highest heightmap elevation is 255 / 8 units. Starting at
            //sea-level distance guarantees the ray can traverse the surface.
            float end = -origin.Z / direction.Z;
            if (end <= 0f) return false;

            const int samples = 96;
            float previousT = 0f;
            for (int i = 1; i <= samples; ++i)
            {
                float t = end * i / samples;
                Vector3 point = origin + direction * t;
                float height;

                if (SampleTerrainHeight(point.X, point.Y, out height) && point.Z <= height)
                {
                    //Refine the crossing to avoid zoom-focus popping on slopes.
                    float low = previousT, high = t;
                    for (int j = 0; j < 12; ++j)
                    {
                        float mid = (low + high) * 0.5f;
                        Vector3 candidate = origin + direction * mid;
                        float candidateHeight;
                        if (SampleTerrainHeight(candidate.X, candidate.Y, out candidateHeight) && candidate.Z <= candidateHeight)
                            high = mid;
                        else
                            low = mid;
                    }
                    focus = origin + direction * high;
                    return true;
                }
                previousT = t;
            }
            return false;
        }

        /// <summary>
        /// Keeps zoom direction and the rendered view on the same orientation.
        /// </summary>
        public void SetOrientation(float yaw, float pitch, float roll)
        {
            m_Yaw = yaw;
            m_Pitch = pitch;
            m_Roll = roll;
            RefreshView();
        }

        private void RefreshView()
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(m_Yaw, m_Pitch, m_Roll);
            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            m_Target = m_Position + forward;
            m_ViewMatrix = Matrix.CreateLookAt(m_Position, m_Target, Vector3.Up);
        }

        public void SetAspectRatio(float aspectRatio)
        {
            m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio, 0.1f, 4096f);
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
        public Vector3 Position { get { return m_Position; } set { m_Position = value; RefreshView(); } }
    }
}
