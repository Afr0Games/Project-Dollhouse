using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using GonzoNet.Packets;

namespace GonzoNet
{
    //delegate void ProcessedPacketDelegate(PacketStream Packet);
    delegate void ProcessedPacketDelegate(Packet packet);

    /// <summary>
    /// A buffer for processing received data, turning it into individual PacketStream instances.
    /// </summary>
    internal class ProcessingBuffer : IDisposable
    {
        public static int MAX_PACKET_SIZE = 1024;

        private BlockingCollection<byte> m_InternalBuffer = new BlockingCollection<byte>();
        private CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Gets, but does NOT set the internal buffer. Used by tests.
        /// </summary>
        public BlockingCollection<byte> InternalBuffer
        {
            get { return m_InternalBuffer; }
        }
        
        private object m_HeaderLock = new object();
        private bool m_HasReadHeader = false;
        private byte m_CurrentID;       //ID of current packet.
        private ushort m_CurrentLength; //Length of current packet.

        public event ProcessedPacketDelegate OnProcessedPacket;
        Task ProcessingTask;

        /// <summary>
        /// Creates a new ProcessingBuffer instance.
        /// </summary>
        public ProcessingBuffer()
        {
            CancellationToken Token = m_CancellationTokenSource.Token;

            ProcessingTask = Task.Run(async () =>
            {
                while (true)
                {
                    if (Token.IsCancellationRequested)
                        return;

                    if (m_InternalBuffer.Count >= (int)PacketHeaders.UNENCRYPTED)
                    {
                        if (!m_HasReadHeader)
                        {
                            m_CurrentID = m_InternalBuffer.Take();

                            byte[] LengthBuf = new byte[2];

                            for (int i = 0; i < LengthBuf.Length; i++)
                                LengthBuf[i] = m_InternalBuffer.Take();

                            m_CurrentLength = BitConverter.ToUInt16(LengthBuf, 0);

                            if (m_CurrentLength == 0)
                                //TODO: Fail gracefully if no handler is registered...
                                m_CurrentLength = PacketHandlers.Get(m_CurrentID).Length;

                            lock (m_HeaderLock)
                                m_HasReadHeader = true;
                        }
                    }

                    if (m_HasReadHeader == true)
                    {
                        //Hurray, enough shit (data) was shoveled into the buffer that we have a new packet!
                        if (m_InternalBuffer.Count >= (m_CurrentLength - 3))
                        {
                            byte[] PacketData = new byte[m_CurrentLength - 3]; //Three bytes is the length of the header.

                            for (int i = 0; i < PacketData.Length; i++)
                                PacketData[i] = m_InternalBuffer.Take();

                            lock (m_HeaderLock)
                                m_HasReadHeader = false;

                            Packet P = new Packet(m_CurrentID, m_CurrentLength, PacketData);
                            OnProcessedPacket(P);
                        }
                    }

                    await Task.Delay(10); //DON'T HOG THE PROCESSOR
                }
            }, Token);
        }

        private void StopProcessing()
        {
            m_CancellationTokenSource.Cancel(); // Request cancellation
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

            for (int i = 0; i < Data.Length; i++)
                m_InternalBuffer.Add(Data[i]);
        }

        ~ProcessingBuffer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this SoundPlayer instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this SoundPlayer instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                StopProcessing();

                // Prevent the finalizer from calling ~NetworkClient, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                Logger.Log("ProcessingBuffer not explicitly disposed!", LogLevel.error);
        }
    }
}
