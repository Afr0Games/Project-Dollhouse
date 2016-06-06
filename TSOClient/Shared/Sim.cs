using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vitaboy;

namespace Shared
{
    /// <summary>
    /// Represents a Sim that can be drawn and animated. Abstraction on top of Vitaboy's
    /// AvatarBase to hold information such as whether this Sim has a house, is a child 
    /// and so on.
    /// </summary>
    public class Sim
    {
        private AvatarBase m_Avatar;
        public bool HasHouse = false;
        public bool IsChild = false;

        public Sim(AvatarBase Avatar, bool Child = false)
        {
            IsChild = Child;
            m_Avatar = Avatar;
        }

        public void Draw(Matrix ViewMatrix, Matrix WorldMatrix, Matrix ProjectionMatrix)
        {
            m_Avatar.Render(ViewMatrix, WorldMatrix, ProjectionMatrix);
        }
    }
}
