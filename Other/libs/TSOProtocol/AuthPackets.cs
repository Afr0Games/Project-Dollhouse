/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the TSOProtocol library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using SecureRemotePassword;
using ZeroFormatter;

namespace TSOProtocol
{
    public enum AuthPacketIDs
    {
        ClientInitialAuth = 0x00,
        ServerInitialAuthResponse = 0x01,
        AuthProof = 0x02
    }

    /// <summary>
    /// The packet sent by the client to the server to initiate authentication.
    /// </summary>
    [ZeroFormattable]
    public class ClientInitialAuth : IPacket
    {
        [Index(0)]
        public string Username;

        [Index(1)]
        public string Ephemeral;
    }

    /// <summary>
    /// The initial response from the server.
    /// </summary>
    [ZeroFormattable]
    public class ServerInitialAuthResponse : IPacket
    {
        [Index(0)]
        public string Salt;
        [Index(1)]
        public string PublicEphemeral;
    }

    /// <summary>
    /// Sent by the client to the server and vice versa.
    /// </summary>
    [ZeroFormattable]
    public class AuthProof : IPacket
    {
        [Index(0)]
        public string SessionProof;
    }
}
