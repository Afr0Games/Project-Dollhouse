﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Vitaboy library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Files.Manager;
using Microsoft.Xna.Framework.Graphics;

namespace Vitaboy
{
    public class CatAvatar : AvatarBase
    {
        public CatAvatar(GraphicsDevice Devc, Effect HeadShader) : base(Devc, FileManager.Instance.GetSkeleton(0x300000005), HeadShader)
        {

        }
    }
}
