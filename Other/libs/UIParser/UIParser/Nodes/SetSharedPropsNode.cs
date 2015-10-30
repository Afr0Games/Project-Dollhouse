using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class SetSharedPropsNode : UINode
    {
        public AssignmentBlockNode AssignmentBlock;

        public int? StringTable;
        public ArrayListNode ControlPosition;
        public ArrayListNode Color;
        public ArrayListNode BackColor;
        public ArrayListNode CursorColor;
        public int? Opaque;
        public int? Transparent;
        public int? Alignment;
        public string Image = "";
        public string Tooltip = "";
        public string Text = "";

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
                }
            }

            AsString = "SetSharedProperties";
        }
    }
}
