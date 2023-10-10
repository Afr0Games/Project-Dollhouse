/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Threading;
using Files.Manager;
using Files.AudioFiles;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
//using MonogameAudio;
using log4net;
using System.Reflection;

namespace Sound
{
    enum StreamingPlaybackState
    {
        Stopped,
        Playing,
        Buffering,
        Paused
    }

    /// <summary>
    /// Represents an active, playable sound.
    /// </summary>
    public class ActiveSound : IDisposable
    {
        public bool LoopIt = true; //Should the MP3 be looped?
        public MP3File MP3;
        //public DynamicSoundEffectInstance DynInstance;

        public SoundEffectInstance Instance;
        public System.Timers.Timer FadeOutTimer;
        public System.Timers.Timer PlayTimer;
        public bool FadeOut = false;

        public BufferedWaveProvider WavProvider;
        public IWavePlayer Player;
        public WaveOutEvent WOut;
        public VolumeWaveProvider16 VolumeProvider;
        public bool FullyStreamed = false;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ~ActiveSound()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this ActiveSound instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this ActiveSound instance.
        /// <param name="CleanUpNativeAndManagedResources">Should both native and managed resources be cleaned?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (Instance != null)
                    Instance.Dispose();
                /*if (DynInstance != null)
                    DynInstance.Dispose();*/
                if (WOut != null)
                    WOut.Dispose();

                // Prevent the finalizer from calling ~ActiveSound, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("ActiveSound instance not explicitly disposed!");
        }
    }
    
    /// <summary>
    /// A player that can play ActiveSound instances.
    /// </summary>
    public class SoundPlayer : IDisposable
    {
        private ActiveSound m_ASound;
        private StreamingPlaybackState m_PlaybackState = StreamingPlaybackState.Stopped;
        private Task m_StreamingTask;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
        /// <param name="MP3Sound">The name of an MP3 to play, or the path of a file to play if called by a test.</param>
        public SoundPlayer(string MP3Sound, bool LoopIt = true)
        {
            MP3File MP3;

            if (FileManager.Instance.IsInitialized) //Only used by Files.Tests!
                MP3 = (MP3File)FileManager.Instance.GetMusic(MP3Sound);
            else
                MP3 = new MP3File(MP3Sound);

            /*DynamicSoundEffectInstance Instance = new DynamicSoundEffectInstance((int)MP3.GetSampleRate(), AudioChannels.Stereo);
            Instance.BufferNeeded += Instance_BufferNeeded;*/

            m_ASound = new ActiveSound();
            m_ASound.MP3 = MP3;
            m_ASound.LoopIt = LoopIt;
            //m_ASound.DynInstance = Instance;

            if (!FileManager.IsLinux)
            {
                m_ASound.PlayTimer = new System.Timers.Timer();
                m_ASound.PlayTimer.Interval = 250;
                m_ASound.PlayTimer.Elapsed += PlayTimer_Elapsed;
                m_ASound.PlayTimer.Start();

                m_StreamingTask = new Task(new Action(ReadFromStream));
                m_PlaybackState = StreamingPlaybackState.Buffering;
                m_StreamingTask.Start();
            }
        }

        private void PlayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (TheLock)
            {
                if (m_PlaybackState != StreamingPlaybackState.Stopped)
                {
                    if (m_ASound != null)
                    {
                        if (m_ASound.WOut == null && m_ASound.WavProvider != null)
                        {
                            m_ASound.WOut = new WaveOutEvent();
                            m_ASound.VolumeProvider = new VolumeWaveProvider16(m_ASound.WavProvider);
                            m_ASound.VolumeProvider.Volume = 0.5f;
                            m_ASound.WOut.Init(m_ASound.VolumeProvider);
                        }
                        else if (m_ASound.WavProvider != null)
                        {
                            double BufferedSeconds = m_ASound.WavProvider.BufferedDuration.TotalSeconds;

                            //Make it stutter less if we buffer up a decent amount before playing
                            if (BufferedSeconds < 0.5 && m_PlaybackState == StreamingPlaybackState.Playing && !m_ASound.FullyStreamed)
                            {
                                m_PlaybackState = StreamingPlaybackState.Buffering;
                                m_ASound.WOut.Pause();
                            }
                            else if (BufferedSeconds > 4 && m_PlaybackState == StreamingPlaybackState.Buffering)
                            {
                                m_ASound.WOut.Play();
                                m_PlaybackState = StreamingPlaybackState.Playing;
                            }
                            else if (m_ASound.FullyStreamed && BufferedSeconds == 0)
                                StopPlayback();
                        }
                    }
                }
            }
        }

        private void StopPlayback()
        {
            if(m_PlaybackState != StreamingPlaybackState.Stopped)
            {
                m_PlaybackState = StreamingPlaybackState.Stopped;
                if (m_ASound.WOut != null)
                {
                    m_ASound.WOut.Stop();
                    m_ASound.WOut.Dispose();
                    m_ASound.WOut = null;
                }

                m_ASound.PlayTimer.Stop();
            }
        }

        /// <summary>
        /// A DynamicSoundEffectBuffer needed more data to continue playing an MP3!
        /// </summary>
        /*private void Instance_BufferNeeded(object sender, EventArgs e)
        {
            Task StreamingTask = new Task(new Action(ReadFromStream));
            StreamingTask.Start();
        }*/

        private byte[] m_Buffer = new byte[16384 * 4];

        private void ReadFromStream()
        {
            IMp3FrameDecompressor Decompressor = null;

            try
            {
                do
                {
                    if (IsBufferNearlyFull)
                        Thread.Sleep(500);
                    else
                    {
                        Mp3Frame Frame;

                        try
                        {
                            Frame = Mp3Frame.LoadFromStream(m_ASound.MP3.RFullyStream);
                        }
                        catch(EndOfStreamException)
                        {
                            m_ASound.FullyStreamed = true;
                            break;
                        }

                        if(Decompressor == null)
                        {
                            Decompressor = CreateFrameDecompressor(Frame);
                            m_ASound.WavProvider = new BufferedWaveProvider(Decompressor.OutputFormat);
                            m_ASound.WavProvider.BufferDuration = TimeSpan.FromSeconds(30);
                        }

                        int Decompressed = Decompressor.DecompressFrame(Frame, m_Buffer, 0);
                        m_ASound.WavProvider.AddSamples(m_Buffer, 0, Decompressed);
                    }
                } while (m_PlaybackState != StreamingPlaybackState.Stopped);
            }
            finally
            {
                if (Decompressor != null)
                    Decompressor.Dispose();
            }
        }

        private IMp3FrameDecompressor CreateFrameDecompressor(Mp3Frame Frame)
        {
            WaveFormat WaveFormat = new Mp3WaveFormat(Frame.SampleRate, Frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                Frame.FrameLength, Frame.BitRate);
            return new AcmMp3FrameDecompressor(WaveFormat);
        }

        private bool IsBufferNearlyFull
        {
            get
            {
                return m_ASound.WavProvider != null &&
                       m_ASound.WavProvider.BufferLength - m_ASound.WavProvider.BufferedBytes
                       < m_ASound.WavProvider.WaveFormat.AverageBytesPerSecond / 4;
            }
        }

        private object TheLock = new object();
        
        /// <summary>
        /// Asynchronously reads data from an MP3.
        /// </summary>
        /*private void ReadFromStream()
        {
            int Count = 3;

            lock (TheLock)
            {
                while (Count > 0)
                {
                    byte[] Buffer = m_ASound.MP3.DecompressedWav();

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

                    m_ASound.DynInstance.SubmitBuffer(Buffer);
                    Count--;
                }
            }
        }*/

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
            lock (TheLock)
            {
                if (LoopIt)
                    m_ASound.Instance.IsLooped = true;

                if (FadeOut)
                    m_ASound.FadeOut = true;

                if (m_ASound.Instance != null)
                    m_ASound.Instance.Play();
                /*else
                    m_ASound.DynInstance.Play();*/
            }
        }

        /// <summary>
        /// Is this sound playing?
        /// </summary>
        /// <returns>True if it is, false otherwise.</returns>
        public bool IsPlaying()
        {
            if (m_ASound == null)
                return false;

            /*if (m_ASound.DynInstance != null)
                return m_ASound.Instance.State == SoundState.Playing;
            else
                return m_ASound.DynInstance.State == SoundState.Playing;*/

            if (m_ASound.Instance != null)
                return m_ASound.Instance.State == SoundState.Playing;
            else
                return m_PlaybackState == StreamingPlaybackState.Playing;
        }

        /// <summary>
        /// Has this sound finished playing?
        /// </summary>
        /// <returns>True if it has, false otherwise.</returns>
        public bool IsEnded()
        {
            if (m_ASound == null)
                return true;

            if (m_ASound.Instance != null)
                return m_ASound.Instance.State == SoundState.Stopped;
            /*else
                return m_ASound.DynInstance.State == SoundState.Stopped;*/
            else
                return m_PlaybackState == StreamingPlaybackState.Stopped;
        }

        /// <summary>
        /// Stops playing a sound. If the sound is meant to fade out, it will fade out before stopping.
        /// </summary>
        public void StopSound()
        {
            lock (TheLock)
            {
                if (m_ASound != null)
                {
                    if (!m_ASound.FadeOut)
                    {
                        if (m_ASound.Instance != null)
                            m_ASound.Instance.Stop();
                        /*else
                        {
                            m_ASound.DynInstance.Stop();
                            m_ASound.DynInstance.BufferNeeded -= Instance_BufferNeeded;
                        }*/
                    }
                    else
                    {
                        m_ASound.FadeOutTimer = new System.Timers.Timer();
                        m_ASound.FadeOutTimer.Interval = 200;
                        m_ASound.FadeOutTimer.Enabled = true;
                        m_ASound.FadeOutTimer.Elapsed += FadeOutTimer_Elapsed;
                        m_ASound.FadeOutTimer.Start();
                    }
                }
            }
        }

        private void FadeOutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Timers.Timer T = (System.Timers.Timer)sender;

            if (m_ASound.FadeOutTimer == T)
            {
                if (m_ASound.Instance != null)
                {
                    if (m_ASound.Instance.Volume > 0)
                    {
                        try
                        {
                            m_ASound.Instance.Volume -= 0.10f;
                        }
                        catch (Exception E)
                        {
                            if(E is ArgumentOutOfRangeException)
                                m_ASound.Instance.Stop();
                            if (E is NullReferenceException)
                                return;
                        }
                    }
                    else
                        m_ASound.Instance.Stop();
                }
                /*else
                {
                    if (m_ASound.DynInstance.Volume > 0)
                    {
                        try
                        {
                            m_ASound.DynInstance.Volume -= 0.10f;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            //m_ASound.DynInstance.Stop();
                        }
                    }
                    else
                        //m_ASound.DynInstance.Stop();
                }*/
                else
                {
                    if (m_ASound.VolumeProvider.Volume > 0)
                    {
                        try
                        {
                            m_ASound.VolumeProvider.Volume -= 0.10f;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            StopPlayback();
                        }
                    }
                }
            }
        }

        ~SoundPlayer()
        {
            Dispose(false); //Cleans up the streaming task.
        }

        /// <summary>
        /// Disposes of the resources used by this SoundPlayer instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this SoundPlayer instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_ASound != null)
                    m_ASound.Dispose();

                //Short answer: Don't bother disposing of your tasks.
                //https://devblogs.microsoft.com/pfxteam/do-i-need-to-dispose-of-tasks/
                /*if (m_StreamingTask != null)
                {
                    m_StreamingTask.Wait();
                    m_StreamingTask.Dispose();
                }*/

                // Prevent the finalizer from calling ~SoundPlayer, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("Soundplayer not explicitly disposed!");
        }
    }
}
