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
    public class AddFormatedTextNode : UINode
    {
        /// <summary>
        /// Defines the UIS identifier of this formatted text control. Required.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Defines the identifier of this listbox control. Required.
        /// </summary>
        public int ID;

        /// <summary>
        /// Position of this formatted text control. Required.
        /// </summary>
        public ArrayListNode FormatedTextPosition;

        /// <summary>
        /// Defines the dimensions of this formatted text control. Syntax: (width,height). Required.
        /// </summary>
        public ArrayListNode Size;

        /// <summary>
        /// Defines the opacity of this formatted text control. 1 means opaque, 0 means transparent.
        /// </summary>
        public int Opaque;

        /// <summary>
        /// Defines the line height of each row of this formatted text control in points.
        /// </summary>
        public int LineHeight;

        /// <summary>
        /// Defines the left edge offset of the text.
        /// </summary>
        public int? EdgeOffsetL;

        /// <summary>
        /// Defines the right edge offset of the text.
        /// </summary>
        public int? EdgeOffsetR;

        /// <summary>
        /// Defines the top edge offset of the text.
        /// </summary>
        public int? EdgeOffsetT;

        /// <summary>
        /// Defines the bottom edge offset of the text.
        /// </summary>
        public int? EdgeOffsetB;

        public AssignmentBlockNode AssignmentBlock { get; private set; }

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes[1].AstNode != null)
                Name = nodes[1].Token.Text;
            if (nodes[2].AstNode != null)
                AssignmentBlock = (AssignmentBlockNode)nodes[2].AstNode;

            foreach (AssignmentNode ANode in AssignmentBlock.ChildNodes[0].ChildNodes)
            {
                switch (ANode.TypeOfAssignment)
                {
                    case AssignmentType.IDAssignment:
                        ID = ANode.NumberValue;
                        break;
                    case AssignmentType.PositionAssignment:
                        FormatedTextPosition = ANode.Array;
                        break;
                    case AssignmentType.SizeAssignment:
                        Size = ANode.Array;
                        break;
                    case AssignmentType.OpaqueAssignment:
                        Opaque = ANode.NumberValue;
                        break;
                    case AssignmentType.LineHeightAssignment:
                        LineHeight = ANode.NumberValue;
                        break;
                    case AssignmentType.EdgeOffsetLAssignment:
                        EdgeOffsetL = ANode.NumberValue;
                        break;
                    case AssignmentType.EdgeOffsetRAssignment:
                        EdgeOffsetR = ANode.NumberValue;
                        break;
                    case AssignmentType.EdgeOffsetTAssignment:
                        EdgeOffsetT = ANode.NumberValue;
                        break;
                    case AssignmentType.EdgeOffsetBAssignment:
                        EdgeOffsetB = ANode.NumberValue;
                        break;
                }
            }

            AsString = "AddFormatedText";
        }
    }
}
