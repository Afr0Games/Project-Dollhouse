using GonzoNet.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GonzoNet.Exceptions
{
    public class PacketHandlerException : Exception
    {
        public EventCodes ErrorCode = EventCodes.PACKET_DECRYPTION_ERROR;

        public PacketHandlerException(string Description = "Packethandler already existed!")
            : base(Description)
        {

        }
    }
}
