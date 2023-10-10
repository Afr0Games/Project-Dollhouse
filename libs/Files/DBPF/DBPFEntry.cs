/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/


namespace Files.DBPF
{
    public class DBPFEntry
    {
        public uint TypeID;
        public uint GroupID;
        public uint InstanceID;
        public uint FileOffset;
        public uint FileSize;
        public UniqueFileID EntryID;
    }
}
