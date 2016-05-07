using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.AudioLogic
{
    /// <summary>
    /// HSM (short for HIT Symbols) is a plain-text format that defines constants for opcodes, registers, subroutine 
    /// address and sound resource IDs for inclusion in the HOT files as well as the MakeTrax source files used to make 
    /// the HIT files of the game. 
    /// </summary>
    public class HSM
    {
        public Dictionary<string, int> Constants = new Dictionary<string, int>();

        /// <summary>
        /// Loads a HSM from a given path.
        /// </summary>
        /// <param name="Path">The path to the HSM to load.</param>
        public HSM(string Path)
        {
            string[] Entries = File.ReadAllLines(Path);

            foreach(string Entry in Entries)
            {
                string[] SplitEntry = Entry.Split(" ".ToCharArray());

                if(!Constants.ContainsKey(SplitEntry[0]))
                    Constants.Add(SplitEntry[0], int.Parse(SplitEntry[1]));
            }
        }
    }
}
