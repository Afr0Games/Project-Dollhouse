/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the SimsLib.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;

namespace Files.Vitaboy
{
    public class Anim
    {
        private FileReader m_Reader;
        public string Name;
        public float Duration;
        public float Distance;
        public bool IsMoving;
        public uint TranslationsCount;
        public float[,] Translations;
        public uint RotationsCount;
        public float[,] Rotations;
        public uint MotionCount;
        public List<Motion> Motions = new List<Motion>(); 

        public Anim(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            m_Reader.ReadUInt32(); //Version

            ASCIIEncoding Enc = new ASCIIEncoding();
            Name = Enc.GetString(m_Reader.ReadBytes(m_Reader.ReadUShort()));
            Duration = m_Reader.ReadFloat();
            Distance = m_Reader.ReadFloat();
            IsMoving = (m_Reader.ReadByte() != 0) ? true : false;
            TranslationsCount = m_Reader.ReadUInt32();

            Translations = new float[TranslationsCount, 3];
            
            for(int i = 0; i < TranslationsCount; i++)
            {
                Translations[i, 0] = m_Reader.ReadFloat();
                Translations[i, 1] = m_Reader.ReadFloat();
                Translations[i, 2] = m_Reader.ReadFloat();
            }

            RotationsCount = m_Reader.ReadUInt32();

            Rotations = new float[RotationsCount, 4];

            for (int i = 0; i < RotationsCount; i++)
            {
                Rotations[i, 0] = m_Reader.ReadFloat();
                Rotations[i, 1] = m_Reader.ReadFloat();
                Rotations[i, 2] = m_Reader.ReadFloat();
                Rotations[i, 3] = m_Reader.ReadFloat();
            }

            MotionCount = m_Reader.ReadUInt32();

            for(int i = 0; i < MotionCount; i++)
                Motions.Add(new Motion(m_Reader));

            m_Reader.Close();
        }
    }
}
