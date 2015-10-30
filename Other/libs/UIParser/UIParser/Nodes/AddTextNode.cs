using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
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
        public int? Alignment;

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

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            AsString = "AddText";
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes[1].AstNode != null)
                Name = nodes[1].Token.Text;
            if (nodes[2].AstNode != null)
                AssignmentBlock = (AssignmentBlockNode)nodes[2].AstNode;

            ID = (int)nodes[2].ChildNodes[0].ChildNodes[0].ChildNodes[2].Token.Value;
            TextPosition = (ArrayListNode)nodes[3].ChildNodes[0].ChildNodes[0].ChildNodes[2].Token.Value;
            Size = (ArrayListNode)nodes[4].ChildNodes[0].ChildNodes[0].ChildNodes[2].Token.Value;
        }
    }
}
