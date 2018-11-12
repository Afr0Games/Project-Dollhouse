/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the UIParser library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Microsoft.Xna.Framework;

namespace UIParser
{
    /// <summary>
    /// Object used to keep track of state when walking an AST tree (see UIScreen.WalkTree in Gonzo).
    /// </summary>
    public class ParserState
    {
        //Are we in a SetSharedProperties group?
        public bool InSharedPropertiesGroup = false;
        public int[] Position = new int[2];
        //Text colors.
        public Color TextColor, TextColorSelected, TextColorHighlighted, TextColorDisabled;
        public Color Color, BackColor, CursorColor, FrameColor;
        //Text properties.
        public bool TextButton = false;
        public bool IsOpaque = false, IsTransparent = false;
        public int Alignment;
        //Image to apply to a bunch of controls/images.
        public string Image = "";
        //Name of tooltip for a control.
        public string Tooltip = "";
        public int CurrentStringTable = -1;
        public string Caption = "";
        //Size of a control.
        public Vector2 Size;
        public int Orientation;
        //Which font is used by a text edit control or label.
        public int Font;
        //Wether or not a font is opaque (I.E non-translucent).
        public int Opaque;
        //Images for navigating a browser control.
        public string LeftArrowImage, RightArrowImage;
    }
}
