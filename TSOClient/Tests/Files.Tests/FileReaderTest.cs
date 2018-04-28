/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files.Test.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Files.Tests
{
    [TestClass]
    public class FileReaderTest
    {
        private BinaryReader m_Reader;
        private bool m_IsBigEndian = false;
        private MemoryMappedFile m_MemFile;

        [TestMethod]
        public void OpenFileTest(string Path, bool BigEndian)
        {
            m_MemFile = MemoryMappedFile.CreateFromFile(Path, FileMode.Open, Guid.NewGuid().ToString(), 0, 
                MemoryMappedFileAccess.Read);

            m_IsBigEndian = BigEndian;

            m_Reader = new BinaryReader(m_MemFile.CreateViewStream(0, 0, MemoryMappedFileAccess.Read));

            Assert.IsNotNull(m_Reader, "Couldn't open file - FileReaderTest.cs\r\n");
        }

        /// <summary>
        /// Read a BigEndian Int16.
        /// </summary>
        /// <returns>A short.</returns>
        public short ReadBigEndianInt16Test()
        {
            byte[] b = ReadBytesTest(2);

            Assert.IsNotNull(b, "Unable to read BigEndian Int16!");
            return (short)(b[1] + (b[0] << 8));
        }

        /// <summary>
        /// Read a BigEndian Int32.
        /// </summary>
        /// <param name="Reader">A BinaryReaderInstance.</param>
        /// <returns>An int.</returns>
        public int ReadBigEndianInt32Test()
        {
            byte[] b = ReadBytesTest(4);

            Assert.IsNotNull(b, "Unable to read BigEndian Int32!");
            return b[3] + (b[2] << 8) + (b[1] << 16) + (b[0] << 24);
        }

        /// <summary>
        /// Read a BigEndian Int64.
        /// </summary>
        /// <returns>A long.</returns>
        public long ReadBigEndianInt64Test()
        {
            byte[] b = ReadBytesTest(8);
            Assert.IsNotNull(b, "Unable to read BigEndian Int64!");
            return b[7] + (b[6] << 8) + (b[5] << 16) + (b[4] << 24) + (b[3] << 32) + (b[2] << 40) + (b[1] << 48) + (b[0] << 56);
        }

        /// <summary>
        /// Reads a ulong from the underlying stream.
        /// </summary>
        /// <returns>A ulong.</returns>
        public ulong ReadUInt64Test()
        {
            lock (m_Reader)
            {
                ulong UInt64 = 0;

                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    UInt64 = Endian.SwapUInt64(m_Reader.ReadUInt64());
                    Assert.IsNotNull(UInt64, "Unable to read BigEndian UInt64!");
                    return UInt64;
                }

                UInt64 = m_Reader.ReadUInt64();
                Assert.IsNotNull(UInt64, "Unable to read LittleEndian UInt64!");
                return UInt64;
            }
        }

        /// <summary>
        /// Reads a uint from the underlying stream.
        /// </summary>
        /// <returns>A uint.</returns>
        public uint ReadUInt32Test()
        {
            lock (m_Reader)
            {
                uint UInt32 = 0;

                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    UInt32 = Endian.SwapUInt32(m_Reader.ReadUInt32());
                    Assert.IsNotNull(UInt32, "Unable to read BigEndian UInt32");
                }

                UInt32 = m_Reader.ReadUInt32();
                Assert.IsNotNull(UInt32, "Unable to read BigEndian UInt32!");
                return UInt32;
            }
        }

        /// <summary>
        /// Reads a ushort from the underlying stream.
        /// </summary>
        /// <returns>A ushort.</returns>
        public ushort ReadUShortTest()
        {
            ushort UInt16 = 0;

            lock (m_Reader)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    UInt16 = Endian.SwapUInt16(m_Reader.ReadUInt16());
                    Assert.IsNotNull(UInt16, "Unable to read BigEndian UInt16!");
                    return UInt16;
                }

                UInt16 = m_Reader.ReadUInt16();
                Assert.IsNotNull(UInt16, "Unable to read BigEndian UInt16!");
                return UInt16;
            }
        }

        /// <summary>
        /// Reads a string from the underlying stream.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadStringTest()
        {
            string Str = "";

            lock (m_Reader)
            {
                Str = m_Reader.ReadString();
                Assert.IsFalse(string.IsNullOrEmpty(Str), "Unable to read string (or read an empty string)!");
                return Str;
            }
        }

        /// <summary>
        /// Reads a string with a pre-determined length from the underlying stream.
        /// </summary>
        /// <param name="NumChars">Number of characters in string.</param>
        /// <returns>A string.</returns>
        public string ReadStringTest(int NumChars)
        {
            lock (m_Reader)
            {
                string ReturnStr = "";

                for (int i = 0; i < NumChars; i++)
                    ReturnStr += m_Reader.ReadChar();

                Assert.IsFalse(string.IsNullOrEmpty(ReturnStr), "Unable to read string (or read an empty string)!");

                return ReturnStr;
            }
        }

        /// <summary>
        /// Reads a null-terminated string from the underlying stream.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadCStringTest()
        {
            lock (m_Reader)
            {
                string ReturnStr = "";
                char InChr = '\r';

                while (InChr != '\0')
                {
                    InChr = m_Reader.ReadChar();
                    ReturnStr += InChr;
                }

                Assert.IsFalse(string.IsNullOrEmpty(ReturnStr), "Unable to read string (or read an empty string)!");

                return ReturnStr.Replace("\0", "");
            }
        }

        /// <summary>
        /// Reads a padded null-terminated string from the underlying stream.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadPaddedCStringTest()
        {
            lock (m_Reader)
            {
                string ReturnStr = "";
                char InChr = '\r';

                while (InChr != 0xA3)
                {
                    InChr = m_Reader.ReadChar();
                    ReturnStr += InChr;
                }

                Assert.IsFalse(string.IsNullOrEmpty(ReturnStr), "Unable to read string (or read an empty string)!");

                return ReturnStr;
            }
        }

        /// <summary>
        /// Reads a pascal string from the underlying stream, where one byte preceeds it denoting the length.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadPascalStringTest()
        {
            string ReturnStr = "";

            lock (m_Reader)
            {
                byte Length = m_Reader.ReadByte();
                ASCIIEncoding Encoding = new ASCIIEncoding();

                Assert.IsFalse(string.IsNullOrEmpty(ReturnStr), "Unable to read string (or read an empty string)!");

                ReturnStr = Encoding.GetString(m_Reader.ReadBytes(Length));
                return ReturnStr;
            }
        }

        /// <summary>
        /// Reads a int from the underlying stream.
        /// </summary>
        /// <returns>A uint.</returns>
        public int ReadInt32Test()
        {
            lock (m_Reader)
            {
                int Int32 = 0;

                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Int32 = Endian.SwapInt32(m_Reader.ReadInt32());
                    Assert.IsNotNull(Int32, "Unable to read BigEndian Int32");
                }

                Int32 = m_Reader.ReadInt32();
                Assert.IsNotNull(Int32, "Unable to read BigEndian Int32!");
                return Int32;
            }
        }

        /// <summary>
        /// Reads a uint from the underlying stream.
        /// </summary>
        /// <returns>A uint.</returns>
        public short ReadInt16Test()
        {
            lock (m_Reader)
            {
                short Int16 = 0;

                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Int16 = Endian.SwapInt16(m_Reader.ReadInt16());
                    Assert.IsNotNull(Int16, "Unable to read BigEndian UInt32");
                }

                Int16 = m_Reader.ReadInt16();
                Assert.IsNotNull(Int16, "Unable to read BigEndian UInt32!");
                return Int16;
            }
        }

        /// <summary>
        /// Peeks the specified number of bytes from the underlying stream.
        /// </summary>
        /// <param name="Count">The number of bytes to peek.</param>
        /// <returns>The number of bytes that were peeked.</returns>
        public byte[] PeekBytesTest(int Count)
        {
            lock (m_Reader)
            {
                byte[] Data = m_Reader.ReadBytes(Count);
                m_Reader.BaseStream.Position -= Count;

                if (Endian.IsBigEndian != m_IsBigEndian)
                    Array.Reverse(Data);

                Assert.IsNotNull(Data, "Unable to peek bytes!");

                return Data;
            }
        }

        /// <summary>
        /// Reads a specified number of bytes from the underlying stream.
        /// </summary>
        /// <param name="Count">Number of bytes to read.</param>
        /// <returns>The data that was read, as an array of bytes.</returns>
        public byte[] ReadBytesTest(int Count)
        {
            lock (m_Reader)
            {
                byte[] Data = m_Reader.ReadBytes(Count);

                if (Endian.IsBigEndian != m_IsBigEndian)
                    Array.Reverse(Data);

                Assert.IsNotNull(Data, "Unable to read bytes!");

                return Data;
            }
        }

        /// <summary>
        /// Reads all the remaining bytes in this stream.
        /// </summary>
        /// <returns>The data that was read, as an array of bytes.</returns>
        public byte[] ReadToEndTest()
        {
            lock (m_Reader)
            {
                byte[] Data = null;

                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Data = m_Reader.ReadBytes((int)(m_Reader.BaseStream.Length - m_Reader.BaseStream.Position));
                    Array.Reverse(Data);

                    Assert.IsNotNull(Data, "Unable to read to end!");

                    return Data;
                }

                Data = m_Reader.ReadBytes((int)(m_Reader.BaseStream.Length - m_Reader.BaseStream.Position));

                Assert.IsNotNull(Data, "Unable to read to end!");

                return Data;
            }
        }

        /// <summary>
        /// Reads a float from the underlying stream.
        /// Doesn't support endian swapping.
        /// </summary>
        /// <returns>A float.</returns>
        public float ReadFloatTest()
        {
            lock (m_Reader)
            {
                float Single = 0.0f;
                Single = m_Reader.ReadSingle();

                Assert.IsNotNull(Single, "Unable to read to float!");

                return Single;
            }
        }

        /// <summary>
        /// Reads a byte from the underlying stream.
        /// </summary>
        /// <returns>A byte.</returns>
        public byte ReadByteTest()
        {
            lock (m_Reader)
            {
                byte B = 0;
                B = m_Reader.ReadByte();

                Assert.IsNotNull(B, "Unable to read byte!");

                return B;
            }
        }

        /// <summary>
        /// Reads a string terminated by \r\n.
        /// </summary>
        /// <param name="KeepLineDelimiters">Should the \r\n delimiters be kept?</param>
        /// <returns>The string, including \r\n.</returns>
        public string ReadRNStringTest(bool KeepLineDelimiters)
        {
            lock (m_Reader)
            {
                string ReturnStr = "";
                char InChr = '0';

                while (InChr != '\n' && (m_Reader.BaseStream.Length - m_Reader.BaseStream.Position) > 0)
                {
                    InChr = m_Reader.ReadChar();

                    if (KeepLineDelimiters)
                        ReturnStr += InChr;
                    else
                    {
                        if (InChr != '\r' && InChr != '\n')
                            ReturnStr += InChr;
                    }
                }

                Assert.IsFalse(string.IsNullOrEmpty(ReturnStr), "Unable to read RN string (or read an empty string)!");

                return ReturnStr;
            }
        }

        /// <summary>
        /// Reads all the lines, delimited by \r\n, of a stream.
        /// </summary>
        /// <param name="KeepLineDelimiters">Should the \r\n delimiters be kept?</param>
        /// <returns>An array of all the strings (lines) in the stream.</returns>
        public string[] ReadAllLinesTest(bool KeepLineDelimiters)
        {
            List<string> Strings = new List<string>();

            while ((m_Reader.BaseStream.Length - m_Reader.BaseStream.Position) > 0)
                Strings.Add(ReadRNStringTest(KeepLineDelimiters));

            return Strings.ToArray();
        }
    }
}
