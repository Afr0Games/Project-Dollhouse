using System;
using System.Collections.Generic;
using System.Collections;

namespace UI
{
    /// <summary>
    /// From: https://www.codeproject.com/Articles/20910/Generic-Gap-Buffer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class GapBuffer<T>
    {
        /// <summary>
        /// Enumerates the elements of a <see cref="GapBuffer{T}"/>. 
        /// </summary>
        [Serializable]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            #region Fields

            private T _current;
            private int _index;
            private GapBuffer<T> _gapBuffer;
            private int _version;

            #endregion Fields


            #region Constructors

            internal Enumerator(GapBuffer<T> buffer)
            {
                _gapBuffer = buffer;
                _index = 0;
                _version = _gapBuffer._version;
                _current = default(T);
            }

            #endregion Constructors


            #region Properties

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <value>The element in the <see cref="GapBuffer{T}"/> at the current 
            /// position of the enumerator.</value>
            public T Current
            {
                get { return _current; }
            }


            // Explicit IEnumerator implementation
            object IEnumerator.Current
            {
                get
                {
                    // Is it possible to have a current item?
                    if (_index == 0 || _index == (_gapBuffer.Count + 1))
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");

                    return Current;
                }
            }

            #endregion Properties


            #region Methods

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="GapBuffer{T}"/>.
            /// </summary>
            /// <returns><b>true</b> if the enumerator was successfully advanced to the next element; 
            /// <b>false</b> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">
            /// The collection was modified after the enumerator was created. 
            /// </exception>
            public bool MoveNext()
            {
                // Check version numbers
                if (this._version != this._gapBuffer._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                // Advance the index
                if (this._index < this._gapBuffer.Count)
                {
                    this._current = this._gapBuffer[this._index];
                    this._index++;
                    return true;
                }

                // The pointer is at the end of the collection
                this._index = this._gapBuffer.Count + 1;
                this._current = default(T);
                return false;
            }


            /// <summary>
            /// Releases all resources used by the <see cref="GapBuffer{T}.Enumerator"/>. 
            /// </summary>
            public void Dispose()
            {
                // Nothing to release here
            }


            // Explicit IEnumerator implementation
            void IEnumerator.Reset()
            {
                // Check the version
                if (_version != _gapBuffer._version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                // Reset the pointer
                this._index = 0;
                this._current = default(T);
            }

            #endregion Methods
        }
    }
}
