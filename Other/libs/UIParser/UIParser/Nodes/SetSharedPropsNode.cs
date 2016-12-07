/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the UIParser library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Irony.Parsing;

namespace UIParser.Nodes
{
    public class SetSharedPropsNode : UINode
    {
        public AssignmentBlockNode AssignmentBlock;

        public int? StringTable;
        public ArrayListNode ControlPosition;
        public ArrayListNode Color;
        public ArrayListNode TextColor;
        public ArrayListNode TextColorSelected;
        public ArrayListNode TextColorHighlighted;
        public ArrayListNode TextColorDisabled;
        public ArrayListNode BackColor;
        public ArrayListNode CursorColor;
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
                        Color = ANode.Array;
                        break;
                    case AssignmentType.TextColorAssignment:
                        TextColor = ANode.Array;
                        break;
                    case AssignmentType.TextColorSelectedAssignment:
                        TextColorSelected = ANode.Array;
                        break;
                    case AssignmentType.TextColorHighlightedAssignment:
                        TextColorHighlighted = ANode.Array;
                        break;
                    case AssignmentType.TextColorDisabledAssignment:
                        TextColorDisabled = ANode.Array;
                        break;
                    case AssignmentType.BackColorAssignment:
                        BackColor = ANode.Array;
                        break;
                    case AssignmentType.CursorAssignment:
                        CursorColor = ANode.Array;
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
