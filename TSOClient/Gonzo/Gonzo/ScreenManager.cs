/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using Shared;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo
{
    /// <summary>
    /// Manager responsible for updating and drawing UIScreen instances.
    /// </summary>
    public class ScreenManager
    {
        private GraphicsDevice m_Graphics;
        private List<UIScreen> m_Screens = new List<UIScreen>();
        private InputHelper m_Input;
        private SpriteFont[] m_Fonts;
        private Camera m_Camera;
        private Effect m_Shader; //TODO: Move this elsewhere.
        private SpriteBatch m_SBatch;

        public event EventHandler<TextInputEventArgs> OnTextInput;

        /// <summary>
        /// A graphicsdevice instance.
        /// </summary>
        public GraphicsDevice Device
        {
            get { return m_Graphics; }
        }

        /// <summary>
        /// Camera used for rendering stuff.
        /// </summary>
        public Camera RenderCamera
        {
            get { return m_Camera; }
        }

        /// <summary>
        /// Gets this ScreenManager's GraphicsDevice instance.
        /// </summary>
        public GraphicsDevice Graphics { get { return m_Graphics; } }

        /// <summary>
        /// The 9px by 9px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font9px { get { return m_Fonts[0]; } }

        /// <summary>
        /// The 10px by 10px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font10px { get { return m_Fonts[1]; } }

        /// <summary>
        /// The 10px by 10px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font11px { get { return m_Fonts[2]; } }

        /// <summary>
        /// The 12px by 12px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font12px { get { return m_Fonts[3]; } }

        /// <summary>
        /// The 14px by 14px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font14px { get { return m_Fonts[4]; } }

        /// <summary>
        /// The 16px by 16px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font16px { get { return m_Fonts[5]; } }

        public Effect HeadShader { get { return m_Shader; } set { m_Shader = value; } }

        /// <summary>
        /// Constructs a new ScreenManager instance.
        /// </summary>
        /// <param name="Input">An InputHelper instance, used for updating screens.</param>
        public ScreenManager(GraphicsDevice Graphics, SpriteFont[] Fonts, InputHelper Input)
        {
            m_Graphics = Graphics;
            m_SBatch = new SpriteBatch(Graphics);
            m_Camera = new Camera(Graphics);
            m_Input = Input;
            m_Fonts = Fonts;
        }

        /// <summary>
        /// This should be called by an instance of Microsoft.XNA.Game.Window
        /// whenever it receives text input. This method will call the 
        /// OnTextInput event.
        /// </summary>
        /// <param name="Sender">The object that invoked this event, sent by the callee of this method.</param>
        /// <param name="TInputEArgs">A TextInputEventArgs instance, sent by the callee of this method.</param>
        public void ReceivedTextInput(object Sender, TextInputEventArgs TInputEArgs)
        {
            OnTextInput?.Invoke(Sender, TInputEArgs);
        }

        /// <summary>
        /// Add a UIScreen instance to this ScreenManager instance.
        /// </summary>
        /// <param name="Screen">The screen to add.</param>
        public void AddScreen(UIScreen Screen)
        {
            m_Screens.Add(Screen);
        }

        /// <summary>
        /// Remove a UIScreen instance from this ScreenManager instance.
        /// </summary>
        /// <param name="Screen">The screen to remove.</param>
        public void RemoveScreen(UIScreen Screen)
        {
            m_Screens.Remove(Screen);

            if (m_Screens.Count > 0) //This screen had a Vitaboy overlay.
                m_Screens.Remove(m_Screens[0]);
        }

        public void Update(GameTime GTime)
        {
            for(int i = 0; i < m_Screens.Count; i++)
                m_Screens[i].Update(m_Input, GTime);
        }

        /// <summary>
        /// Draws 2D scenes.
        /// </summary>
        public void Draw()
        {
            for(int i = 0; i < m_Screens.Count; i++)
            {
                if (!m_Screens[i].IsVitaboyScreen)
                {
                    m_Screens[i].Draw();
                }
            }
        }

        /// <summary>
        /// Draws Vitaboy scenes (3D).
        /// </summary>
        public void Draw3D()
        {
            for (int i = 0; i < m_Screens.Count; i++)
            {
                if (m_Screens[i].IsVitaboyScreen)
                    m_Screens[i].Draw();
            }
        }
    }
}
