using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Vitaboy
{
    internal class Helpers
    {
        /// <summary>
        /// Finds the matrix for a given quaternion.
        /// </summary>
        /// <param name="Quat">The quaternion for which to find a matrix.</param>
        /// <returns>A Matrix for the given quaternion.</returns>
        public static Matrix FindQuaternionMatrix(Quaternion Quat)
        {
            float x2 = Quat.X * Quat.X;
            float y2 = Quat.Y * Quat.Y;
            float z2 = Quat.Z * Quat.Z;
            float xy = Quat.X * Quat.Y;
            float xz = Quat.X * Quat.Z;
            float yz = Quat.Y * Quat.Z;
            float wx = Quat.W * Quat.X;
            float wy = Quat.W * Quat.Y;
            float wz = Quat.W * Quat.Z;

            var mtxIn = new Matrix();

            mtxIn.M11 = 1.0f - 2.0f * (y2 + z2);
            mtxIn.M12 = 2.0f * (xy - wz);
            mtxIn.M13 = 2.0f * (xz + wy);
            mtxIn.M14 = 0.0f;
            mtxIn.M21 = 2.0f * (xy + wz);
            mtxIn.M22 = 1.0f - 2.0f * (x2 + z2);
            mtxIn.M23 = 2.0f * (yz - wx);
            mtxIn.M24 = 0.0f;
            mtxIn.M31 = 2.0f * (xz - wy);
            mtxIn.M32 = 2.0f * (yz + wx);
            mtxIn.M33 = 1.0f - 2.0f * (x2 + y2);
            mtxIn.M34 = 0.0f;
            mtxIn.M41 = 0.0f;
            mtxIn.M42 = 0.0f;
            mtxIn.M43 = 0.0f;
            mtxIn.M44 = 1.0f;

            return mtxIn;
        }

        public static float DotProduct(Quaternion Rotation1, Quaternion Rotation2)
        {
            return Rotation1.X * Rotation2.X + Rotation1.Y * Rotation2.Y + Rotation1.Z * Rotation2.Z +
                Rotation1.W * Rotation2.W;
        }
    }
}
