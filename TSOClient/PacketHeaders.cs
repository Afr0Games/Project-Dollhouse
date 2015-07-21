/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GonzoNet
{
    /// <summary>
    /// Size of packet headers.
    /// </summary>
    public enum PacketHeaders
    {
        UNENCRYPTED = 3,
        ENCRYPTED = 5
    }
}
