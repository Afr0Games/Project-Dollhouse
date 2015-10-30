using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class DefineStringNode : UINode
    {
        public string Name = "";
        public int StrIndex = 0;

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes[1].AstNode != null)
                Name = nodes[1].Token.Text;

            StrIndex = (int)nodes[2].ChildNodes[0].ChildNodes[0].ChildNodes[2].Token.Value;

            AsString = "DefineString";
        }
    }
}
