namespace SimAntics
{
    public class ObjectModule
    {
        private Simulator m_Simulator;
        private XPerson m_SelectedPerson;

        public ObjectModule(Simulator Sim)
        {
            m_Simulator = Sim;
        }

        public XObject GetObjectByGUID()
        {

        }

        public XPerson GetPersonByGUID()
        {

        }

        public XPerson GetSelectedPerson()
        {
            return m_SelectedPerson;
        }

        /// <summary>
        /// Updates all objects in the simulation by simulating a tick.
        /// </summary>
        public void UpdateSimObjects()
        {

        }
    }
}
