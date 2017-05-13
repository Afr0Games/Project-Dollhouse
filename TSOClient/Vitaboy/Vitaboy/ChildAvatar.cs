/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Vitaboy library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Vitaboy
{
    public class ChildAvatar : AvatarBase
    {
        public ChildAvatar(GraphicsDevice Devc, Effect HeadShader) : base(Devc, FileManager.GetSkeleton(0x200000005), HeadShader)
        {

        }
    }
}
