using System;
using System.Collections.Generic;
using System.Text;
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

        public GraphicsDevice Device
        {
            get { return m_Graphics; }
        }

        /// <summary>
        /// Gets this ScreenManager's GraphicsDevice instance.
        /// </summary>
        public GraphicsDevice Graphics { get { return m_Graphics; } }

        /// <summary>
        /// The 10px by 10px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font10px { get { return m_Fonts[0]; } }

        /// <summary>
        /// The 12px by 12px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font12px { get { return m_Fonts[1]; } }

        /// <summary>
        /// The 14px by 14px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font14px { get { return m_Fonts[2]; } }

        /// <summary>
        /// The 16px by 16px font for this ScreenManager instance.
        /// </summary>
        public SpriteFont Font16px { get { return m_Fonts[3]; } }

        /// <summary>
        /// Constructs a new ScreenManager instance.
        /// </summary>
        /// <param name="Input">An InputHelper instance, used for updating screens.</param>
        public ScreenManager(GraphicsDevice Graphics, SpriteFont[] Fonts, InputHelper Input)
        {
            m_Graphics = Graphics;
            m_Input = Input;
            m_Fonts = Fonts;
        }

        /// <summary>
        /// Add a UIScreen instance to this ScreenManager instance.
        /// </summary>
        /// <param name="Screen"></param>
        public void AddScreen(UIScreen Screen)
        {
            m_Screens.Add(Screen);
        }

        public void Update()
        {
            for(int i = 0; i < m_Screens.Count; i++)
                m_Screens[i].Update(m_Input);
        }

        /// <summary>
        /// Draws 2D scenes.
        /// </summary>
        public void Draw()
        {
            for(int i = 0; i < m_Screens.Count; i++)
            {
                if(!m_Screens[i].IsVitaboyScreen)
                    m_Screens[i].Draw();
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
