using System;

namespace SimAntics
{
    public class Simulator
    {
        private ObjectModule m_Module = new ObjectModule();

        /// <summary>
        /// Ticks all objects and persons. Call once per frame.
        /// </summary>
        public void Tick()
        {
            foreach (XObject Obj in m_Module.Objects)
                Obj.process();

            foreach(XPerson Person in m_Module.Persons)
                Person.process();
        }
    }
}
