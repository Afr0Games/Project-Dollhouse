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
using Files.AudioFiles;
using Files.AudioLogic;
using Files.Manager;
using System.Diagnostics;

namespace Sound
{
    public class HITNoteEntry
    {
        public uint SoundID;
        public ISoundCodec Sound;

        public HITNoteEntry(uint ID, ISoundCodec Snd)
        {
            SoundID = ID;
            Sound = Snd;
        }
    }

    public class SubRoutine : Coroutine
    {
        public Hit HitParent;
        public uint TrackID;
        public uint Address;

        private uint m_InstCounter = 0;
        private int m_WaitRemaining; //How many milliseconds remain before the subroutine can proceed?
        private Stack<uint> m_Stack = new Stack<uint>();
        private uint m_LoopPoint = 0; //Used for looping.
        public bool SimpleMode = false; //Plays a sound with no associated HIT code.

        private Dictionary<byte, int> m_Registers = new Dictionary<byte, int>();
        private Dictionary<byte, int> m_LocalVars = new Dictionary<byte, int>();
        private Dictionary<int, int> m_ObjectVars = new Dictionary<int, int>();
        private Dictionary<byte, uint> m_Args = new Dictionary<byte, uint>();
        private bool m_ZeroFlag = false, m_SignFlag = false;

        private Random m_Rand = new Random(); //Used by rand and smart_choose.

        private HLS m_Hitlist;
        private TRK m_Track;
        private List<HITNoteEntry> m_Notes = new List<HITNoteEntry>();
        private uint m_SoundID = 0;

        /// <summary>
        /// Creates a new SubRoutine instance.
        /// </summary>
        /// <param name="TID">ID of the track for this SubRoutine.</param>
        /// <param name="Address">Address of this SubRoutine in HIT.</param>
        /// <param name="Parent">The HIT that contains this SubRoutine.</param>
        public SubRoutine(uint TID, uint Address, Hit Parent)
        {
            HitParent = Parent;

            TrackID = TID;

            if (Address != 0)
                m_InstCounter = Address;
            else
            {
                m_Track = FileManager.GetTRK(TrackID);
                m_SoundID = m_Track.SoundID;
                SimpleMode = true;
            }

            m_Registers.Add(0x5, 0); //v1
            m_Registers.Add(0x6, 0); //v2
            m_Registers.Add(0x7, 0); //v3
            m_Registers.Add(0x8, 0); //v4
            m_Registers.Add(0x9, 0); //v5
            m_Registers.Add(0xa, 0); //v6
            m_Registers.Add(0xb, 0); //v7
            m_Registers.Add(0xc, 0); //v8
            m_Registers.Add(0xd, 0); //h1
            m_Registers.Add(0xe, 0); //h2
            m_Registers.Add(0xf, 0); //h3
            m_Registers.Add(0x10, 0); //h4

            m_LocalVars.Add(0x10, 0); //argstype
            m_LocalVars.Add(0x11, 0); //trackdatasource
            m_LocalVars.Add(0x12, 0); //patch
            m_LocalVars.Add(0x13, 0); //priority
            m_LocalVars.Add(0x14, 0); //vol
            m_LocalVars.Add(0x15, 0); //extvol
            m_LocalVars.Add(0x16, 0); //pan
            m_LocalVars.Add(0x17, 0); //pitch
            m_LocalVars.Add(0x18, 0); //paused
            m_LocalVars.Add(0x19, 0); //fxtype
            m_LocalVars.Add(0x1a, 0); //fxlevel
            m_LocalVars.Add(0x1b, 0); //duckpri
            m_LocalVars.Add(0x1c, 0); //Is3d
            m_LocalVars.Add(0x1d, 0); //IsHeadRelative
            //m_LocalVars.Add(0x1d, 0); //HeadRelative
            m_LocalVars.Add(0x1e, 0); //MinDistance
            m_LocalVars.Add(0x1f, 0); //MaxDistance
            m_LocalVars.Add(0x20, 0); //X
            m_LocalVars.Add(0x21, 0); //Y
            m_LocalVars.Add(0x22, 0); //Z
            m_LocalVars.Add(0x23, 0); //attack
            m_LocalVars.Add(0x24, 0); //decay
            m_LocalVars.Add(0x25, 0); //isStreamed
            //m_LocalVars.Add(0x25, 0); //stream
            m_LocalVars.Add(0x26, 0); //bufsizemult
            m_LocalVars.Add(0x27, 0); //fade_dest
            m_LocalVars.Add(0x28, 0); //fade_var
            m_LocalVars.Add(0x29, 0); //fade_speed
            m_LocalVars.Add(0x2a, 0); //fade_on
            m_LocalVars.Add(0x2b, 0); //Preload
            m_LocalVars.Add(0x2c, 0); //isplaying
            m_LocalVars.Add(0x2d, 0); //whattodowithupdate
            m_LocalVars.Add(0x2e, 0); //tempo
            m_LocalVars.Add(0x2f, 0); //target
            m_LocalVars.Add(0x30, 0); //ctrlgroup
            m_LocalVars.Add(0x31, 0); //interrupt
            m_LocalVars.Add(0x32, 0); //ispositioned
            m_LocalVars.Add(0x33, 0); //loop
            m_LocalVars.Add(0x34, 0); //source
            //m_LocalVars.Add(0x34, 0); //AppObjectId
            m_LocalVars.Add(0x35, 0); //callbackarg
            m_LocalVars.Add(0x36, 0); //pitchrandmin
            m_LocalVars.Add(0x37, 0); //pitchrandmax
            m_LocalVars.Add(0x38, 0); //spl
            m_LocalVars.Add(0x39, 0); //sem
            m_LocalVars.Add(0x3a, 0); //starttrackid
            m_LocalVars.Add(0x3b, 0); //endtrackid
            m_LocalVars.Add(0x3c, 0); //startdelay
            m_LocalVars.Add(0x3d, 0); //fadeinspeed
            m_LocalVars.Add(0x3e, 0); //fadeoutspeed
            m_LocalVars.Add(0x3f, 0); //hitlist
            m_LocalVars.Add(0x40, 0); //velocx
            m_LocalVars.Add(0x41, 0); //velocy
            m_LocalVars.Add(0x42, 0); //velocz
            m_LocalVars.Add(0x43, 0); //orientx
            m_LocalVars.Add(0x44, 0); //orienty
            m_LocalVars.Add(0x45, 0); //orientz

            m_ObjectVars.Add(0x271a, 0); //IsInsideViewFrustum
            m_ObjectVars.Add(0x271b, 0); //PositionX
            m_ObjectVars.Add(0x271c, 0); //PositionY
            m_ObjectVars.Add(0x271d, 0); //PositionZ
            m_ObjectVars.Add(0x271e, 0); //OrientationX
            m_ObjectVars.Add(0x271f, 0); //OrientationY
            m_ObjectVars.Add(0x2720, 0); //OrientationZ
            m_ObjectVars.Add(0x2721, 0); //ViewUpX
            m_ObjectVars.Add(0x2722, 0); //ViewUpY
            m_ObjectVars.Add(0x2723, 0); //ViewUpZ
            m_ObjectVars.Add(0x2725, 0); //IsObscured
            m_ObjectVars.Add(0x2726, 0); //Gender
            m_ObjectVars.Add(0x2727, 0); //Age
            m_ObjectVars.Add(0x2728, 0); //CookingSkill
            m_ObjectVars.Add(0x2729, 0); //CleaningSkill
            m_ObjectVars.Add(0x272a, 0); //CreativitySkill
            m_ObjectVars.Add(0x272b, 0); //RepairSkill
            m_ObjectVars.Add(0x272c, 0); //GardeningSkill
            m_ObjectVars.Add(0x272d, 0); //MusiccSkillSkill
            m_ObjectVars.Add(0x272e, 0); //LiteracySkill
            m_ObjectVars.Add(0x272f, 0); //LogicSkill
            m_ObjectVars.Add(0x2730, 0); //Mood
            m_ObjectVars.Add(0x2731, 0); //Niceness
            m_ObjectVars.Add(0x2732, 0); //Activeness
            m_ObjectVars.Add(0x2733, 0); //Generousness
            m_ObjectVars.Add(0x2734, 0); //Playfulness
            m_ObjectVars.Add(0x2735, 0); //Outgoingness
            m_ObjectVars.Add(0x2736, 0); //Neatness

            m_Args.Add(0x0, 0); //arg0
            m_Args.Add(0x1, 0); //arg1
            m_Args.Add(0x2, 0); //arg2
            m_Args.Add(0x3, 0); //arg3
            m_Args.Add(0x4, 0); //arg4
        }

        #region Reading

        private byte ReadByte()
        {
            return HitParent.InstructionData[m_InstCounter++];
        }

        private int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        private uint ReadUInt32()
        {
            uint result = 0;
            result |= ReadByte();
            result |= ((uint)ReadByte() << 8);
            result |= ((uint)ReadByte() << 16);
            result |= ((uint)ReadByte() << 24);
            return result;
        }

        private int GetVariable(int Location)
        {
            if (Location < 5)
                return (int)m_Args[(byte)Location];
            else if (Location < 0x10)
                return m_Registers[(byte)Location];
            else if (Location < 0x46)
                return m_LocalVars[(byte)(Location - 0x10)];
            else if (Location < 0x88)
                return HitVM.GetGlobalVar(Location - 0x64);
            else if (Location < 2736)
                return m_ObjectVars[(short)(Location - 0x271a)];

            return 0;
        }

        #endregion

        #region Writing
        
        /// <summary>
        /// Sets a register to a specific value.
        /// </summary>
        /// <param name="Location">Which register to set.</param>
        /// <param name="Value">The value to set.</param>
        private void SetVariable(short Location, int Value)
        {
            if (Location < 5)
                m_Args[(byte)Location] = (uint)Value;
            if (Location < 0x10)
                m_Registers[(byte)Location] = Value;
            else if(Location < 0x46)
            {
                SetLocal(Location, Value);
                m_LocalVars[(byte)(Location - 0x10)] = Value;
            }
            else if(Location < 88)
            {
                HitVM.SetGlobalVar(Location - 0x64, Value);
            }
            else if(Location < 2736)
            {
                m_ObjectVars[Location - 0x271a] = Value;
            }
        }

        /// <summary>
        /// Sets a local variable to a value.
        /// </summary>
        /// <param name="Location">Which variable to set.</param>
        /// <param name="Value">The value to set.</param>
        private void SetLocal(int Location, int Value)
        {
            switch (Location)
            {
                case 0x12:
                    m_SoundID = (uint)Value;
                    break;
            }
        }

        /// <summary>
        /// Sets a track for this Subroutine.
        /// </summary>
        /// <param name="Index">The ID of a Hitlist to load.</param>
        /// <returns>The ID of the track that was set.</returns>
        private uint SetTrack(uint Index)
        {
            m_Hitlist = FileManager.GetHLS(Index);
            m_Track = FileManager.GetTRK(m_Hitlist.SoundsAndHitlists[(int)Index]);
            m_SoundID = m_Track.SoundID;

            return m_Hitlist.SoundsAndHitlists[(int)Index];
        }

        #endregion

        /// <summary>
        /// This runs one HIT instruction, and should be run once every frame for every subroutine.
        /// </summary>
        /// <returns>True if still running, otherwise yields.</returns>
        public override IEnumerable<object> process()
        {
            byte Var1 = 0, Var2 = 0; //Used by waiteq, waitne
            byte Datafield = 0; //Used by set/getsrcdatafield.
            byte TrackID = 0, Dest = 0;
            int Src = 0;

            if (!SimpleMode)
            {
                while (true)
                {
                    byte Opcode = ReadByte();

                    switch (Opcode)
                    {
                        case 0x2: //note_on - play a note, whose ID resides in the specified variable.
                            Dest = ReadByte();

                            if (m_SoundID == 0)
                                m_SoundID = m_Track.SoundID;

                            ISoundCodec Snd = FileManager.GetSound(m_SoundID);

                            if (Snd != null)
                            {
                                m_Notes.Add(new HITNoteEntry(m_SoundID, Snd));

                                SetVariable(Dest, m_Notes.Count - 1);
                                SoundPlayer.PlaySound(Snd.DecompressedWav(), m_SoundID, Snd.GetSampleRate());
                            }
                            else
                                Debug.WriteLine("SubRoutine.cs: Couldn't find sound " + m_SoundID);

                            break;
                        case 0x4: //loadb - sign-extend a 1-byte constant to 4 bytes and write to a variable.
                            Dest = ReadByte();
                            var Constant = (sbyte)ReadByte();
                            SetVariable(Dest, Constant);

                            m_ZeroFlag = (Dest == 0);
                            m_SignFlag = (Dest < 0); //TODO: When to set this to false again?

                            break;
                        case 0x5: //loadl - write a 4-byte constant to a variable.
                            Dest = ReadByte();
                            Src = ReadInt32();

                            SetVariable(Dest, Src);

                            break;
                        case 0x6: //set/settt - copy the contents of one variable into another.
                            Dest = ReadByte();
                            Src = GetVariable(ReadByte());

                            SetVariable(Dest, Src);
                            m_ZeroFlag = (Dest == 0);
                            m_SignFlag = (Dest < 0); //TODO: When to set this to false again?

                            break;
                        case 0x7: //call - push the instruction pointer and jump to the given address.
                            m_Stack.Push(m_InstCounter);
                            m_InstCounter = (uint)ReadInt32();

                            break;
                        case 0x8: //return - kill this thread.
                            YieldComplete();
                            yield return false;
                            break;
                        case 0x9: //wait - wait for a length of time in milliseconds, specified by a variable.
                            Src = ReadByte();

                            if (m_WaitRemaining == -1) m_WaitRemaining = m_Registers[(byte)Src];
                            m_WaitRemaining -= 16; //assuming tick rate is 60 times a second
                            if (m_WaitRemaining > 0)
                            {
                                m_InstCounter -= 2;
                                yield return false;
                            }
                            else
                            {
                                m_WaitRemaining = -1;
                                yield return false;
                            }

                            break;
                        case 0xb: //wait_samp -  wait for the previously selected note to finish playing.
                            break;
                        case 0xc: //end - return from this function; pop the instruction pointer from the stack and jump.
                            YieldComplete(); //Not sure if this is correct?
                            yield return true;
                            break;
                        case 0xd: //jump - jump to a given address.
                            byte JmpAddress = ReadByte();

                            if (JmpAddress > 15)
                            {
                                m_InstCounter--;
                                m_InstCounter = ReadUInt32();
                            }
                            else
                            {
                                m_InstCounter = (uint)GetVariable(JmpAddress);
                                if (ReadByte() == 0) m_InstCounter += 2;
                                else m_InstCounter--;
                            }

                            break;
                        case 0xe: //test - examine the variable and set the flags.
                            Dest = ReadByte();

                            m_ZeroFlag = (Dest == 0);
                            m_SignFlag = (Dest < 0);

                            break;
                        case 0xf: //nop - no operation.
                            break;
                        case 0x10: //add - increment a "dest" variable by a "src" variable
                            m_Registers[ReadByte()] += m_Registers[ReadByte()];
                            m_ZeroFlag = (Dest == 0);
                            m_SignFlag = (Dest < 0);

                            break;
                        case 0x11: //sub - decrement a "dest" variable by a "src" variable.
                            m_Registers[ReadByte()] -= m_Registers[ReadByte()];
                            m_ZeroFlag = (Dest == 0);
                            m_SignFlag = (Dest < 0);

                            break;
                        case 0x12: //div - divide a "dest" variable by a "src" variable.
                            m_Registers[ReadByte()] /= m_Registers[ReadByte()];
                            m_ZeroFlag = (Dest == 0);
                            m_SignFlag = (Dest < 0);

                            break;
                        case 0x13: //mul - multiply a "dest" variable by a "src" variable.
                            m_Registers[ReadByte()] *= m_Registers[ReadByte()];
                            m_ZeroFlag = (Dest == 0);
                            m_SignFlag = (Dest < 0);

                            break;
                        case 0x14: //cmp - compare two variables and set the flags.
                            m_Registers[ReadByte()] -= m_Registers[ReadByte()];
                            m_ZeroFlag = (Dest == 0);
                            m_SignFlag = (Dest < 0);

                            break;
                        case 0x18: //rand - generate a random number between "low" and "high" variables, inclusive, and store
                                   //the result in the "dest" variable.
                            SetVariable(ReadByte(), m_Rand.Next((int)ReadByte(), (int)ReadByte()));
                            break;
                        case 0x20: //loop - jump back to the loop point (start of track subroutine by default).
                            if (m_LoopPoint != 0)
                                m_InstCounter = m_LoopPoint;
                            else
                                m_InstCounter = Address;

                            break;
                        case 0x021: //set_loop - set the loop point to the current position.
                            m_LoopPoint = m_InstCounter;

                            break;
                        case 0x27: //smart_choose - Set the specified variable to a random entry from the selected hitlist.
                            Dest = ReadByte();
                            int Max = m_Hitlist.SoundsAndHitlists.Count;

                            SetVariable(Dest, (int)m_Hitlist.SoundsAndHitlists[m_Rand.Next(Max)]);

                            break;
                        case 0x2d: //max - find the higher of a "dest" variable and a "src" constant and store the result 
                                   //in the variable.

                            Dest = ReadByte();
                            Src = ReadInt32();

                            if (Src > Dest)
                                SetVariable(Dest, Src);

                            break;
                        case 0x32: //play_trk - play a track (by sending it the kSndobPlay event), whose ID resides in the 
                                   //specified variable.

                            TrackID = ReadByte();

                            if (HitVM.IsInitialized)
                                HitVM.PlayTrack((uint)GetVariable(TrackID));

                            break;
                        case 0x33: //kill_trk - kill a track (by sending it the kSndobKill event), whose ID resides in the 
                                   //specified variable.

                            TrackID = ReadByte();

                            if (HitVM.IsInitialized)
                                HitVM.KillTrack((uint)GetVariable(TrackID));

                            break;
                        case 0x3a: //test1 - unknown
                            break;
                        case 0x3b: //test2 - unknown
                            break;
                        case 0x3c: //test3 - unknown
                            break;
                        case 0x3d: //test4 - unknown
                            break;
                        case 0x3e: //ifeq - if the zero flag is set,  jump to the given address.
                            Src = ReadInt32();

                            if (m_ZeroFlag)
                                m_InstCounter = (uint)Src;

                            break;
                        case 0x3f: //ifne - if the zero flag is not set, jump to the given address.
                            Src = ReadInt32();

                            if (!m_ZeroFlag)
                                m_InstCounter = (uint)Src;

                            break;
                        case 0x40: //ifgt - if the sign flag is not set and the zero flag is not set, jump to the given address.
                            Src = ReadInt32();

                            if (!m_ZeroFlag && !m_SignFlag)
                                m_InstCounter = (uint)Src;

                            break;
                        case 0x41: //iflt - if the sign flag is set, jump to the given address.
                            Src = ReadInt32();

                            if (m_SignFlag)
                                m_InstCounter = (uint)Src;

                            break;
                        case 0x42: //ifge - if the sign flag is not set, jump to the given address.
                            Src = ReadInt32();

                            if (!m_SignFlag)
                                m_InstCounter = (uint)Src;

                            break;
                        case 0x43: //ifle - if the sign flag is set or the zero flag is set, jump to the given address.
                            Src = ReadInt32();

                            if (m_ZeroFlag || m_SignFlag)
                                m_InstCounter = (uint)Src;

                            break;
                        case 0x44: //smart_setlist - choose a global hitlist, or 0 for the one local to the track.
                            Src = ReadByte();

                            if (Src != 0)
                                m_Hitlist = FileManager.GetHLS((uint)GetVariable(Src));
                            else
                            {
                                uint SoundID = FileManager.GetTRK(TrackID).SoundID;

                                try
                                {
                                    FileManager.GetSound(SoundID);
                                }
                                catch
                                {
                                    m_Hitlist = FileManager.GetHLS(SoundID);
                                }
                            }

                            break;
                        case 0x45: //seqgroup_kill - kill all sounds belonging to the sequence group specified by the "group" 
                                   //variable.
                            Src = ReadByte();

                            break;
                        case 0x47: //seqgroup_return - unknown.
                            byte Group = ReadByte();

                            break;
                        case 0x48: //getsrcdatafield - Read an object variable (whose ID is specified by the "field" 
                                   //variable) of a source object (whose object ID is specified by the "source" variable), 
                                   //store it in the "dest" variable, and update the flags.
                            Dest = ReadByte();
                            Src = ReadByte();
                            Datafield = ReadByte();

                            int ObjectVar = GetVariable(Src);
                            SetVariable(Dest, ObjectVar);
                            m_ZeroFlag = (ObjectVar == 0);
                            m_SignFlag = (ObjectVar < 0);

                            break;
                        case 0x49: //seqgroup_trkid - unknown.
                            Dest = ReadByte();
                            Src = ReadByte();

                            break;
                        case 0x4a: //setll - Copy the contents of one variable into another (equivalent to set and settt; 
                                   //defaultsyms.txt says "ISN'T THIS THE SAME AS SET TOO?")
                            Dest = ReadByte();
                            Src = ReadByte();

                            SetVariable(Dest, Src);

                            break;
                        case 0x4b: //setlt - unknown.
                            Dest = ReadByte();
                            Src = ReadByte();

                            break;
                        case 0x4d: //waiteq - wait until two variables are equal.
                            Var1 = ReadByte();
                            Var2 = ReadByte();

                            if (GetVariable(Var1) != GetVariable(Var2))
                            {
                                m_InstCounter -= 3;
                                yield return false;
                            }

                            break;
                        case 0x53: //duck - unknown.
                            break;
                        case 0x54: //unduck - unknown.
                            break;
                        case 0x56: //setlg - set global = local (source: defaultsyms.txt).
                            Dest = ReadByte();
                            Src = ReadInt32();

                            HitVM.SetGlobalVar(Src, GetVariable(Dest));

                            break;
                        case 0x57: //setgl - read globally, set locally (source: defaultsyms.txt).
                            Dest = ReadByte();
                            Src = ReadInt32();

                            SetVariable(Dest, HitVM.GetGlobalVar(Src));

                            break;
                        case 0x59: //setsrcdatafield - set an object variable (whose ID is specified by the "field" variable) of
                                   //a source object (whose object ID is specified by the "source" variable) to the value 
                                   //specified by the "value" variable.
                            Dest = ReadByte();
                            Src = ReadByte();
                            Datafield = ReadByte();

                            break;
                        case 0x5f: //smart_index - find the entry at the index specified by the "index" variable in the hitlist 
                                   //specified by the "dest" variable and store that entry in the "dest" variable.
                            Dest = ReadByte();
                            byte Index = ReadByte();

                            uint HitlistID = (uint)GetVariable(Index);
                            uint TRKID = SetTrack(HitlistID);
                            SetVariable(Dest, (int)TRKID);

                            break;
                        case 0x60: //note_on_loop - play a note, whose ID resides in the specified variable, and immediately loop
                                   //it indefinitely.
                            Dest = ReadByte();

                            HITNoteEntry Note = new HITNoteEntry(m_SoundID, FileManager.GetSound(m_SoundID));
                            m_Notes.Add(Note);

                            SetVariable(Dest, m_Notes.Count - 1);
                            SoundPlayer.PlaySound(Note.Sound.DecompressedWav(), m_SoundID, Note.Sound.GetSampleRate(), true);

                            break;
                    }
                }
            }
            else
            {
                ISoundCodec Snd = FileManager.GetSound(m_SoundID);
                SoundPlayer.PlaySound(Snd.DecompressedWav(), m_SoundID, Snd.GetSampleRate(), false);
                yield return true;
            }
        }
    }
}
