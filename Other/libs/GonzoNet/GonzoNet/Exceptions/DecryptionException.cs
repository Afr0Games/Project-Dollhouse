/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.*/

using System;
using System.Collections.Generic;
using System.Text;
using GonzoNet.Events;

namespace GonzoNet.Exceptions
{
    public class DecryptionException : Exception
    {
        public EventCodes ErrorCode = EventCodes.PACKET_DECRYPTION_ERROR;

        public DecryptionException(string Description)
            : base(Description)
        {

        }
    }
}
