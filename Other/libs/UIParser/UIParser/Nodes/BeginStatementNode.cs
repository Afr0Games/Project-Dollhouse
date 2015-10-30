using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
using Irony.Interpreter.Ast;
using Irony.Ast;

namespace UIParser.Nodes
{
    public class BeginStatementNode : UINode
    {
        List<UINode> Children = new List<UINode>();

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);
        }
    }
}
