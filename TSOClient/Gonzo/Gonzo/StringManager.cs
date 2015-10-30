using System;
using System.Collections.Generic;
using System.Text;
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
        public void Initialize(string Path, string StringDir, string Language)
        {
            foreach (string CSTPath in GetFileList("*.cst", Path + StringDir + "\\" + Language + ".dir"))
            {
                CaretSeparatedText StringTable = new CaretSeparatedText(CSTPath);
                int ID = int.Parse(System.IO.Path.GetFileName(CSTPath).Split('_')[0]);

                m_StringTables.Add(ID, StringTable);
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
    }
}
