/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the CityRenderer.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;

namespace Cityrenderer
{
    /// <summary>
    /// A controller for the camera. Responsible for updating calculating view, projection and so on.
    /// </summary>
    public abstract class CameraController
    {
        protected Camera m_Camera;

        /// <summary>
        /// The projection of this camera controller's camera.
        /// </summary>
        public Matrix Projection
        {
            get
            {
                if (m_Camera.ProjectionDirty)
                    CalculateProjection();

                return m_Camera.Projection;
            }
        }

        /// <summary>
        /// The view of this camera controller's camera.
        /// </summary>
        public Matrix View
        {
            get
            {
                if (m_Camera.ViewDirty)
                    CalculateView();

                return m_Camera.View;
            }
        }

        protected abstract void CalculateProjection();
        protected abstract void CalculateView();
    }
}
