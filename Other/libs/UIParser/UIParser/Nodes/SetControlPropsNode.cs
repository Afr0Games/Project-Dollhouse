using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class SetControlPropsNode : UINode
    {
        public AssignmentBlockNode AssignmentBlock;

        public string Control { get; private set; }
        public string Image { get; private set; }
        public int[] PositionAssignment { get; private set; }
        public ArrayListNode Size;
        public ArrayListNode ThumbSize;
        public ArrayListNode ThumbMargins;
        public ArrayListNode ThumbImageSize;
        public ArrayListNode ThumbImageOffsets;
        public string ThumbButtonImage;
        public List<AssignmentNode> Assignments = new List<AssignmentNode>();

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            Control = nodes[1].Token.Text;
            Image = "";

            if (nodes[2].AstNode != null)
                AssignmentBlock = (AssignmentBlockNode)nodes[2].AstNode;

            foreach(AssignmentNode ANode in AssignmentBlock.ChildNodes[0].ChildNodes)
            {
                switch(ANode.TypeOfAssignment)
                {
                    case AssignmentType.ImageAssignment:
                        Image = ANode.StrValue;
                        break;
                    case AssignmentType.PositionAssignment:
                        PositionAssignment = new int[2];
                        PositionAssignment[0] = ANode.Array.Numbers[0];
                        PositionAssignment[1] = ANode.Array.Numbers[1];
                        break;
                    case AssignmentType.SizeAssignment:
                        Size = ANode.Array;
                        break;
                    case AssignmentType.ThumbSizeAssignment:
                        ThumbSize = ANode.Array;
                        break;
                    case AssignmentType.ThumbMarginsAssignment:
                        ThumbMargins = ANode.Array;
                        break;
                    case AssignmentType.ThumbImageSizeAssignment:
                        ThumbImageSize = ANode.Array;
                        break;
                    case AssignmentType.ThumbImageOffsetsAssignment:
                        ThumbImageOffsets = ANode.Array;
                        break;
                    case AssignmentType.ThumbButtonImageAssignment:
                        ThumbButtonImage = ANode.StrValue;
                        break;
                }
            }

            AsString = "SetControlProperties";
        }
    }
}
