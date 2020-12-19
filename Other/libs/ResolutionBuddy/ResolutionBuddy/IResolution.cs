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
    public interface IResolution
    {
        /// <summary>
        /// The rectangle of the title safe area (game coords)
        /// </summary>
        Rectangle TitleSafeArea { get; }

        /// <summary>
        /// The rectangle of the entire screen (game coords)
        /// </summary>
        Rectangle ScreenArea { get; }

        /// <summary>
        /// Matrix to convert screen coordinates to game coordinates. Used for mouse clicks, touches, etc.
        /// </summary>
        Matrix ScreenMatrix { get; }

        Point VirtualResolution { get; set; }

        Point ScreenResolution { get; set; }

        /// <summary>
        /// The ratio between the ScreenResolution and the VirtualResolution.
        /// Can be used for manually scaling things.
        /// </summary>
        float ScalingRatio { get; }

        /// <summary>
        /// Matrix to convert game coordinates to screen coordinates. Pass into Spritebatch.BeginDraw or this thing won't work
        /// </summary>
        /// <returns></returns>
        Matrix TransformationMatrix();

        /// <summary>
        /// Given a screen coordinate, convert it to the cooresponding game coord
        /// </summary>
        /// <param name="screenCoord"></param>
        /// <returns></returns>
        Vector2 ScreenToGameCoord(Vector2 screenCoord);

        void ResetViewport();
    }
}