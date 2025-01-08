/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;

namespace Terrain
{
    interface ICamera
    {
        /// <summary>
        /// Gets this camera's pitch.
        /// </summary>
        float Pitch { get; }

        /// <summary>
        /// Gets this camera's yaw.
        /// </summary>
        float Yaw { get; }

        /// <summary>
        /// Gets this camera's roll.
        /// </summary>
        float Roll { get; }

        /// <summary>
        /// Gets this camera's zoom.
        /// </summary>
        float Zoom { get; }

        /// <summary>
        /// Gets this camera's world matrix.
        /// </summary>
        Matrix WorldMatrix { get; }

        /// <summary>
        /// Gets or sets this camera's view matrix.
        /// </summary>
        Matrix ViewMatrix { get; set; }

        /// <summary>
        /// Gets this camera's projection matrix.
        /// </summary>
        Matrix ProjectionMatrix { get; }

        /// <summary>
        /// Gets this camera's target.
        /// </summary>
        Vector3 Target { get; }

        /// <summary>
        /// Gets or sets this camera's position.
        /// </summary>
        Vector3 Position { get; set; }
    }
}
