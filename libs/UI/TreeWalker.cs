﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Gonzo library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Irony.Parsing;
using Irony.Interpreter.Ast;
using UIParser;
using UIParser.Nodes;
using Microsoft.Xna.Framework;
using UI.Elements;

namespace UI
{
    /// <summary>
    /// A container containing all elements, controls, and strings created by the TreeWalker,
    /// as well as the current state of the parser.
    /// </summary>
    public class ParseResult
    {
        public Dictionary<string, UIElement> Elements = new Dictionary<string, UIElement>();
        public Dictionary<string, UIControl> Controls = new Dictionary<string, UIControl>();
        public Dictionary<string, string> Strings = new Dictionary<string, string>();

        public UIParser.ParserState State = new UIParser.ParserState();
    }

    /// <summary>
    /// Used tp walk a generated AST (Abstract Syntax Tree) for UI scripts.
    /// </summary>
    public class TreeWalker
    {
        private UIScreen m_Screen;

        /// <summary>
        /// Constructs a new TreeWalker instance.
        /// </summary>
        /// <param name="Screen">A UIScreen instance.</param>
        public TreeWalker(UIScreen Screen)
        {
            m_Screen = Screen;
        }

        /// <summary>
        /// Parses a UI script and walks the generated AST.
        /// </summary>
        /// <param name="Path">The path to the script.</param>
        public void Initialize(string Path, ref ParseResult Result)
        {
            StringBuilder SBuilder = new StringBuilder();
            LanguageData LangData = new LanguageData(new UIGrammar());
            Parser Pars = new Parser(LangData);

            foreach (string Statement in File.ReadLines(Path))
                SBuilder.Append(Statement + "\r\n");

            ParseTree Tree = Pars.Parse(SBuilder.ToString());
            WalkTree(Result.State, (AstNode)Tree.Root.AstNode, ref Result);
        }

        /// <summary>
        /// Walks a generated AST (Abstract Syntax Tree) and creates the elements of this UIScreen.
        /// </summary>
        /// <param name="State">A ParserState instance.</param>
        /// <param name="node">The root node of the AST.</param>
        private void WalkTree(UIParser.ParserState State, AstNode node, ref ParseResult Result)
        {
            NodeType NType = (NodeType)Enum.Parse(typeof(NodeType), node.ToString(), true);

            switch (NType)
            {
                case NodeType.DefineImage: //Defines an image and loads a texture for it.
                    DefineImageNode ImgNode = (DefineImageNode)UINode.GetNode(node);
                    if (string.CompareOrdinal(ImgNode.Name, "\"BackgroundImage\"") == 0)
                    {
                        UIBackgroundImage Img = new UIBackgroundImage(ImgNode, m_Screen);
                        Result.Elements.Add(ImgNode.Name, Img);
                    }
                    else
                    {
                        UIImage Img = new UIImage(ImgNode, m_Screen);
                        Result.Elements.Add(ImgNode.Name, Img);
                    }
                    break;
                case NodeType.DefineString: //Defines a string with a name.
                    DefineStringNode StrNode = (DefineStringNode)UINode.GetNode(node);
                    Result.Strings.Add(StrNode.Name, StringManager.StrTable(State.CurrentStringTable)[StrNode.StrIndex]);
                    break;
                case NodeType.AddButton: //Defines a button.
                    AddButtonNode ButtonNode = (AddButtonNode)UINode.GetNode(node);
                    UIButton Btn = new UIButton(ButtonNode, Result, m_Screen);
                    Result.Elements.Add(ButtonNode.Name, Btn);

                    break;
                case NodeType.AddText:
                    AddTextNode TextNode = (AddTextNode)UINode.GetNode(node);
                    UILabel Lbl = new UILabel(TextNode, Result, m_Screen);
                    Result.Elements.Add(TextNode.Name, Lbl);
                    break;
                case NodeType.AddTextEdit:
                    AddTextEditNode TextEditNode = (AddTextEditNode)UINode.GetNode(node);
                    UITextEdit Txt = new UITextEdit(TextEditNode, State, m_Screen);
                    Result.Elements.Add(TextEditNode.Name, Txt);
                    break;
                case NodeType.AddSlider:
                    AddSliderNode SliderNode = (AddSliderNode)UINode.GetNode(node);
                    UISlider Slider = new UISlider(SliderNode, State, m_Screen);
                    Result.Elements.Add(SliderNode.Name, Slider);
                    break;
                case NodeType.SetSharedProperties: //Assigns a bunch of shared properties to declarations following the statement.
                    State.InSharedPropertiesGroup = true;
                    SetSharedPropsNode SharedPropsNode = (SetSharedPropsNode)UINode.GetNode(node);

                    if (SharedPropsNode.StringTable != null)
                        State.CurrentStringTable = (int)SharedPropsNode.StringTable;

                    if (SharedPropsNode.ControlPosition != null)
                    {
                        State.Position = new Vector2(SharedPropsNode.ControlPosition.Numbers[0], 
                            SharedPropsNode.ControlPosition.Numbers[1]);
                        break;
                    }

                    if (SharedPropsNode.Color != null)
                        State.Color = SharedPropsNode.Color;

                    if (SharedPropsNode.TextColor != null)
                        State.TextColor = SharedPropsNode.TextColor;

                    if (SharedPropsNode.TextColorSelected != null)
                        State.TextColorSelected = SharedPropsNode.TextColorSelected;

                    if (SharedPropsNode.TextColorHighlighted != null)
                        State.TextColorHighlighted = SharedPropsNode.TextColorHighlighted;

                    if (SharedPropsNode.TextColorDisabled != null)
                        State.TextColorDisabled = SharedPropsNode.TextColorDisabled;

                    if (SharedPropsNode.BackColor != null)
                        State.BackColor = SharedPropsNode.BackColor;

                    if (SharedPropsNode.CursorColor != null)
                        State.CursorColor = SharedPropsNode.CursorColor;

                    if(SharedPropsNode.FrameColor != null)
                        State.FrameColor = SharedPropsNode.FrameColor;

                    if (SharedPropsNode.TextButton)
                        State.TextButton = true;

                    if (SharedPropsNode.Opaque != null)
                        State.IsOpaque = (SharedPropsNode.Opaque == 1) ? true : false;

                    if (SharedPropsNode.Transparent != null)
                        State.IsTransparent = (SharedPropsNode.Transparent == 1) ? true : false;

                    if (SharedPropsNode.Alignment != null)
                        State.Alignment = (int)SharedPropsNode.Alignment;

                    if (SharedPropsNode.Image != "")
                        State.Image = SharedPropsNode.Image;

                    if (SharedPropsNode.Tooltip != "")
                        State.Tooltip = SharedPropsNode.Tooltip;

                    if (SharedPropsNode.Text != "")
                        State.Caption = SharedPropsNode.Text;

                    if (SharedPropsNode.Size != null)
                        State.Size = new Vector2(SharedPropsNode.Size.Numbers[0], SharedPropsNode.Size.Numbers[1]);

                    if (SharedPropsNode.Orientation != null)
                        State.Orientation = (int)SharedPropsNode.Orientation;

                    if (SharedPropsNode.Font != null)
                        State.Font = (int)SharedPropsNode.Font;

                    if (SharedPropsNode.Opaque != null)
                        State.Opaque = (int)SharedPropsNode.Opaque;

                    break;
                case NodeType.SetControlProperties: //Sets a bunch of properties to a specified control.
                    SetControlPropsNode ControlPropsNode = (SetControlPropsNode)UINode.GetNode(node);

                    UIControl Ctrl = new UIControl(ControlPropsNode, m_Screen, State);
                    Result.Controls.Add(ControlPropsNode.Control, Ctrl);

                    if (State.InSharedPropertiesGroup)
                    {
                        UIElement Test = new UIElement(m_Screen, null);
                        //Script implicitly created an object... :\
                        if (!Result.Elements.TryGetValue(ControlPropsNode.Control, out Test))
                        {
                            Result.Elements.Add(ControlPropsNode.Control, new UIElement(m_Screen, null));

                            if (Ctrl.Image != null)
                                Result.Elements[ControlPropsNode.Control].Image = new UIImage(Ctrl.Image);

                            Result.Elements[ControlPropsNode.Control].Position = Ctrl.Position;
                        }
                    }

                    break;
                case NodeType.End:
                    State.InSharedPropertiesGroup = false;
                    State.Image = ""; //Reset
                    State.TextButton = false; //Reset 
                    State.Color = new Color();
                    State.Caption = "";
                    State.Size = new Vector2(0, 0);
                    State.Alignment = 0;
                    State.Font = 0;
                    //TODO: Reset more?
                    break;
            }

            foreach (AstNode child in node.ChildNodes)
                WalkTree(State, child, ref Result);
        }
    }
}
