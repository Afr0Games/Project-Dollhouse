using System;
using System.Collections.Concurrent;
using GonzoNet.Exceptions;

namespace GonzoNet
{
    /// <summary>
    /// Framework for registering packet handlers with GonzoNet.
    /// </summary>
    public class PacketHandlers
    {
        /**
         * Framework
         */
        private static ConcurrentDictionary<byte, PacketHandler> m_Handlers = new ConcurrentDictionary<byte, PacketHandler>();

        /// <summary>
        /// Registers a PacketHandler with GonzoNet.
        /// </summary>
        /// <param name="id">The ID of the packet.</param>
        /// <param name="size">The size of the packet. 0 means variable length.</param>
        /// <param name="handler">The handler for the packet.</param>
        public static void Register(byte id, bool Encrypted, ushort size, OnPacketReceive handler)
        {
            if (!m_Handlers.TryAdd(id, new PacketHandler(id, Encrypted, size, handler)))
                throw new PacketHandlerException(); //Handler already existed.
        }

        /// <summary>
        /// Gets a handler based on its ID.
        /// </summary>
        /// <param name="id">The ID of the handler.</param>
        /// <returns>The handler with the specified ID, or null if the handler didn't exist.</returns>
        public static PacketHandler Get(byte id)
        {
            if (m_Handlers.ContainsKey(id))
                return m_Handlers[id];
            else return null;
        }
    }
}
