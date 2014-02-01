/*
This Source Code Form is subject to the terms of the 
Mozilla Public License, v. 2.0. If a copy of the MPL was not 
distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Text;
using GonzoNet;

namespace TSO_CityServer.Network
{
    public class LoginPacketHandlers
    {
        public static void HandleClientToken(NetworkClient Client, ProcessedPacket P)
        {
            try
            {
                ClientToken Token = new ClientToken();
                Token.AccountID = P.ReadInt32();
                Token.ClientIP = P.ReadPascalString();
                Token.CharacterGUID = P.ReadPascalString();
                Token.Token = P.ReadPascalString();

                NetworkFacade.TransferringClients.AddItem(Token);
            }
            catch (Exception E)
            {
                Logger.LogDebug("Exception in HandleClientToken: " + E.ToString());
            }
        }
    }
}
