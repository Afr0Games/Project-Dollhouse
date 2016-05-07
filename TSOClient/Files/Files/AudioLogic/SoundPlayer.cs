using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Files.AudioLogic
{
    /// <summary>
    /// Called upon by HIT subroutines to play sounds.
    /// </summary>
    public class SoundPlayer //TODO: Move this class into different library...
    {
        private static List<SFX> m_SEffects = new List<SFX>();

        /// <summary>
        /// Starts playing a sound.
        /// </summary>
        /// <param name="WavData">The wav data for this sound.</param>
        /// <param name="Bitrate">The bitrate of the data.</param>
        /// <param name="LoopIt">Wether or not to loop the sound.</param>
        public static void PlaySound(byte[] WavData, ushort Bitrate, bool LoopIt = false)
        {
            SoundEffect Efx = new SoundEffect(WavData, Bitrate, AudioChannels.Stereo);
            SoundEffectInstance Inst = Efx.CreateInstance();

            if (LoopIt)
                Inst.IsLooped = true;

            Efx.Play();
            m_SEffects.Add(new SFX(Efx, Inst));
        }
    }

    /// <summary>
    /// Holds a SoundEffectInstance and SoundEffect that is used by the SoundPlayer.
    /// </summary>
    public class SFX
    {
        public SoundEffectInstance Instance;
        public SoundEffect Efx;

        public SFX(SoundEffect Sound, SoundEffectInstance Inst)
        {
            Instance = Inst;
            Efx = Sound;
        }
    }
}
