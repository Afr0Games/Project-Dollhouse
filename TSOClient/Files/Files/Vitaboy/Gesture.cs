/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

namespace Files.Vitaboy
{
    public class Gesture
    {
        public UniqueFileID AppearanceID;

        /// <summary>
        /// Constructs a new instance of the Gesture class.
        /// </summary>
        /// <param name="Reader">A FileReader used to read a HandGroup file.</param>
        public Gesture(FileReader Reader)
        {
            uint FileID = Reader.ReadUInt32();
            uint TypeID = Reader.ReadUInt32();
            AppearanceID = new UniqueFileID(TypeID, FileID);
        }
    }
}
