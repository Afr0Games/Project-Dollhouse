/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Microsoft.Xna.Framework.Audio;

namespace Sound
{
    /// <summary>
    /// A sounds that is currently being played.
    /// </summary>
    public class ActiveSound
    {
        public SoundEffectInstance Instance;
        public bool FadeOut = false;
        public Timer FadeOutTimer;
    }

    /// <summary>
    /// Called upon by HIT subroutines to play sounds.
    /// </summary>
    public class SoundPlayer
    {
        private static Dictionary<uint, ActiveSound> m_ActiveSounds = new Dictionary<uint, ActiveSound>();

        /// <summary>
        /// Starts playing a sound.
        /// </summary>
        /// <param name="WavData">The wav data for this sound.</param>
        /// <param name="SampleRate">The sample rate of the data.</param>
        /// <param name="LoopIt">Wether or not to loop the sound.</param>
        /// <param name="FadeOut">Should this sound fade out?</param>
        public static void PlaySound(byte[] WavData, uint SoundID, uint SampleRate, bool LoopIt = false,
            bool FadeOut = false)
        {
            SoundEffect Efx = new SoundEffect(WavData, (int)SampleRate, AudioChannels.Stereo);
            SoundEffectInstance Inst = Efx.CreateInstance();

            ActiveSound ASound = new ActiveSound();
            ASound.Instance = Inst;
            if (FadeOut) ASound.FadeOut = true;

            m_ActiveSounds.Add(SoundID, ASound);

            if (LoopIt)
                Inst.IsLooped = true;

            Inst.Play();
        }

        /// <summary>
        /// Gets the sample rate from wav data which includes the RIFF header.
        /// </summary>
        /// <param name="FullWavData">Wav data including RIFF header.</param>
        /// <returns>The wav's sampling rate.</returns>
        public static uint GetSampleRate(byte[] FullWavData)
        {
            BinaryReader Reader = new BinaryReader(new MemoryStream(FullWavData));
            //SampleRate starts at offset 24 - http://soundfile.sapp.org/doc/WaveFormat/
            Reader.BaseStream.Seek(24, SeekOrigin.Begin);
            return Reader.ReadUInt32();
        }

        /// <summary>
        /// Stops playing a sound. If the sound is meant to fade out, it will fade out before stopping.
        /// </summary>
        /// <param name="SoundID">ID of the sound to stop.</param>
        public void StopSound(uint SoundID)
        {
            if (!m_ActiveSounds[SoundID].FadeOut)
            {
                m_ActiveSounds[SoundID].Instance.Stop();
                m_ActiveSounds.Remove(SoundID);
            }
            else
            {
                m_ActiveSounds[SoundID].FadeOutTimer = new Timer();
                m_ActiveSounds[SoundID].FadeOutTimer.Interval = 200;
                m_ActiveSounds[SoundID].FadeOutTimer.Enabled = true;
                m_ActiveSounds[SoundID].FadeOutTimer.Elapsed += FadeOutTimer_Elapsed;
                m_ActiveSounds[SoundID].FadeOutTimer.Start();
            }
        }

        private void FadeOutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer T = (Timer)sender;

            foreach (KeyValuePair<uint, ActiveSound> KVP in m_ActiveSounds)
            {
                if (KVP.Value.FadeOutTimer == T)
                {
                    if (KVP.Value.Instance.Volume > 0)
                        KVP.Value.Instance.Volume -= 0.20f;
                    else
                        KVP.Value.Instance.Stop();
                }
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
}
