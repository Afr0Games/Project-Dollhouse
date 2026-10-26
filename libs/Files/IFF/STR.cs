/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Text;
using System.IO;
using System;
using System.Reflection;
using System.Diagnostics;

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

    /// <summary>
    /// String indices for objects. These might be different for strings in different IFFs.
    /// </summary>
    public enum ObjectStringIndices
    {
        Name = 0,
        Price = 1,
        Description = 2
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
        public string GetString(LanguageCodes LangCode, ObjectStringIndices Index)
        {
            try
            {
                return Strings[LangCode][(int)Index].TranslatedStr;
            }
            catch(Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Adds a string to this STR chunk.
        /// If it exists, it will be replaced
        /// </summary>
        /// <param name="Str">The TranslatedString instance to add.</param>
        /// <param name="Index">The index of the string to add. Typically 0 through 2 for objects
        /// where 0 is the name, 1 is the price and 2 is the description.</param>
        public void AddString(TranslatedString Str, ObjectStringIndices Index)
        {
            Strings[Str.LangCode][(int)Index] = Str;
        }

        /// <summary>
        /// Returns a list of strings for a specified language code.
        /// </summary>
        /// <param name="LangCode">The language code to retrieve strings for.</param>
        /// <returns>A list of strings for the specified language code.</returns>
        public List<TranslatedString> GetStringList(LanguageCodes LangCode)
        {
            if (Strings.ContainsKey(LangCode))
                return Strings[LangCode];
            else
                return new List<TranslatedString>();
        }

        /// <summary>
        /// Constructs a new STR instatance from an IFFChunk instance. 
        /// </summary>
        /// <param name="BaseChunk"></param>
        public STR(IFFChunk BaseChunk) : base(BaseChunk)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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

        protected override byte[] GetPayloadForWriting()
        {
            if (!m_SemanticPayloadChanged) return base.GetPayloadForWriting();
            // Explicit raw overrides are handled without re-encoding strings.
            if (m_RawPayloadOverride) return base.GetPayloadForWriting();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding(1252);
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream, encoding, true);
            writer.Write(Version); // STR# payloads are little endian.

            if (Version == 0 || Version == -1 || Version == -2)
            {
                Strings.TryGetValue(LanguageCodes.unused, out var values);
                values ??= new List<TranslatedString>();
                writer.Write(checked((ushort)values.Count));
                foreach (var entry in values)
                {
                    if (Version == 0) WritePascal(writer, encoding, entry.TranslatedStr);
                    else
                    {
                        WriteCString(writer, encoding, entry.TranslatedStr);
                        if (Version == -2) WriteCString(writer, encoding, entry.Comment);
                    }
                }
            }
            else if (Version == -3)
            {
                writer.Write(checked((ushort)FindNumberOfStrings()));
                foreach (var group in Strings)
                    foreach (var entry in group.Value)
                    {
                        writer.Write((byte)group.Key);
                        WriteCString(writer, encoding, entry.TranslatedStr);
                        WriteCString(writer, encoding, entry.Comment);
                    }
            }
            else if (Version == -4)
            {
                writer.Write(checked((byte)Strings.Count));
                foreach (var group in Strings)
                {
                    if (group.Key == LanguageCodes.unused)
                        throw new InvalidDataException("STR# version -4 cannot encode language 0 in its +1 language mapping.");
                    writer.Write(checked((ushort)group.Value.Count));
                    foreach (var entry in group.Value)
                    {
                        writer.Write(checked((byte)((byte)group.Key - 1)));
                        writer.Write(entry.TranslatedStr ?? ""); // BinaryWriter 7-bit length UTF/CP1252 encoding
                        writer.Write(entry.Comment ?? "");
                    }
                }
            }
            else throw new NotSupportedException("Unsupported edited STR# version " + Version);
            writer.Flush();
            return stream.ToArray();
        }

        private static void WriteCString(BinaryWriter writer, Encoding encoding, string value)
        {
            writer.Write(encoding.GetBytes(value ?? ""));
            writer.Write((byte)0);
        }

        private static void WritePascal(BinaryWriter writer, Encoding encoding, string value)
        {
            byte[] bytes = encoding.GetBytes(value ?? "");
            writer.Write(checked((byte)bytes.Length));
            writer.Write(bytes);
        }

        public override Stream ToStream() { return base.ToStream(); }

        /// <summary>
        /// Finds the number of translated strings in this STR instance.
        /// </summary>
        /// <returns>The number of strings.</returns>
        private int FindNumberOfStrings()
        {
            int NumStrings = 0;

            foreach (KeyValuePair<LanguageCodes, List<TranslatedString>> KVP in Strings)
            {
                for (int i = 0; i < KVP.Value.Count; i++)
                    NumStrings++;
            }

            return NumStrings;
        }
    }
}
