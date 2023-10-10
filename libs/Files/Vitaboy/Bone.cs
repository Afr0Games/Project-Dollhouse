/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Files.Vitaboy
{
    public class Bone
    {
        public string Name;
        public string ParentName;
        public bool HasPropertyList;
        public List<Property> PropertyList = new List<Property>();
        public Vector3 Translation;
        public Quaternion Rotation;
        public bool CanTranslate, CanRotate, CanBlend;
        public Vector3 AbsolutePosition;
        public Matrix AbsoluteMatrix;
        public Bone[] Children;

        /// <summary>
        /// Index of this bone in a skeleton's list of bones.
        /// </summary>
        public int BoneIndex = 0;

        public Bone(FileReader Reader, int Index)
        {
            BoneIndex = Index;

            Reader.ReadUInt32(); //Unknown
            Name = Reader.ReadPascalString();
            ParentName = Reader.ReadPascalString();
            HasPropertyList = (Reader.ReadByte() != 0) ? true : false;

            if(HasPropertyList)
            {
                uint PropertyCount = Reader.ReadUInt32();
                for (int i = 0; i < PropertyCount; i++)
                    PropertyList.Add(new Property(Reader));
            }

            Translation = new Vector3(Reader.ReadFloat(), Reader.ReadFloat(), Reader.ReadFloat());
            Rotation = new Quaternion(Reader.ReadFloat(), -Reader.ReadFloat(), -Reader.ReadFloat(), Reader.ReadFloat());

            CanTranslate = (Reader.ReadUInt32() != 0) ? true : false;
            CanRotate = (Reader.ReadUInt32() != 0) ? true : false;
            CanBlend = (Reader.ReadUInt32() != 0) ? true : false;

            //Don Hopkins says the Wiggle parameters are left over from an attempt to use Perlin noise 
            //introduce some randomness into the animations, so that an animation would look a little different 
            //each time it was run.
            Reader.ReadFloat();
            Reader.ReadFloat();
        }
    }
}
