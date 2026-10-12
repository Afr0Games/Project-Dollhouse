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
using Microsoft.Xna.Framework.Graphics;

namespace Terrain
{
    /// <summary>
    /// Bridges the original ViewerClient's screen-space scale conditions to
    /// Hoodtest's EXISTING 3D perspective camera; never changes camera matrices,
    /// mesh coordinates, elevation, or terrain heights.
    /// </summary>
    public static class MaxisCity3DScale
    {
        //Verified from TSOViewerClientD.dll, renderer+0x14 (Sy) comparison.
        public const float TransitionThresholdPixels = 16f;
        //Original 2D renderer's vertical pixel shift per byte of elevation.
        public const float OriginalElevationPixelsPerSample = 2f;
        //Existing Terrain quadtree and height sampler (do not modify).
        public const float TerrainHeightUnitsPerSample = 1f / 8f;

        public static Vector2 ProjectedTileSizePixels(Viewport viewport, CityCamera camera)
        {
            Vector3 focus;
            if (!camera.TryGetFocus(out focus)) return Vector2.Zero;
            if (!IsInFront(camera, focus)) return Vector2.Zero;

            // A 1x1 tile's four ground-plane corners, with a constant local Z.
            // The full vertical span corresponds to the diamond's Sy in the
            // original isometric tile projection, measured through our 3D view.
            float x = (float)Math.Floor(focus.X);
            float y = (float)Math.Floor(focus.Y);
            float z = focus.Z;
            float minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;
            for (int dy = 0; dy <= 1; ++dy)
            {
                for (int dx = 0; dx <= 1; ++dx)
                {
                    Vector3 position = new Vector3(x + dx, y + dy, z);
                    Vector3 screen = viewport.Project(position,
                        camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                    if (float.IsNaN(screen.X) || float.IsInfinity(screen.X) ||
                        float.IsNaN(screen.Y) || float.IsInfinity(screen.Y)) return Vector2.Zero;
                    minX = Math.Min(minX, screen.X);
                    maxX = Math.Max(maxX, screen.X);
                    minY = Math.Min(minY, screen.Y);
                    maxY = Math.Max(maxY, screen.Y);
                }
            }
            return new Vector2(maxX - minX, maxY - minY);
        }

        // Diagnostic only: evaluates how many pixels ONE original 8-bit height
        // sample moves through Hoodtest's perspective projection. The original
        // screen-space value was 2 pixels; do NOT use that as a world Z factor.
        public static float ElevationPixelsPerSample(Viewport viewport, CityCamera camera)
        {
            Vector3 focus;
            if (!camera.TryGetFocus(out focus)) return 0f;
            if (!IsInFront(camera, focus)) return 0f;
            Vector3 raised = focus + Vector3.UnitZ * TerrainHeightUnitsPerSample;
            Vector3 p0 = viewport.Project(focus, camera.ProjectionMatrix,
                camera.ViewMatrix, Matrix.Identity);
            Vector3 p1 = viewport.Project(raised, camera.ProjectionMatrix,
                camera.ViewMatrix, Matrix.Identity);
            return Math.Abs(p0.Y - p1.Y);
        }

        private static bool IsInFront(CityCamera camera, Vector3 point)
        {
            Vector3 forward = Vector3.Normalize(camera.Target - camera.Position);
            return Vector3.Dot(point - camera.Position, forward) > 0.2f;
        }
    }
}
