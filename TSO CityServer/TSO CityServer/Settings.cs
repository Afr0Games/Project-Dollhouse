/*
This Source Code Form is subject to the terms of the 
Mozilla Public License, v. 2.0. If a copy of the MPL was not 
distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Configuration;

namespace TSO_CityServer
{
    public class Settings
    {
        public static IPEndPoint BINDING
        {
            get
            {
                var binding = System.Configuration.ConfigurationManager.AppSettings["BINDING"];
                if (binding == null)
                {
                    return new IPEndPoint(IPAddress.Any, 2107);
                }
                string[] components = binding.Split(new char[]{':'}, 2);
                if (components.Length == 0)
                {
                    return new IPEndPoint(IPAddress.Any, 2107);
                }
                else if (components.Length == 1)
                {
                    return new IPEndPoint(IPAddress.Parse(components[0]), 2107);
                }
                else
                {
                    return new IPEndPoint(IPAddress.Parse(components[0]), int.Parse(components[1]));
                }
            }
        }
    }
}
