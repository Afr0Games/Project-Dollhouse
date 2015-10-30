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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.Vitaboy
{
    public class TimeProperty
    {
        public uint ID;
        public List<Property> PropertyList = new List<Property>();

        /// <summary>
        /// Constructs a new TimeProperty instance.
        /// </summary>
        /// <param name="Reader">A FileReader instance used to read a TimeProperty.</param>
        public TimeProperty(FileReader Reader)
        {
            ID = Reader.ReadUInt32();
            uint PropsCount = Reader.ReadUInt32();

            for (int i = 0; i < PropsCount; i++)
                PropertyList.Add(new Property(Reader));
        }
    }
}
