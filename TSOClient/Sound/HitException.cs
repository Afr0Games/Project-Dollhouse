﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is the Files library.
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;

namespace Sound
{
    [Serializable]
    public class HitException : Exception
    {
        public HitException(string Message) : base(Message)
        {

        }
    }
}
