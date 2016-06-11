using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

                if(SplitString)
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
            }
        }

        private string SanitizeString(string Input)
        {
            MatchCollection MC = Regex.Matches(Input, @"[\d]{1,2} ");

            if (MC.Count > 0)
				#if LINUX //Linux font doesn't like special chars...
				return Input.Remove(0, MC[0].Length).Replace("^", "").Replace("™", "");
				#else
                return Input.Remove(0, MC[0].Length).Replace("^", "");
				#endif
            else
				#if LINUX
				return Input.Replace("^", "").Replace("™", "");
				#else
                return Input.Replace("^", "");
				#endif
        }
    }
}
