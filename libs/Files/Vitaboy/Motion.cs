/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Collections.Generic;

namespace Files.Vitaboy
{
    public class Motion
    {
        public string BoneName;
        public uint FrameCount;
        public float Duration;
        public bool HasTranslation;
        public bool HasRotation;
        public uint FirstTranslationIndex;
        public uint FirstRotationIndex;
        public bool HasPropsList;
        public List<List<Property>> PropertyLists = new List<List<Property>>();
        public bool HasTimepropertyLists;
        public List<List<TimeProperty>> TimePropertyLists = new List<List<TimeProperty>>();

        public Motion(FileReader Reader)
        {
            Reader.ReadUInt32(); //Unknown
            BoneName = Reader.ReadPascalString();
            FrameCount = Reader.ReadUInt32();
            Duration = Reader.ReadFloat();
            HasTranslation = (Reader.ReadByte() != 0) ? true : false;
            HasRotation = (Reader.ReadByte() != 0) ? true : false;
            FirstTranslationIndex = Reader.ReadUInt32();
            FirstRotationIndex = Reader.ReadUInt32();
            HasPropsList = (Reader.ReadByte() != 0) ? true : false;

            if(HasPropsList)
            {
                uint PropsListCount = Reader.ReadUInt32();

                for(int i = 0; i < PropsListCount; i++)
                {
                    uint PropsCount = Reader.ReadUInt32();
                    PropertyLists.Add(new List<Property>());

                    for (int j = 0; j < PropsCount; j++)
                        PropertyLists[i].Add(new Property(Reader));
                }
            }

            HasTimepropertyLists = (Reader.ReadByte() != 0) ? true : false;

            if(HasTimepropertyLists)
            {
                uint TimePropsListCount = Reader.ReadUInt32();

                for (int i = 0; i < TimePropsListCount; i++)
                {
                    uint PropsCount = Reader.ReadUInt32();
                    TimePropertyLists.Add(new List<TimeProperty>());

                    for (int j = 0; j < PropsCount; j++)
                        TimePropertyLists[i].Add(new TimeProperty(Reader));
                }
            }
        }
    }
}
