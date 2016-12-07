/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo.Elements
{
    public class WillWrightDiag : UIDialog
    {
        private UIImage m_WillWrightImg;

        public WillWrightDiag(UIImage Img, UIScreen Screen, Vector2 Position) : base(Screen, Position, true, true, true)
        {
            m_WillWrightImg = Img;
            m_WillWrightImg.Position = Position;
            Image.SetSize(m_WillWrightImg.Texture.Width + 50, m_WillWrightImg.Texture.Height + 55);
            CenterAround(m_WillWrightImg, -22, -42);
        }

        public override void Update(InputHelper Helper, GameTime GTime)
        {
            if (IsDrawn)
            {
                if (m_DoDrag)
                    m_WillWrightImg.Position = Position - new Vector2(-22, -42);
            }

            base.Update(Helper, GTime);
        }

        public override void Draw(SpriteBatch SBatch, float? LayerDepth)
        {
            float Depth;
            if (LayerDepth != null)
                Depth = (float)LayerDepth;
            else
                Depth = 0.10f;

            if (IsDrawn)
                SBatch.Draw(m_WillWrightImg.Texture, m_WillWrightImg.Position, null, null, new Vector2(0.0f, 0.0f), 0.0f, null, 
                    Color.White, SpriteEffects.None, Depth);

            base.Draw(SBatch, LayerDepth);
        }
    }
}
