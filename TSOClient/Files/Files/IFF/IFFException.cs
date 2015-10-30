using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.IFF
{
    public class IFFException : Exception
    {
        public IFFException(string Message) : base(Message)
        {

        }
    }
}
