using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GonzoNet
{
    delegate void ProcessedPacketDelegate(PacketStream Packet); 

    /// <summary>
    /// A buffer for processing received data, turning it into individual PacketStream instances.
    /// </summary>
    internal class ProcessingBuffer
    {
        public static int MAX_PACKET_SIZE = 1024;

        private static object m_BufferLock = new object();
        private Queue<byte> m_InternalBuffer = new Queue<byte>();

        /// <summary>
        /// Gets, but does NOT set the internal buffer. Used by tests.
        /// </summary>
        public Queue<byte> InternalBuffer
        {
            get { return m_InternalBuffer; }
        }
        
        private object m_HeaderLock = new object();
        private bool m_HasReadHeader = false;
        private byte m_CurrentID;       //ID of current packet.
        private ushort m_CurrentLength; //Length of current packet.

        public event ProcessedPacketDelegate OnProcessedPacket;
        Task ProcessingTask;

        public ProcessingBuffer()
        {
            ProcessingTask = Task.Run(() =>
            {
                while (true)
                {
                    if (m_InternalBuffer.Count >= (int)PacketHeaders.UNENCRYPTED)
                    {
                        if (!m_HasReadHeader)
                        {
                            lock (m_BufferLock)
                            {
                                m_CurrentID = m_InternalBuffer.Dequeue();
                                byte[] LengthBuf = new byte[2];

                                for (int i = 0; i < LengthBuf.Length; i++)
                                    LengthBuf[i] = m_InternalBuffer.Dequeue();

                                m_CurrentLength = BitConverter.ToUInt16(LengthBuf, 0);
                            }

                            if (m_CurrentLength == 0)
                                //TODO: Fail gracefully if no handler is registered...
                                m_CurrentLength = PacketHandlers.Get(m_CurrentID).Length;

                            lock (m_HeaderLock)
                                 m_HasReadHeader = true;
                        }
                    }

                    if (m_HasReadHeader == true)
                    {
                        //Hurray, enough shit was shoveled into the buffer that we have a new packet!
                        if (m_InternalBuffer.Count >= (m_CurrentLength - 3))
                        {
                            byte[] PacketData = new byte[m_CurrentLength - 3]; //3 bytes is the size of the header.

                            lock (m_BufferLock)
                            {
                                for (int i = 0; i < PacketData.Length; i++)
                                    PacketData[i] = m_InternalBuffer.Dequeue();
                            }

                            lock (m_HeaderLock)
                                m_HasReadHeader = false;

                            PacketStream Packet = new PacketStream(m_CurrentID, m_CurrentLength, PacketData);
                            OnProcessedPacket(Packet);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Shovels shit (data) into the buffer.
        /// </summary>
        /// <param name="Data">The data to add. Needs to be no bigger than MAX_PACKET_SIZE!</param>
        public void AddData(byte[] Data)
        {
            //This protects us from attacks with malicious packets that specify a header size of several gigs...
            if (Data.Length > MAX_PACKET_SIZE)
            {
                Logger.Log("Tried adding too much data to ProcessingBuffer!", LogLevel.error);
                return;
            }

            lock (m_BufferLock)
            {
                for (int i = 0; i < Data.Length; i++)
                    m_InternalBuffer.Enqueue(Data[i]);
            }
        }
    }
}
