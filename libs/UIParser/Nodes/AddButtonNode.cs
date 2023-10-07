/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the UIParser library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Irony.Parsing;
using Microsoft.Xna.Framework;

namespace UIParser.Nodes
{
    public class AddButtonNode : UINode
    {
        //Required
        public string Name = "";
        //Required
        public int ID = 0;

        /// <summary>
        /// Position of this button. Required.
        /// </summary>
        public Vector2 ButtonPosition;

        /// <summary>
        /// Defines the dimensions of this button control. Syntax: (width,height)
        /// </summary>
        public Vector2 ButtonSize;

        /// <summary>
        /// Defines whether or not this button control uses highlighted text. 1=Yes, 0=No. Default is no.
        /// </summary>
        public int? TextHighlighted;

        /// <summary>
        /// Defines the font size of this text control in points.
        /// </summary>      
        public int? Font;

        /// <summary>
        /// Defines the regular text color of this button control.
        /// </summary>
        public Color? TextColor; 

        /// <summary>
        /// Defines the selected text color of this button control.
        /// </summary>
        public Color? TextColorSelected;

        /// <summary>
        /// Defines the highlighted text color of this button control.
        /// </summary>
        public Color? TextColorHighlighted;

        /// <summary>
        /// Defines the disabled text color of this button control.
        /// </summary>
        public Color? TextColorDisabled;

        /// <summary>
        /// Defines whether or not this button renders as a "textButton", meaning horizontally segmented into 3 and scaled to fit the 
        /// explicitly defined size or width of the inner text. If this is set, buttons with no image will automatically assume the 
        /// default button image, otherwise they will have no button image. 1 for true, 0 for false. Defaults to 1 if text is defined.
        /// </summary>
        public int? TextButton;

        /// <summary>
        /// Defines a UIS string resource to use as text.
        /// </summary>
        public string Text = "";

        /// <summary>
        /// Defines a UIS image resource to associate with this button control
        /// </summary>          
        public string Image;

        /// <summary>
        /// Defines the number of image states associated with this button control.
        /// </summary>
        public int? ImageStates;

        /// <summary>
        /// Defines whether or not this button will trigger. 1=Yes, 0=No. Default is no trigger.
        /// </summary>
        public int? Trigger;

        /// <summary>
        /// Defines the type of tracking this button will employ. Valid options are 0, 1, or 2. Default is 0.
        /// </summary>
        public int? Tracking;

        public string Tooltip = "";
        public AssignmentBlockNode AssignmentBlock { get; private set; }

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes[1].AstNode != null)
                Name = nodes[1].Token.Text;
            if (nodes[2].AstNode != null)
                AssignmentBlock = (AssignmentBlockNode)nodes[2].AstNode;

            foreach (AssignmentNode ANode in AssignmentBlock.ChildNodes[0].ChildNodes)
            {
                switch (ANode.TypeOfAssignment)
                {
                    case AssignmentType.PositionAssignment:
                        ButtonPosition = new Vector2(ANode.Array.Numbers[0], ANode.Array.Numbers[1]);
                        break;
                    case AssignmentType.ImageAssignment:
                        Image = ANode.StrValue;
                        break;
                    case AssignmentType.IDAssignment:
                        ID = ANode.NumberValue;
                        break;
                    case AssignmentType.SizeAssignment:
                        ButtonSize = new Vector2(ANode.Array.Numbers[0], ANode.Array.Numbers[1]);
                        break;
                    case AssignmentType.TextAssignment:
                        Text = ANode.StrValue;
                        break;
                    case AssignmentType.TriggerAssignment:
                        Trigger = ANode.NumberValue;
                        break;
                    case AssignmentType.TrackingAssignment:
                        Tracking = ANode.NumberValue;
                        break;
                    case AssignmentType.TooltipAssignment:
                        Tooltip = ANode.StrValue;
                        break;
                    case AssignmentType.ImageStatesAssignment:
                        ImageStates = ANode.NumberValue;
                        break;
                    case AssignmentType.TextButtonAssignment:
                        TextButton = ANode.NumberValue;
                        break;
                    case AssignmentType.FontAssignment:
                        Font = ANode.NumberValue;
                        break;
                    case AssignmentType.TextColorAssignment:
                        TextColor = new Color((byte)ANode.Array.Numbers[0], 
                            (byte)ANode.Array.Numbers[1], (byte)ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.TextColorDisabledAssignment:
                        TextColorDisabled = new Color((byte)ANode.Array.Numbers[0],
                            (byte)ANode.Array.Numbers[1], (byte)ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.TextColorHighlightedAssignment:
                        TextColorHighlighted = new Color((byte)ANode.Array.Numbers[0], 
                            (byte)ANode.Array.Numbers[1], (byte)ANode.Array.Numbers[2]);
                        break;
                    case AssignmentType.TextColorSelectedAssignment:
                        TextColorSelected = new Color((byte)ANode.Array.Numbers[0], 
                            (byte)ANode.Array.Numbers[1], (byte)ANode.Array.Numbers[2]);
                        break;
                }
            }

            AsString = "AddButton";
        }
    }
}
