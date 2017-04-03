/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.IFF
{
    public enum LanguageCodes : byte
    {
        unused = 0,
        EngUS = 1,
        EngInternational = 2,
        French = 3,
        German = 4,
        Italian = 5,
        Spanish = 6,
        Dutch = 7,
        Danish = 8,
        Swedish = 9,
        Norwegian = 10,
        Finnish = 11,
        Hebrew = 12,
        Russian = 13,
        Portugese = 14,
        Japanese = 15,
        Polish = 16,
        ChineseSimple = 17,
        ChineseTrad = 18,
        Thai = 19,
        Korean = 20 
    }

    public struct TranslatedString
    {
        public LanguageCodes LangCode;
        public string TranslatedStr;
    }

    /// <summary>
    /// This chunk type holds text strings.
    /// </summary>
    public class STR : IFFChunk
    {
        public short Version = 0;
        private Dictionary<LanguageCodes, List<TranslatedString>> Strings = new Dictionary<LanguageCodes, List<TranslatedString>>();

        /// <summary>
        /// Gets a specific string for a specified language code.
        /// </summary>
        /// <param name="LangCode">The language code.</param>
        /// <param name="Index">The index of the string to retrieve.</param>
        /// <returns>A string.</returns>
        public string GetString(LanguageCodes LangCode, int Index)
        {
            return Strings[LangCode][Index].TranslatedStr;
        }

        /// <summary>
        /// Returns a list of strings for a specified language code.
        /// </summary>
        /// <param name="LangCode">The language code to retrieve strings for.</param>
        /// <returns>A list of strings for the specified language code.</returns>
        public List<TranslatedString> GetStringList(LanguageCodes LangCode)
        {
            return Strings[LangCode];
        }

        public STR(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false, Encoding.GetEncoding(1252));

            Version = Reader.ReadInt16();
            ushort NumStrings = 0;

            if((Reader.StreamLength - Reader.Position) > 2)
            {
                switch(Version)
                {
                    case 0:
                        NumStrings = Reader.ReadUShort();

                        for (int i = 0; i < NumStrings; i++)
                        {
                            TranslatedString Str = new TranslatedString();
                            Str.LangCode = LanguageCodes.unused;
                            Str.TranslatedStr = Reader.ReadPascalString();

                            if (Strings.ContainsKey(Str.LangCode))
                                Strings[Str.LangCode].Add(Str);
                            else
                            {
                                List<TranslatedString> LanguageSet = new List<TranslatedString>();
                                LanguageSet.Add(Str);
                                Strings.Add(Str.LangCode, LanguageSet);
                            }
                        }

                        break;
                    case -1:
                        NumStrings = Reader.ReadUShort();

                        for (int i = 0; i < NumStrings; i++)
                        {
                            TranslatedString Str = new TranslatedString();
                            Str.LangCode = LanguageCodes.unused;
                            Str.TranslatedStr = Reader.ReadCString();

                            if (Strings.ContainsKey(Str.LangCode))
                                Strings[Str.LangCode].Add(Str);
                            else
                            {
                                List<TranslatedString> LanguageSet = new List<TranslatedString>();
                                LanguageSet.Add(Str);
                                Strings.Add(Str.LangCode, LanguageSet);
                            }
                        }

                        break;
                    case -2:
                        NumStrings = Reader.ReadUShort();

                        for (int i = 0; i < NumStrings; i++)
                        {
                            TranslatedString Str = new TranslatedString();
                            Str.LangCode = LanguageCodes.unused;
                            Str.TranslatedStr = Reader.ReadCString();
                            Reader.ReadCString(); //Comment

                            if (Strings.ContainsKey(Str.LangCode))
                                Strings[Str.LangCode].Add(Str);
                            else
                            {
                                List<TranslatedString> LanguageSet = new List<TranslatedString>();
                                LanguageSet.Add(Str);
                                Strings.Add(Str.LangCode, LanguageSet);
                            }
                        }

                        break;
                    case -3:
                        NumStrings = Reader.ReadUShort();

                        for (int i = 0; i < NumStrings; i++)
                        {
                            TranslatedString Str = new TranslatedString();
                            Str.LangCode = (LanguageCodes)Reader.ReadByte();
                            Str.TranslatedStr = Reader.ReadCString();
                            Reader.ReadCString(); //Comment

                            if (Strings.ContainsKey(Str.LangCode))
                                Strings[Str.LangCode].Add(Str);
                            else
                            {
                                List<TranslatedString> LanguageSet = new List<TranslatedString>();
                                LanguageSet.Add(Str);
                                Strings.Add(Str.LangCode, LanguageSet);
                            }
                        }

                        break;
                    case -4:
                        byte LanguageSets = Reader.ReadByte();
                        for (int i = 0; i < LanguageSets; i++)
                        {
                            NumStrings = Reader.ReadUShort();

                            for (int j = 0; j < NumStrings; j++)
                            {
                                TranslatedString Str = new TranslatedString();
                                Str.LangCode = (LanguageCodes)(Reader.ReadByte() + 1);
                                Str.TranslatedStr = Reader.ReadString();

                                Reader.ReadString(); //Comment

                                if (Strings.ContainsKey(Str.LangCode))
                                    Strings[Str.LangCode].Add(Str);
                                else
                                {
                                    List<TranslatedString> LanguageSet = new List<TranslatedString>();
                                    LanguageSet.Add(Str);
                                    Strings.Add(Str.LangCode, LanguageSet);
                                }
                            }
                        }

                        break;
                }
            }

            Reader.Close();
            m_Data = null;
        }
    }
}
