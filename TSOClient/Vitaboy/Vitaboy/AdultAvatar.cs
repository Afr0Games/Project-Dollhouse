using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Files.Manager;
using Files.Vitaboy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Vitaboy
{
    public class AdultAvatar : AvatarBase
    {
        public AdultAvatar(GraphicsDevice Devc) : base(Devc, FileManager.GetSkeleton(0x100000005))
        {

        }
    }
}
