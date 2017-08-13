using System;
using System.Collections.Generic;
using Files.IFF;

namespace SimAntics
{
    /// <summary>
    /// A Simantics object.
    /// </summary>
    public class XObject : XMTObject
    {
        private ObjectModule m_Module;

        /// <summary>
        /// Creates a new XObject instance.
        /// </summary>
        /// <param name="Obj">The IFF from which to load this object.</param>
        public XObject(Iff Obj, ObjectModule Module) : base(Obj)
        {
            m_Module = Module;
            BHAV Main;

            if (Obj.Master.MainID < 8192) //The main subroutine is local.
                Main = Obj.GetBHAV(Obj.Master.MainID);
        }
    }
}
