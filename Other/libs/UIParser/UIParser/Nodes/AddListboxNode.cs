using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class AddListboxNode : UINode
    {
        /// <summary>
        /// Defines the UIS identifier of this listbox control. Required.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Defines the identifier of this listbox control. Required.
        /// </summary>
        public int ID;

        /// <summary>
        /// Defines the position of this listbox control in x,y coordinates. Syntax: (x,y). Required.
        /// </summary>
        public ArrayListNode ListBoxPosition;

        /// <summary>
        /// Defines the dimensions of this listbox control. Syntax: (width,height). Required.
        /// </summary>
        public ArrayListNode Size;

        /// <summary>
        /// Defines the height of the list box in rows.
        /// </summary>
        public int VisibleRows;

        /// <summary>
        /// Defines the number of columns used in this list box. Required.
        /// </summary>
        public int Columns;

        /// <summary>
        /// Defines the alignment of this list box control. Required.
        /// </summary>
        public int Alignments;

        /// <summary>
        /// Defines the font size of this list box control in points.
        /// </summary>
        public int? Font;

        /// <summary>
        /// Defines the line height of each row of this list box control in points.
        /// </summary>
        public int? RowHeight;

        /// <summary>
        /// Defines the transparency of this list box control. 1 is transparent and 0 is opaque. Required.
        /// </summary>
        public int Transparent;

        /// <summary>
        /// Defines the fill color of this list box control. Syntax: (Red, Green, Blue) where each of 
        /// these colors is a number in the range of [0-255].
        /// </summary>
        public ArrayListNode FillColor;

        /// <summary>
        /// Defines the selection fill color of this list box control. Syntax: (Red, Green, Blue) where 
        /// each of these colors is a number in the range of [0-255].
        /// </summary>
        public ArrayListNode SelectionFillColor;

        /// <summary>
        /// Defines whether or not this list box control will show a cursor. 1=Yes, 0=No. Default is yes.
        /// </summary>
        public int Cursor;

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
                        ListBoxPosition = ANode.Array;
                        break;
                    case AssignmentType.SizeAssignment:
                        Size = ANode.Array;
                        break;
                    case AssignmentType.VisibleRowsAssignment:
                        VisibleRows = ANode.NumberValue;
                        break;
                    case AssignmentType.ColumnsAssignment:
                        Columns = ANode.NumberValue;
                        break;
                    case AssignmentType.AlignmentsAssignment:
                        Alignments = ANode.NumberValue;
                        break;
                    case AssignmentType.FontAssignment:
                        Font = ANode.NumberValue;
                        break;
                    case AssignmentType.RowHeightAssignment:
                        RowHeight = ANode.NumberValue;
                        break;
                    case AssignmentType.TransparentAssignment:
                        Transparent = ANode.NumberValue;
                        break;
                    case AssignmentType.FillColorAssignment:
                        FillColor = ANode.Array;
                        break;
                    case AssignmentType.SelectionFillColorAssignment:
                        SelectionFillColor = ANode.Array;
                        break;
                    case AssignmentType.CursorAssignment:
                        Cursor = ANode.NumberValue;
                        break;
                }
            }

            AsString = "AddListBox";
        }
    }
}
