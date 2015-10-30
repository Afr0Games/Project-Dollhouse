/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the SimsLib.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Files.AudioLogic
{
    public class SubRoutine
    {
        public Hit HitParent;
        public uint TrackID;
        public uint Address;

        public SubRoutine(FileReader Reader, Hit Parent)
        {
            HitParent = Parent;

            TrackID = Reader.ReadUInt32();
            Address = Reader.ReadUInt32();
        }
    }
}
