using System;
using Files.Manager;
using System.Collections.Generic;
using Files.IFF;

namespace SimAntics
{
    /// <summary>
    /// Manages Sinantics objects.
    /// </summary>
    public class ObjectModule
    {
        private List<XObject> m_Objects = new List<XObject>();
        private List<XPerson> m_Persons = new List<XPerson>();

        public Iff Globals;

        public ObjectModule()
        {
            Globals = FileManager.GetIFF("globals.iff");
        }

        /// <summary>
        /// A list of all objects loaded by the VM.
        /// </summary>
        public List<XObject> Objects
        {
            get { return m_Objects; }
        }

        public List<XPerson> Persons
        {
            get { return m_Persons; }
        }
        
        /// <summary>
        /// Loads a simantics object.
        /// </summary>
        /// <param name="ObjToLoad">The IFF from which to load the object.</param>
        public void Load(Iff ObjToLoad)
        {
            XObject Obj;
            XPerson Pers;

            if (ObjToLoad.Master.ObjectType == OBJDType.Person)
            {
                Pers = new XPerson(ObjToLoad, this);
                m_Persons.Add(Pers);
            }
            else
            {
                Obj = new XObject(ObjToLoad, this);
                m_Objects.Add(Obj);
            }
        }
    }
}
