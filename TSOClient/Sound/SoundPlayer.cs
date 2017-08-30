/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Timers;
using Files.Manager;
using Files.AudioFiles;
using Microsoft.Xna.Framework.Audio;

namespace Sound
{
    /// <summary>
    /// Represents an active, playable sound.
    /// </summary>
    public class ActiveSound : IDisposable
    {
        public bool LoopIt = true; //Should the MP3 be looped?
        public MP3File MP3;
        public DynamicSoundEffectInstance DynInstance;

        public SoundEffectInstance Instance;
        public Timer FadeOutTimer;
        public bool FadeOut = false;

        /// <summary>
        /// Disposes of the resources used by this ActiveSound instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the resources used by this ActiveSound instance.
        /// <param name="CleanUpNativeAndManagedResources">Should both native and managed resources be cleaned?</param>
        /// </summary>
        protected virtual void Dispose(bool CleanUpNativeAndManagedResources)
        {
            if (CleanUpNativeAndManagedResources)
            {
                if (Instance != null)
                    Instance.Dispose();
                if (DynInstance != null)
                    DynInstance.Dispose();
            }
        }
    }
    
    /// <summary>
    /// A player that can play ActiveSound instances.
    /// </summary>
    public class SoundPlayer : IDisposable
    {
        private ActiveSound m_ASound;

        /// <summary>
        /// Creates a new SoundPlayer instance.
        /// </summary>
        /// <param name="DecompressedWavData">The decompressed wav data that makes up the sound.</param>
        /// <param name="SampleRate">The sample rate of the sound to play.</param>
        public SoundPlayer(byte[] DecompressedWavData, uint SampleRate)
        {
            SoundEffect Efx = new SoundEffect(DecompressedWavData, (int)SampleRate, AudioChannels.Stereo);
            SoundEffectInstance Instance = Efx.CreateInstance();

            m_ASound = new ActiveSound();
            m_ASound.Instance = Instance;
        }

        /// <summary>
        /// Creates a new SoundPlayer instance from a string,
        /// for playing MP3 music.
        /// </summary>
        /// <param name="MP3Sound">The name of an MP3 to play.</param>
        public SoundPlayer(string MP3Sound, bool LoopIt = true)
        {
            MP3File MP3 = (MP3File)FileManager.GetMusic(MP3Sound);
            DynamicSoundEffectInstance Instance = new DynamicSoundEffectInstance((int)MP3.GetSampleRate(), AudioChannels.Stereo);
            Instance.BufferNeeded += Instance_BufferNeeded;

            m_ASound = new ActiveSound();
            m_ASound.MP3 = MP3;
            m_ASound.LoopIt = LoopIt;
            m_ASound.DynInstance = Instance;
            Instance.Play();
        }

        /// <summary>
        /// A DynamicSoundEffectBuffer needed more data to continue playing an MP3!
        /// </summary>
        private void Instance_BufferNeeded(object sender, EventArgs e)
        {
            byte[] Buffer = m_ASound.MP3.DecompressedWav();
            m_ASound.DynInstance.SubmitBuffer(Buffer);

            if (Buffer.Length != m_ASound.MP3.BufferSize)
            {
                if (m_ASound.LoopIt == true)
                {
                    m_ASound.DynInstance.SubmitBuffer(m_ASound.MP3.Reset(Buffer.Length, 
                        m_ASound.MP3.BufferSize - Buffer.Length));
                }
                else
                {
                    if (Buffer.Length == 0)
                    {
                        StopSound();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new SoundPlayer instance.
        /// </summary>
        public SoundPlayer()
        {
        }

        /// <summary>
        /// Starts playing a sound.
        /// </summary>
        /// <param name="LoopIt">Wether or not to loop the sound.</param>
        /// <param name="FadeOut">Should this sound fade out?</param>
        public void PlaySound(bool LoopIt = false, bool FadeOut = false)
        {
            if (LoopIt)
                m_ASound.Instance.IsLooped = true;

            if (FadeOut)
                m_ASound.FadeOut = true;

            m_ASound.Instance.Play();
        }

        /// <summary>
        /// Is this sound playing?
        /// </summary>
        /// <returns>True if it is, false otherwise.</returns>
        public bool IsPlaying()
        {
            if (m_ASound == null)
                return false;

            return m_ASound.Instance.State == SoundState.Playing;
        }

        /// <summary>
        /// Has this sound finished playing?
        /// </summary>
        /// <returns>True if it has, false otherwise.</returns>
        public bool IsEnded()
        {
            if (m_ASound == null)
                return true;

            return m_ASound.Instance.State == SoundState.Stopped;
        }

        /// <summary>
        /// Stops playing a sound. If the sound is meant to fade out, it will fade out before stopping.
        /// </summary>
        public void StopSound()
        {
            if (m_ASound != null)
            {
                if (!m_ASound.FadeOut)
                {
                    if (m_ASound.Instance != null)
                        m_ASound.Instance.Stop();
                    else
                    {
                        m_ASound.DynInstance.Stop();
                        m_ASound.DynInstance.BufferNeeded -= Instance_BufferNeeded;
                    }
                }
                else
                {
                    m_ASound.FadeOutTimer = new Timer();
                    m_ASound.FadeOutTimer.Interval = 200;
                    m_ASound.FadeOutTimer.Enabled = true;
                    m_ASound.FadeOutTimer.Elapsed += FadeOutTimer_Elapsed;
                    m_ASound.FadeOutTimer.Start();
                }
            }
        }

        private void FadeOutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer T = (Timer)sender;

            if (m_ASound.FadeOutTimer == T)
            {
                if (m_ASound.Instance.Volume > 0)
                {
                    try
                    {
                        m_ASound.Instance.Volume -= 0.10f;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        m_ASound.Instance.Stop();
                    }
                }
                else
                    m_ASound.Instance.Stop();
            }
        }

        /// <summary>
        /// Disposes of the resources used by this MP3Player instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the resources used by this MP3Player instance.
        /// <param name="CleanUpNativeAndManagedResources">Should both native and managed resources be cleaned?</param>
        /// </summary>
        protected virtual void Dispose(bool CleanUpNativeAndManagedResources)
        {
            if (CleanUpNativeAndManagedResources)
            {
                if (m_ASound != null)
                    m_ASound.Dispose();
            }
        }
    }
}
