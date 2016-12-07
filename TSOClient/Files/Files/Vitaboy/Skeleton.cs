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
using System.Linq;

namespace Files.Vitaboy
{
    public class Skeleton : IDisposable
    {
        private FileReader m_Reader;
        public string Name;
        public ushort BoneCount;
        public List<Bone> Bones = new List<Bone>();
        public Bone RootBone;

        public Skeleton(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            m_Reader.ReadUInt32(); //Version
            Name = m_Reader.ReadPascalString();
            BoneCount = m_Reader.ReadUShort();

            for(int i = 0; i < BoneCount; i++)
            {
                Bones.Add(new Bone(m_Reader, i));
            }

            /** Construct tree **/
            foreach (Bone bone in Bones)
                bone.Children = Bones.Where(x => x.ParentName == bone.Name).ToArray();

            RootBone = Bones.FirstOrDefault(x => x.ParentName == "NULL");

            m_Reader.Close();
        }

        /// <summary>
        /// Finds the bone in this skeleton with the given name.
        /// </summary>
        /// <param name="BoneName">Name of bone to find.</param>
        /// <returns>Index of bone in this Skeleton.</returns>
        public int FindBone(string BoneName)
        {
            return Bones.FirstOrDefault(x => x.Name == BoneName).BoneIndex;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool CleanUpManagedResources)
        {
            if (CleanUpManagedResources)
            {
                if (m_Reader != null)
                    m_Reader.Dispose();
            }
        }
    }
}
