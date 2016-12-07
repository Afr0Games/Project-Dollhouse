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
using System.IO;

namespace Gonzo
{
    public class StringManager
    {
        private static Dictionary<int, CaretSeparatedText> m_StringTables = new Dictionary<int, CaretSeparatedText>();

        public static CaretSeparatedText StrTable(int ID) { return m_StringTables[ID]; }

        /// <summary>
        /// Initializes the StringManager.
        /// </summary>
        /// <param name="Path">Path to TSO directory.</param>
        /// <param name="StringDir">TSO's string directory.</param>
        /// <param name="Language">The language to use, without ".dir" extension.</param>
        public static void Initialize(string Path, string StringDir, string Language)
        {
			if (IsLinux)
			{
				foreach (string CSTPath in GetFileList("*.cst", Path + StringDir + "/" + Language + ".dir")) {
					CaretSeparatedText StringTable = new CaretSeparatedText (CSTPath);
					int ID = int.Parse (System.IO.Path.GetFileName (CSTPath).Split ("_".ToCharArray (),
						         StringSplitOptions.RemoveEmptyEntries) [0]);

					m_StringTables.Add (ID, StringTable);
				}
			} 
			else
			{
				foreach (string CSTPath in GetFileList("*.cst", Path + StringDir + "\\" + Language + ".dir")) {
					CaretSeparatedText StringTable = new CaretSeparatedText (CSTPath);
					int ID = int.Parse (System.IO.Path.GetFileName (CSTPath).Split ("_".ToCharArray (),
						                    StringSplitOptions.RemoveEmptyEntries) [0]);

					m_StringTables.Add (ID, StringTable);
				}
			}
        }

        private static IEnumerable<string> GetFileList(string fileSearchPattern, string rootFolderPath)
        {
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern);
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
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
