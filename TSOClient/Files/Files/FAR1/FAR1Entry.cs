/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

namespace Files.FAR1
{
    public class FAR1Entry
    {
        public uint DecompressedDataSize;
        public uint CompressedDataSize;
        public uint DataOffset;
        public ushort FilenameLength;
        public byte[] FilenameHash;
    }
}
