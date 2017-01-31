/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Files library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.IO;
using Files.AudioLogic;

namespace Sound
{
    /// <summary>
    /// Groups together related HIT resources.
    /// </summary>
    public class HitResourcegroup : IDisposable
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool CleanUpNativeAndManagedResources)
        {
            if (CleanUpNativeAndManagedResources)
            {
                if (HitResource != null)
                    HitResource.Dispose();
                if (Events != null)
                    Events.Dispose();
            }
        }
    }
}
