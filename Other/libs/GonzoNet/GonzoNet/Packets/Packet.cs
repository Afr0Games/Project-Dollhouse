using System;
using System.Collections.Generic;
using System.Linq;

namespace GonzoNet.Packets
{
    public class Packet
    {
        private byte m_ID;
        private ushort m_Length;
        protected byte[] m_Data; //Should be serialized.

        public Packet(byte ID, ushort Length, byte[] SerializedData)
        { 
            m_ID = ID;
            m_Length = Length;
            m_Data = SerializedData;
        }

        /// <summary>
        /// The ID of the packet.
        /// </summary>
        public byte ID
        { 
            get { return m_ID; } 
        }

        /// <summary>
        /// The length of the packet. 0 if variable length.
        /// </summary>
        public ushort Length
        { 
            get { return m_Length; } 
        }

        /// <summary>
        /// The packet's serialized data.
        /// </summary>
        public byte[] Data
        {
            get { return m_Data; }
        }

        /// <summary>
        /// Builds a packet ready for sending.
        /// </summary>
        /// <param name="ID">The ID of the packet.</param>
        /// <param name="Length">The length of the packet. Set to 0 for variable length.</param>
        /// <param name="SerializedData">The serialized data of the packet.</param>
        /// <returns>The packet as an array of bytes.</returns>
        public virtual byte[] BuildPacket()
        {
            List<byte> PacketData = m_Data.ToList();
            PacketData.Insert(0, ID);

            byte[] PacketLength = BitConverter.GetBytes(Length);

            PacketData.Insert(1, PacketLength[0]);
            PacketData.Insert(2, PacketLength[1]);

            return PacketData.ToArray();
        }
    }
}
