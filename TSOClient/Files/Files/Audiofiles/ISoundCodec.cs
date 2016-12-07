/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

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
