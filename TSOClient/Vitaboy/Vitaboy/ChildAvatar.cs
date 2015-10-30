using System;
using System.Collections.Generic;
using System.Text;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Vitaboy
{
    public class ChildAvatar : AvatarBase
    {
        public ChildAvatar(GraphicsDevice Devc, Matrix WorldMatrix) : base(Devc, FileManager.GetSkeleton(0x200000005))
        {

        }
    }
}
