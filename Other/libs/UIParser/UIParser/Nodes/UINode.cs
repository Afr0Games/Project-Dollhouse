using System;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using Irony.Interpreter.Ast;

namespace UIParser.Nodes
{
    public interface IUIVisitor
    {
        void Visit(BeginStatementNode Node);
        void Visit(AssignmentBlockNode Node);
        void Visit(AnyAssignmentNode Node);
        void Visit(AssignmentNode Node);
        void Visit(LineContentNode Node);
        void Visit(StartStatementNode Node);
        void Visit(ArrayNode Node);
        void Visit(ArrayListNode Node);
        void Visit(BeginNode Node);
        void Visit(AddButtonNode Node);
        void Visit(AddTextNode Node);
        void Visit(AddSliderNode Node);
        void Visit(AddListboxNode Node);
        void Visit(AddProgressBarNode Node);
        void Visit(AddTextEditNode Node);
        void Visit(AddFormatedTextNode Node);
        void Visit(DefineImageNode Node);
        void Visit(DefineStringNode Node);
        void Visit(StringValueNode Node);
        void Visit(NumberValueNode Node);
        void Visit(SetControlPropsNode Node);
        void Visit(SetSharedPropsNode Node);
        void Visit(CommaNode Node);
        void Visit(EndNode Node);
    }

    public abstract class UINode : AstNode
    {
        /// <summary>
        /// Sets the last children as tail
        /// </summary>
        /// <returns>False if this node is empty</returns>
        protected bool SetTailChildren()
        {
            if (ChildNodes.Count == 0)
                return false;
            ChildNodes[ChildNodes.Count - 1].Flags |= AstNodeFlags.IsTail;
            return true;
        }

        protected virtual void InitChildren(ParseTreeNodeList nodes)
        {

        }

        protected virtual void AfterInit()
        {

        }

        /// <summary>
        /// Converts an AstNode to its AstNode extended equivalent.
        /// </summary>
        /// <param name="node">The node to convert.</param>
        /// <returns>A node of the correct type.</returns>
        public static object GetNode(AstNode node)
        {
            switch (node.ToString())
            {
                case "DefineImage":
                    return (DefineImageNode)node;
                case "SetSharedProperties":
                    return (SetSharedPropsNode)node;
                case "SetControlProperties":
                    return (SetControlPropsNode)node;
                case "DefineString":
                    return (DefineStringNode)node;
                case "AddText":
                    return (AddTextNode)node;
                case "AddButton":
                    return (AddButtonNode)node;
                case "AddTextEdit":
                    return (AddTextEditNode)node;
                case "AddSlider":
                    return (AddSliderNode)node;
                case "AddProgressBar":
                    return (AddProgressBarNode)node;
                case "AddFormatedText":
                    return (AddFormatedTextNode)node;
                default:
                    return node;
            }
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            var nodes = treeNode.GetMappedChildNodes();
            InitChildren(nodes);
            AfterInit();
        }

        protected void InitChildrenAsList(ParseTreeNodeList nodes)
        {
            foreach (var node in nodes)
            {
                if (node.AstNode != null)
                    AddChild(string.Empty, node);
            }
        }

        public abstract void Accept(IUIVisitor visitor);
    }
}
