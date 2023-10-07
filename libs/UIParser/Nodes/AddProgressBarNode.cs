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
    public class AddProgressBarNode : UINode
    {
        /// <summary>
        /// Defines the UIS identifier of this progressbar control. Required.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Defines the identifier of this progressbar control. Required.
        /// </summary>
        public int ID;

        /// <summary>
        /// Position of this progressbar control. Required.
        /// </summary>
        public Vector2 ProgressbarPosition;

        /// <summary>
        /// Defines a UIS image resource to associate as the foreground image for this button control.
        /// </summary>
        public string ForegroundImage = "";

        /// <summary>
        /// Defines a UIS image resource to associate as the background image for this button control.
        /// </summary>
        public string BackgroundImage = "";

        /// <summary>
        /// Defines the value associated with the start end of this progress bar control.
        /// </summary>
        public int? MinimumValue;

        /// <summary>
        /// Defines the value associated with the finish end of this progress bar control.
        /// </summary>
        public int? MaximumValue;

        /// <summary>
        /// Defines the initial value which this progress bar control is set to.
        /// </summary>
        public int? Value;

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
                    case AssignmentType.IDAssignment:
                        ID = ANode.NumberValue;
                        break;
                    case AssignmentType.PositionAssignment:
                        ProgressbarPosition = new Vector2(ANode.Array.Numbers[0], ANode.Array.Numbers[1]);
                        break;
                    case AssignmentType.ForegroundImageAssignment:
                        ForegroundImage = ANode.StrValue;
                        break;
                    case AssignmentType.BackgroundImageAssignment:
                        BackgroundImage = ANode.StrValue;
                        break;
                    case AssignmentType.MaxValueAssignment:
                        MaximumValue = ANode.NumberValue;
                        break;
                    case AssignmentType.MinValueAssignment:
                        MinimumValue = ANode.NumberValue;
                        break;
                    case AssignmentType.ValueAssignment:
                        Value = ANode.NumberValue;
                        break;
                }
            }

            AsString = "AddProgressBar";
        }
    }
}
