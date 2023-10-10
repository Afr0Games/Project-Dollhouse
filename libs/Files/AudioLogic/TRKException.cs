using System;

namespace Files.AudioLogic
{
    [Serializable]
    public class TRKException : Exception
    {
        public TRKException(string Message) : base(Message)
        {

        }
    }
}
