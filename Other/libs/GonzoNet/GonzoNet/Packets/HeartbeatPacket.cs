using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GonzoNet.Packets
{
    [Serializable]
    internal class HeartbeatPacket : IPacket
    {
        private TimeSpan m_TimeSinceLast;
        
        /// <summary>
        /// The time since the last heartbeat packet was sent.
        /// </summary>
        public TimeSpan TimeSinceLast
        {
            get { return m_TimeSinceLast; }
        }

        public HeartbeatPacket(TimeSpan TimeSinceLast)
        {
            m_TimeSinceLast = TimeSinceLast;
        }

        /// <summary>
        /// Serializes this class to a byte array.
        /// From: https://gist.github.com/LorenzGit/2cd665b6b588a8bb75c1a53f4d6b240a
        /// </summary>
        /// <returns>A byte array representation of this class.</returns>
        public byte[] ToByteArray()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes this class from a byte array.
        /// From: https://gist.github.com/LorenzGit/2cd665b6b588a8bb75c1a53f4d6b240a
        /// </summary>
        /// <returns>A Heartbeat packet instance.</returns>
        public static HeartbeatPacket ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (HeartbeatPacket)obj;
            }
        }
    }
}
