/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ResolutionBuddy;

namespace Gonzo
{
    /// <summary>
    /// an enum of all available mouse buttons.
    /// </summary>
    public enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton,
        ExtraButton1,
        ExtraButton2
    }

    public class InputHelper
    {
        private GamePadState _lastGamepadState;
        private GamePadState _currentGamepadState;
#if (!XBOX)
        private KeyboardState _lastKeyboardState;
        private KeyboardState _currentKeyboardState;
        private MouseState _lastMouseState;
        private MouseState _currentMouseState;
#endif
        private PlayerIndex _index = PlayerIndex.One;
        private bool refreshData = false;

        /// <summary>
        /// The current input language used by the user on his/her OS. Not sure if this is multi platform?
        /// </summary>
        public System.Windows.Forms.InputLanguage InputRegion = System.Windows.Forms.InputLanguage.CurrentInputLanguage;

        /// <summary>
        /// Fetches the latest input states.
        /// </summary>
        public void Update()
        {
            if (!refreshData)
                refreshData = true;
            if (_lastGamepadState == null && _currentGamepadState == null)
            {
                _lastGamepadState = _currentGamepadState = GamePad.GetState(_index);
            }
            else
            {
                _lastGamepadState = _currentGamepadState;
                _currentGamepadState = GamePad.GetState(_index);
            }
#if (!XBOX)
            if (_lastKeyboardState == null && _currentKeyboardState == null)
            {
                _lastKeyboardState = _currentKeyboardState = Keyboard.GetState();
            }
            else
            {
                _lastKeyboardState = _currentKeyboardState;
                _currentKeyboardState = Keyboard.GetState();
            }
            if (_lastMouseState == null && _currentMouseState == null)
            {
                _lastMouseState = _currentMouseState = Mouse.GetState();
            }
            else
            {
                _lastMouseState = _currentMouseState;
                _currentMouseState = Mouse.GetState();
            }
#endif
        }

        /// <summary>
        /// The previous state of the gamepad. 
        /// Exposed only for convenience.
        /// </summary>
        public GamePadState LastGamepadState
        {
            get { return _lastGamepadState; }
        }
        /// <summary>
        /// the current state of the gamepad.
        /// Exposed only for convenience.
        /// </summary>
        public GamePadState CurrentGamepadState
        {
            get { return _currentGamepadState; }
        }
        /// <summary>
        /// the index that is used to poll the gamepad. 
        /// </summary>
        public PlayerIndex Index
        {
            get { return _index; }
            set
            {
                _index = value;
                if (refreshData)
                {
                    Update();
                    Update();
                }
            }
        }
#if (!XBOX)
        /// <summary>
        /// The previous keyboard state.
        /// Exposed only for convenience.
        /// </summary>
        public KeyboardState LastKeyboardState
        {
            get { return _lastKeyboardState; }
        }
        /// <summary>
        /// The current state of the keyboard.
        /// Exposed only for convenience.
        /// </summary>
        public KeyboardState CurrentKeyboardState
        {
            get { return _currentKeyboardState; }
        }
        /// <summary>
        /// The previous mouse state.
        /// Exposed only for convenience.
        /// </summary>
        public MouseState LastMouseState
        {
            get { return _lastMouseState; }
        }
        /// <summary>
        /// The current state of the mouse.
        /// Exposed only for convenience.
        /// </summary>
        public MouseState CurrentMouseState
        {
            get { return _currentMouseState; }
        }
#endif
        /// <summary>
        /// The current position of the left stick. 
        /// Y is automatically reversed for you.
        /// </summary>
        public Vector2 LeftStickPosition
        {
            get
            {
                return new Vector2(
                    _currentGamepadState.ThumbSticks.Left.X,
                    -CurrentGamepadState.ThumbSticks.Left.Y);
            }
        }
        /// <summary>
        /// The current position of the right stick.
        /// Y is automatically reversed for you.
        /// </summary>
        public Vector2 RightStickPosition
        {
            get
            {
                return new Vector2(
                    _currentGamepadState.ThumbSticks.Right.X,
                    -_currentGamepadState.ThumbSticks.Right.Y);
            }
        }
        /// <summary>
        /// The current velocity of the left stick.
        /// Y is automatically reversed for you.
        /// expressed as: 
        /// current stick position - last stick position.
        /// </summary>
        public Vector2 LeftStickVelocity
        {
            get
            {
                Vector2 temp =
                    _currentGamepadState.ThumbSticks.Left -
                    _lastGamepadState.ThumbSticks.Left;
                return new Vector2(temp.X, -temp.Y);
            }
        }
        /// <summary>
        /// The current velocity of the right stick.
        /// Y is automatically reversed for you.
        /// expressed as: 
        /// current stick position - last stick position.
        /// </summary>
        public Vector2 RightStickVelocity
        {
            get
            {
                Vector2 temp =
                    _currentGamepadState.ThumbSticks.Right -
                    _lastGamepadState.ThumbSticks.Right;
                return new Vector2(temp.X, -temp.Y);
            }
        }
        /// <summary>
        /// the current position of the left trigger.
        /// </summary>
        public float LeftTriggerPosition
        {
            get { return _currentGamepadState.Triggers.Left; }
        }
        /// <summary>
        /// the current position of the right trigger.
        /// </summary>
        public float RightTriggerPosition
        {
            get { return _currentGamepadState.Triggers.Right; }
        }
        /// <summary>
        /// the velocity of the left trigger.
        /// expressed as: 
        /// current trigger position - last trigger position.
        /// </summary>
        public float LeftTriggerVelocity
        {
            get
            {
                return
                    _currentGamepadState.Triggers.Left -
                    _lastGamepadState.Triggers.Left;
            }
        }
        /// <summary>
        /// the velocity of the right trigger.
        /// expressed as: 
        /// current trigger position - last trigger position.
        /// </summary>
        public float RightTriggerVelocity
        {
            get
            {
                return _currentGamepadState.Triggers.Right -
                    _lastGamepadState.Triggers.Right;
            }
        }
#if (!XBOX)
        /// <summary>
        /// the current mouse position.
        /// </summary>
        public Vector2 MousePosition
        {
            get
            {
                //From: http://www.discussiongenerator.com/2012/09/15/resolution-independent-2d-rendering-in-xna-4/
                /*Vector2 virtualViewPort = new Vector2(Resolution.VirtualViewportX, Resolution.VirtualViewportY);
                return Vector2.Transform(new Vector2(_currentMouseState.X, _currentMouseState.Y) - virtualViewPort, Matrix.Invert(Resolution.getTransformationMatrix()));*/
                return Resolution.ScreenToGameCoord(new Vector2(_currentMouseState.X, _currentMouseState.Y));
            }
        }

        /// <summary>
        /// the current mouse velocity.
        /// Expressed as: 
        /// current mouse position - last mouse position.
        /// </summary>
        public Vector2 MouseVelocity
        {
            get
            {
                return (
                    new Vector2(_currentMouseState.X, _currentMouseState.Y) -
                    new Vector2(_lastMouseState.X, _lastMouseState.Y)
                    );
            }
        }
        /// <summary>
        /// the current mouse scroll wheel position.
        /// See the Mouse's ScrollWheel property for details.
        /// </summary>
        public float MouseScrollWheelPosition
        {
            get
            {
                return _currentMouseState.ScrollWheelValue;
            }
        }
        /// <summary>
        /// the mouse scroll wheel velocity.
        /// Expressed as:
        /// current scroll wheel position - 
        /// the last scroll wheel position.
        /// </summary>
        public float MouseScrollWheelVelocity
        {
            get
            {
                return (_currentMouseState.ScrollWheelValue - _lastMouseState.ScrollWheelValue);
            }
        }
#endif
        /// <summary>
        /// Used for debug purposes.
        /// Indicates if the user wants to exit immediately.
        /// </summary>
        public bool ExitRequested
        {
#if (!XBOX)
            get
            {
                return (
                    (IsCurPress(Buttons.Start) &&
                    IsCurPress(Buttons.Back)) ||
                    IsCurPress(Keys.Escape));
            }
#else
            get { return (IsCurPress(Buttons.Start) && IsCurPress(Buttons.Back)); }
#endif
        }
        /// <summary>
        /// Checks if the requested button is a new press.
        /// </summary>
        /// <param name="button">
        /// The button to check.
        /// </param>
        /// <returns>
        /// a bool indicating whether the selected button is being 
        /// pressed in the current state but not the last state.
        /// </returns>
        public bool IsNewPress(Buttons button)
        {
            return (
                _lastGamepadState.IsButtonUp(button) &&
                _currentGamepadState.IsButtonDown(button));
        }
        /// <summary>
        /// Checks if the requested button is a current press.
        /// </summary>
        /// <param name="button">
        /// the button to check.
        /// </param>
        /// <returns>
        /// a bool indicating whether the selected button is being 
        /// pressed in the current state and in the last state.
        /// </returns>
        public bool IsCurPress(Buttons button)
        {
            return (
                _lastGamepadState.IsButtonDown(button) &&
                _currentGamepadState.IsButtonDown(button));
        }
        /// <summary>
        /// Checks if the requested button is an old press.
        /// </summary>
        /// <param name="button">
        /// the button to check.
        /// </param>
        /// <returns>
        /// a bool indicating whether the selected button is not being
        /// pressed in the current state and is being pressed in the last state.
        /// </returns>
        public bool IsOldPress(Buttons button)
        {
            return (
                _lastGamepadState.IsButtonDown(button) &&
                _currentGamepadState.IsButtonUp(button));
        }
#if (!XBOX)
        /// <summary>
        /// Checks if the requested key is a new press.
        /// </summary>
        /// <param name="key">
        /// the key to check.
        /// </param>
        /// <returns>
        /// a bool that indicates whether the selected key is being 
        /// pressed in the current state and not in the last state.
        /// </returns>
        public bool IsNewPress(Keys key)
        {
            return (
                _lastKeyboardState.IsKeyUp(key) &&
                _currentKeyboardState.IsKeyDown(key));
        }
        /// <summary>
        /// Checks if the requested key is a current press.
        /// </summary>
        /// <param name="key">
        /// the key to check.
        /// </param>
        /// <returns>
        /// a bool that indicates whether the selected key is being 
        /// pressed in the current state and in the last state.
        /// </returns>
        public bool IsCurPress(Keys key)
        {
            return (
                _lastKeyboardState.IsKeyDown(key) &&
                _currentKeyboardState.IsKeyDown(key));
        }
        /// <summary>
        /// Checks if the requested button is an old press.
        /// </summary>
        /// <param name="key">
        /// the key to check.
        /// </param>
        /// <returns>
        /// a bool indicating whether the selectde button is not being
        /// pressed in the current state and being pressed in the last state.
        /// </returns>
        public bool IsOldPress(Keys key)
        {
            return (
                _lastKeyboardState.IsKeyDown(key) &&
                _currentKeyboardState.IsKeyUp(key));
        }
        /// <summary>
        /// Checks if the requested mosue button is a new press.
        /// </summary>
        /// <param name="button">
        /// teh mouse button to check.
        /// </param>
        /// <returns>
        /// a bool indicating whether the selected mouse button is being
        /// pressed in the current state but not in the last state.
        /// </returns>
        public bool IsNewPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (
                        _lastMouseState.LeftButton == ButtonState.Released &&
                        _currentMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (
                        _lastMouseState.MiddleButton == ButtonState.Released &&
                        _currentMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (
                        _lastMouseState.RightButton == ButtonState.Released &&
                        _currentMouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.ExtraButton1:
                    return (
                        _lastMouseState.XButton1 == ButtonState.Released &&
                        _currentMouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.ExtraButton2:
                    return (
                        _lastMouseState.XButton2 == ButtonState.Released &&
                        _currentMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }
        /// <summary>
        /// Checks if the requested mosue button is a current press.
        /// </summary>
        /// <param name="button">
        /// the mouse button to be checked.
        /// </param>
        /// <returns>
        /// a bool indicating whether the selected mouse button is being 
        /// pressed in the current state and in the last state.
        /// </returns>
        public bool IsCurPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (
                        _lastMouseState.LeftButton == ButtonState.Pressed &&
                        _currentMouseState.LeftButton == ButtonState.Pressed);
                case MouseButtons.MiddleButton:
                    return (
                        _lastMouseState.MiddleButton == ButtonState.Pressed &&
                        _currentMouseState.MiddleButton == ButtonState.Pressed);
                case MouseButtons.RightButton:
                    return (
                        _lastMouseState.RightButton == ButtonState.Pressed &&
                        _currentMouseState.RightButton == ButtonState.Pressed);
                case MouseButtons.ExtraButton1:
                    return (
                        _lastMouseState.XButton1 == ButtonState.Pressed &&
                        _currentMouseState.XButton1 == ButtonState.Pressed);
                case MouseButtons.ExtraButton2:
                    return (
                        _lastMouseState.XButton2 == ButtonState.Pressed &&
                        _currentMouseState.XButton2 == ButtonState.Pressed);
                default:
                    return false;
            }
        }
        /// <summary>
        /// Checks if the requested mosue button is an old press.
        /// </summary>
        /// <param name="button">
        /// the mouse button to check.
        /// </param>
        /// <returns>
        /// a bool indicating whether the selected mouse button is not being 
        /// pressed in the current state and is being pressed in the old state.
        /// </returns>
        public bool IsOldPress(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.LeftButton:
                    return (
                        _lastMouseState.LeftButton == ButtonState.Pressed &&
                        _currentMouseState.LeftButton == ButtonState.Released);
                case MouseButtons.MiddleButton:
                    return (
                        _lastMouseState.MiddleButton == ButtonState.Pressed &&
                        _currentMouseState.MiddleButton == ButtonState.Released);
                case MouseButtons.RightButton:
                    return (
                        _lastMouseState.RightButton == ButtonState.Pressed &&
                        _currentMouseState.RightButton == ButtonState.Released);
                case MouseButtons.ExtraButton1:
                    return (
                        _lastMouseState.XButton1 == ButtonState.Pressed &&
                        _currentMouseState.XButton1 == ButtonState.Released);
                case MouseButtons.ExtraButton2:
                    return (
                        _lastMouseState.XButton2 == ButtonState.Pressed &&
                        _currentMouseState.XButton2 == ButtonState.Released);
                default:
                    return false;
            }
        }
#endif
    }
}