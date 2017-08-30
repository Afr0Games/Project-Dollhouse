/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.IO;
using System.Collections.Generic;

namespace Files.AudioLogic
{
    /// <summary>
    /// A section of a ini file, grouped by [EXAMPLE] headers.
    /// </summary>
    public class IniSection
    {
        public string Name;
        public Dictionary<string, string[]> Entries = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Represents a ini file, as found in the sys folder.
    /// </summary>
    public class Ini
    {
        private FileReader m_Reader;
        public Dictionary<string, IniSection> Sections = new Dictionary<string, IniSection>();

        /// <summary>
        /// Constructs a new instance of the Ini class.
        /// </summary>
        /// <param name="Data">The stream of data to parse.</param>
        public Ini(Stream Data)
        {
            if (Data != null)
            {
                m_Reader = new FileReader(Data, false);
                m_Reader.Seek(0);

                ParseIni();
            }
            else
                throw new FileNotFoundException("Data was null: " + " , Ini.cs");
        }

        /// <summary>
        /// Constructs a new instance of the Ini class.
        /// </summary>
        /// <param name="IniPath">The path to a ini file.</param>
        public Ini(string IniPath)
        {
            if (File.Exists(IniPath))
            {
                m_Reader = new FileReader(File.Open(IniPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), false);
                m_Reader.Seek(0);
            }
            else
                throw new FileNotFoundException("Couldn't find file: " + IniPath + " , Ini.cs");

            ParseIni();
        }

        /// <summary>
        /// Parses the data from a ini file.
        /// </summary>
        private void ParseIni()
        {
            string[] Strs = m_Reader.ReadAllLines(false);
            IniSection Section = new IniSection();

            foreach (string Str in Strs)
            {
                if (Str.Length > 0 && Str[0].Equals('['))
                {
                    Section = new IniSection();
                    Section.Name = Str.Replace("[", "").Replace("]", "");
                    Sections.Add(Section.Name, Section);
                }
                else
                {
                    if (Str.Length > 0 && Str[0].Equals(';') == false && Str[0].Equals('\r') == false)
                    {
                        string[] SectionEntry = Str.Split("=".ToCharArray());

                        string[] Parameters = SectionEntry[1].Split(",".ToCharArray());
                        //Many section entries follow this format: 0=PARAMETER1,PARAMETER2,PARAMETER3
                        if (Parameters.Length > 0)
                            Section.Entries.Add(SectionEntry[0], Parameters);
                        else
                            Section.Entries.Add(SectionEntry[0], new string[] { SectionEntry[1] });
                    }
                }
            }
        }
    }
}
