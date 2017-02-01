/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework.Audio;

namespace Sound
{
    /// <summary>
    /// Called upon by HIT subroutines to play sounds.
    /// </summary>
    public class SoundPlayer
    {
        /// <summary>
        /// Starts playing a sound.
        /// </summary>
        /// <param name="WavData">The wav data for this sound.</param>
        /// <param name="SampleRate">The sample rate of the data.</param>
        /// <param name="LoopIt">Wether or not to loop the sound.</param>
        public static void PlaySound(byte[] WavData, uint SampleRate, bool LoopIt = false)
        {
            SoundEffect Efx = new SoundEffect(WavData, (int)SampleRate / 2, AudioChannels.Stereo);
            SoundEffectInstance Inst = Efx.CreateInstance();

            if (LoopIt)
                Inst.IsLooped = true;

            Inst.Play();
        }
    }

    /// <summary>
    /// Holds a SoundEffectInstance and SoundEffect that is used by the SoundPlayer.
    /// </summary>
    /*public class SFX
    {
        public SoundEffectInstance Instance;
        public SoundEffect Efx;

        /// <summary>
        /// Creates a new SFX instance.
        /// </summary>
        /// <param name="Sound">The SoundEffect instance used to store the sound.</param>
        /// <param name="ID">The InstanceID (from a DBPF) of the sound.</param>
        public SFX(SoundEffect Sound, SoundEffectInstance Inst)
        {
            Instance = Inst;
            Efx = Sound;
        }
    }*/
}
