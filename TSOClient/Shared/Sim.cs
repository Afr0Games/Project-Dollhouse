/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Shared library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vitaboy;
using Files.Vitaboy;

namespace Shared
{
    /// <summary>
    /// Represents a Sim that can be drawn and animated. Abstraction on top of Vitaboy's
    /// AvatarBase to hold information such as whether this Sim has a house, is a child 
    /// and so on.
    /// </summary>
    public class Sim
    {
        public UICameraController CameraController;
        private AvatarBase m_Avatar;
        public bool HasHouse = false;
        public bool IsChild = false;

        /// <summary>
        /// Creates a new Sim.
        /// </summary>
        /// <param name="Devc">A GraphicsDevice instance.</param>
        /// <param name="Avatar">The Avatar used for this sim (can be AdultAvatar, ChildAvatar, DogAvatar or CatAvatar.</param>
        /// <param name="Child">Is this Sim a child?</param>
        public Sim(GraphicsDevice Devc, Camera Cam, AvatarBase Avatar, bool Child = false)
        {
            IsChild = Child;
            m_Avatar = Avatar;

            CameraController = new UICameraController(Cam);
        }

        /// <summary>
        /// Should this sim be rotated when rendered?
        /// Used for rendering in UI.
        /// </summary>
        public bool ShouldRotate
        {
            get{ return m_Avatar.ShouldRotate; }
            set { m_Avatar.ShouldRotate = value; }
        }

        /// <summary>
        /// Sets the animation for this Sim.
        /// </summary>
        /// <param name="Animation">The animation to play.</param>
        public void SetAnimation(Anim Animation)
        {
            m_Avatar.Animation = Animation;
        }

        /// <summary>
        /// Change the outfit for this Sim.
        /// </summary>
        /// <param name="Oft">The outfit to change into.</param>
        /// <param name="Skin">The sim's skin type.</param>
        public void ChangeOutfit(Outfit Oft, SkinType Skin)
        {
            m_Avatar.ChangeOutfit(Oft, Skin);
        }

        /// <summary>
        /// Sets the head for this Sim.
        /// </summary>
        /// <param name="Head">The Appearance of the head to set.</param>
        public void Head(Outfit Head, SkinType Type)
        {
            m_Avatar.SetHead(Head, Type);
        }

        /// <summary>
        /// Updates this Sim, which includes advancing the current animation frame, 
        /// computing bone positions and updating the avatar's position.
        /// </summary>
        /// <param name="GTime">A GameTime instance.</param>
        public void Update(GameTime GTime)
        {
            if(m_Avatar.Animation != null)
                m_Avatar.AdvanceFrame(m_Avatar.Animation, 0.03f);

            m_Avatar.ComputeBonePositions(m_Avatar.Skel.RootBone, m_Avatar.WorldMatrix);

            m_Avatar.Update(GTime);
        }

        public void Draw()
        {
            m_Avatar.Render(CameraController.View, m_Avatar.WorldMatrix, CameraController.Projection);
        }
    }
}
