﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the UIParser library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using Irony.Parsing;

namespace UIParser.Nodes
{
    public class StartStatementNode : UINode
    {
        public LineContentNode LineContent { get; private set; }

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes.Count > 0)
            {
                if(nodes[0].AstNode != null)
                    LineContent = (LineContentNode)AddChild("LineContent", nodes[0]);
            }

            AsString = "StartStatement";
        }
    }
}
