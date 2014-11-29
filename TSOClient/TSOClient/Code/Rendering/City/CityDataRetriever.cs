﻿/*The contents of this file are subject to the Mozilla Public License Version 1.1
(the "License"); you may not use this file except in compliance with the
License. You may obtain a copy of the License at http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

The Original Code is the TSOClient.

The Initial Developer of the Original Code is
Rhys Simpson. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TSOClient.Code.Rendering.City
{
    public class CityDataRetriever
    {
        public LotTileEntry[] LotTileData; //the renderer requests this on entry, so it must be populated before initilizing the CityRenderer!

        public void RetrieveHouseGFX(int id, Dictionary<int, Texture2D> destination, GraphicsDevice GfxDevice) {
            try
            {
                destination[id] = Texture2D.FromFile(GfxDevice, "housethumb\\" + id.ToString() + ".png"); //interesting challenge o'clock - this will need to be an asynchronous load from the server in the real game!
                //the lot will display as the default house until the Texture2D is set otherwise. Right now it is set synchronously above, so you
                //cannot see this behaviour. If the lot image cannot load (right now if the file doesn't exist, in practice probably if image 
                //download somehow fails) it will be stuck as the default image and will not load again.

            } catch (Exception) {
                //do nothing... It will stay as the old image
            }
        }

        public CityDataRetriever()
        {
            LotTileData = new LotTileEntry[] {  //fun, exciting data to use as a placeholder
                new LotTileEntry(44630806, 176, 296, 0),
                new LotTileEntry(44696332, 177, 309, 1),
                new LotTileEntry(44761872, 161, 221, 0),
                new LotTileEntry(44827399, 159, 221, 0),
                new LotTileEntry(44827403, 157, 223, 1),
                new LotTileEntry(44827406, 159, 224, 3),
                new LotTileEntry(44827409, 159, 228, 1),
                new LotTileEntry(44827410, 322, 198, 3),
                new LotTileEntry(44827410, 324, 197, 0),
                new LotTileEntry(44827417, 329, 201, 0),
                new LotTileEntry(44892941, 331, 196, 1),
                new LotTileEntry(44892953, 322, 186, 0),
                new LotTileEntry(44958480, 310, 194, 0),
                new LotTileEntry(45024008, 178, 309, 1),
                new LotTileEntry(45024018, 180, 307, 3),
                new LotTileEntry(45220628, 175, 305, 0),
                new LotTileEntry(45286160, 176, 296, 1),
                new LotTileEntry(45417213, 148, 273, 0),
                new LotTileEntry(45744910, 145, 272, 3),
                new LotTileEntry(45744911, 146, 270, 0),
                new LotTileEntry(45810447, 143, 270, 1),
                new LotTileEntry(46596866, 144, 268, 1),
                new LotTileEntry(46727935, 146, 267, 0),
                new LotTileEntry(46793470, 143, 266, 1),
                new LotTileEntry(46793471, 226, 172, 0),
                new LotTileEntry(46859005, 231, 167, 0),
                new LotTileEntry(49808168, 232, 176, 1),
                new LotTileEntry(49808169, 234, 172, 0),
                new LotTileEntry(49873702, 235, 171, 3),
                new LotTileEntry(49939238, 236, 173, 1),
                new LotTileEntry(50004754, 238, 168, 1),
                new LotTileEntry(50070305, 238, 175, 0),
            };

        }
    }
}