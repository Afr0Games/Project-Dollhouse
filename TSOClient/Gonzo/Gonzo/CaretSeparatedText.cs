using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gonzo
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
            get { return m_Strings[ID]; }
        }

        public CaretSeparatedText(string Path)
        {
            int ID = 1, LoadedID = 0;
            bool AppendString = false;

            foreach (string CaretString in File.ReadLines(Path))
            {
                if (!CaretString.Contains("^")) //Comments
                    continue;

                if (AppendString)
                {
                    m_Strings[ID - 1] += CaretString.Replace("^", "");
                    AppendString = false;
                    continue;
                }

                //Sometimes a string will end with /r/n and continue on the next line...
                if (!CaretString.EndsWith("^", StringComparison.CurrentCultureIgnoreCase))
                    AppendString = true;

                string[] Strs = CaretString.Split(' ');
                bool HasID = true;

                try
                {
                    LoadedID = int.Parse(Strs[0]);
                }
                catch (Exception)
                {
                    HasID = false;
                }

                if (HasID)
                    m_Strings.Add(LoadedID, BuildString(Strs));
                else
                    m_Strings.Add(ID, CaretString.Replace("^", ""));

                ID++;
            }
        }

        /// <summary>
        /// Builds a string with spaces from an array of strings split on spaces.
        /// </summary>
        /// <param name="SplitStr">An array of strings split on spaces.</param>
        /// <returns>A string with spaces.</returns>
        private string BuildString(string[] SplitStr)
        {
            StringBuilder SBuilder = new StringBuilder();

            for (int i = 0; i < SplitStr.Length; i++)
            {
                if (i < (SplitStr.Length - 1))
                    SBuilder.Append(SplitStr[i].Replace("^", "") + " ");
                else
                    SBuilder.Append(SplitStr[i].Replace("^", ""));
            }

            return SBuilder.ToString();
        }
    }
}
