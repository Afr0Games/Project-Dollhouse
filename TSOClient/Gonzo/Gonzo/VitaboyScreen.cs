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
                S.Draw(m_Manager.ViewMatrix, m_Manager.WorldMatrix, m_Manager.ProjectionMatrix);
        }
    }
}
