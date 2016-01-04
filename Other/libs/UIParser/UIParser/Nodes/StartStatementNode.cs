using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class StartStatementNode : UINode
    {
        public LineContentNode LineContent { get; private set; }

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes.Count > 0)
            {
                if(nodes[0].AstNode != null)
                    LineContent = (LineContentNode)AddChild("LineContent", nodes[0]);
            }

            AsString = "StartStatement";
        }
    }
}
