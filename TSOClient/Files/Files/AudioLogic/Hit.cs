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

namespace Files.AudioLogic
{
    public class Hit
    {
        private FileReader m_Reader;
        public SymbolTable SymTable;
        private MemoryStream SymbolData;

        public Hit(Stream Data)
        {
            m_Reader = new FileReader(Data, false);

            ASCIIEncoding Enc = new ASCIIEncoding();
            string MagicNumber = Enc.GetString(m_Reader.ReadBytes(4));

            if (!MagicNumber.Equals("HIT!", StringComparison.InvariantCultureIgnoreCase))
                throw new HitException("MagicNumber was wrong - Hit.cs!");

            m_Reader.ReadUInt32(); //MajorVersion
            m_Reader.ReadUInt32(); //MinorVersion

            string Trax = Enc.GetString(m_Reader.ReadBytes(4));
            if (!Trax.Equals("TRAX", StringComparison.InvariantCultureIgnoreCase))
                throw new HitException("Invalid TRAX header - Hit.cs!");

            SymbolData = new MemoryStream(m_Reader.ReadToEnd());
            m_Reader = new FileReader(SymbolData, false);

            m_Reader.Seek(SearchForSymTable(m_Reader));
            SymTable = new SymbolTable(m_Reader, this);

            m_Reader.Close();
        }

        private int SearchForSymTable(FileReader Reader)
        {
            ASCIIEncoding Enc = new ASCIIEncoding();
            //TODO: This might be horribly inefficient?
            string DataStr = Enc.GetString(SymbolData.ToArray());
            return DataStr.IndexOf("ENTP");
        }
    }
}
