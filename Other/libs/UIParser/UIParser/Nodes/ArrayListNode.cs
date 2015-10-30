using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class ArrayListNode : UINode
    {
        public List<int> Numbers = new List<int>();

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            //InitChildrenAsList(nodes);
            foreach(ParseTreeNode Node in nodes)
            {
                if (Node.AstNode != null)
                    Numbers.Add((int)Node.Token.Value);
            }

            AsString = "ArrayListNode";
        }
    }
}
