using Microsoft.Xna.Framework;
using System;

namespace Hoodtest
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
