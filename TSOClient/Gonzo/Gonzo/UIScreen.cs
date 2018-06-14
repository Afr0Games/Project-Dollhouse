/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gonzo.Elements;
using Gonzo.Dialogs;

namespace Gonzo
{
    /// <summary>
    /// The most basic component of Gonzo. Responsible for drawing, updating and creating UI components.
    /// UI components are created from UI scripts (*.uis).
    /// </summary>
    public class UIScreen
    {
        private ScreenManager m_Manager;

        /// <summary>
        /// Gets this screen's ScreenManager.
        /// </summary>
        public ScreenManager Manager { get { return m_Manager; } }

        protected TreeWalker m_Walker;
        protected ParseResult m_PResult = new ParseResult();

        protected SpriteBatch m_SBatch;
        
        public Vector2 Position;
        private Vector2 m_Size;

        public bool IsVitaboyScreen = false;

        /// <summary>
        /// This is the standard text color for TSO.
        /// </summary>
        public Color StandardTxtColor = new Color(255, 249, 157);

        /// <summary>
        /// 9px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font9px { get { return m_Manager.Font9px; } }

        /// <summary>
        /// 10px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font10px { get { return m_Manager.Font10px; } }

        /// <summary>
        /// 11px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font11px { get { return m_Manager.Font11px; } }

        /// <summary>
        /// 12px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font12px { get { return m_Manager.Font12px; } }

        /// <summary>
        /// 14px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font14px { get { return m_Manager.Font14px; } }

        /// <summary>
        /// 16px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font16px { get { return m_Manager.Font16px; } }

        /// <summary>
        /// Creates a new UIScreen instance.
        /// </summary>
        /// <param name="Manager">A ScreenManager instance.</param>
        /// <param name="Name">The name of this UIScreen instance.</param>
        /// <param name="SBatch">A SpriteBatch instance.</param>
        /// <param name="ScreenPosition">Position of this UIScreen instance.</param>
        /// <param name="ScreenSize">Size of this UIScreen instance.</param>
        /// <param name="UIScriptPath">Path of script (*.uis) from which to create UI elements.</param>
        public UIScreen(ScreenManager Manager, string Name, SpriteBatch SBatch, 
            Vector2 ScreenPosition, Vector2 ScreenSize, string UIScriptPath = "")
        {
            m_Manager = Manager;

            m_SBatch = SBatch;
            Position = ScreenPosition;
            m_Size = ScreenSize;

            m_Walker = new TreeWalker(this);

            if (UIScriptPath != "")
                m_Walker.Initialize(UIScriptPath, ref m_PResult);
        }

        /// <summary>
        /// A new UIElement was clicked on and is ready to receive keyboard input.
        /// </summary>
        public void OverrideFocus(UIElement Element)
        {
            if(Element.ListensToKeyboard)
            {
                foreach (KeyValuePair<string, UIElement> KVP in m_PResult.Elements)
                    KVP.Value.HasFocus = false;

                m_PResult.Elements[Element.Name].HasFocus = true;
            }
        }

        /// <summary>
        /// Tries to retrieve a string from this UIScreen's loaded strings.
        /// </summary>
        /// <param name="Name">Name of string to retrieve.</param>
        /// <returns>String with the given name. Throws exception if not found.</returns>
        public string GetString(string Name)
        {
            try
            {
                return m_PResult.Strings[Name];
            }
            catch (Exception)
            {
                throw new Exception("Couldn't find string: " + Name + " in UIScreen.cs");
            }
        }

        /// <summary>
        /// Gets a UIImage instance.
        /// </summary>
        /// <param name="Name">Name of the UIImage instance to get.</param>
        /// <param name="Copy">Should this element be deep copied?</param>
        /// <returns>A shallow or deep copy of the specified UIImage instance.</returns>
        public UIImage GetImage(string Name, bool Copy = false)
        {
            if (Copy)
            {
                UIImage Value = new UIImage((UIImage)m_PResult.Elements[Name]);
                return Value;
            }
            else
            {
                UIImage Value = (UIImage)m_PResult.Elements[Name];
                return Value;
            }
        }

        public virtual void Update(InputHelper Input, GameTime GTime)
        {
            Input.Update();

            foreach (KeyValuePair<string, UIElement> KVP in m_PResult.Elements)
                KVP.Value.Update(Input, GTime);
        }

        public virtual void Draw()
        {
            foreach (KeyValuePair<string, UIElement> KVP in m_PResult.Elements)
            {
                try
                {
                    if (KVP.Value.NeedsClipping == false)
                    {
                        if (KVP.Value is UIDialog || KVP.Value is WillWrightDiag || KVP.Value is ExitDialog)
                            KVP.Value.Draw(m_SBatch, UIElement.GetLayerDepth(LayerDepth.DialogLayer));
                        else if (KVP.Value is UIButton)
                            KVP.Value.Draw(m_SBatch, UIElement.GetLayerDepth(LayerDepth.ButtonLayer));
                        else if (KVP.Value is UIImage)
                            KVP.Value.Draw(m_SBatch, UIElement.GetLayerDepth(LayerDepth.ImageLayer));
                        else if (KVP.Value is UILabel)
                            KVP.Value.Draw(m_SBatch, UIElement.GetLayerDepth(LayerDepth.TextLayer));
                        else
                            KVP.Value.Draw(m_SBatch, UIElement.GetLayerDepth(LayerDepth.Default));
                    }
                }
                catch(Exception)
                {
                    continue;
                }
            }
        }
    }
}
