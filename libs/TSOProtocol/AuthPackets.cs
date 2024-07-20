/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the TSOProtocol library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Parlo.Packets;
using ZeroFormatter;

namespace TSOProtocol
{
    public enum AuthPacketIDs
    {
        ClientSignup = 0x00,
        ClientInitialAuth = 0x01,
        ServerInitialAuthResponse = 0x02,
        CAuthProof = 0x03, //Sent by client
        SAuthProof = 0x04 //Sent by server
    }

    /// <summary>
    /// The packet sent by the client to the server (may or may not be a different server from the login server)
    /// to sign up (I.E create an account in the DB).
    /// </summary>
    [ZeroFormattable]
    public struct ClientSignup : IPacket
    {
        public ClientSignup(string userName, string salt, string verifier)
        {
            Username = userName;
            Salt = salt;
            Verifier = verifier;
        }

        [Index(0)]
        public string Username = default!;

        [Index(1)]
        public string Salt = default!;

        [Index(2)]
        public string Verifier = default!;
    }

    /// <summary>
    /// The packet sent by the client to the server to initiate authentication.
    /// </summary>
    [ZeroFormattable]
    public struct ClientInitialAuth : IPacket
    {
        public ClientInitialAuth(string userName, string ephemeral)
        {
            Username = userName;
            Ephemeral = ephemeral;
        }

        [Index(0)]
        public string Username = default!;

        [Index(1)]
        public string Ephemeral = default!;
    }

    /// <summary>
    /// The initial response from the server.
    /// </summary>
    [ZeroFormattable]
    public struct ServerInitialAuthResponse : IPacket
    {
        public ServerInitialAuthResponse(string salt, string publicEphemeral)
        {
            Salt = salt;
            PublicEphemeral = publicEphemeral;
        }

        [Index(0)]
        public string Salt = default!;

        [Index(1)]
        public string PublicEphemeral = default!;
    }

    /// <summary>
    /// Sent by the client to the server and vice versa.
    /// </summary>
    [ZeroFormattable]
    public struct AuthProof : IPacket
    {
        public AuthProof(string sessionProof)
        {
            SessionProof = sessionProof;
        }

        [Index(0)]
        public string SessionProof = default!;
    }
}
