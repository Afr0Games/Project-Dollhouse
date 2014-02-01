﻿/*
This Source Code Form is subject to the terms of the 
Mozilla Public License, v. 2.0. If a copy of the MPL was not 
distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TSO_CityServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
