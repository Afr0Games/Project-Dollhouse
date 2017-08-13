using System;
using System.Collections.Generic;

namespace SimAntics
{
    public class SimanticsThread : Coroutine
    {
        /// <summary>
        /// The Simantics object that this thread belongs to.
        /// </summary>
        XMTObject m_Obj;

        public SimanticsThread(XMTObject Obj)
        {
            m_Obj = Obj;
        }

        /// <summary>
        /// Runs one Simantics instruction, and should be run once every frame.
        /// </summary>
        /// <returns>True if still running, otherwise yields.</returns>
        public override IEnumerable<object> process()
        {
            throw new NotImplementedException();
        }
    }
}
