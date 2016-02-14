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

namespace Files.IFF
{
    public struct OBJfFunctionPair
    {
        /// <summary>
        /// A 2-byte unsigned integer specifying the chunk ID of the conditional BHAV subroutine. 
        /// If specified, the action function will be executed only if the condition function 
        /// returns true; otherwise, the action function will simply always be executed.
        /// </summary>
        public ushort ConditionFunction;

        /// <summary>
        /// A 2-byte unsigned integer specifying the chunk ID of the action BHAV subroutine.
        /// </summary>
        public ushort ActionFunction; 
    }

    /// <summary>
    /// This chunk type assigns BHAV subroutines to a number of events that occur in 
    /// (or outside of?) the object, which are described in behavior.iff chunk 00F5.
    /// </summary>
    public class OBJf : IFFChunk
    {
        public Dictionary<int, OBJfFunctionPair> FunctionTable = new Dictionary<int, OBJfFunctionPair>();

        public OBJf(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Reader.ReadBytes(4); //Zero
            Reader.ReadBytes(4); //Version
            Reader.ReadBytes(4); //Magic

            uint Count = Reader.ReadUInt32();

            for (int i = 0; i < Count; i++)
            {
                OBJfFunctionPair FuncPair = new OBJfFunctionPair();
                FuncPair.ConditionFunction = Reader.ReadUShort();
                FuncPair.ActionFunction = Reader.ReadUShort();
                FunctionTable.Add(i, FuncPair);
            }

            Reader.Close();
            m_Data = null;
        }
    }
}
