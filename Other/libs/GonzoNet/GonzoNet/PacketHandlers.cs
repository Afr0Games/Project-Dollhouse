using System;
using System.Collections.Concurrent;
using GonzoNet.Exceptions;
using GonzoNet.Packets;

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
        /// The IDs 0xFE (254) and 0xFF (255) are reserved
        /// for use by GonzoNet, so can't be used.
        /// </summary>
        /// <param name="ID">The ID of the packet.</param>
        /// <param name="size">The size of the packet. 0 means variable length.</param>
        /// <param name="handler">The handler for the packet.</param>
        public static void Register(byte ID, bool Encrypted, ushort size, OnPacketReceive handler)
        {
            if(ID == (byte)GonzoNetIDs.CGoodbye || ID == (byte)GonzoNetIDs.SGoodbye)
                throw new PacketHandlerException("Tried to register reserved handler!"); //Handler is internal.

            if (!m_Handlers.TryAdd(ID, new PacketHandler(ID, Encrypted, size, handler)))
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
