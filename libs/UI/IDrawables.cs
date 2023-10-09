using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UI
{
    /// <summary>
    /// An interface for UIElements that need to implement only the most basic draw call.
    /// Implemented by UIButton, UIControl, UILabel and UIProgressBar.
    /// </summary>
    public interface IBasicDrawable
    {
        /// <summary>
        /// Draws this UIElement.
        /// </summary>
        /// <param name="SBatch">A SpriteBatch instance to draw with.</param>
        void Draw(SpriteBatch SBatch);
    }
}
