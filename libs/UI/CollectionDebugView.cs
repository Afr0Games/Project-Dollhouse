using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UI
{
    internal sealed class CollectionDebugView<T>
    {
        #region Fields

        private ICollection<T> _collection;

        #endregion Fields


        #region Constructors

        public CollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            this._collection = collection;
        }

        #endregion Constructors


        #region Properties

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[this._collection.Count];
                this._collection.CopyTo(array, 0);
                return array;
            }
        }

        #endregion Properties
    }
}
