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
    public class AddSliderNode : UINode
    {
        /// <summary>
        /// Defines the UIS identifier of this slider control. Required.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Defines the UIS identifier of this slider control. Required.
        /// </summary>
        public int ID;

        /// <summary>
        /// Defines a UIS image resource to associate with this slider control. Required.
        /// </summary>
        public string Image = "";

        /// <summary>
        /// Defines the position of this slider control in x,y coordinates. Syntax: (x,y)
        /// </summary>
        public ArrayListNode SliderPosition;

        /// <summary>
        /// Defines the value associated with the small end of this slider control.
        /// </summary>
        public int? MinimumValue;

        /// <summary>
        /// Defines the value associated with the big end of this slider control.
        /// </summary>
        public int? MaximumValue;

        /// <summary>
        /// Defines the dimensions of this slider control. Syntax: (width,height). Required.
        /// </summary>
        public ArrayListNode Size;

        /// <summary>
        /// Defines the orientation of this slider control. 0 means horizontal and 1 means vertical.
        /// </summary>
        public int Orientation;

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
                    case AssignmentType.PositionAssignment:
                        SliderPosition = ANode.Array;
                        break;
                    case AssignmentType.ImageAssignment:
                        Image = ANode.StrValue;
                        break;
                    case AssignmentType.IDAssignment:
                        ID = ANode.NumberValue;
                        break;
                    case AssignmentType.SizeAssignment:
                        Size = ANode.Array;
                        break;
                    case AssignmentType.MinValueAssignment:
                        MinimumValue = ANode.NumberValue;
                        break;
                    case AssignmentType.MaxValueAssignment:
                        MaximumValue = ANode.NumberValue;
                        break;
                    case AssignmentType.OrientationAssignment:
                        Orientation = ANode.NumberValue;
                        break;
                }
            }

            AsString = "AddSlider";
        }
    }
}
