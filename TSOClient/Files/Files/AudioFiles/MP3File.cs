/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.IO;
using NAudio.Wave;

namespace Files.AudioFiles
{
    public class MP3File : ISoundCodec
    {
        private Mp3FileReader m_FReader;

        /// <summary>
        /// Creates a new instance of MP3File.
        /// </summary>
        /// <param name="FileData">The stream of data from which to create this instance.</param>
        public MP3File(Stream FileData)
        {
            m_FReader = new Mp3FileReader(FileData);
        }

        /// <summary>
        /// Creates a new instance of MP3File.
        /// </summary>
        /// <param name="Path">The path to the file from which to create this instance.</param>
        public MP3File(string Path)
        {
            m_FReader = new Mp3FileReader(Path);
        }

        /// <summary>
        /// Returns the sample rate for the wav data that makes up this sound.
        /// </summary>
        /// <returns>A uint denoting the sample rate of the wav data that makes up this sound.</returns>
        public uint GetSampleRate()
        {
            return (uint)m_FReader.Mp3WaveFormat.SampleRate;
        }

        /// <summary>
        /// Returns the decompressed wav data for this MP3File instance.
        /// </summary>
        /// <returns>The decompressed wav data as a byte array.</returns>
        public byte[] DecompressedWav()
        {
            byte[] Outputbuffer = new byte[m_FReader.Length];
            m_FReader.Read(Outputbuffer, 0, Outputbuffer.Length);

            return Outputbuffer;
        }
    }
}
