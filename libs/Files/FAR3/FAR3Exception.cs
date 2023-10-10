/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.FAR3
{
    [Serializable]
    public class FAR3Exception : Exception
    {
        /// <summary>
        /// An exception thrown by the FAR3Archive class when it failed to read a FAR3 archive.
        /// </summary>
        /// <param name="Message">The message that was passed by the class.</param>
        public FAR3Exception(string Message) : base(Message)
        {

        }
    }
}
