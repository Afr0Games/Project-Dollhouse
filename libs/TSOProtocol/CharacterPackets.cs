using System;
using System.Collections.Generic;
using ZeroFormatter;
using Files.Vitaboy;
using Vitaboy;

namespace TSOProtocol
{
    public enum CharPacketIDs
    {
        /// <summary>
        /// Sent by server to client after authentication.
        /// Lists all characters for the username.
        /// </summary>
        ListChars = 0x05,

        /// <summary>
        /// Sent by client to server to create a new character.
        /// </summary>
        CNewChar = 0x06,

        /// <summary>
        /// Sent by server to client to confirm the creation of a new character.
        /// </summary>
        SNewChar = 0x07,

        /// <summary>
        /// Sent by client to server to delete a character.
        /// </summary>
        CDeleteChar = 0x08, //Sent by client

        /// <summary>
        /// Sent by server to client to confirm the deletion of a character.
        /// </summary>
        SDeleteChar = 0x04 //Sent by server
    }

    public enum CreateCharacterErrorCodes
    {
        OK = 0x00 //Everything OK.
    }

    /// <summary>
    /// Groups together the head and body outfit, as well as the 
    /// name and description of a character stored on the server.
    /// </summary>
    [ZeroFormattable]
    public class CharacterInfo
    {
        /// <summary>
        /// The name of the character.
        /// </summary>
        public string Name = default;

        /// <summary>
        /// The character's description, as defined in the Create A Sim screen.
        /// </summary>
        public string Description = default;

        /// <summary>
        /// The character's head outfit.
        /// </summary>
        public ulong HeadOutfit = default;

        /// <summary>
        /// The character's body outfit.
        /// </summary>
        public ulong BodyOutfit = default;

        public SkinType Skintype = default;
    }

    /// <summary>
    /// Sent by server to client after authentication.
    /// Lists all characters for the username.
    /// </summary>
    [ZeroFormattable]
    public class ListCharsPacket
    {
        /// <summary>
        /// Constructs a new ListCharsPacket.
        /// </summary>
        /// <param name="Chars">The characters to list.</param>
        public ListCharsPacket(CharacterInfo[] Chars) 
        {
            Characters = Chars;
        }

        /// <summary>
        /// The characters stored on the server.
        /// </summary>
        [Index(0)]
        public CharacterInfo[] Characters = new CharacterInfo[3];
    }

    /// <summary>
    /// Sent by client to server to create a new character.
    /// </summary>
    [ZeroFormattable]
    public class ClientNewCharPacket
    {
        /// <summary>
        /// Constructs a new ClientNewCharPacket.
        /// </summary>
        /// <param name="Char">A new character.</param>
        public ClientNewCharPacket(CharacterInfo Char)
        {
            NewCharacter = Char;
        }

        [Index(0)]
        public CharacterInfo NewCharacter;
    }

    /// <summary>
    /// Sent by server to client to confirm that a nw character has been created.
    /// </summary>
    [ZeroFormattable]
    public class ServerNewCharPacket
    {
        [Index(0)]
        public CreateCharacterErrorCodes Error;
    }
}
