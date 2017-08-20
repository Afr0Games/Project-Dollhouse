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
using System.Collections.Generic;
using Files.Manager;
using Files.AudioFiles;

namespace Sound
{
    /// <summary>
    /// Replacement for SubRoutine to handle the kTurnOnTV event. Used by radio, TV and UI music.
    /// </summary>
    public class HITTVOn
    {
        private uint m_ID = 0;
        private List<string> m_Sounds = new List<string>();

        private SoundPlayer m_MusicInstance;
        private SoundPlayer m_SoundInstance;

        private bool m_IsMusic = false;
        private bool m_Dead = false;
        private bool m_EverHadOwners = false; //Has this thread ever had owners? If not, don't kill the thread.

        List<int> m_Owners = new List<int>();

        /// <summary>
        /// Add an owner for this thread.
        /// </summary>
        /// <param name="ID">The ID of the owner to add.</param>
        public void AddOwner(int ID)
        {
            m_EverHadOwners = true;
            m_Owners.Add(ID);
        }

        /// <summary>
        /// Remove an owner for this thread.
        /// </summary>
        /// <param name="ID">The ID of the owner to remove.</param>
        public void RemoveOwner(int ID)
        {
            m_Owners.Remove(ID);
        }

        /// <summary>
        /// The SoundID for the sound that this HITTVOn event handles.
        /// </summary>
        public uint SoundID
        {
            get { return m_ID; }
        }

        /// <summary>
        /// Kills this thread.
        /// </summary>
        public void Kill()
        {
            if (m_MusicInstance != null || m_SoundInstance != null)
                StopLastSound();

            m_Dead = true;
        }

        public HITTVOn(uint ID)
        {
            m_ID = ID;

            m_SoundInstance = new SoundPlayer();

            if (ID == 5) //Loadloop, play the sound directly.
            {
                ISoundCodec Loadloop = FileManager.GetSound(0x00004f85);
                m_IsMusic = true;
                m_MusicInstance = new SoundPlayer(Loadloop.DecompressedWav(), Loadloop.GetSampleRate());
                m_MusicInstance.PlaySound(true, true);
            }
            else
            {
                m_SoundInstance = new SoundPlayer();
                m_MusicInstance = new SoundPlayer();

                string StrID = "";
                HitVM.MusicModes.TryGetValue((int)ID, out StrID);
                string Path = HitVM.GetStationPath(StrID);

                if (Path != "")
                    LoadMusicFromPath(Path);
            }
        }

        /// <summary>
        /// Ticks this HITTVOn thread. Should be called once per frame.
        /// </summary>
        /// <returns></returns>
        public bool Tick()
        {
            if (m_Dead) return false;

            if (m_EverHadOwners && m_Owners.Count == 0)
            {
                Kill();
                return false;
            }

            if ((m_IsMusic && m_MusicInstance.IsEnded()) || m_IsMusic != true && m_SoundInstance.IsPlaying() != true)
            {
                if (PlayNextSound()) return true;
                else
                {
                    m_Dead = true;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Loads music or a station from a path. 
        /// </summary>
        /// <param name="MusicOrStationPath"></param>
        private void LoadMusicFromPath(string MusicOrStationPath)
        {
            Random Rand = new Random();

            string[] Files = Directory.GetFiles(FileManager.BaseDirectory + MusicOrStationPath, "*.xa", SearchOption.AllDirectories);

            if (Files.Length > 0) //tvstations
            {
                string BaseDir = Path.GetDirectoryName(FileManager.BaseDirectory + MusicOrStationPath);

                int Index = BaseDir.LastIndexOf('/'); //Linux
                if (Index == -1) Index = BaseDir.LastIndexOf('\\');

                //Add commercials from previous directory.
                string[] Files2 = Directory.GetFiles(BaseDir.Substring(0, Index), "*.xa", SearchOption.TopDirectoryOnly);
                foreach (string Fle in Files2)
                    m_Sounds.Insert(Rand.Next(m_Sounds.Count + 1), Fle);
            }
            else //Music
            {
                m_IsMusic = true;
                Files = Directory.GetFiles(FileManager.BaseDirectory + MusicOrStationPath, "*.mp3", SearchOption.AllDirectories);
            }

            foreach (string Fle in Files)
                m_Sounds.Insert(Rand.Next(m_Sounds.Count + 1), Path.GetFileName(Fle));
        }

        /// <summary>
        /// Stops playing the last sound playing/that was played.
        /// </summary>
        private void StopLastSound()
        {
            if(m_IsMusic)
            {
                if (m_MusicInstance != null)
                {
                    m_MusicInstance.StopSound();
                    m_MusicInstance.Dispose();
                }
            }
            else
            {
                if (m_SoundInstance != null)
                {
                    m_SoundInstance.StopSound();
                    m_SoundInstance.Dispose();
                }
            }

            m_SoundInstance = null;
            m_MusicInstance = null;
        }

        /// <summary>
        /// Plays the next available sound.
        /// </summary>
        /// <returns>Returns true if this thread has one or more sounds that it can play.</returns>
        public bool PlayNextSound()
        {
            if (m_Sounds.Count == 0) return false;

            StopLastSound();

            string Sound = m_Sounds[0];
            m_Sounds.RemoveAt(0);
            m_Sounds.Insert(m_Sounds.Count + (new Random()).Next(1), Sound);

            if (m_IsMusic)
            {
                m_MusicInstance = new SoundPlayer(Sound);
                m_MusicInstance.PlaySound(false, true);
            }
            else
            {
                XAFile XA = new XAFile(Sound);
                m_SoundInstance = new SoundPlayer(XA.DecompressedStream.ToArray(), XA.GetSampleRate());
                m_SoundInstance.PlaySound();
            }

            return true;
        }
    }
}
