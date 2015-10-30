using System;
using System.Collections.Generic;
using System.Text;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Vitaboy
{
    public class DogAvatar : AvatarBase
    {
        public DogAvatar(GraphicsDevice Devc) : base(Devc, FileManager.GetSkeleton(0x400000005))
        {

        }
    }
}
