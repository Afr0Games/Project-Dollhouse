using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GonzoNet.Packets
{
    /// <summary>
    /// IDs used by internal packets.
    /// These IDs are reserved, I.E
    /// should not be used by a protocol.
    /// </summary>
    public enum GonzoNetIDs
    {
        SGoodbye = 0xFE,
        CGoodbye = 0xFF //Should be sufficiently large, no protocol should need this many packets.
    }

    /// <summary>
    /// Number of seconds for server or client to disconnect by default.
    /// </summary>
    public enum GonzoDefaultTimeouts : int
    {
        Server = 60,
        Client = 5,
    }

    /// <summary>
    /// Internal packet sent by client and server before disconnecting.
    /// </summary>
    [Serializable]
    public class GoodbyePacket : IPacket
    {
        /// <summary>
        /// The timeout, I.E how many seconds until the sender will disconnect.
        /// </summary>
        public TimeSpan TimeOut { get; }
        
        /// <summary>
        /// The time when this packet was sent.
        /// </summary>
        public DateTime SentTime { get; }

        /// <summary>
        /// Initializes a new GoodbyePacket instance.
        /// </summary>
        /// <param name="timeOut">A value that indicates, in seconds,
        /// how long till the sender disconnects.</param>
        public GoodbyePacket(int timeOut)
        {
            TimeOut = TimeSpan.FromSeconds(timeOut);
            SentTime = DateTime.UtcNow;
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
        /// <returns>A GoodbyePacket instance.</returns>
        public static GoodbyePacket ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (GoodbyePacket)obj;
            }
        }
    }
}
