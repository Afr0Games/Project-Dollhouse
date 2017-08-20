/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Collections.Generic;
using System.Linq;

namespace Files.FAR1
{
    /// <summary>
    /// A container for storing FAR1Entry instances and retrieving them.
    /// </summary>
    public class EntryContainer : List<FAR1Entry>
    {
        private List<FAR1Entry> m_Entries = new List<FAR1Entry>();

        public new void Add(FAR1Entry Entry)
        {
            m_Entries.Add(Entry);
        }

        /// <summary>
        /// Gets an entry from this EntryContainer instance.
        /// </summary>
        /// <param name="Filename">Hashed filename of entry to get.</param>
        /// <returns>A FAR1Entry instance if found, null if not found.</returns>
        public FAR1Entry this[byte[] FilenameHash]
        {
            get
            {
                foreach (FAR1Entry Entry in m_Entries)
                {
                    if (Entry.FilenameHash.SequenceEqual(FilenameHash))
                        return Entry;
                }

                return null;
            }
        }

        public new IEnumerator<FAR1Entry> GetEnumerator()
        {
            using (IEnumerator<FAR1Entry> ie = base.GetEnumerator())
            {
                while (ie.MoveNext())
                    yield return ie.Current;
            }
        }

        /// <summary>
        /// Does this EntryContainer instance contain the specified FAR1Entry instance?
        /// </summary>
        /// <param name="Filename">Filename of entry to look for.</param>
        /// <returns>True if found, false if not.</returns>
        public bool Contains(byte[] FilenameHash)
        {
            foreach (FAR1Entry Entry in m_Entries)
            {
                if (Entry.FilenameHash.SequenceEqual(FilenameHash))
                    return true;
            }

            return false;
        }
    }
}
