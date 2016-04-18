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
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using System.Security.Principal;
using System.Security.AccessControl;

namespace Files
{
    public class FileReader
    {
        private BinaryReader m_Reader;
        private bool m_IsBigEndian = false;
        private MemoryMappedFile m_MemFile;

        /// <summary>
        /// Returns the length of this FileReader's underlying stream in bytes.
        /// </summary>
        public long StreamLength
        {
            get
            {
                return m_Reader.BaseStream.Length;
            }
        }

        /// <summary>
        /// Returns this FileReader's position with the underlying stream.
        /// </summary>
        public long Position
        {
            get
            {
                return m_Reader.BaseStream.Position;
            }
        }

        /// <summary>
        /// Creates a new FileReader instance.
        /// </summary>
        /// <param name="Data">The data with which to create this FileReader instance.</param>
        /// <param name="BigEndian">Is the data stored as big endian?</param>
        public FileReader(Stream DataStream, bool BigEndian)
        {
            m_Reader = new BinaryReader(DataStream);
            m_IsBigEndian = BigEndian;
        }

        /// <summary>
        /// Creates a new FileReader instance.
        /// </summary>
        /// <param name="Path">The path of the file to read.</param>
        /// <param name="BigEndian">Is the filed stored as big endian?</param>
        public FileReader(string Path, bool BigEndian)
        {
            m_MemFile = MemoryMappedFile.CreateFromFile(Path, FileMode.Open, Guid.NewGuid().ToString(), 0, 
                MemoryMappedFileAccess.Read);

            try
            {
                m_Reader = new BinaryReader(m_MemFile.CreateViewStream(0, 0, MemoryMappedFileAccess.Read));
            }
            catch(IOException Exception)
            {
                throw new ReadingException("Couldn't open file - FileReader.cs\r\n" + Exception.ToString());
            }
        }

        #region Read

        /// <summary>
        /// Reads a ulong from the underlying stream.
        /// </summary>
        /// <returns>A ulong.</returns>
        public ulong ReadUInt64()
        {
            lock (m_Reader)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                    return Endian.SwapUInt64(m_Reader.ReadUInt64());

                return m_Reader.ReadUInt64();
            }
        }

        /// <summary>
        /// Reads a uint from the underlying stream.
        /// </summary>
        /// <returns>A uint.</returns>
        public uint ReadUInt32()
        {
            lock (m_Reader)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                    return Endian.SwapUInt32(m_Reader.ReadUInt32());

                return m_Reader.ReadUInt32();
            }
        }

        /// <summary>
        /// Reads a ushort from the underlying stream.
        /// </summary>
        /// <returns>A ushort.</returns>
        public ushort ReadUShort()
        {
            lock (m_Reader)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                    return Endian.SwapUInt16(m_Reader.ReadUInt16());

                return m_Reader.ReadUInt16();
            }
        }

        /// <summary>
        /// Reads a string from the underlying stream.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadString()
        {
            lock(m_Reader)
                return m_Reader.ReadString();
        }

        /// <summary>
        /// Reads a string with a pre-determined length from the underlying stream.
        /// </summary>
        /// <param name="NumChars">Number of characters in string.</param>
        /// <returns>A string.</returns>
        public string ReadString(int NumChars)
        {
            lock (m_Reader)
            {
                string ReturnStr = "";

                for (int i = 0; i < NumChars; i++)
                    ReturnStr += m_Reader.ReadChar();

                return ReturnStr;
            }
        }

        /// <summary>
        /// Reads a null-terminated string from the underlying stream.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadCString()
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

                return ReturnStr.Replace("\0", "");
            }
        }

        /// <summary>
        /// Reads a padded null-terminated string from the underlying stream.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadPaddedCString()
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

                return ReturnStr;
            }
        }

        /// <summary>
        /// Reads a pascal string from the underlying stream, where one byte preceeds it denoting the length.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadPascalString()
        {
            byte Length = m_Reader.ReadByte();
            ASCIIEncoding Encoding = new ASCIIEncoding();

            return Encoding.GetString(m_Reader.ReadBytes(Length));
        }

        /// <summary>
        /// Reads an int from the underlying stream.
        /// </summary>
        /// <returns>An int.</returns>
        public int ReadInt32()
        {
            lock (m_Reader)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                    return Endian.SwapInt32(m_Reader.ReadInt32());

                return m_Reader.ReadInt32();
            }
        }

        /// <summary>
        /// Reads a short from the underlying stream.
        /// </summary>
        /// <returns>A short.</returns>
        public short ReadInt16()
        {
            lock (m_Reader)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                    return Endian.SwapInt16(m_Reader.ReadInt16());

                return m_Reader.ReadInt16();
            }
        }

        /// <summary>
        /// Reads a specified number of bytes from the underlying stream.
        /// </summary>
        /// <param name="Count">Number of bytes to read.</param>
        /// <returns>The data that was read, as an array of bytes.</returns>
        public byte[] ReadBytes(int Count)
        {
            lock (m_Reader)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    byte[] Data = m_Reader.ReadBytes(Count);
                    Array.Reverse(Data);
                    return Data;
                }

                return m_Reader.ReadBytes(Count);
            }
        }

        /// <summary>
        /// Reads all the remaining bytes in this stream.
        /// </summary>
        /// <returns>The data that was read, as an array of bytes.</returns>
        public byte[] ReadToEnd()
        {
            lock (m_Reader)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    byte[] Data = m_Reader.ReadBytes((int)(m_Reader.BaseStream.Length - m_Reader.BaseStream.Position));
                    Array.Reverse(Data);
                    return Data;
                }

                return m_Reader.ReadBytes((int)(m_Reader.BaseStream.Length - m_Reader.BaseStream.Position));
            }
        }

        /// <summary>
        /// Reads a float from the underlying stream.
        /// Doesn't support endian swapping.
        /// </summary>
        /// <returns>A float.</returns>
        public float ReadFloat()
        {
            lock(m_Reader)
                return m_Reader.ReadSingle();
        }

        /// <summary>
        /// Reads a byte from the underlying stream.
        /// </summary>
        /// <returns>A byte.</returns>
        public byte ReadByte()
        {
            lock(m_Reader)
                return m_Reader.ReadByte();
        }

        #endregion

        #region Seek

        /// <summary>
        /// Seeks in the underlying stream, always assuming an origin of the beginning of the stream.
        /// </summary>
        /// <param name="Offset">The offset to seek to.</param>
        public void Seek(long Offset)
        {
            lock(m_Reader)
                m_Reader.BaseStream.Seek(Offset, SeekOrigin.Begin);
        }

        #endregion

        /// <summary>
        /// Closes this FileReader instance.
        /// </summary>
        public void Close()
        {
            m_Reader.Close();
        }
    }
}
