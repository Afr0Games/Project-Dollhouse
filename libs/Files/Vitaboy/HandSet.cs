/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

namespace Files.Vitaboy
{
    public class HandSet
    {
        public Hand Right, Left;

        /// <summary>
        /// Constructs a new instance of the HandSet class.
        /// </summary>
        /// <param name="Reader">A FileReader used to read a HandGroup file.</param>
        public HandSet(FileReader Reader)
        {
            Right = new Hand(Reader);
            Left = new Hand(Reader);
        }
    }
}
