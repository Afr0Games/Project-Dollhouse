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
using System.IO;

namespace Files.Vitaboy
{
    public class HandGroup
    {
        private FileReader m_Reader;

        public HandSet Light, Medium, Dark;

        /// <summary>
        /// Constructs a new instance of the HandGroup class.
        /// </summary>
        /// <param name="Data">A stream of data retrieved from a FAR3 archive.</param>
        public HandGroup(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            m_Reader.ReadUInt32(); //Version.

            Light = new HandSet(m_Reader);
            Medium = new HandSet(m_Reader);
            Dark = new HandSet(m_Reader);
        }
    }
}
