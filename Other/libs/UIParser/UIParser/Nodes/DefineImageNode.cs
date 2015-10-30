using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class DefineImageNode : UINode
    {
        public string Name = "";
        public string AssetID = "";

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes[1].AstNode != null)
                Name = nodes[1].Token.Text;

            AssetID = nodes[2].ChildNodes[0].ChildNodes[0].ChildNodes[2].Token.Text;

            AsString = "DefineImage";
        }
    }
}
