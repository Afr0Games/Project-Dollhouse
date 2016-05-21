using System;
using System.Collections.Generic;
using System.Text;

namespace Files.AudioFiles
{
    public interface ISoundCodec
    {
        /// <summary>
        /// Returns the sample rate for the wav data that makes up this sound.
        /// </summary>
        /// <returns>A uint denoting the sample rate of the wav data that makes up this sound.</returns>
        uint GetSampleRate();

        /// <summary>
        /// Gets the decompressed wav data for this sound codec.
        /// </summary>
        /// <returns>The decompressed wav data as an array of bytes.</returns>
        byte[] DecompressedWav();
    }
}
