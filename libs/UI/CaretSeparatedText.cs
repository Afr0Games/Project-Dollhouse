/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace UI
{
    /// <summary>
    /// Loads a Caret Separated Text (CST) file.
    /// </summary>
    public class CaretSeparatedText
    {
        private Dictionary<int, string> m_Strings = new Dictionary<int, string>();

        /// <summary>
        /// Gets a string from this CaretSeparatedText instance with the specified ID.
        /// </summary>
        /// <param name="ID">ID of string to get.</param>
        /// <returns>A string with the specified ID.</returns>
        public string this[int ID]
        {
            get { return (string)m_Strings[ID]; }
        }

        public CaretSeparatedText(string Path)
        {
            int ID = 1;
            string UnfinishedString = "";
            bool SplitString = false;

            foreach (string CaretString in File.ReadLines(Path))
            {
                if (!CaretString.Contains("^")) //Comments
                    continue;

                if (CaretString.EndsWith("^", StringComparison.CurrentCultureIgnoreCase))
                {
                    m_Strings.Add(ID, SanitizeString(CaretString));
                    ID++;
                }
                else
                {
                    UnfinishedString = SanitizeString(CaretString);
                    SplitString = true;
                }

                if (SplitString)
                {
                    if (CaretString.EndsWith("^"))
                    {
                        m_Strings.Add(ID, SanitizeString(UnfinishedString + CaretString));
                        ID++;

                        SplitString = false;
                    }
                    else
                        UnfinishedString += CaretString;

                    continue;
                }
            }
        }

        private string SanitizeString(string Input)
        {
            MatchCollection MC = Regex.Matches(Input, @"[\d]{1,2} ");

			if (MC.Count > 0) 
			{
				if(IsLinux) //Linux font doesn't like special chars...
					return Input.Remove(0, MC[0].Length).Replace("^", "").Replace("™", "");
				else
					return Input.Remove (0, MC [0].Length).Replace ("^", "");
			}
            else
			{
				if(IsLinux)
					return Input.Replace("^", "").Replace("™", "");
				else
                	return Input.Replace("^", "");
			}
        }

		/// <summary>
		/// Gets a value indicating if platform is linux.
		/// </summary>
		/// <value><c>true</c> if is linux; otherwise, <c>false</c>.</value>
		private static bool IsLinux
		{
			get
			{
				int p = (int) Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}
    }
}
