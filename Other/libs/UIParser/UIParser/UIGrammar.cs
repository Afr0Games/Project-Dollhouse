using System;
using System.Collections.Generic;
using System.Text;
using Irony.Interpreter.Ast;
using UIParser.Nodes;
using Irony.Parsing;
using Irony.Ast;

namespace UIParser
{
    [Language("TSO UIScript", "1.0", "TSO UIScript parser")]
    public class UIGrammar : Grammar
    {
        public UIGrammar() : base(/*caseSensitive: false*/)
        {
            LanguageFlags = LanguageFlags.CreateAst;

            StringLiteral STRING = new StringLiteral("STRING", "\"", StringOptions.None);
            NumberLiteral NUMBER = new NumberLiteral("NUMBER", NumberOptions.None);
            //ints that are too long for int32 are converted to int64
            NUMBER.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64 };

            STRING.AstConfig.NodeType = typeof(StringValueNode);
            NUMBER.AstConfig.NodeType = typeof(NumberValueNode);

            CommentTerminal LINE_COMMENT = new CommentTerminal("LINE_COMMENT", "#", "\n", "\r\n");
            NonGrammarTerminals.Add(LINE_COMMENT);

            NonTerminal BEGIN = new NonTerminal("BEGIN", typeof(BeginNode));
            NonTerminal START_STATEMENT = new NonTerminal("START_STATEMENT", typeof(StartStatementNode));                      //< LINE_CONTENT >
            NonTerminal LINE_CONTENT = new NonTerminal("LINE_CONTENT", typeof(LineContentNode));                               //Any of the below.
            NonTerminal BEGIN_STATEMENT = new NonTerminal("BEGIN_STATEMENT", typeof(BeginStatementNode));                      //<Begin>
            NonTerminal ANYASSIGNMENT = new NonTerminal("ANYASSIGNMENT", typeof(AnyAssignmentNode));
            NonTerminal ASSIGNMENT = new NonTerminal("ASSIGNMENT", typeof(UIParser.Nodes.AssignmentNode));                     //Argument = STRING | NUMBER
            NonTerminal ASSIGNMENTBLOCK = new NonTerminal("ASSIGNMENTBLOCK", typeof(AssignmentBlockNode));
            NonTerminal DEFINEIMAGE_STATEMENT = new NonTerminal("DEFINEIMAGE_STATEMENT", typeof(DefineImageNode));             //<DefineImage>
            NonTerminal DEFINESTRING_STATEMENT = new NonTerminal("DEFINESTRING_STATEMENT", typeof(DefineStringNode));          //<DefineString>
            NonTerminal SETSHAREDPROPS_STATEMENT = new NonTerminal("SETSHAREDPROPS_STATEMENT", typeof(SetSharedPropsNode));    //<SetSharedProperties>
            NonTerminal ADDBUTTON_STATEMENT = new NonTerminal("ADDBUTTON_STATEMENT", typeof(AddButtonNode));                   //<AddButton>
            NonTerminal ADDTEXT_STATEMENT = new NonTerminal("ADDTEXT_STATEMENT", typeof(AddTextNode));                         //<AddText>         
            NonTerminal ADDTEXTEDIT_STATEMENT = new NonTerminal("ADDTEXTEDIT_STATEMENT", typeof(AddTextEditNode));             //<AddTextEdit>
            NonTerminal ADDSLIDER_STATEMENT = new NonTerminal("ADDSLIDER_STATEMENT", typeof(AddSliderNode));                   //<AddSlider>
            NonTerminal ADDPROGRESSBAR_STATEMENT = new NonTerminal("ADDPROGRESSBAR_STATEMENT", typeof(AddProgressBarNode));    //<AddProgressBar>     
            NonTerminal ADDLISTBOX_STATEMENT = new NonTerminal("ADDLISTBOX_STATEMENT", typeof(AddListboxNode));                //<AddListBox>      
            NonTerminal ADDFORMATEDTEXT_STATEMENT = new NonTerminal("ADDFORMATEDTEXT", typeof(AddFormatedTextNode));           //<AddFormattedText>
            NonTerminal SETCONTROLPROPS_STATEMENT = new NonTerminal("SETCONTROLPROPS_STATEMENT", typeof(SetControlPropsNode)); //<SetControlProperties>                    
            NonTerminal ARRAY = new NonTerminal("ARRAY", typeof(ArrayNode));
            NonTerminal ARRAYLIST = new NonTerminal("ARRAYLIST", typeof(ArrayListNode));                                       //(1,2,3,4)

            NonTerminal END_IDENTIFIER = new NonTerminal("END_IDENTIFIER", typeof(EndNode));                                   //<End>

            KeyTerm COMMA = new KeyTerm(",", "COMMA");
            COMMA.AstConfig.NodeType = typeof(CommaNode);

            //Rules
            START_STATEMENT.Rule = Empty | ToTerm("<") + LINE_CONTENT + ToTerm(">");
            LINE_CONTENT.Rule = Empty | BEGIN_STATEMENT | DEFINEIMAGE_STATEMENT | SETSHAREDPROPS_STATEMENT | 
                SETCONTROLPROPS_STATEMENT | ADDBUTTON_STATEMENT | ADDTEXT_STATEMENT | ADDTEXTEDIT_STATEMENT | 
                ADDSLIDER_STATEMENT | ADDPROGRESSBAR_STATEMENT | ADDLISTBOX_STATEMENT | ADDFORMATEDTEXT_STATEMENT | 
                DEFINESTRING_STATEMENT | END_IDENTIFIER;
            BEGIN_STATEMENT.Rule = "Begin";
            DEFINEIMAGE_STATEMENT.Rule = ToTerm("DefineImage") + STRING + ASSIGNMENTBLOCK;
            DEFINESTRING_STATEMENT.Rule = ToTerm("DefineString") + STRING + ASSIGNMENTBLOCK;

            ASSIGNMENT.Rule = ToTerm("stringDir") + ToTerm("=") + STRING | ToTerm("stringTable") + ToTerm("=") + NUMBER |
                ToTerm("leftArrowImage") + ToTerm("=") + STRING | ToTerm("rightArrowImage") + ToTerm("=") + STRING |
                ToTerm("tracking") + ToTerm("=") + NUMBER | ToTerm("trigger") + ToTerm("=") + NUMBER |
                ToTerm("image") + ToTerm("=") + STRING | ToTerm("position") + ToTerm("=") + ARRAY |
                ToTerm("size") + ToTerm("=") + ARRAY | ToTerm("tooltip") + ToTerm("=") + STRING |
                ToTerm("toolTip") + ToTerm("=") + STRING | ToTerm("id") + ToTerm("=") + NUMBER |
                ToTerm("orientation") + ToTerm("=") + NUMBER | ToTerm("opaque") + ToTerm("=") + NUMBER |
                ToTerm("assetID") + ToTerm("=") + STRING | ToTerm("stringIndex") + ToTerm("=") + NUMBER |
                ToTerm("text") + ToTerm("=") + STRING | ToTerm("font") + ToTerm("=") + NUMBER |
                ToTerm("textColor") + ToTerm("=") + ARRAY | ToTerm("textColorSelected") + ToTerm("=") + ARRAY |
                ToTerm("textColorHighlighted") + ToTerm("=") + ARRAY | ToTerm("textColorDisabled") + ToTerm("=") + ARRAY |
                ToTerm("textButton") + ToTerm("=") + NUMBER | ToTerm("color") + ToTerm("=") + ARRAY |
                ToTerm("alignment") + ToTerm("=") + NUMBER | ToTerm("backColor") + ToTerm("=") + ARRAY |
                ToTerm("frameOnFocus") + ToTerm("=") + NUMBER | ToTerm("cursorColor") + ToTerm("=") + ARRAY |
                ToTerm("transparent") + ToTerm("=") + NUMBER | ToTerm("frameColor") + ToTerm("=") + ARRAY |
                ToTerm("capacity") + ToTerm("=") + NUMBER | ToTerm("lines") + ToTerm("=") + NUMBER |
                ToTerm("flashOnEmpty") + ToTerm("=") + NUMBER | ToTerm("thumbSize") + ToTerm("=") + ARRAY |
                ToTerm("thumbMargins") + ToTerm("=") + ARRAY | ToTerm("thumbImageSize") + ToTerm("=") + ARRAY |
                ToTerm("thumbImageOffsets") + ToTerm("=") + ARRAY | ToTerm("thumbButtonImage") + ToTerm("=") + STRING |
                ToTerm("wrapped") + ToTerm("=") + NUMBER | ToTerm("textColor") + ToTerm("=") + ARRAY |
                ToTerm("mode") + ToTerm("=") + STRING | ToTerm("scrollBarImage") + ToTerm("=") + STRING |
                ToTerm("scrollBarGutter") + ToTerm("=") + NUMBER | ToTerm("scrollBarType") + ToTerm("=") + NUMBER |
                ToTerm("enableIME") + ToTerm("=") + NUMBER | ToTerm("highlighted") + ToTerm("=") + NUMBER |
                ToTerm("minValue") + ToTerm("=") + NUMBER | ToTerm("maxValue") + ToTerm("=") + NUMBER |
                ToTerm("pageSize") + ToTerm("=") + NUMBER | ToTerm("MouseTransparent") + ToTerm("=") + NUMBER |
                ToTerm("backgroundColor") + ToTerm("=") + ARRAY + ToTerm("alignments") + ToTerm("=") + NUMBER |
                ToTerm("visibleRows") + ToTerm("=") + NUMBER | ToTerm("columns") + ToTerm("=") + NUMBER |
                ToTerm("rowHeight") + ToTerm("=") + NUMBER | ToTerm("fillColor") + ToTerm("=") + ARRAY |
                ToTerm("selectionFillColor") + ToTerm("=") + ARRAY | ToTerm("cursor") + ToTerm("=") + NUMBER |
                ToTerm("lineHeight") + ToTerm("=") + NUMBER | ToTerm("edgeOffsetL") + ToTerm("=") + NUMBER |
                ToTerm("edgeOffsetR") + ToTerm("=") + NUMBER | ToTerm("edgeoffsetT") + ToTerm("=") + NUMBER |
                ToTerm("edgeOffsetB") + ToTerm("=") + NUMBER | ToTerm("foregroundImage") + ToTerm("=") + STRING |
                ToTerm("backgroundImage") + ToTerm("=") + STRING | ToTerm("value") + ToTerm("=") + NUMBER |
                ToTerm("useBackgroundImage") + ToTerm("=") + NUMBER | ToTerm("enabled") + ToTerm("=") + NUMBER;

            ANYASSIGNMENT.Rule = MakeStarRule(ANYASSIGNMENT, ASSIGNMENT);
            //This means "assignmentblock is assignment followed by n assignments". 
            ASSIGNMENTBLOCK.Rule = MakePlusRule(ASSIGNMENTBLOCK, ANYASSIGNMENT);

            SETSHAREDPROPS_STATEMENT.Rule = ToTerm("SetSharedProperties") + ASSIGNMENTBLOCK;
            SETCONTROLPROPS_STATEMENT.Rule = ToTerm("SetControlProperties") + STRING + ASSIGNMENTBLOCK | 
                ToTerm("SetControlProperties") + STRING;

            ADDBUTTON_STATEMENT.Rule = ToTerm("AddButton") + STRING + ASSIGNMENTBLOCK;
            ADDTEXT_STATEMENT.Rule = ToTerm("AddText") + STRING + ASSIGNMENTBLOCK;
            ADDTEXTEDIT_STATEMENT.Rule = ToTerm("AddTextEdit") + STRING + ASSIGNMENTBLOCK;
            ADDSLIDER_STATEMENT.Rule = ToTerm("AddSlider") + STRING + ASSIGNMENTBLOCK;
            ADDPROGRESSBAR_STATEMENT.Rule = ToTerm("AddProgressBar") + STRING + ASSIGNMENTBLOCK;
            ADDLISTBOX_STATEMENT.Rule = ToTerm("AddListBox") + STRING + ASSIGNMENTBLOCK;
            ADDFORMATEDTEXT_STATEMENT.Rule = ToTerm("AddFormatedText") + STRING + ASSIGNMENTBLOCK;

            ARRAY.Rule = ToTerm("(") + ARRAYLIST + ")";
            ARRAYLIST.Rule = MakeStarRule(ARRAYLIST, COMMA, NUMBER);

            END_IDENTIFIER.Rule = "End";
            BEGIN.Rule = MakePlusRule(BEGIN, START_STATEMENT);

            this.Root = BEGIN;
        }
    }
}
