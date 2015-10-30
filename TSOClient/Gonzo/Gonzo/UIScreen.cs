using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Files.Manager;
using Irony.Parsing;
using Irony.Interpreter.Ast;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Gonzo
{
    public class UIScreen : UIElement
    {
        protected SpriteBatch m_SBatch;

        public UIScreen(string Name, SpriteBatch SBatch, Vector2 Size, string UIScriptPath = "") : base(Name, new Vector2(0.0f, 0.0f), Size)
        {
            m_SBatch = SBatch;
            m_Size = Size;

            if (UIScriptPath != "")
                Initialize(UIScriptPath);
        }

        /// <summary>
        /// Parses the script for this UIScreen instance and walks the generated AST.
        /// </summary>
        /// <param name="Path">The path to the script.</param>
        private void Initialize(string Path)
        {
            StringBuilder SBuilder = new StringBuilder();
            LanguageData LangData = new LanguageData(new UIGrammar());
            Irony.Parsing.Parser Pars = new Irony.Parsing.Parser(LangData);

            foreach (string Statement in File.ReadLines(Path))
                SBuilder.Append(Statement + "\r\n");

            ParseTree Tree = Pars.Parse(SBuilder.ToString());
            WalkTree(new UIParser.ParserState(), (AstNode)Tree.Root.AstNode);
        }

        public override void Update(InputHelper Input)
        {
            Input.Update();

            foreach (KeyValuePair<string, UIElement> KVP in m_Elements)
                KVP.Value.Update(Input);
        }

        public override void Draw()
        {
            foreach (KeyValuePair<string, UIElement> KVP in m_Elements)
            {
                try
                {
                    KVP.Value.Draw(m_SBatch);
                }
                catch(Exception)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Walks a generated AST (Abstract Syntax Tree) and creates the elements of this UIScreen.
        /// </summary>
        /// <param name="State">A ParserState instance.</param>
        /// <param name="node">The root node of the AST.</param>
        private void WalkTree(UIParser.ParserState State, AstNode node)
        {
            switch (node.ToString())
            {
                case "DefineImage": //Defines an image and loads a texture for it.
                    DefineImageNode ImgNode = (DefineImageNode)UINode.GetNode(node);
                    UIImage Img = new UIImage(ImgNode, this);
                    m_Elements.Add(ImgNode.Name, Img);
                    break;
                case "DefineString": //Defines a string with a name.
                    DefineStringNode StrNode = (DefineStringNode)UINode.GetNode(node);
                    m_Strings.Add(StrNode.Name, m_StringTables[State.CurrentStringTable][StrNode.StrIndex]);
                    break;
                case "AddButton": //Defines a button.
                    AddButtonNode ButtonNode = (AddButtonNode)UINode.GetNode(node);
                    UIButton Btn = new UIButton(ButtonNode, State, this);
                    m_Elements.Add(ButtonNode.Name, Btn);

                    break;
                case "AddText":
                    AddTextNode TextNode = (AddTextNode)UINode.GetNode(node);
                    UILabel Lbl = new UILabel(TextNode, this);
                    m_Elements.Add(TextNode.Name, Lbl);
                    break;
                case "AddTextEdit":
                    AddTextEditNode TextEditNode = (AddTextEditNode)UINode.GetNode(node);
                    UITextEdit Txt = new UITextEdit(TextEditNode, State, this);
                    m_Elements.Add(TextEditNode.Name, Txt);
                    break;
                case "AddSlider":
                    AddSliderNode SliderNode = (AddSliderNode)UINode.GetNode(node);
                    UISlider Slider = new UISlider(SliderNode, this);
                    m_Elements.Add(SliderNode.Name, Slider);
                    break;
                case "SetSharedProperties": //Assigns a bunch of shared properties to declarations following the statement.
                    State.InSharedPropertiesGroup = true;
                    SetSharedPropsNode SharedPropsNode = (SetSharedPropsNode)UINode.GetNode(node);

                    if(SharedPropsNode.StringTable != null)
                    {
                        m_StringTables.Add(StringManager.StrTable((int)SharedPropsNode.StringTable));
                        State.CurrentStringTable++;
                    }

                    if(SharedPropsNode.ControlPosition != null)
                    {
                        State.Position[0] = SharedPropsNode.ControlPosition.Numbers[0];
                        State.Position[1] = SharedPropsNode.ControlPosition.Numbers[1];
                        break;
                    }

                    if (SharedPropsNode.Color != null)
                    {
                        State.Color = new Color(new Vector3(SharedPropsNode.Color.Numbers[0], SharedPropsNode.Color.Numbers[1],
                            SharedPropsNode.Color.Numbers[2]));
                    }

                    if(SharedPropsNode.BackColor != null)
                    {
                        State.BackColor = new Color(new Vector3(SharedPropsNode.BackColor.Numbers[0], 
                            SharedPropsNode.BackColor.Numbers[1], SharedPropsNode.BackColor.Numbers[2]));
                    }

                    if (SharedPropsNode.CursorColor != null)
                    {
                        State.CursorColor = new Color(new Vector3(SharedPropsNode.CursorColor.Numbers[0],
                            SharedPropsNode.CursorColor.Numbers[1], SharedPropsNode.CursorColor.Numbers[2]));
                    }

                    if(SharedPropsNode.Opaque != null)
                        State.IsOpaque = (SharedPropsNode.Opaque == 1) ? true : false;

                    if(SharedPropsNode.Transparent != null)
                        State.IsTransparent = (SharedPropsNode.Transparent == 1) ? true : false;

                    if (SharedPropsNode.Alignment != null)
                        State.Alignment = (int)SharedPropsNode.Alignment;

                    if (SharedPropsNode.Image != "")
                        State.Image = SharedPropsNode.Image;

                    if (SharedPropsNode.Tooltip != "")
                        State.Tooltip = SharedPropsNode.Tooltip;

                    if (SharedPropsNode.Text != "")
                        State.Caption = SharedPropsNode.Text;

                    break;
                case "SetControlProperties": //Sets a bunch of properties to a specified control.
                    int X, Y = 0;
                    SetControlPropsNode ControlPropsNode = (SetControlPropsNode)UINode.GetNode(node);

                    foreach (UIParser.Nodes.AssignmentNode Assignment in ControlPropsNode.Assignments)
                    {
                        switch (Assignment.TypeOfAssignment)
                        {
                            case AssignmentType.PositionAssignment:
                                X = Assignment.Array.Numbers[0];
                                Y = Assignment.Array.Numbers[1];
                                m_Elements[ControlPropsNode.Control].SetPosition(X, Y);
                                break;
                            case AssignmentType.SizeAssignment:
                                X = Assignment.Array.Numbers[0];
                                Y = Assignment.Array.Numbers[1];
                                m_Elements[ControlPropsNode.Control].SetSize(X, Y);
                                break;
                        }
                    }

                    if(State.InSharedPropertiesGroup)
                    {
                        m_Elements[ControlPropsNode.Control].Image = m_Elements[State.Image].Image;
                    }

                    break;
                case "End":
                    State.InSharedPropertiesGroup = false;
                    break;
            }

            foreach (AstNode child in node.ChildNodes)
                WalkTree(State, child);
        }
    }
}
