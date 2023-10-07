/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the UIParser library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Irony.Parsing;
using Microsoft.Xna.Framework;

namespace UIParser.Nodes
{
    /// <summary>
    /// The type of properties that
    /// can be set on a control.
    /// </summary>
    public enum ControlProperties
    {
        Position,
        Size
    }

    public class SetSharedPropsNode : UINode
    {
        public AssignmentBlockNode AssignmentBlock;

        public int? StringTable;
        public ArrayListNode ControlPosition;
        public Microsoft.Xna.Framework.Color Color;
        public Color TextColor;
        public Color TextColorSelected;
        public Color TextColorHighlighted;
        public Color TextColorDisabled;
        public Color BackColor;
        public Color CursorColor;
        public Color FrameColor;
        public int? Opaque;
        public int? Transparent;
        public int? Alignment;
        public string Image = "";
        public string Tooltip = "";
        public string Text = "";
        public int? Orientation;
        public ArrayListNode Size;
        public int? Font;
        public bool TextButton = false;

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes[1].AstNode != null)
                AssignmentBlock = (AssignmentBlockNode)nodes[1].AstNode;

            foreach (AssignmentNode ANode in AssignmentBlock.ChildNodes[0].ChildNodes)
            {
                switch (ANode.TypeOfAssignment)
                {
                    case AssignmentType.StringTableAssignment:
                        StringTable = ANode.NumberValue;
                        break;
                    case AssignmentType.PositionAssignment:
                        ControlPosition = ANode.Array;
                        break;
                    case AssignmentType.ColorAssignment:
                        Color = new Color(ANode.Array.Numbers[0], ANode.Array.Numbers[1], ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.TextColorAssignment:
                        TextColor = new Color(ANode.Array.Numbers[0], ANode.Array.Numbers[1], ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.TextColorSelectedAssignment:
                        TextColorSelected = new Color(ANode.Array.Numbers[0], ANode.Array.Numbers[1], ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.TextColorHighlightedAssignment:
                        TextColorHighlighted = new Color(ANode.Array.Numbers[0], ANode.Array.Numbers[1], ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.TextColorDisabledAssignment:
                        TextColorDisabled = new Color(ANode.Array.Numbers[0], ANode.Array.Numbers[1], ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.BackColorAssignment:
                        BackColor = new Color(ANode.Array.Numbers[0], ANode.Array.Numbers[1], ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.FrameColorAssignment:
                        FrameColor = new Color(ANode.Array.Numbers[0], ANode.Array.Numbers[1], ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.CursorAssignment:
                        CursorColor = new Color(ANode.Array.Numbers[0], ANode.Array.Numbers[1], ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.OpaqueAssignment:
                        Opaque = ANode.NumberValue;
                        break;
                    case AssignmentType.TransparentAssignment:
                        Transparent = ANode.NumberValue;
                        break;
                    case AssignmentType.AlignmentAssignment:
                        Alignment = ANode.NumberValue;
                        break;
                    case AssignmentType.ImageAssignment:
                        Image = ANode.StrValue;
                        break;
                    case AssignmentType.TooltipAssignment:
                        Tooltip = ANode.StrValue;
                        break;
                    case AssignmentType.TextAssignment:
                        Text = ANode.StrValue;
                        break;
                    case AssignmentType.SizeAssignment:
                        Size = ANode.Array;
                        break;
                    case AssignmentType.OrientationAssignment:
                        Orientation = ANode.NumberValue;
                        break;
                    case AssignmentType.FontAssignment:
                        Font = ANode.NumberValue;
                        break;
                    case AssignmentType.TextButtonAssignment:
                        TextButton = (ANode.NumberValue == 1 ? true : false);
                        break;
                }
            }

            AsString = "SetSharedProperties";
        }
    }
}
