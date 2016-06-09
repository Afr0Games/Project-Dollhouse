using System;
using System.Collections.Generic;
using System.Text;
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
        private UICamera m_UICamera;
        private AvatarBase m_Avatar;
        public bool HasHouse = false;
        public bool IsChild = false;

        /// <summary>
        /// Creates a new Sim.
        /// </summary>
        /// <param name="Devc">A GraphicsDevice instance.</param>
        /// <param name="Avatar">The Avatar used for this sim (can be AdultAvatar, ChildAvatar, DogAvatar or CatAvatar.</param>
        /// <param name="Child">Is this Sim a child?</param>
        public Sim(GraphicsDevice Devc, AvatarBase Avatar, bool Child = false)
        {
            IsChild = Child;
            m_Avatar = Avatar;

            m_UICamera = new UICamera(Devc);
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
        /// <param name="Oft">THe outfit to change into.</param>
        /// <param name="Skin">The sim's skin type.</param>
        public void ChangeOutfit(Outfit Oft, SkinType Skin)
        {
            m_Avatar.ChangeOutfit(Oft, Skin);
        }

        /// <summary>
        /// Sets the head for this Sim.
        /// </summary>
        /// <param name="Head">The Appearance of the head to set.</param>
        public void Head(Appearance Head)
        {
            m_Avatar.Head = Head;
        }

        public void Update()
        {
            if(m_Avatar.Animation != null)
                m_Avatar.AdvanceFrame(m_Avatar.Animation, 0.03f);

            //TODO: Change from Matrix.Identity??
            m_Avatar.ComputeBonePositions(m_Avatar.Skel.RootBone, Matrix.Identity);
        }

        public void Draw()
        {
            m_Avatar.Render(m_UICamera.View, Matrix.Identity, m_UICamera.Projection);
        }
    }
}
