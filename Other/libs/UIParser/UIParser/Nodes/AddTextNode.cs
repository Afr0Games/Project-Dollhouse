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
    public enum TextAlignment
    {
        Left_Top = 0,
        Left_Center = 1,
        Center_Top = 2,
        Center_Center = 3,
        Right_Top = 4,
        Right_Center = 5
    }

    public class AddTextNode : UINode
    {
        /// <summary>
        /// Defines the UIS identifier of this text control.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Defines the identifier of this text control. Required.
        /// </summary>
        public int ID;

        /// <summary>
        /// Defines the position of this text control in x,y coordinates. Syntax: (x,y). Required.
        /// </summary>
        public ArrayListNode TextPosition;

        /// <summary>
        /// Defines the dimensions of this text control. Syntax: (width,height). Required.
        /// </summary>
        public ArrayListNode Size;

        /// <summary>
        /// Defines a UIS string resource to use as text.
        /// </summary>
        public string Text = "";

        /// <summary>
        /// Defines the font size of this text control in points.
        /// </summary>
        public int? Font;

        /// <summary>
        /// Defines whether or not the text in this text control is initially highlted. 1=Yes, 0=No. Default is no.
        /// </summary>
        public int? Highlighted;
        
        /// <summary>
        /// Required. Defines the font color of this text control. 
        /// </summary>
        public ArrayListNode Color;

        /// <summary>
        /// Required. Defines the alignment of this text control.
        /// </summary>
        public TextAlignment Alignment;

        /// <summary>
        /// Required. Defines the opacity of this text control. 1 is opaque and 0 is transparent.
        /// </summary>
        public int? Opaque;

        /// <summary>
        /// Defines whether or not text wraps on to new lines. 1=Yes, 0=No. Default is no.
        /// </summary>
        public int? Wrapped;

        /// <summary>
        /// Defines whether or not this text should be fit into its bounding box. 1=Yes, 0=No. Default is no.
        /// </summary>
        public int? FitText;
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
                        TextPosition = ANode.Array;
                        break;
                    case AssignmentType.SizeAssignment:
                        Size = ANode.Array;
                        break;
                    case AssignmentType.ColorAssignment:
                        Color = ANode.Array;
                        break;
                    case AssignmentType.TextAssignment:
                        Text = ANode.StrValue;
                        break;
                    case AssignmentType.AlignmentAssignment:
                        Alignment = (TextAlignment)ANode.NumberValue;
                        break;
                    case AssignmentType.FontAssignment:
                        Font = ANode.NumberValue;
                        break;
                    case AssignmentType.OpaqueAssignment:
                        Opaque = ANode.NumberValue;
                        break;
                }
            }

            AsString = "AddText";
        }
    }
}
