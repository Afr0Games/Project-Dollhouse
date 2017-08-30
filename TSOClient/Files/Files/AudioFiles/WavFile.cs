/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Text;
using System.IO;

namespace Files.AudioFiles
{
    /// <summary>
    /// Represents a wav file. Mostly used by music.
    /// </summary>
    public class WavFile : ISoundCodec, IDisposable
    {
        //RIFF header
        //Should be "RIFF"
        private string m_ChunkID;
        private uint m_ChunkSize;
        //"WAVE"
        private string m_FormatTag;
        //"fmt"
        private string m_SubChunk1ID;
        private uint m_SubChunk1Size;
        private ushort m_AudioFormat;
        private ushort m_NumChannels;
        private uint m_SampleRate;
        private uint m_ByteRate;
        private ushort m_BlockAlign;
        private ushort m_BitsPerSample;
        private string m_SubChunk2ID;
        private uint m_Subchunk2Size;

        private MemoryStream m_DecompressedStream;
        private FileReader m_Reader;

        /// <summary>
        /// Creates a new WavFile instance.
        /// </summary>
        /// <param name="FileData">The file data to read from.</param>
        public WavFile(Stream FileData)
        {
            m_DecompressedStream = new MemoryStream();
            m_Reader = new FileReader(FileData, false);
            m_Reader.Seek(0);

            ReadHeader();

            m_DecompressedStream = new MemoryStream(m_Reader.ReadBytes((int)m_Subchunk2Size));
        }

        /// <summary>
        /// Creates a new WavFile instance.
        /// </summary>
        /// <param name="Filepath">The path to a *.utk file to read from.</param>
        public WavFile(string Filepath)
        {
            if (File.Exists(Filepath))
            {
                m_DecompressedStream = new MemoryStream();

                m_Reader = new FileReader(File.Open(Filepath, FileMode.Open, FileAccess.Read, FileShare.Read), false);
                m_Reader.Seek(0);

                ReadHeader();

                m_DecompressedStream = new MemoryStream(m_Reader.ReadBytes((int)m_Subchunk2Size));
            }
            else
                throw new FileNotFoundException("Couldn't find file: " + Filepath + " , WavFile.cs");
        }

        public void ReadHeader()
        {
            m_ChunkID = Encoding.ASCII.GetString(m_Reader.ReadBytes(4));

            if(m_ChunkID.Equals("RIFF", StringComparison.InvariantCultureIgnoreCase))
            {
                m_ChunkSize = m_Reader.ReadUInt32();
                m_FormatTag = Encoding.ASCII.GetString(m_Reader.ReadBytes(4));

                if (!m_FormatTag.Equals("WAVE", StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("WavFile.cs: Not a proper wav file!");

                m_SubChunk1ID = Encoding.ASCII.GetString(m_Reader.ReadBytes(4));
                m_SubChunk1Size = m_Reader.ReadUInt32();
                m_AudioFormat = m_Reader.ReadUShort();
                m_NumChannels = m_Reader.ReadUShort();
                m_SampleRate = m_Reader.ReadUInt32();
                m_ByteRate = m_Reader.ReadUInt32();
                m_BlockAlign = m_Reader.ReadUShort();
                m_BitsPerSample = m_Reader.ReadUShort();
                m_SubChunk2ID = Encoding.ASCII.GetString(m_Reader.ReadBytes(4));
                m_Subchunk2Size = m_Reader.ReadUInt32();
            }
        }

        public uint GetSampleRate()
        {
            return m_SampleRate;
        }

        /// <summary>
        /// Gets the decompressed wav data for this WavFile instance.
        /// </summary>
        /// <returns>The decompressed wav data as an array of bytes.</returns>
        public byte[] DecompressedWav()
        {
            return m_DecompressedStream.ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool CleanUpManagedResources)
        {
            if (CleanUpManagedResources)
            {
                if (m_Reader != null)
                    m_Reader.Close();

                if (m_DecompressedStream != null)
                    m_DecompressedStream.Close();
            }
        }
    }
}
