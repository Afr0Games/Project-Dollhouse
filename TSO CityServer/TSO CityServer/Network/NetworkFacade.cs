/*
This Source Code Form is subject to the terms of the 
Mozilla Public License, v. 2.0. If a copy of the MPL was not 
distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GonzoNet;
using ProtocolAbstractionLibraryD;

namespace TSO_CityServer.Network
{
    public class NetworkFacade
    {
        public static SharedArrayList TransferringClients;

        static NetworkFacade()
        {
            TransferringClients = new SharedArrayList();
            PacketHandlers.Register(0x01, false, 0, new OnPacketReceive(LoginPacketHandlers.HandleClientToken));
            PacketHandlers.Register((byte)PacketType.CHARACTER_CREATE_CITY, false, 0, new OnPacketReceive(ClientPacketHandlers.HandleCharacterCreate));
            PacketHandlers.Register((byte)PacketType.CITY_TOKEN, false, 0, new OnPacketReceive(ClientPacketHandlers.HandleCityToken));
        }
    }
}
