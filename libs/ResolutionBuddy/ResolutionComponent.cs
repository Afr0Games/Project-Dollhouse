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
using System;

namespace ResolutionBuddy
{
    public class ResolutionComponent : DrawableGameComponent, IResolution
    {
        #region Fields

        private Point _virtualResolution;

        private Point _screenResolution;

        private bool _fullscreen;

        private bool _letterbox;

        private readonly GraphicsDeviceManager _graphics;

        #endregion //Fields

        #region Properties

        private ResolutionAdapter ResolutionAdapter { get; set; }

        public Rectangle TitleSafeArea
        {
            get
            {
                return Resolution.TitleSafeArea;
            }
        }

        public Rectangle ScreenArea
        {
            get
            {
                return Resolution.ScreenArea;
            }
        }

        public Matrix ScreenMatrix
        {
            get
            {
                return Resolution.ScreenMatrix;
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
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change VirtualResolution after the ResolutionComponent has been initialized");
                }
                _virtualResolution = value;
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
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change ScreenResolution after the ResolutionComponent has been initialized");
                }
                _screenResolution = value;
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

        public bool FullScreen
        {
            get
            {
                return _fullscreen;
            }
            set
            {
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change FullScreen after the ResolutionComponent has been initialized");
                }
                _fullscreen = value;
            }
        }

        bool? _useDeviceResolution;
        public bool? UseDeviceResolution
        {
            get
            {
                return _useDeviceResolution;
            }
            set
            {
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change UseDeviceResolution after the ResolutionComponent has been initialized");
                }
                _useDeviceResolution = value;
            }
        }

        public bool LetterBox
        {
            get
            {
                return _letterbox;
            }
            set
            {
                if (null != ResolutionAdapter)
                {
                    throw new Exception("Can't change LetterBox after the ResolutionComponent has been initialized");
                }
                _letterbox = value;
            }
        }

        #endregion //Properties

        #region Methods

        /// <summary>
        /// Create the resolution component!
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphics"></param>
        /// <param name="virtualResolution">The dimensions of the desired virtual resolution</param>
        /// <param name="screenResolution">The desired screen dimensions</param>
        /// <param name="fullscreen">Whether or not to fullscreen the game</param>
        /// <param name="letterbox">Whether to add letterboxing, or change the virtual resoltuion to match aspect ratio of screen resolution.</param>
        public ResolutionComponent(Game game, GraphicsDeviceManager graphics, Point virtualResolution, Point screenResolution, bool fullscreen, bool letterbox, bool? useDeviceResolution) : base(game)
        {
            _graphics = graphics;
            VirtualResolution = virtualResolution;
            ScreenResolution = screenResolution;
            _fullscreen = fullscreen;
            _letterbox = letterbox;
            _useDeviceResolution = useDeviceResolution;

            //Add to the game components
            Game.Components.Add(this);

            //add to the game services
            Game.Services.AddService<IResolution>(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            //initialize the ResolutionAdapter object
            ResolutionAdapter = new ResolutionAdapter(_graphics);
            ResolutionAdapter.SetVirtualResolution(VirtualResolution.X, VirtualResolution.Y);
            ResolutionAdapter.SetScreenResolution(ScreenResolution.X, ScreenResolution.Y, _fullscreen, _letterbox, _useDeviceResolution);
            ResolutionAdapter.ResetViewport();

            //initialize the Resolution singleton
            Resolution.Init(ResolutionAdapter);
        }

        public Vector2 ScreenToGameCoord(Vector2 screenCoord)
        {
            return ResolutionAdapter.ScreenToGameCoord(screenCoord);
        }

        public Matrix TransformationMatrix()
        {
            return ResolutionAdapter.TransformationMatrix();
        }

        public override void Draw(GameTime gameTime)
        {
            //Calculate Proper Viewport according to Aspect Ratio
            ResolutionAdapter.ResetViewport();

            base.Draw(gameTime);
        }

        public void ResetViewport()
        {
            //Calculate Proper Viewport according to Aspect Ratio
            ResolutionAdapter.ResetViewport();
        }

        #endregion //Methods
    }
}