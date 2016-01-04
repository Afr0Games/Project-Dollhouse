using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class LineContentNode : UINode
    {
        public BeginStatementNode BeginStatement { get; private set; }

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            AsString = "LineContent";
        }
    }
}
