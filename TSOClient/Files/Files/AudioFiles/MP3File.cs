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
using Files.Manager;
using System.Reflection;
using log4net;

namespace Files.AudioFiles
{
    public enum MP3Channels
    {
        Stereo = 00,
        JointStereo = 01,
        DualChannel = 10,
        SingleChannel = 11
    }

    public class MP3File : ISoundCodec, IDisposable
    {
        private MP3Stream m_Stream;
        public ReadFullyStream RFullyStream;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// How many channels in this MP3 file.
        /// </summary>
        public int Channels = 0;

        /// <summary>
        /// How many bytes to read when calling DecompressedWav().
        /// </summary>
        public int BufferSize = 4096;

        public MP3File(string Path)
        {
            if(FileManager.IsLinux)
                m_Stream = new MP3Stream(File.Open(Path, FileMode.Open, FileAccess.ReadWrite));
            else //Used by NAudio, only on Windows.
                RFullyStream = new ReadFullyStream(m_Stream);
        }

        public MP3File(Stream Data)
        {
            if (FileManager.IsLinux)
            {
                m_Stream = new MP3Stream(Data);
                Channels = m_Stream.ChannelCount;
            }
            else //Used by NAudio, only on Windows.
                RFullyStream = new ReadFullyStream(Data);
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
            /*byte[] Buffer = new byte[BufferSize];
            int BytesRead = m_Stream.Read(Buffer, 0, BufferSize);

            if (BytesRead != 0)
            {
                if (BytesRead < BufferSize)
                    Array.Resize(ref Buffer, BytesRead);

                return Buffer;
            }

            return null;*/

            m_Stream.DecodeFrames(1);
            byte[] Buffer = new byte[m_Stream.Length];
            int BytesRead = 1;

            while (BytesRead > 0)
                BytesRead = m_Stream.Read(Buffer, 0, BufferSize);

            return Buffer;
        }

        ~MP3File()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR3Archive instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this FAR3Archive instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_Stream != null)
                    m_Stream.Close();
                if (RFullyStream != null)
                    RFullyStream.Close();

                // Prevent the finalizer from calling ~MP3File, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("MP3File not explicitly disposed!");
        }
    }
}
