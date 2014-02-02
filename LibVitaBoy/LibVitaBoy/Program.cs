/*
This Source Code Form is subject to the terms of the 
Mozilla Public License, v. 2.0. If a copy of the MPL was not 
distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
*/

using System;

namespace LibVitaBoy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}

