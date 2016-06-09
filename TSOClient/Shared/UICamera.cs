using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shared
{
    /// <summary>
    /// Camera used to render 3D elements on the UI.
    /// </summary>
    public class UICamera
    {
        private GraphicsDevice m_Device;
        private Matrix m_ProjectionMat, m_ViewMat;
        private Vector2 m_ProjectionOrigin;
        private float m_NearPlane, m_FarPlane;

        private float m_Zoom = 1.0f;
        private Vector3 m_Position = new Vector3(0.0f, 7.0f, -17.0f);
        private Vector3 m_UpVector = Vector3.Up;
        private Vector3 m_Target = Vector3.Zero;
        private Vector3 m_Translation = Vector3.Zero;


        /// <summary>
        /// The zoom used when rendering Vitaboy elements.
        /// </summary>
        public float Zoom
        {
            get { return m_Zoom; }
            set
            {
                m_Zoom = value;
                CalculateProjection();
                CalculateView();
            }
        }

        /// <summary>
        /// The position used when rendering Vitaboy elements.
        /// </summary>
        public Vector3 Position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;
                CalculateProjection();
                CalculateView();
            }
        }

        /// <summary>
        /// Up vector used when rendering Vitaboy elements.
        /// </summary>
        public Vector3 UpVector
        {
            get { return m_UpVector; }
            set
            {
                m_UpVector = value;
                CalculateProjection();
                CalculateView();
            }
        }

        /// <summary>
        /// This UICamera's projection matrix, used for rendering.
        /// </summary>
        public Matrix Projection
        {
            get { return m_ProjectionMat; }   
        }

        /// <summary>
        /// This UICamera's view matrix, used for rendering.
        /// </summary>
        public Matrix View
        {
            get { return m_ViewMat; }
        }

        public UICamera(GraphicsDevice Devc)
        {
            m_Device = Devc;

            m_NearPlane = 1.0f;
            m_FarPlane = 800.0f; //TODO: Should this be changed based on resolution?

            //Assume the projection is full screen, center origin.
            m_ProjectionOrigin = new Vector2(
                m_Device.Viewport.Width / 2.0f,
                m_Device.Viewport.Height / 2.0f);

            CalculateProjection();
            CalculateView();
        }

        private void CalculateProjection()
        {
            float Aspect = m_Device.Viewport.AspectRatio * 1.0f;

            float RatioX = m_ProjectionOrigin.X / m_Device.Viewport.Width;
            float RatioY = m_ProjectionOrigin.Y / m_Device.Viewport.Height;

            float ProjectionX = 0.0f - (1.0f * RatioX);
            float ProjectionY = (1.0f * RatioY);

            m_ProjectionMat = Matrix.CreatePerspectiveOffCenter(ProjectionX, ProjectionX + 1.0f,
                ((ProjectionY - 1.0f) / Aspect), (ProjectionY) / Aspect, m_NearPlane, m_FarPlane);
            m_ProjectionMat = Matrix.CreateScale(Zoom, Zoom, 1.0f) * m_ProjectionMat;
        }

        private void CalculateView()
        {
            Matrix translate = Matrix.CreateTranslation(m_Translation);
            Vector3 position = Vector3.Transform(Position, translate);
            Vector3 target = Vector3.Transform(m_Target, translate);

            m_ViewMat = Matrix.CreateLookAt(position, target, UpVector);
        }
    }
}
