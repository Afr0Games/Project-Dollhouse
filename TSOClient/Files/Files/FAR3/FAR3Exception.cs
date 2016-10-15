using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.FAR3
{
    [Serializable]
    public class FAR3Exception : Exception
    {
        /// <summary>
        /// An exception thrown by the FAR3Archive class when it failed to read a FAR3 archive.
        /// </summary>
        /// <param name="Message">The message that was passed by the class.</param>
        public FAR3Exception(string Message) : base(Message)
        {

        }
    }
}
