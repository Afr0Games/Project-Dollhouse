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
        private MP3Stream m_Stream;

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
            return (uint)m_Stream.Frequency;
        }

        /// <summary>
        /// Reads PCM data from this MP3.
        /// </summary>
        /// <returns>Returns exactly BufferSize (default: 4096) bytes of data.
        /// If less data is returned, it means the end of the file was reached.</returns>
        public byte[] DecompressedWav()
        {
            byte[] Buffer = new byte[BufferSize];
            int BytesRead = m_Stream.Read(Buffer, 0, BufferSize);

            if (BytesRead < BufferSize)
                Array.Resize(ref Buffer, BytesRead);

            return Buffer;
        }
    }
}
