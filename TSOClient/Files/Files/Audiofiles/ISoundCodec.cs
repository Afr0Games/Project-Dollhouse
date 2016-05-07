using System;
using System.Collections.Generic;
using System.Text;

namespace Files.AudioFiles
{
    public interface ISoundCodec
    {
        /// <summary>
        /// Returns the bitrate for the wav data that makes up this sound.
        /// </summary>
        /// <returns>A ushort denoting the bitrate of the wav data that makes up this sound.</returns>
        ushort GetBitrate();

        /// <summary>
        /// Gets the decompressed wav data for this sound codec.
        /// </summary>
        /// <returns>The decompressed wav data as an array of bytes.</returns>
        byte[] DecompressedWav();
    }
}
