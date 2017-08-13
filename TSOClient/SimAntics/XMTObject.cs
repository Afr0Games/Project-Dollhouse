using System;
using Files.IFF;


namespace SimAntics
{
    public class XMTObject
    {
        private Iff m_IffObj;
        private SimanticsThread m_Thread;

        public XMTObject(Iff IffObj)
        {
            m_IffObj = IffObj;
            m_Thread = new SimanticsThread(this);
        }

        /// <summary>
        /// Runs one Simantics instruction in this object's thread. Should be called once every frame.
        /// </summary>
        public virtual void process()
        {
            m_Thread.process();
        }

        /// <summary>
        /// The GUID of this object.
        /// </summary>
        public uint GUID
        {
            get { return m_IffObj.Master.GUID; }
        }
    }
}
