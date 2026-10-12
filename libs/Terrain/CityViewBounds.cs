/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Terrain library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using Microsoft.Xna.Framework;

namespace Terrain
{
    /// <summary>
    /// Hoodtest-specific limit for where the CENTER of the 3D city view may pan.
    /// Uses the same diamond as QuadtreeNode.IsValidCoord, with a small inset
    /// so a focused point isn't perched on an invalid edge texel.
    /// This is NOT the rectangular 2D viewport clamp from TSOViewerClientD.
    /// </summary>
    public static class CityViewBounds
    {
        //Original city raster: 0 <= X <= 511, -511 <= Y <= 0.
        //The quadtree accepts X >= |306+Y| and X <= 511-|205+Y|.
        //These are the four vertices of that exact convex polygon, CCW.
        private static readonly Vector2[] CityCorners =
        {
            new Vector2(0f, -306f),
            new Vector2(205f, -511f),
            new Vector2(511f, -205f),
            new Vector2(306f, 0f)
        };

        private static readonly Vector2 Center = new Vector2(255.5f, -255.5f);

        //Roughly 6-8 terrain cells at the corners. Keeps a focus target away
        //from the masked edges; camera itself may remain beyond the perimeter.
        private const float InsetScale = 0.975f;

        /// <summary>
        /// Return the nearest legal focus point. Interior points are unchanged,
        /// and outside points slide along the nearest diamond edge.
        /// </summary>
        public static Vector2 ClampFocus(Vector2 focus)
        {
            if (float.IsNaN(focus.X) || float.IsNaN(focus.Y) ||
                float.IsInfinity(focus.X) || float.IsInfinity(focus.Y))
                return Center;

            bool inside = true;
            float bestSquaredDistance = float.MaxValue;
            Vector2 nearest = Center;

            for (int i = 0; i < CityCorners.Length; ++i)
            {
                Vector2 a = Inset(CityCorners[i]);
                Vector2 b = Inset(CityCorners[(i + 1) % CityCorners.Length]);
                Vector2 edge = b - a;
                Vector2 relative = focus - a;

                //Positive cross product is inside a counterclockwise polygon.
                if (edge.X * relative.Y - edge.Y * relative.X < 0f)
                    inside = false;

                float lengthSquared = edge.LengthSquared();
                float t = lengthSquared > 0f
                    ? MathHelper.Clamp(Vector2.Dot(relative, edge) / lengthSquared, 0f, 1f)
                    : 0f;
                Vector2 candidate = a + edge * t;
                float distSquared = Vector2.DistanceSquared(focus, candidate);
                if (distSquared < bestSquaredDistance)
                {
                    bestSquaredDistance = distSquared;
                    nearest = candidate;
                }
            }
            return inside ? focus : nearest;
        }

        private static Vector2 Inset(Vector2 corner)
        {
            return Center + (corner - Center) * InsetScale;
        }
    }
}
