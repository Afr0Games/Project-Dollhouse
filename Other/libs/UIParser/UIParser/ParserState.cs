using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Irony.Parsing;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Interpreter.Evaluator;

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
        public Color Color, BackColor, CursorColor;
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
    }
}
