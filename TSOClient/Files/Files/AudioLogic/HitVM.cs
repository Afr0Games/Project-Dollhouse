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
using Files.Manager;

namespace Files.AudioLogic
{
    public class HitVM
    {
        private static List<SubRoutine> m_CurrentlyPlayingTracks = new List<SubRoutine>();

        private static Dictionary<int, int> m_GlobalVars = new Dictionary<int, int>();

        /// <summary>
        /// Sets a global variable to a value.
        /// </summary>
        /// <param name="Location">The location of the global variable to set.</param>
        /// <param name="Value">The value to set the variable to.</param>
        public static void SetGlobalVar(int Location, int Value)
        {
            m_GlobalVars[Location] = Value;
        }

        /// <summary>
        /// Reads a global variable.
        /// </summary>
        /// <param name="Location">The location to read from.</param>
        /// <returns>The global variable at the location.</returns>
        public static int GetGlobalVar(int Location)
        {
            return m_GlobalVars[Location];
        }

        /// <summary>
        /// All playable events registered by the VM.
        /// </summary>
        public static Dictionary<string, RegisteredEvent> Events = new Dictionary<string, RegisteredEvent>();

        private static HitResourcegroup Rsc_newmain, Rsc_relationships, Rsc_tsoep5, Rsc_tsov2, Rsc_tsov3, Rsc_turkey;
        public static bool IsInitialized = false;

        public HitVM(string StartupDir)
        {
			if (IsLinux)
			{
				Rsc_newmain = new HitResourcegroup (StartupDir + "sounddata/newmain.hit", 
					StartupDir + "sounddata/eventlist.txt", StartupDir + "sounddata/newmain.hsm");
				Rsc_relationships = new HitResourcegroup (StartupDir + "sounddata/relationships.hit", 
					StartupDir + "sounddata/relationships.evt", StartupDir + "sounddata/relationships.hsm");
				Rsc_tsoep5 = new HitResourcegroup (StartupDir + "sounddata/tsoep5.hit", 
					StartupDir + "sounddata/tsoep5.evt", StartupDir + "sounddata/tsoep5.hsm");
				Rsc_tsov2 = new HitResourcegroup (StartupDir + "sounddata/tsov2.hit", 
					StartupDir + "sounddata/tsov2.evt", "");
				Rsc_tsov3 = new HitResourcegroup (StartupDir + "sounddata/tsov3.hit",
					StartupDir + "sounddata/tsov3.evt", StartupDir + "sounddata/tsov3.hsm");
				Rsc_turkey = new HitResourcegroup (StartupDir + "sounddata/turkey.hit", 
					StartupDir + "sounddata/turkey.evt", StartupDir + "sounddata/turkey.hsm");
			} 
			else
			{
				Rsc_newmain = new HitResourcegroup (StartupDir + "sounddata\\newmain.hit", 
					StartupDir + "sounddata\\eventlist.txt", StartupDir + "sounddata\\newmain.hsm");
				Rsc_relationships = new HitResourcegroup (StartupDir + "sounddata\\relationships.hit", 
					StartupDir + "sounddata\\relationships.evt", StartupDir + "sounddata\\relationships.hsm");
				Rsc_tsoep5 = new HitResourcegroup (StartupDir + "sounddata\\tsoep5.hit", 
					StartupDir + "sounddata\\tsoep5.evt", StartupDir + "sounddata\\tsoep5.hsm");
				Rsc_tsov2 = new HitResourcegroup (StartupDir + "sounddata\\tsov2.hit", 
					StartupDir + "sounddata\\tsov2.evt", "");
				Rsc_tsov3 = new HitResourcegroup (StartupDir + "sounddata\\tsov3.hit",
					StartupDir + "sounddata\\tsov3.evt", StartupDir + "sounddata\\tsov3.hsm");
				Rsc_turkey = new HitResourcegroup (StartupDir + "sounddata\\turkey.hit", 
					StartupDir + "sounddata\\turkey.evt", StartupDir + "sounddata\\turkey.hsm");
			}

            RegisterEvent(Rsc_newmain);
            RegisterEvent(Rsc_relationships);
            RegisterEvent(Rsc_tsoep5);
            RegisterEvent(Rsc_tsov2);
            RegisterEvent(Rsc_tsov3);
            RegisterEvent(Rsc_turkey);

            m_GlobalVars = new Dictionary<int, int>();
            m_GlobalVars.Add(0x64, 0); //SimSpeed
            m_GlobalVars.Add(0x65, 0); //test_g1
            m_GlobalVars.Add(0x66, 0); //test_g2
            m_GlobalVars.Add(0x67, 0); //test_g3
            m_GlobalVars.Add(0x68, 0); //test_g4
            m_GlobalVars.Add(0x69, 0); //test_g5
            m_GlobalVars.Add(0x6a, 0); //test_g6
            m_GlobalVars.Add(0x6b, 0); //test_g7
            m_GlobalVars.Add(0x6c, 0); //test_g8
            m_GlobalVars.Add(0x6d, 0); //test_g9
            m_GlobalVars.Add(0x6e, 0); //main_songnum
            m_GlobalVars.Add(0x6f, 0); //main_musichitlistid
            m_GlobalVars.Add(0x70, 0); //campfire_nexttrack
            m_GlobalVars.Add(0x71, 0); //campfire_busy
            m_GlobalVars.Add(0x7b, 0); //main_duckpri
            m_GlobalVars.Add(0x7c, 0); //main_vol
            m_GlobalVars.Add(0x7d, 0); //main_fxtype
            m_GlobalVars.Add(0x7e, 0); //main_fxlevel
            m_GlobalVars.Add(0x7f, 0); //main_pause
            m_GlobalVars.Add(0x80, 0); //CurrentFloor
            m_GlobalVars.Add(0x81, 0); //Hour
            m_GlobalVars.Add(0x82, 0); //RoomSize
            m_GlobalVars.Add(0x83, 0); //OutdoorRatio
            m_GlobalVars.Add(0x84, 0); //OptionSfxVol
            m_GlobalVars.Add(0x85, 0); //OptionVoxVol
            m_GlobalVars.Add(0x86, 0); //OptionMusicVol
            m_GlobalVars.Add(0x87, 0); //CampfireSize

            IsInitialized = true;
        }

        /// <summary>
        /// Registers all the events in a resource group.
        /// </summary>
        /// <param name="RscGroup">The resource group to go through.</param>
        private void RegisterEvent(HitResourcegroup RscGroup)
        {
            foreach(TrackEvent TEvent in  RscGroup.Events.Events)
            {
                RegisteredEvent Event = new RegisteredEvent();
                Event.Name = TEvent.Name;
                Event.TrackID = TEvent.TrackID;
                Event.Rsc = RscGroup;
                if(!Events.ContainsKey(TEvent.Name))
                    Events.Add(TEvent.Name, Event);
            }
        }

        /// <summary>
        /// Runs one instruction for each track that is currently playing.
        /// </summary>
        public static void Step()
        {
            for(int i = 0; i < m_CurrentlyPlayingTracks.Count; i++)
            {
                if (!m_CurrentlyPlayingTracks[i].is_complete)
                    m_CurrentlyPlayingTracks[i].next();
                else
                    m_CurrentlyPlayingTracks.Remove(m_CurrentlyPlayingTracks[i]);
            }
        }

        /// <summary>
        /// Starts playing a track with the given ID.
        /// </summary>
        /// <param name="TrackID">ID of the track to play.</param>
        public static void PlayTrack(uint TrackID)
        {
            foreach (KeyValuePair<string, RegisteredEvent> KVP in Events)
            {
                if (KVP.Value.TrackID == TrackID)
                {
                    m_CurrentlyPlayingTracks.Add(new SubRoutine(TrackID,
                        KVP.Value.Rsc.HitResource.ExTable.FindSubroutineForTrack(TrackID), KVP.Value.Rsc.HitResource));
                }
            }
        }

        /// <summary>
        /// Plays a track for a registered event.
        /// </summary>
        /// <param name="Event">The name of the registered event to play.</param>
        public static void PlayEvent(string Event)
        {
            uint SubroutinePointer = 0, TrackID = 0;
            bool NoHSMExisted = false;

            if(Events.ContainsKey(Event))
            {
                Dictionary<string, int> Constants = Events[Event].Rsc.Symbols.Constants;
                if (Constants.ContainsKey(Event)) SubroutinePointer = (uint)Constants[Event];
                string TrackIDName = "guid_tkd_" + Event;
                if (Constants.ContainsKey(TrackIDName)) TrackID = (uint)Constants[TrackIDName];
                else TrackID = Events[Event].TrackID;
            }
            else
            { //No HSM existed!
                ExportTable ExTable = Events[Event].Rsc.HitResource.ExTable;
                TrackID = Events[Event].TrackID;
                m_CurrentlyPlayingTracks.Add(new SubRoutine(TrackID, ExTable.FindSubroutineForTrack(TrackID), 
                    Events[Event].Rsc.HitResource));
                NoHSMExisted = true;
            }

            if (NoHSMExisted != true)
            {
                if (SubroutinePointer != 0)
                {
                    m_CurrentlyPlayingTracks.Add(new SubRoutine(TrackID, SubroutinePointer, Events[Event].Rsc.HitResource));
                    return;
                }
                else if (TrackID != 0 && FileManager.TrackExists(TrackID))
                    m_CurrentlyPlayingTracks.Add(new SubRoutine(TrackID, 0, Events[Event].Rsc.HitResource));
            }
        }

        /// <summary>
        /// Kills a track with the given ID.
        /// </summary>
        /// <param name="TrackID">ID of the track to kill.</param>
        public static void KillTrack(uint TrackID)
        {
            for(int i = 0; i < m_CurrentlyPlayingTracks.Count; i++)
            {
                if (m_CurrentlyPlayingTracks[i].TrackID == TrackID)
                    m_CurrentlyPlayingTracks.Remove(m_CurrentlyPlayingTracks[i]);
            }
        }

		/// <summary>
		/// Gets a value indicating if platform is linux.
		/// </summary>
		/// <value><c>true</c> if is linux; otherwise, <c>false</c>.</value>
		private static bool IsLinux
		{
			get
			{
				int p = (int) Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}
    }
}
