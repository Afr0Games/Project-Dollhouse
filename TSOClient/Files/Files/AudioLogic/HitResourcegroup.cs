using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.AudioLogic
{
    /// <summary>
    /// Groups together related HIT resources.
    /// </summary>
    public class HitResourcegroup
    {
        public Hit HitResource;
        public EVT Events;
        public HSM Symbols;

        public HitResourcegroup(string HitPath, string EVTPath, string HSMPath)
        {
            HitResource = new Hit(File.Open(HitPath, FileMode.Open, FileAccess.Read, FileShare.Read));
            Events = new EVT(File.Open(EVTPath, FileMode.Open, FileAccess.Read, FileShare.Read));
            if(HSMPath != "")
                Symbols = new HSM(HSMPath);
        }
    }
}
