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
using System.Reflection;
using Files.AudioLogic;
using log4net;

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

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HitResourcegroup(string HitPath, string EVTPath, string HSMPath)
        {
            HitResource = new Hit(File.Open(HitPath, FileMode.Open, FileAccess.Read, FileShare.Read));
            Events = new EVT(File.Open(EVTPath, FileMode.Open, FileAccess.Read, FileShare.Read));
            if(HSMPath != "")
                Symbols = new HSM(HSMPath);
        }

        ~HitResourcegroup()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this HitResourcegroup instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this HitResourcegroup instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (HitResource != null)
                    HitResource.Dispose();
                if (Events != null)
                    Events.Dispose();

                // Prevent the finalizer from calling ~HitResourcegroup, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("HitResourcegroup not explicitly disposed!");
        }
    }
}
