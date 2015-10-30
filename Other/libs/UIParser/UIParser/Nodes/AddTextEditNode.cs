using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class AddTextEditNode : UINode
    {
        /// <summary>
        /// Defines the UIS identifier of this text edit control. Required.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Defines the identifier of this text control. Required.
        /// </summary>
        public int ID;

        /// <summary>
        /// Defines the position of this text control in x,y coordinates. Syntax: (x,y). Required.
        /// </summary>
        public ArrayListNode TextEditPosition;

        /// <summary>
        /// Defines the dimensions of this text edit control. Syntax: (width,height). Required.
        /// </summary>
        public ArrayListNode Size;

        /// <summary>
        /// Defines the font size of this text edit control in points. Required.
        /// </summary>
        public int Font;

        /// <summary>
        /// Defines the number of lines in this text edit control.
        /// </summary>
        public int? Lines;

        /// <summary>
        /// Defines the maximum character limit of this text control. Default is 0 for unlimited.
        /// </summary>
        public int? Capacity;

        /// <summary>
        /// Defines whether this text edit control will receive a border on focus. 1=Yes, 0=No. Default is yes.
        /// </summary>
        public int? FrameOnFocus;

        /// <summary>
        /// Defines the font color of this text edit control. Syntax: (Red, Green, Blue) where each of these colors is a 
        /// number in the range of [0-255].
        /// </summary>
        public ArrayListNode Color;

        /// <summary>
        /// Defines the transparency of this text edit control. 1 is transparent and 0 is opaque.
        /// </summary>
        public int Transparent;

        /// <summary>
        /// Defines the alignment of this text control. 0=Left-Top, 1=Left-Center, 2=Center-Top, 
        /// 3=Center-Center, 4=Right-Top, 5=Right-Center. Default is left-top.
        /// </summary>
        public int? Alignment;

        /// <summary>
        /// Defines whether or not this text edit control will flash when it is empty. 1=Yes, 0=No. Default is no flashing.
        /// </summary>
        public int? FlashOnEmpty;

        /// <summary>
        /// Defines whether or not this text edit control will be notified when the user presses the enter key. 1=Yes, 0=No. 
        /// Default is no notification.
        /// </summary>
        public ArrayListNode BackColor;

        /// <summary>
        /// Defines extra attributes for this text edit control. Valid values are kInsert and kReadOnly.
        /// </summary>
        public string Mode = "";

        /// <summary>
        /// Defines a UIS image resource to associate with the scrollbar of this text edit control.
        /// </summary>
        public string ScrollbarImage = "";

        /// <summary>
        /// Defines the width of the scrollbar in pixels. This should match the width of scrollbarImage.
        /// </summary>
        public int? ScrollbarGutter;

        /// <summary>
        /// Defines the type of the scrollbar of this text edit control. 0=slider, 1=scrollbar. Default is scrollbar.
        /// </summary>
        public int? ScrollbarType;

        /// <summary>
        /// Defines whether this text edit control should resize for exact line heights. 1=Yes, 0=No. Default is no.
        /// </summary>
        public int? ResizeForExactLineHeight;

        /// <summary>
        /// Defines whether or not to enable input method editing. 1=Yes, 0=No. Default is no IME.
        /// </summary>
        public int? EnableIME;

        /// <summary>
        /// Color of the cursor in this text edit control.
        /// </summary>
        public ArrayListNode CursorColor;

        /// <summary>
        /// Color of frame for this text edit control.
        /// </summary>
        public ArrayListNode FrameColor;

        /// <summary>
        /// Name of a UIS text resource to use as a tooltip for this text edit control.
        /// </summary>
        public string Tooltip = "";

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
                        TextEditPosition = ANode.Array;
                        break;
                    case AssignmentType.SizeAssignment:
                        Size = ANode.Array;
                        break;
                    case AssignmentType.FontAssignment:
                        Font = ANode.NumberValue;
                        break;
                    case AssignmentType.LinesAssignment:
                        Lines = ANode.NumberValue;
                        break;
                    case AssignmentType.CapacityAssignment:
                        Capacity = ANode.NumberValue;
                        break;
                    case AssignmentType.FrameOnFocusAssignment:
                        FrameOnFocus = ANode.NumberValue;
                        break;
                    case AssignmentType.ColorAssignment:
                        Color = ANode.Array;
                        break;
                    case AssignmentType.BackColorAssignment:
                        BackColor = ANode.Array;
                        break;
                    case AssignmentType.CursorColorAssignment:
                        CursorColor = ANode.Array;
                        break;
                    case AssignmentType.FrameColorAssignment:
                        FrameColor = ANode.Array;
                        break;
                    case AssignmentType.TransparentAssignment:
                        Transparent = ANode.NumberValue;
                        break;
                    case AssignmentType.AlignmentAssignment:
                        Alignment = ANode.NumberValue;
                        break;
                    case AssignmentType.FlashOnEmptyAssignment:
                        FlashOnEmpty = ANode.NumberValue;
                        break;
                    case AssignmentType.TooltipAssignment:
                        Tooltip = ANode.StrValue;
                        break;
                }
            }

            AsString = "AddTextEdit";
        }
    }
}
