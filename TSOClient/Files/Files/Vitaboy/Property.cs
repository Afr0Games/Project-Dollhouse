/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Collections.Generic;

namespace Files.Vitaboy
{
    public class Property
    {
        public Dictionary<string, string> PropertyPairs = new Dictionary<string, string>();

        /// <summary>
        /// Constructs a new Property instance.
        /// </summary>
        /// <param name="Reader">A FileReader instance, used to read the Property.</param>
        public Property(FileReader Reader)
        {
            uint PairCount = Reader.ReadUInt32();

            for (int i = 0; i < PairCount; i++)
                PropertyPairs.Add(Reader.ReadPascalString(), Reader.ReadPascalString());
        }
    }
}
