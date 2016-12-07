/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Collections.Generic;
using Shared;
using Microsoft.Xna.Framework;

namespace Gonzo
{
    /// <summary>
    /// Used for rendering Vitaboy elements onto UI surfaces.
    /// In practice, these will always be instances of Sim.
    /// </summary>
    public class VitaboyScreen : UIScreen
    {
        private ScreenManager m_Manager;
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

        public VitaboyScreen(ScreenManager Manager, Vector2 ScreenPosition, Vector2 ScreenSize) : 
            base(Manager, "VitaboyScreen", null, ScreenPosition, ScreenSize)
        {
            m_Manager = Manager;
            IsVitaboyScreen = true;
        }

        /// <summary>
        /// Updates all the Vitaboy elements in this VitaboyScreen instance.
        /// </summary>
        /// <param name="Input"></param>
        public override void Update(InputHelper Input, GameTime GTime)
        {
            foreach (Sim S in m_Avatars)
                S.Update(GTime);
        }

        /// <summary>
        /// Draws Vitaboy elements onto this VitaboyScreen instance.
        /// </summary>
        public override void Draw()
        {
            foreach (Sim S in m_Avatars)
                S.Draw();
        }
    }
}
