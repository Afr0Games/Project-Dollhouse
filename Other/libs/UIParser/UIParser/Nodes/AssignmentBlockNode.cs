using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class AssignmentBlockNode : UINode
    {
        public List<AssignmentNode> AssignmentNodes = new List<AssignmentNode>();

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            foreach (ParseTreeNode Node in nodes)
                AssignmentNodes.Add((AssignmentNode)Node.ChildNodes[0].AstNode);

            AsString = "AssignmentBlock";
        }
    }
}
