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

namespace Files
{
    public class UniqueFileID
    {
        private uint m_TypeID, m_FileID, m_GroupID;

        /// <summary>
        /// The TypeID portion of this UniqueFileID instance.
        /// </summary>
        public uint TypeID
        {
            get { return m_TypeID; }
        }

        /// <summary>
        /// The FileID portion of this UniqueFileID instance.
        /// </summary>
        public uint FileID
        {
            get { return m_FileID; }
        }

        public uint GroupID
        {
            get { return m_GroupID; }
        }

        /// <summary>
        /// The UniqueID portion of this UniqueFileID instance.
        /// This is TypeID combined with FileID.
        /// </summary>
        public ulong UniqueID
        {
            get
            {
                var fileIDLong = ((ulong)FileID) << 32;
                return fileIDLong | TypeID;
            }
        }

        /// <summary>
        /// Constructs a new instance of UniqueFileID.
        /// Used by FAR3 archives.
        /// </summary>
        /// <param name="TypeID">TypeID of entry.</param>
        /// <param name="FileID">FileID of entry.</param>
        public UniqueFileID(uint TypeID, uint FileID)
        {
            m_TypeID = TypeID;
            m_FileID = FileID;
        }

        /// <summary>
        /// Constructs a new instance of UniqueFileID.
        /// Used by DBPF archives.
        /// </summary>
        /// <param name="TypeID">TypeID of entry.</param>
        /// <param name="InstanceID">InstanceID of entry.</param>
        /// <param name="GroupID">GroupID of entry.</param>
        public UniqueFileID(uint TypeID, uint InstanceID, uint GroupID)
        {
            m_TypeID = TypeID;
            m_FileID = InstanceID;
            m_GroupID = GroupID;
        }
    }
}
