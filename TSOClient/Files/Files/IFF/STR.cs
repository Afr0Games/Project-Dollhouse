using System;
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

        public string GetString(LanguageCodes LangCode, int Index)
        {
            return Strings[LangCode][Index].TranslatedStr;
        }

        public STR(IFFChunk BaseChunk) : base(BaseChunk)
        {
            FileReader Reader = new FileReader(new MemoryStream(m_Data), false);

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
