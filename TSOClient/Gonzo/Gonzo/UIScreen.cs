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
using Gonzo.Elements;

namespace Gonzo
{
    public class UIScreen
    {
        private ScreenManager m_Manager;

        /// <summary>
        /// Gets this screen's ScreenManager.
        /// </summary>
        public ScreenManager Manager { get { return m_Manager; } }

        protected Dictionary<string, UIElement> m_Elements = new Dictionary<string, UIElement>();
        protected Dictionary<string, UIControl> m_Controls = new Dictionary<string, UIControl>();
        protected SpriteBatch m_SBatch;

        private List<CaretSeparatedText> m_StringTables = new List<CaretSeparatedText>();
        private Dictionary<string, string> m_Strings = new Dictionary<string, string>();
        
        public Vector2 Position;
        private Vector2 m_Size;
        
        /// <summary>
        /// 10px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font10px { get { return m_Manager.Font10px; } }

        /// <summary>
        /// 12px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font12px { get { return m_Manager.Font12px; } }

        /// <summary>
        /// 14px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font14px { get { return m_Manager.Font14px; } }

        /// <summary>
        /// 16px font used to render text by this UIScreen instance.
        /// </summary>
        public SpriteFont Font16px { get { return m_Manager.Font16px; } }

        public UIScreen(ScreenManager Manager, string Name, SpriteBatch SBatch, Vector2 ScreenPosition, 
            Vector2 ScreenSize, string UIScriptPath = "")
        {
            m_Manager = Manager;

            m_SBatch = SBatch;
            Position = ScreenPosition;
            m_Size = ScreenSize;

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

        /// <summary>
        /// Tries to retrieve a string from this UIScreen's loaded strings.
        /// </summary>
        /// <param name="Name">Name of string to retrieve.</param>
        /// <returns>String with the given name. Throws exception if not found.</returns>
        public string GetString(string Name)
        {
            try
            {
                return m_Strings[Name];
            }
            catch (Exception)
            {
                throw new Exception("Couldn't find string: " + Name + " in UIScreen.cs");
            }
        }

        public UIImage GetImage(string Name, bool Copy = false)
        {
            if (Copy)
            {
                UIImage Value = new UIImage((UIImage)m_Elements[Name]);
                return Value;
            }
            else
            {
                UIImage Value = (UIImage)m_Elements[Name];
                return Value;
            }
        }

        public virtual void Update(InputHelper Input)
        {
            Input.Update();

            foreach (KeyValuePair<string, UIElement> KVP in m_Elements)
                KVP.Value.Update(Input);
        }

        public virtual void Draw()
        {
            foreach (KeyValuePair<string, UIElement> KVP in m_Elements)
            {
                try
                {
                    KVP.Value.Draw(m_SBatch, 0.0f);
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
            NodeType NType = (NodeType)Enum.Parse(typeof(NodeType), node.ToString(), true);

            switch (NType)
            {
                case NodeType.DefineImage: //Defines an image and loads a texture for it.
                    DefineImageNode ImgNode = (DefineImageNode)UINode.GetNode(node);
                    UIImage Img = new UIImage(ImgNode, this);
                    m_Elements.Add(ImgNode.Name, Img);
                    break;
                case NodeType.DefineString: //Defines a string with a name.
                    DefineStringNode StrNode = (DefineStringNode)UINode.GetNode(node);
                    m_Strings.Add(StrNode.Name, m_StringTables[State.CurrentStringTable][StrNode.StrIndex]);
                    break;
                case NodeType.AddButton: //Defines a button.
                    AddButtonNode ButtonNode = (AddButtonNode)UINode.GetNode(node);
                    UIButton Btn = new UIButton(ButtonNode, State, this);
                    m_Elements.Add(ButtonNode.Name, Btn);

                    break;
                case NodeType.AddText:
                    AddTextNode TextNode = (AddTextNode)UINode.GetNode(node);
                    UILabel Lbl = new UILabel(TextNode, State, this);
                    m_Elements.Add(TextNode.Name, Lbl);
                    break;
                case NodeType.AddTextEdit:
                    AddTextEditNode TextEditNode = (AddTextEditNode)UINode.GetNode(node);
                    UITextEdit Txt = new UITextEdit(TextEditNode, State, this);
                    m_Elements.Add(TextEditNode.Name, Txt);
                    break;
                case NodeType.AddSlider:
                    AddSliderNode SliderNode = (AddSliderNode)UINode.GetNode(node);
                    UISlider Slider = new UISlider(SliderNode, State, this);
                    m_Elements.Add(SliderNode.Name, Slider);
                    break;
                case NodeType.SetSharedProperties: //Assigns a bunch of shared properties to declarations following the statement.
                    State.InSharedPropertiesGroup = true;
                    SetSharedPropsNode SharedPropsNode = (SetSharedPropsNode)UINode.GetNode(node);

                    if (SharedPropsNode.StringTable != null)
                    {
                        m_StringTables.Add(StringManager.StrTable((int)SharedPropsNode.StringTable));
                        State.CurrentStringTable++;
                    }

                    if (SharedPropsNode.ControlPosition != null)
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

                    if (SharedPropsNode.BackColor != null)
                    {
                        State.BackColor = new Color(new Vector3(SharedPropsNode.BackColor.Numbers[0],
                            SharedPropsNode.BackColor.Numbers[1], SharedPropsNode.BackColor.Numbers[2]));
                    }

                    if (SharedPropsNode.CursorColor != null)
                    {
                        State.CursorColor = new Color(new Vector3(SharedPropsNode.CursorColor.Numbers[0],
                            SharedPropsNode.CursorColor.Numbers[1], SharedPropsNode.CursorColor.Numbers[2]));
                    }

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

                    break;
                case NodeType.SetControlProperties: //Sets a bunch of properties to a specified control.
                    SetControlPropsNode ControlPropsNode = (SetControlPropsNode)UINode.GetNode(node);

                    UIControl Ctrl = new UIControl(ControlPropsNode, this, State);
                    m_Controls.Add(ControlPropsNode.Control, Ctrl);

                    if (State.InSharedPropertiesGroup)
                    {
                        UIElement Test = new UIElement(this, null);
                        //Script implicitly created an object... :\
                        if (!m_Elements.TryGetValue(ControlPropsNode.Control, out Test))
                        {
                            m_Elements.Add(ControlPropsNode.Control, new UIElement(this, null));

                            if(Ctrl.Image != null)
                                m_Elements[ControlPropsNode.Control].Image = new UIImage(Ctrl.Image);

                            m_Elements[ControlPropsNode.Control].Position = Ctrl.Position;
                        }
                    }

                    break;
                case NodeType.End:
                    State.InSharedPropertiesGroup = false;
                    State.Image = ""; //Reset
                    //TODO: Reset more?
                    break;
            }

            foreach (AstNode child in node.ChildNodes)
                WalkTree(State, child);
        }
    }
}
