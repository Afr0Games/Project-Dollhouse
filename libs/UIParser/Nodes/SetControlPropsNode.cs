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
    public class SetControlPropsNode : UINode
    {
        public AssignmentBlockNode AssignmentBlock;

        public string Control { get; private set; }
        public string Image { get; private set; }
        public Vector2? PositionAssignment { get; private set; }
        public Vector2? Size;
        public Vector2? ThumbSize;
        public ArrayListNode ThumbMargins;
        public Vector2 ThumbImageSize;
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
                        PositionAssignment = new Vector2(ANode.Array.Numbers[0], ANode.Array.Numbers[1]);
                        break;
                    case AssignmentType.SizeAssignment:
                        Size = new Vector2(ANode.Array.Numbers[0], ANode.Array.Numbers[1]);
                        break;
                    case AssignmentType.ThumbSizeAssignment:
                        ThumbSize = new Vector2(ANode.Array.Numbers[0], ANode.Array.Numbers[1]);
                        break;
                    case AssignmentType.ThumbMarginsAssignment:
                        ThumbMargins = ANode.Array;
                        break;
                    case AssignmentType.ThumbImageSizeAssignment:
                        ThumbImageSize = new Vector2(ANode.Array.Numbers[0], ANode.Array.Numbers[1]);
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
