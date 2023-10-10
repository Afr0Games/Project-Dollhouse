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
using System.IO.MemoryMappedFiles;
using System.Reflection;
using log4net;

namespace Files
{
    public class FileWriter : IDisposable
    {
        private BinaryWriter m_Writer;
        private bool m_IsBigEndian = false;
        private MemoryMappedFile m_MemFile;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Returns the length of this FileReader's underlying stream in bytes.
        /// </summary>
        public long StreamLength
        {
            get
            {
                lock (m_Writer)
                    return m_Writer.BaseStream.Length;
            }
        }

        /// <summary>
        /// Returns this FileReader's position with the underlying stream.
        /// </summary>
        public long Position
        {
            get
            {
                lock (m_Writer)
                    return m_Writer.BaseStream.Position;
            }
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        public void Flush()
        {
            m_Writer.Flush();
        }

        /// <summary>
        /// Gets the writer's underlying stream.
        /// </summary>
        public Stream BaseStream
        {
            get { return m_Writer.BaseStream; }
        }

        /// <summary>
        /// Creates a new FileReader instance.
        /// </summary>
        /// <param name="DataStream">The stream this FileReader instance will be writing to.</param>
        /// <param name="BigEndian">Is the data stored as big endian?</param>
        public FileWriter(Stream DataStream, bool BigEndian)
        {
            m_Writer = new BinaryWriter(DataStream);

            m_IsBigEndian = BigEndian;
        }

        /// <summary>
        /// Creates a new FileReader instance.
        /// </summary>
        /// <param name="Path">The path of the file to read.</param>
        /// <param name="BigEndian">Is the filed stored as big endian?</param>
        public FileWriter(string Path, bool BigEndian)
        {
            m_MemFile = MemoryMappedFile.CreateFromFile(File.Open(Path, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite), null, 0L, MemoryMappedFileAccess.Read, HandleInheritability.None, false);

            m_IsBigEndian = BigEndian;

            try
            {
                m_Writer = new BinaryWriter(m_MemFile.CreateViewStream(0, 0, MemoryMappedFileAccess.Read));
            }
            catch (IOException Exception)
            {
                throw new ReadingException("Couldn't open file - WriterReader.cs\r\n" + Exception.ToString());
            }
        }

        #region Write

        /// <summary>
        /// Updates an offset in the stream with a given offset.
        /// </summary>
        /// <param name="OffsetToUpdate">The offset (or, position of offset) to update.</param>
        /// <param name="OffsetToUpdateWith">The offset to update it with.</param>
        public void UpdateOffset(long OffsetToUpdate, uint OffsetToUpdateWith)
        {
            long OrigOffset = Position;
            Seek(OffsetToUpdate);
            WriteUInt32(OffsetToUpdateWith);
            Seek(OrigOffset);
        }

        /// <summary>
        /// Writes a long to the underlying stream.
        /// </summary>
        /// <returns>A ulong.</returns>
        public void WriteInt64(long Val)
        {
            lock (m_Writer)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Endian.SwapInt64(Val);
                    m_Writer.Write(Val);

                    return;
                }

                m_Writer.Write(Val);
            }
        }

        /// <summary>
        /// Writes a ulong to the underlying stream.
        /// </summary>
        /// <returns>A ulong.</returns>
        public void WriteUInt64(ulong Val)
        {
            lock (m_Writer)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Endian.SwapUInt64(Val);
                    m_Writer.Write(Val);

                    return;
                }

                m_Writer.Write(Val);
            }
        }

        /// <summary>
        /// Writes a uint to the underlying stream.
        /// </summary>
        /// <param name="Val">The uint to write.</param>
        public void WriteUInt32(uint Val)
        {
            lock (m_Writer)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Endian.SwapUInt32(Val);
                    m_Writer.Write(Val);

                    return;
                }

                m_Writer.Write(Val);
            }
        }

        /// <summary>
        /// Writes a ushort to the underlying stream.
        /// </summary>
        /// <param name="Val">The ushort to write.</param>
        public void WriteUShort(ushort Val)
        {
            lock (m_Writer)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Endian.SwapUInt16(Val);
                    m_Writer.Write(Val);

                    return;
                }

                m_Writer.Write(Val);
            }
        }

        /// <summary>
        /// Writes an array of chars with a pre-determined length to the underlying stream.
        /// </summary>
        /// <param name="Str">The string to write.</param>
        /// <param name="Enc">The encoding to use, defaults to ASCII.</param>
        public void WriteChars(string Str, Encoding Enc = null)
        {
            lock (m_Writer)
            {
                if (Enc == null)
                    m_Writer.Write(Encoding.ASCII.GetBytes(Str));
                else
                    m_Writer.Write(Enc.GetBytes(Str));
            }
        }

        /// <summary>
        /// Writes a null-terminated string to the underlying stream.
        /// </summary>
        /// <param name="Str">The string to write.</param>
        /// <param name="Enc">The encoding to use, defaults to ASCII.</param>
        public void WriteCString(string Str, Encoding Enc = null)
        {
            lock (m_Writer)
            {
                if (Enc == null)
                    m_Writer.Write(Encoding.ASCII.GetBytes(Str + '\0'));
                else
                    m_Writer.Write(Enc.GetBytes(Str + '\0'));
            }
        }

        /// <summary>
        /// Writes a padded null-terminated string to the underlying stream.
        /// </summary>
        /// <param name="Str">The string to write.</param>
        /// <param name="PadLength">The length to pad the string to - should always be 
        /// AT LEAST equal to the length of the string + 1!</param>
        /// <param name="Enc">The encoding to use, defaults to null.</param>
        public void WritePaddedCString(string Str, int PadLength, Encoding Enc = null)
        {
            lock (m_Writer)
            {
                if (Enc == null)
                {
                    m_Writer.Write(Encoding.ASCII.GetBytes(Str));

                    for (int i = Str.Length; i < (Str.Length - 1) + PadLength; i++)
                        m_Writer.Write(Encoding.ASCII.GetBytes(new char[] { '\0' }));
                }
                else
                {
                    m_Writer.Write(Enc.GetBytes(Str));

                    for (int i = Str.Length; i < (Str.Length - 1) + PadLength; i++)
                        m_Writer.Write(Enc.GetBytes(new char[] { '\0' }));
                }

                m_Writer.Write((char)0xA3);
            }
        }

        /// <summary>
        /// Writes a pascal string to the underlying stream, where one byte preceeds it denoting the length.
        /// </summary>
        /// <param name="Enc">The encoding to use, degfaults to ASCII.</param>
        /// <param name="Str">The string to write.</param>
        public void WritePascalString(string Str, Encoding Enc = null)
        {
            lock (m_Writer)
            {
                m_Writer.Write((byte)Str.Length);

                if (Enc == null)
                    m_Writer.Write(Encoding.ASCII.GetBytes(Str));
                else
                    m_Writer.Write(Enc.GetBytes(Str));
            }
        }

        /// <summary>
        /// Writes an int to the underlying stream.
        /// </summary>
        public void WriteInt32(int Val)
        {
            lock (m_Writer)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Endian.SwapInt32(Val);
                    m_Writer.Write(Val);
                    return;
                }

                m_Writer.Write(Val);
            }
        }

        /// <summary>
        /// Writes a short to the underlying stream.
        /// </summary>
        public void WriteInt16(short Val)
        {
            lock (m_Writer)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Endian.SwapInt16(Val);
                    m_Writer.Write(Val);
                    return;
                }

                m_Writer.Write(Val);
            }
        }

        /// <summary>
        /// Writes a short to the underlying stream.
        /// </summary>
        public void WriteUInt16(ushort Val)
        {
            lock (m_Writer)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Endian.SwapUInt16(Val);
                    m_Writer.Write(Val);
                    return;
                }

                m_Writer.Write(Val);
            }
        }

        /// <summary>
        /// Peeks the specified number of bytes to the underlying stream.
        /// </summary>
        /// <param name="Count">The number of bytes to peek.</param>
        /// <returns>The number of bytes that were peeked.</returns>
        /*public void PeekBytes(int Count)
        {
            lock (m_Writer)
            {
                byte[] Data = m_Writer.ReadBytes(Count);
                m_Writer.BaseStream.Position -= Count;

                if (Endian.IsBigEndian != m_IsBigEndian)
                    Array.Reverse(Data);

                return Data;
            }
        }*/

        /// <summary>
        /// Writes a specified number of bytes to the underlying stream.
        /// </summary>
        /// <param name="Data">Bytes to write</param>
        public void WriteBytes(byte[] Data)
        {
            lock (m_Writer)
            {
                if (Endian.IsBigEndian != m_IsBigEndian)
                {
                    Array.Reverse(Data);
                    m_Writer.Write(Data);

                    return;
                }

                 m_Writer.Write(Data);
            }
        }

        /// <summary>
        /// Writes a float to the underlying stream.
        /// Doesn't support endian swapping.
        /// </summary>
        public void WriteFloat(float Val)
        {
            lock (m_Writer)
                m_Writer.Write(Val);
        }

        /// <summary>
        /// Writes a byte to the underlying stream.
        /// </summary>
        public void WriteByte(byte Val)
        {
            lock (m_Writer)
                m_Writer.Write(Val);
        }

        /// <summary>
        /// Writes a string terminated by \r\n.
        /// </summary>
        /// <param name="Str">The string to write.</param>
        public void WriteRNString(string Str)
        {
            lock (m_Writer)
                WriteChars(Str + '\r' + '\n');
        }

        #endregion

        #region Seek

        /// <summary>
        /// Seeks in the underlying stream, always assuming an origin of the beginning of the stream.
        /// </summary>
        /// <param name="Offset">The offset to seek to.</param>
        public void Seek(long Offset)
        {
            lock (m_Writer)
                m_Writer.BaseStream.Seek(Offset, SeekOrigin.Begin);
        }

        #endregion

        /// <summary>
        /// Closes this FileWriter instance.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        ~FileWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this FileWriter instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this FileWriter instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                m_Writer.Dispose();

                // Prevent the finalizer to calling ~FileWriter, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("FileWriter not explicitly disposed!");
        }
    }
}
