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

namespace Files.AudioLogic
{
    public class SymbolTable
    {
        public List<SubRoutine> SubRoutines = new List<SubRoutine>();

        public SymbolTable(FileReader Reader, Hit Parent)
        {
            int NumSubroutines = (int)(Reader.StreamLength) / 8;

            for (int i = 0; i < NumSubroutines; i++)
            {
                if((Reader.StreamLength - Reader.Position) > 8)
                    SubRoutines.Add(new SubRoutine(Reader, Parent));
            }
        }
    }
}
