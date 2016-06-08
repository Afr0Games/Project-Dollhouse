using System;
using System.Collections.Generic;
using System.Text;
using Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo
{
    /// <summary>
    /// Used for rendering Vitaboy elements onto UI surfaces.
    /// In practice, these will always be instances of Sim.
    /// </summary>
    public class VitaboyScreen : IUIScreen
    {
        private ScreenManager m_Manager;
        private SpriteBatch m_SBatch;
        private List<Sim> m_Avatars = new List<Sim>();

        private Matrix m_ProjectionMat, m_ViewMat;
        private Vector2 m_ProjectionOrigin;
        private float m_NearPlane, m_FarPlane;

        private float m_Zoom = 1.0f;
        private Vector3 m_Position = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 m_UpVector = Vector3.Up;
        private Vector3 m_Target = Vector3.Zero;

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
        /// Adds an instance of Sin to this instance of VitaboyScreen so it can
        /// be drawn and updated.
        /// </summary>
        /// <param name="S">The instance to add.</param>
        public void AddSim(Sim S)
        {
            m_Avatars.Add(S);
        }

        /// <summary>
        /// Removes an instance of Sim from this instance of VitaboyScreen.
        /// </summary>
        /// <param name="S">The instance of Sim to remove.</param>
        public void RemoveSim(Sim S)
        {
            m_Avatars.Remove(S);
        }

        public VitaboyScreen(ScreenManager Manager, SpriteBatch SBatch)
        {
            m_Manager = Manager;
            m_SBatch = SBatch;

            m_NearPlane = 1.0f;
            m_FarPlane = 800.0f; //TODO: Should this be changed based on resolution?

            //Assume the projection is full screen, center origin.
            m_ProjectionOrigin = new Vector2(
                m_Manager.Device.Viewport.Width / 2.0f,
                m_Manager.Device.Viewport.Height / 2.0f);

            CalculateProjection();
            CalculateView();
        }

        private void CalculateProjection()
        {
            float Aspect = m_Manager.Device.Viewport.AspectRatio * 1.0f;

            float RatioX = m_ProjectionOrigin.X / m_Manager.Device.Viewport.Width;
            float RatioY = m_ProjectionOrigin.Y / m_Manager.Device.Viewport.Height;

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

        public void Update(InputHelper Input)
        {

        }

        /// <summary>
        /// Draws Vitaboy elements onto this VitaboyScreen instance.
        /// </summary>
        public void Draw()
        {
            foreach (Sim S in m_Avatars)
                S.Draw(m_ViewMat, Matrix.Identity, m_ProjectionMat);
        }
    }
}
