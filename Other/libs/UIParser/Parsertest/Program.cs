using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using UIParser;
using UIParser.Nodes;
using Irony.Parsing;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Interpreter.Evaluator;

namespace Parsertest
{
    class Program
    {
        static void Main(string[] args)
        {
            LanguageData LangData = new LanguageData(new UIGrammar());
            Irony.Parsing.Parser Pars = new Irony.Parsing.Parser(LangData);

            StringBuilder SBuilder = new StringBuilder();

            foreach (string Statement in File.ReadLines("C:\\Program Files (x86)\\Maxis\\The Sims Online\\TSOClient\\gamedata\\uiscripts\\personselectionedit.uis"))
            {
                SBuilder.Append(Statement + "\r\n");
            }

            Debug.WriteLine("Attempting to parse: " + SBuilder.ToString());
            ParseTree Tree = Pars.Parse(SBuilder.ToString());
            //DisplayTree((AstNode)Tree.Root.AstNode, 0);
            WalkTree(Tree.Root);

            Console.ReadLine();
        }

        public static void DisplayTree(AstNode node, int level)
        {
            for (int i = 0; i < level; i++)
                Console.Write("  ");
            Console.WriteLine(node.ToString());

            foreach (AstNode child in node.ChildNodes)
                DisplayTree(child, level + 1);
        }

        public static void WalkTree(ParseTreeNode Node)
        {
            //TODO: Pass UIScene into this so it can be filled with the correct elements.

            if (Node.AstNode != null)
            {
                AstNode ANode = (AstNode)Node.AstNode;

                switch (Node.AstNode.ToString())
                {
                    case "DefineImage":
                        Console.WriteLine("DefineImage: " + ANode.ChildNodes[0].ToString());
                        break;
                    case "SetSharedProperties":
                        SetSharedPropsNode SharedPropertiesNode = (SetSharedPropsNode)UINode.GetNode(ANode);

                        Console.Write("SetSharedProperties: ");

                        if (SharedPropertiesNode.StringTable != null)
                            Console.WriteLine("StringTable: " + SharedPropertiesNode.StringTable);
                        break;
                    case "DefineString":
                        Console.WriteLine("DefineString: " + ANode.ChildNodes[0].ToString());
                        break;
                }
            }

            foreach (ParseTreeNode Child in Node.ChildNodes)
                WalkTree(Child);
        }
    }
}
