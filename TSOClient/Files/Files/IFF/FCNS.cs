/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.IO;

namespace Files.IFF
{
    public class FCNSConstant
    {
        public string Name = "";
        public string Value = "";
    }

    public class FCNS : IFFChunk
    {
        public FCNS(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

            Reader.ReadInt32(); //4 bytes always set to 0.
            int Version = Reader.ReadInt32();
            Reader.ReadInt32(); //'SNCF'

            uint Count = Reader.ReadUInt32();

            for(int i = 0; i < Count; i++)
            {
                if(Version == 1)
                {
                    FCNSConstant Constant = new FCNSConstant();
                    Constant.Name = Reader.ReadPaddedCString();
                    Constant.Value = Reader.ReadPaddedCString();
                    Reader.ReadPaddedCString(); //Description
                }
                else
                {
                    FCNSConstant Constant = new FCNSConstant();
                    Constant.Name = Reader.ReadString();
                    Constant.Value = Reader.ReadString();
                    Reader.ReadString(); //Description
                }
            }

            Reader.Close();
            m_Data = null;
        }
    }
}
