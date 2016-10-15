using System;
using System.Collections.Generic;
using System.Text;

namespace Files.AudioLogic
{
    [Serializable]
    public class HLSException : Exception
    {
        public HLSException(string Message) : base(Message)
        {

        }
    }
}
