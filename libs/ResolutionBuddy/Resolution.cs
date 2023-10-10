/// The MIT License (MIT)

///Copyright(c) 2013 Dan Manning

///Permission is hereby granted, free of charge, to any person obtaining a copy
///of this software and associated documentation files (the "Software"), to deal
///in the Software without restriction, including without limitation the rights
///to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
///copies of the Software, and to permit persons to whom the Software is
///furnished to do so, subject to the following conditions:

///The above copyright notice and this permission notice shall be included in
///all copies or substantial portions of the Software.

///THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
///IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
///FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
///AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
///LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
///OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
///THE SOFTWARE.
///


using Microsoft.Xna.Framework;

namespace ResolutionBuddy
{
    /// <summary>
    /// Singleton for easy access to various resolution items
    /// </summary>
    public static class Resolution
    {
        #region Properties

        private static IResolution _resolution;

        public static Rectangle TitleSafeArea
        {
            get { return _resolution.TitleSafeArea; }
        }

        public static Rectangle ScreenArea
        {
            get { return _resolution.ScreenArea; }
        }

        public static Matrix ScreenMatrix
        {
            get
            {
                return _resolution.ScreenMatrix;
            }
        }

        #endregion //Properties

        #region Methods

        #region Initialization

        public static void Init(IResolution resolution)
        {
            _resolution = resolution;
        }

        #endregion Initialization

        /// <summary>
        /// Get the transformation matrix for when you call SpriteBatch.Begin
        /// To add this to a camera matrix, do CameraMatrix * TransformationMatrix
        /// </summary>
        /// <returns>The matrix.</returns>
        public static Matrix TransformationMatrix()
        {
            return _resolution.TransformationMatrix();
        }

        /// <summary>
        /// Given a screen coord, convert to game coordinate system.
        /// </summary>
        /// <param name="screenCoord"></param>
        /// <returns></returns>
        public static Vector2 ScreenToGameCoord(Vector2 screenCoord)
        {
            return _resolution.ScreenToGameCoord(screenCoord);
        }

        public static void ResetViewport()
        {
            _resolution.ResetViewport();
        }

        #endregion //Methods
    }
}