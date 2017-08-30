/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s): Rhys Simpson
*/

using System;
using System.IO;
using MP3Sharp;

namespace Files.AudioFiles
{
    public enum MP3Channels
    {
        Stereo = 00,
        JointStereo = 01,
        DualChannel = 10,
        SingleChannel = 11
    }

    public class MP3File : ISoundCodec
    {
        private int[,,] BITRATES = new int[,,] { { { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448, 0, },
                                                   { 0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, 0, },
                                                   { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0, },
                                                   { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
                                                   { { 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, 0 },
                                                   { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0 },
                                                   { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0, },
                                                   { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } } };

        private int[,] SAMPLE_RATES = new int[,] { { 44100, 48000, 32000, 0 }, { 22050, 24000, 16000, 0 } };

        private MP3Stream m_Stream;
        private int m_MPEGVersion;
        private int m_LayerVersion;
        private MP3Channels m_Channels;
        private int m_Bitrate;
        private int m_SampleRate;
        private int m_Offset; //Current offset in the MP3 file.

        /// <summary>
        /// Which channels does this MP3 support?
        /// </summary>
        public MP3Channels Channels
        {
            get { return m_Channels; }
        }

        /// <summary>
        /// How many bytes to read when calling DecompressedWav().
        /// </summary>
        public int BufferSize = 4096;

        public MP3File(string Path)
        {
            m_Stream = new MP3Stream(File.Open(Path, FileMode.Open, FileAccess.ReadWrite));
        }

        public MP3File(Stream Data)
        {
            m_Stream = new MP3Stream(Data);
        }

        /// <summary>
        /// Sets this MP3 file's stream's position to 0, and starts reading
        /// from the beginning of the stream.
        /// </summary>
        public byte[] Reset(int Offset, int Count)
        {
            byte[] ResetBuffer = new byte[Count];

            m_Stream.Position = 0;
            m_Stream.Read(ResetBuffer, Offset, Count);

            return ResetBuffer;
        }

        /// <summary>
        /// The sample rate for this MP3 file.
        /// </summary>
        /// <returns>The sample rate for this MP3 file.</returns>
        public uint GetSampleRate()
        {
            return (uint)m_SampleRate;
        }

        /// <summary>
        /// Reads PCM data from this MP3.
        /// </summary>
        /// <returns>Returns exactly BufferSize (default: 4096) bytes of data.
        /// If less data is returned, it means the end of the file was reached.</returns>
        public byte[] DecompressedWav()
        {
            byte[] Buffer = new byte[BufferSize];
            int BytesRead = m_Stream.Read(Buffer, m_Offset, BufferSize);

            if (BytesRead < BufferSize)
                Array.Resize(ref Buffer, BytesRead);

            return Buffer;
        }
    }
}
