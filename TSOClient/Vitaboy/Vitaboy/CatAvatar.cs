using System;
using System.Collections.Generic;
using System.Text;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Vitaboy
{
    public class CatAvatar : AvatarBase
    {
        public CatAvatar(GraphicsDevice Devc) : base(Devc, FileManager.GetSkeleton(0x300000005))
        {

        }
    }
}
