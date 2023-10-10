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

using System;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ResolutionBuddy
{
    internal class ResolutionAdapter : IResolution
    {
        #region Fields

        /// <summary>
        /// The title safe area for our virtual resolution
        /// </summary>
        /// <value>The title safe area.</value>
        private Rectangle _titleSafeArea;

        /// <summary>
        /// This will be a rectangle of the whole screen in our "virtual resolution"
        /// </summary>
        private Rectangle _screenArea;

        /// <summary>
        /// The actual screen resolution
        /// </summary>
        private Point _screenResolution = new Point(1280, 720);

        /// <summary>
        /// The screen rect we want for our game, and are going to fake
        /// </summary>
        private Point _virtualResolution = new Point(1280, 720);

        /// <summary>
        /// The scale matrix from the desired rect to the screen rect
        /// </summary>
        private Matrix _scaleMatrix;

        /// <summary>
        /// Scale matrix used to convert screen coords (mouse click, touch events) to game coords.
        /// </summary>
        private Matrix _screenMatrix;

        /// <summary>
        /// whether or not we want full screen 
        /// </summary>
        private bool _fullScreen;

        private bool _useDeviceResolution;

        /// <summary>
        /// whether or not the matrix needs to be recreated
        /// </summary>
        private bool _dirtyMatrix = true;

        private bool _letterBox;

        private Vector2 _pillarBox;

        /// <summary>
        /// The graphics device
        /// </summary>
        /// <value>The device.</value>
        private GraphicsDeviceManager Device { get; set; }

        #endregion //Fields

        #region Properties

        public Rectangle TitleSafeArea
        {
            get { return _titleSafeArea; }
        }

        public Rectangle ScreenArea
        {
            get { return _screenArea; }
        }

        public Matrix ScreenMatrix
        {
            get
            {
                return _screenMatrix;
            }
        }

        /// <summary>
        /// Get virtual Mode Aspect Ratio
        /// </summary>
        /// <returns>aspect ratio</returns>
        private float VirtualAspectRatio
        {
            get
            {
                return _virtualResolution.X / (float)_virtualResolution.Y;
            }
        }

        /// <summary>
        /// Get virtual Mode Aspect Ratio
        /// </summary>
        /// <returns>aspect ratio</returns>
        private float ScreenAspectRatio
        {
            get
            {
                return _screenResolution.X / (float)_screenResolution.Y;
            }
        }

        public Point VirtualResolution
        {
            get
            {
                return _virtualResolution;
            }
            set
            {
                SetVirtualResolution(value.X, value.Y);
            }
        }

        public Point ScreenResolution
        {
            get
            {
                return _screenResolution;
            }
            set
            {
                SetScreenResolution(value.X, value.Y, _fullScreen, _letterBox, _useDeviceResolution);
            }
        }

        public float ScalingRatio
        {
            get
            {
                float ScreenResX = (float)Convert.ToDouble(ScreenResolution.X);
                float VirtualResX = (float)Convert.ToDouble(VirtualResolution.X);
                return ScreenResX / VirtualResX;
            }
        }

        #endregion //Properties

        #region Methods

        #region Initialization

        /// <summary>
        /// default constructor for testing
        /// </summary>
        public ResolutionAdapter()
        {
        }

        /// <summary>
        /// Init the specified device.
        /// </summary>
        /// <param name="deviceMananger">Device.</param>
        public ResolutionAdapter(GraphicsDeviceManager deviceMananger)
        {
            Device = deviceMananger;
            _screenResolution.X = Device.PreferredBackBufferWidth;
            _screenResolution.Y = Device.PreferredBackBufferHeight;
        }

        /// <summary>
        /// The the resolution our game is designed to run in.
        /// </summary>
        /// <param name="Width">Width.</param>
        /// <param name="Height">Height.</param>
        public void SetVirtualResolution(int Width, int Height)
        {
            _virtualResolution = new Point(Width, Height);

            _screenArea = new Rectangle(0, 0, _virtualResolution.X, _virtualResolution.Y);

            //set up the title safe area
            _titleSafeArea.X = (int)(_virtualResolution.X / 20.0f);
            _titleSafeArea.Y = (int)(_virtualResolution.Y / 20.0f);
            _titleSafeArea.Width = (int)(_virtualResolution.X - (2.0f * TitleSafeArea.X));
            _titleSafeArea.Height = (int)(_virtualResolution.Y - (2.0f * TitleSafeArea.Y));

            _dirtyMatrix = true;
        }

        /// <summary>
        /// Sets the screen we are going to use for the screen
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="fullScreen">If set to <c>true</c> full screen.</param>
        public void SetScreenResolution(int width, int height, bool fullScreen, bool letterbox, bool? useDeviceResolution)
        {
            _screenResolution.X = width;
            _screenResolution.Y = height;
            _letterBox = letterbox;
            _useDeviceResolution = useDeviceResolution.HasValue ? useDeviceResolution.Value : fullScreen;

            _fullScreen = fullScreen;

            ApplyResolutionSettings();
        }

        protected virtual void ApplyResolutionSettings()
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (!_fullScreen && !_useDeviceResolution)
            {
                //Make sure the width isn't bigger than the screen
                if (_screenResolution.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    _screenResolution.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                }

                //Make sure the height isn't bigger than the screen
                if (_screenResolution.Y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
                {
                    _screenResolution.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate through the display modes supported by the adapter and check them against
                // the mode we want to set.
                bool bFound = false;
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == _screenResolution.X) && (dm.Height == _screenResolution.Y))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    _screenResolution.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    _screenResolution.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
            }

            //ok, found a good set of stuff... set the graphics device
            Device.PreferredBackBufferWidth = _screenResolution.X;
            Device.PreferredBackBufferHeight = _screenResolution.Y;
            Device.IsFullScreen = _fullScreen;
            Device.ApplyChanges();

            //Update the virtual resolution to match the aspect ratio of the new actual resolution
            if (!_letterBox)
            {
                UpdateVirtualResolution();
            }

            //we are gonna have to redo that scale matrix
            _dirtyMatrix = true;
        }

        protected void UpdateVirtualResolution()
        {
            if (ScreenAspectRatio < VirtualAspectRatio)
            {
                //the width needs to be pulled in to match the screen aspect ratio
                var width = ((_screenResolution.X * _virtualResolution.Y) / _screenResolution.Y);
                SetVirtualResolution(width, _virtualResolution.Y);
            }
            else if (ScreenAspectRatio > VirtualAspectRatio)
            {
                //the height needs to be pulled in to match the screen aspect ratio
                var height = ((_virtualResolution.X * _screenResolution.Y) / _screenResolution.X);
                SetVirtualResolution(_virtualResolution.X, height);
            }
        }

        #endregion Initialization

        /// <summary>
        /// Get the transformation matrix for when you call SpriteBatch.Begin
        /// To add this to a camera matrix, do CameraMatrix * TransformationMatrix
        /// </summary>
        /// <returns>The matrix.</returns>
        public Matrix TransformationMatrix()
        {
            if (_dirtyMatrix)
            {
                RecreateScaleMatrix(new Point(
                    Device.GraphicsDevice.Viewport.Width,
                    Device.GraphicsDevice.Viewport.Height));
            }

            return _scaleMatrix;
        }

        /// <summary>
        /// Given a screen coord, convert to game coordinate system.
        /// </summary>
        /// <param name="screenCoord"></param>
        /// <returns></returns>
        public Vector2 ScreenToGameCoord(Vector2 screenCoord)
        {
            return MatrixExt.Multiply(_screenMatrix, screenCoord);
        }

        protected virtual void RecreateScaleMatrix(Point vp)
        {
            _dirtyMatrix = false;
            _scaleMatrix = Matrix.CreateScale(
                ((float)vp.X / (float)_virtualResolution.X),
                ((float)vp.Y / (float)_virtualResolution.Y),
                1.0f);

            //trasnlate by the pillar box to account for the border on top/bottom/left/right
            var translation = Matrix.CreateTranslation(_pillarBox.X, _pillarBox.Y, 0f);

            _screenMatrix = Matrix.Multiply(translation, Matrix.CreateScale(
                ((float)_virtualResolution.X / (float)vp.X),
                ((float)_virtualResolution.Y / (float)vp.Y),
                1.0f));
        }

        public void ResetViewport()
        {
            // figure out the largest area that fits in this resolution at the desired aspect ratio
            int width = Device.GraphicsDevice.Viewport.Width;
            var height = (int)(width / VirtualAspectRatio + .5f);
            bool changed = false;

            if (height != Device.GraphicsDevice.Viewport.Height || width != Device.GraphicsDevice.Viewport.Width)
            {
                if (height > Device.GraphicsDevice.Viewport.Height)
                {
                    // PillarBox
                    height = Device.PreferredBackBufferHeight;
                    width = (int)(height * VirtualAspectRatio + .5f);
                    changed = true;
                }

                // set up the new viewport centered in the backbuffer
                var viewport = new Viewport()
                {
                    X = (Device.PreferredBackBufferWidth / 2) - (width / 2),
                    Y = (Device.PreferredBackBufferHeight / 2) - (height / 2),
                    Width = width,
                    Height = height,
                    MinDepth = 0,
                    MaxDepth = 1
                };

                _pillarBox = new Vector2(-viewport.X, -viewport.Y);

                if (changed)
                {
                    _dirtyMatrix = true;
                }

                Device.GraphicsDevice.Viewport = viewport;
            }
        }

        #endregion //Methods
    }
}