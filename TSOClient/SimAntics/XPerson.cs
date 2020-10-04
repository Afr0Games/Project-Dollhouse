using System;
using System.Collections.Generic;
using Files.Manager;
using Files.IFF;

namespace SimAntics
{
    /// <summary>
    /// A Simantics person.
    /// </summary>
    public class XPerson : XMTObject
    {
        private ObjectModule m_Module;

        /// <summary>
        /// Creates a new XPerson instance.
        /// </summary>
        /// <param name="Obj">The IFF from which to load this object.</param>
        public XPerson(Iff Obj, ObjectModule Module) : base(Obj)
        {
            bool IsGlobal = false;

            m_Module = Module;
            BHAV Main;

            if(Obj.Master.MasterID < 4096) //The main subroutine is global.
            {
                Main = m_Module.Globals.GetBHAV(Obj.Master.MainID);
                IsGlobal = true;
            }
            if (Obj.Master.MainID < 8192 && IsGlobal != true) //The main subroutine is local.
                Main = Obj.GetBHAV(Obj.Master.MainID);
            else if(Obj.Master.MainID > 8192) //The main subroutine is semi-global.
            {
                //TODO: Is this the correct ID of the GLOB chunk?
                Iff SemiGlobal = FileManager.Instance.GetIFF(Obj.GetGLOB(Obj.Master.Global).SemiGlobalIFF);
                Main = SemiGlobal.GetBHAV(Obj.Master.MainID);
            }
        }
    }
}
