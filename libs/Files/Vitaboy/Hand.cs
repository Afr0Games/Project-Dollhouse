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
    public class Hand
    {
        public Gesture Idle, Pointing, Fist;

        /// <summary>
        /// Constructs a new instance of the Hand class.
        /// </summary>
        /// <param name="Reader">A FileReader used to read a HandGroup file.</param>
        public Hand(FileReader Reader)
        {
            Idle = new Gesture(Reader);
            Pointing = new Gesture(Reader);
            Fist = new Gesture(Reader);
        }
    }
}
