using System;
using System.Collections.Generic;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public enum AssignmentType
    {
        StringDirAssignment = 1,
        StringTableAssignment = 2,
        LeftArrowImageAssignment = 3,
        RightArrowImageAssignment = 4,
        TrackingAssignment = 5,
        TriggerAssignment = 6,
        ImageAssignment = 7,
        PositionAssignment = 8,
        SizeAssignment = 9,
        TooltipAssignment = 10,
        IDAssignment = 11,
        OrientationAssignment = 12,
        OpaqueAssignment = 13,
        AssetIDAssignment = 14,
        StringIndexAssignment = 15,
        TextAssignment = 16,
        FontAssignment = 17,
        TextColorSelectedAssignment = 18,
        TextColorHighlightedAssignment = 19,
        TextColorDisabledAssignment = 20,
        TextButtonAssignment = 21,
        ColorAssignment = 22,
        AlignmentAssignment = 23,
        BackColorAssignment = 24,
        FrameOnFocusAssignment = 25,
        TransparentAssignment = 26,
        CapacityAssignment = 27,
        LinesAssignment = 28,
        FlashOnEmptyAssignment = 29,
        ThumbSizeAssignment = 30,
        ThumbMarginsAssignment = 31,
        ThumbImageSizeAssignment = 32,
        ThumbImageOffsetsAssignment = 33,
        ThumbButtonImageAssignment = 34,
        WrappedAssignment = 35,
        TextColorAssignment = 36,
        ModeAssignment = 37,
        ScrollBarImageAssignment = 38,
        ScrollBarGutterAssignment = 39,
        ScrollBarTypeAssignment = 40,
        EnableIMEAssignment = 41,
        HighlightedAssignment = 42,
        MinValueAssignment = 43,
        MaxValueAssignment = 44,
        PageSizeAssignment = 45,
        CursorColorAssignment = 46,
        MouseTransparentAssignment = 47,
        ImageStatesAssignment = 48,
        BackgroundColorAssignment = 49,
        FrameColorAssignment = 50,
        AlignmentsAssignment = 51,
        VisibleRowsAssignment = 52,
        ColumnsAssignment = 53,
        RowHeightAssignment = 54,
        FillColorAssignment = 55,
        SelectionFillColorAssignment = 56,
        CursorAssignment = 57,
        LineHeightAssignment = 58,
        EdgeOffsetLAssignment = 59,
        EdgeOffsetRAssignment = 60,
        EdgeOffsetTAssignment = 61,
        EdgeOffsetBAssignment = 62,
        ForegroundImageAssignment = 63,
        BackgroundImageAssignment = 64,
        ValueAssignment = 65,
        EnabledAssignment = 66,
        UseBackgroundImageAssignment = 67
    }

    public class AssignmentNode : UINode
    {
        public AssignmentType TypeOfAssignment;
        private object Value;
        public ArrayListNode Array { get { return (ArrayListNode)Value; } }
        public string StrValue { get { return (string)Value; } }
        public int NumberValue { get { return (int)Value; } }

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            switch(nodes[0].Token.Text)
            {
                case "stringDir":
                    TypeOfAssignment = AssignmentType.StringDirAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "stringTable":
                    TypeOfAssignment = AssignmentType.StringTableAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "leftArrowImage":
                    TypeOfAssignment = AssignmentType.LeftArrowImageAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "rightArrowImage":
                    TypeOfAssignment = AssignmentType.RightArrowImageAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "tracking":
                    TypeOfAssignment = AssignmentType.TrackingAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "trigger":
                    TypeOfAssignment = AssignmentType.TriggerAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "image":
                    TypeOfAssignment = AssignmentType.ImageAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "position":
                    TypeOfAssignment = AssignmentType.PositionAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "size":
                    TypeOfAssignment = AssignmentType.SizeAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "id":
                    TypeOfAssignment = AssignmentType.IDAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "orientation":
                    TypeOfAssignment = AssignmentType.OrientationAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "opaque":
                    TypeOfAssignment = AssignmentType.OpaqueAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "assetID":
                    TypeOfAssignment = AssignmentType.AssetIDAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "stringIndex":
                    TypeOfAssignment = AssignmentType.StringIndexAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "text":
                    TypeOfAssignment = AssignmentType.TextAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "font":
                    TypeOfAssignment = AssignmentType.FontAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "textColorSelected":
                    TypeOfAssignment = AssignmentType.TextColorSelectedAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "textColorHighlighted":
                    TypeOfAssignment = AssignmentType.TextColorHighlightedAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "textColorDisabled":
                    TypeOfAssignment = AssignmentType.TextColorDisabledAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "textButton":
                    TypeOfAssignment = AssignmentType.TextButtonAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "color":
                    TypeOfAssignment = AssignmentType.ColorAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "alignment":
                    TypeOfAssignment = AssignmentType.AlignmentAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "backColor":
                    TypeOfAssignment = AssignmentType.BackColorAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "frameOnFocus":
                    TypeOfAssignment = AssignmentType.FrameOnFocusAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "transparent":
                    TypeOfAssignment = AssignmentType.TransparentAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "capacity":
                    TypeOfAssignment = AssignmentType.CapacityAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "lines":
                    TypeOfAssignment = AssignmentType.LinesAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "flashOnEmpty":
                    TypeOfAssignment = AssignmentType.FlashOnEmptyAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "thumbImageSize":
                    TypeOfAssignment = AssignmentType.ThumbImageSizeAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "thumbImageOffsets":
                    TypeOfAssignment = AssignmentType.ThumbImageOffsetsAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "thumbButtonImage":
                    TypeOfAssignment = AssignmentType.ThumbButtonImageAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "wrapped":
                    TypeOfAssignment = AssignmentType.WrappedAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "textColor":
                    TypeOfAssignment = AssignmentType.TextColorAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "mode":
                    TypeOfAssignment = AssignmentType.ModeAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "scrollBarImage":
                    TypeOfAssignment = AssignmentType.ScrollBarImageAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "scrollBarGutter":
                    TypeOfAssignment = AssignmentType.ScrollBarGutterAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "scrollBarType":
                    TypeOfAssignment = AssignmentType.ScrollBarTypeAssignment;
                    Value = Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "enableIME":
                    TypeOfAssignment = AssignmentType.EnableIMEAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "highlighted":
                    TypeOfAssignment = AssignmentType.HighlightedAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "minValue":
                    TypeOfAssignment = AssignmentType.MinValueAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "maxValue":
                    TypeOfAssignment = AssignmentType.MaxValueAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "pageSize":
                    TypeOfAssignment = AssignmentType.PageSizeAssignment;
                    Value = int.Parse(nodes[2].Token.Text);
                    break;
                case "cursorColor":
                    TypeOfAssignment = AssignmentType.CursorColorAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "toolTip":
                    TypeOfAssignment = AssignmentType.TooltipAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "tooltip":
                    TypeOfAssignment = AssignmentType.TooltipAssignment;
                    Value = (string)nodes[2].Token.Text;
                    break;
                case "MouseTransparent":
                    TypeOfAssignment = AssignmentType.MouseTransparentAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "imagestates":
                    TypeOfAssignment = AssignmentType.ImageStatesAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "backgroundColor":
                    TypeOfAssignment = AssignmentType.BackgroundColorAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "framecolor":
                    TypeOfAssignment = AssignmentType.FrameColorAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "alignments":
                    TypeOfAssignment = AssignmentType.AlignmentsAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "visibleRows":
                    TypeOfAssignment = AssignmentType.VisibleRowsAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "columns":
                    TypeOfAssignment = AssignmentType.ColumnsAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "rowHeight":
                    TypeOfAssignment = AssignmentType.RowHeightAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "fillColor":
                    TypeOfAssignment = AssignmentType.FillColorAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "selectionFillColor":
                    TypeOfAssignment = AssignmentType.SelectionFillColorAssignment;
                    Value = (ArrayListNode)nodes[2].ChildNodes[1].AstNode;
                    break;
                case "cursor":
                    TypeOfAssignment = AssignmentType.CursorAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "lineHeight":
                    TypeOfAssignment = AssignmentType.LineHeightAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "edgeOffsetL":
                    TypeOfAssignment = AssignmentType.EdgeOffsetLAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "edgeOffsetR":
                    TypeOfAssignment = AssignmentType.EdgeOffsetRAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "edgeOffsetT":
                    TypeOfAssignment = AssignmentType.EdgeOffsetTAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "edgeOffsetB":
                    TypeOfAssignment = AssignmentType.EdgeOffsetBAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "foregroundImage":
                    TypeOfAssignment = AssignmentType.ForegroundImageAssignment;
                    Value = nodes[2].Token.Text;
                    break;
                case "backgroundImage":
                    TypeOfAssignment = AssignmentType.BackgroundImageAssignment;
                    Value = nodes[2].Token.Text;
                    break;
                case "value":
                    TypeOfAssignment = AssignmentType.ValueAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "useBackgroundImage":
                    TypeOfAssignment = AssignmentType.UseBackgroundImageAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
                case "enabled":
                    TypeOfAssignment = AssignmentType.EnabledAssignment;
                    Value = (int)nodes[2].Token.Value;
                    break;
            }

            AsString = "Assignment";
        }
    }
}
