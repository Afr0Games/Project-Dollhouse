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
    public enum FAR1EntryType
    {
        UNK = 0x00, //Unknown
        IFF = 0x01,
        OTF = 0x02,
        SPF = 0x03,
        FLR = 0x04,
        WLL = 0x05,
        BMP = 0x06
    }

    /// <summary>
    /// The entry of a FAR1 archive.
    /// </summary>
    public class FAR1Entry
    {
        public uint DecompressedDataSize;
        public uint CompressedDataSize;

        /// <summary>
        /// The offset of the data for this entry in the FAR1 archive.
        /// </summary>
        public uint DataOffset;

        public ushort FilenameLength;
        public byte[] FilenameHash;

        /// <summary>
        /// The nme of this entry. ONLY assigned to in debug builds!
        /// </summary>
        public string Filename;

        /// <summary>
        /// The type of this entry. ONLY assigned to in debug builds!
        /// </summary>
        public FAR1EntryType EntryType;
    }
}
