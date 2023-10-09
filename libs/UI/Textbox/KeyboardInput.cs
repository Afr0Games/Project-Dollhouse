// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UI;

namespace UI.Textbox
{
    /// <summary>
    ///     All printable characters are returned using the CharPressed event and captured using the Game.Window.TextInput
    ///     event exposed by MonoGame.
    ///     Those automatically honor keyboard-layout and repeat-frequency of the keyboard for all platforms.
    ///     The flag filterSpecialCharactersFromCharPressed you may specify when calling Initialize tells the class to filter
    ///     those characters exposed in the SpecialCharacters char[] or not.
    ///     When running on a OpenGl-system those characters are not captured by the system. So the default for filtering them
    ///     out is for the sake of compatibility.
    ///     The repetition of the special characters as well as the arrow keys, etc, is handled by the class itself using the
    ///     values you pass it when calling Initialize.
    ///     The repetition is not done by sending the characters through the CharPressed event (since they may be not even
    ///     characters and some of them are omitted by the OpenGl platform), but through the KeyPressed event and the keys are
    ///     captured by getting the KeyboardState from MonoGame in the Update-method.
    ///     So if you want to capture those, use that one.
    ///     The KeyDown and KeyUp event are standard events being getting the KeyboardState from MonoGame in the Update-method.
    /// </summary>
    public static class KeyboardInput
    {
        public class CharacterEventArgs : EventArgs
        {
            public char Character { get; private set; }

            public CharacterEventArgs(char character)
            {
                Character = character;
            }
        }

        public class KeyEventArgs : EventArgs
        {
            public Keys KeyCode { get; private set; }

            public KeyEventArgs(Keys keyCode)
            {
                KeyCode = keyCode;
            }
        }

        public delegate void CharEnteredHandler(object sender, CharacterEventArgs e, KeyboardState ks);

        public delegate void KeyEventHandler(object sender, KeyEventArgs e, KeyboardState ks);

        public static readonly char[] SPECIAL_CHARACTERS = { '\a', '\b', '\n', '\r', '\f', '\t', '\v' };

        private static ScreenManager m_manager;

        public static event CharEnteredHandler CharPressed;
        public static event KeyEventHandler KeyPressed;
        public static event KeyEventHandler KeyDown;
        public static event KeyEventHandler KeyUp;

        private static KeyboardState prevKeyState;

        private static Keys? repChar;
        private static DateTime downSince = DateTime.Now;
        private static float timeUntilRepInMillis;
        private static int repsPerSec;
        private static DateTime lastRep = DateTime.Now;
        private static bool filterSpecialCharacters;

        public static void Initialize(ScreenManager manager, float timeUntilRepInMilliseconds, int repsPerSecond,
            bool filterSpecialCharactersFromCharPressed = true)
        {
            m_manager = manager;
            timeUntilRepInMillis = timeUntilRepInMilliseconds;
            repsPerSec = repsPerSecond;
            filterSpecialCharacters = filterSpecialCharactersFromCharPressed;
            m_manager.OnTextInput += TextEntered;
        }

        public static bool ShiftDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
            }
        }

        public static bool CtrlDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl);
            }
        }

        public static bool AltDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt);
            }
        }

        private static void TextEntered(object sender, TextInputEventArgs e)
        {
            if (CharPressed != null)
            {
                if (!filterSpecialCharacters || !SPECIAL_CHARACTERS.Contains(e.Character))
                {
                    CharPressed(null, new CharacterEventArgs(e.Character), Keyboard.GetState());
                }
            }
        }

        public static void Update()
        {
            KeyboardState keyState = Keyboard.GetState();

            foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
            {
                if (JustPressed(keyState, key))
                {
                    KeyDown?.Invoke(null, new KeyEventArgs(key), keyState);
                    if (KeyPressed != null)
                    {
                        downSince = DateTime.Now;
                        repChar = key;
                        KeyPressed(null, new KeyEventArgs(key), keyState);
                    }
                }
                else if (JustReleased(keyState, key))
                {
                    if (KeyUp != null)
                    {
                        if (repChar == key)
                        {
                            repChar = null;
                        }
                        KeyUp(null, new KeyEventArgs(key), keyState);
                    }
                }

                if (repChar != null && repChar == key && keyState.IsKeyDown(key))
                {
                    DateTime now = DateTime.Now;
                    TimeSpan downFor = now.Subtract(downSince);
                    if (downFor.CompareTo(TimeSpan.FromMilliseconds(timeUntilRepInMillis)) > 0)
                    {
                        // Should repeat since the wait time is over now.
                        TimeSpan repeatSince = now.Subtract(lastRep);
                        if (repeatSince.CompareTo(TimeSpan.FromMilliseconds(1000f / repsPerSec)) > 0)
                        {
                            // Time for another key-stroke.
                            if (KeyPressed != null)
                            {
                                lastRep = now;
                                KeyPressed(null, new KeyEventArgs(key), keyState);
                            }
                        }
                    }
                }
            }

            prevKeyState = keyState;
        }

        private static bool JustPressed(KeyboardState keyState, Keys key)
        {
            return keyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key);
        }

        private static bool JustReleased(KeyboardState keyState, Keys key)
        {
            return prevKeyState.IsKeyDown(key) && keyState.IsKeyUp(key);
        }

        public static void Dispose()
        {
            CharPressed = null;
            KeyDown = null;
            KeyPressed = null;
            KeyUp = null;
        }
    }
}