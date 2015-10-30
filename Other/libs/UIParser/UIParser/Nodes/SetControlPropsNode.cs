using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class SetControlPropsNode : UINode
    {
        public string Control { get; private set; }
        public List<AssignmentNode> Assignments = new List<AssignmentNode>();

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            Control = nodes[1].Token.Text;

            foreach (ParseTreeNode Node in nodes[1].ChildNodes)
            {
                if (Node.AstNode != null)
                    Assignments.Add((AssignmentNode)AddChild("AssignmentNode", Node));
            }

            AsString = "SetControlProperties";
        }
    }
}
