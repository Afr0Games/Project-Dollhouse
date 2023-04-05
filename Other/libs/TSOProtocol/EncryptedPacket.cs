using System;
using System.Collections.Generic;
using System.Linq;
using GonzoNet.Packets;
using GonzoNet.Encryption;

namespace TSOProtocol
{
    public class EncryptedPacket : Packet
    {
        SRPEncryptionArgs m_Args = new SRPEncryptionArgs();

        /// <summary>
        /// Creates a new instance of EncryptedPacket and adds +1 to the Length
        /// to accomodate whether or not the packet is encrypted.
        /// </summary>
        /// <param name="ID">The ID of the packet.</param>
        /// <param name="Length">The length of the packet.</param>
        /// <param name="SerializedData">The serialized data to send.</param>
        public EncryptedPacket(SRPEncryptionArgs Args, byte ID, ushort Length, byte[] SerializedData) : 
            base(ID, (ushort)(Length + 1), SerializedData)
        {
            m_Args = Args;
        }


        /// <summary>
        /// Builds a encrypted packet ready for sending.
        /// An encrypted packet has an extra byte after 
        /// the length indicating that it is encrypted.
        /// </summary>
        /// <param name="ID">The ID of the packet.</param>
        /// <param name="Length">The length of the packet. Set to 0 for variable length.</param>
        /// <param name="SerializedData">The serialized data of the packet.</param>
        /// <returns>The packet as an array of bytes.</returns>
        public override byte[] BuildPacket()
        {
            List<byte> UnencryptedPacketData = m_Data.ToList();
            UnencryptedPacketData.Insert(0, ID);

            byte[] PacketLength = BitConverter.GetBytes(Length);

            UnencryptedPacketData.Insert(1, PacketLength[0]);
            UnencryptedPacketData.Insert(2, PacketLength[1]);

            AES Enc = new AES(m_Args.Session, HexStringToByteArray(m_Args.Salt));
            List<byte> EncryptedPacketData = Enc.Encrypt(UnencryptedPacketData.ToArray()).ToList();

            EncryptedPacketData.Insert(0, 1); //Indicates that the packet is encrypted.

            return EncryptedPacketData.ToArray();
        }

        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        /// <param name="Hex">The hex string to convert.</param>
        /// <returns>A byte array containing the converted string.</returns>
        private byte[] HexStringToByteArray(string Hex)
        {
            int Length = Hex.Length;
            byte[] Bytes = new byte[Length / 2];

            for (int i = 0; i < Length; i += 2)
                Bytes[i / 2] = Convert.ToByte(Hex.Substring(i, 2), 16);

            return Bytes;
        }
    }
}
