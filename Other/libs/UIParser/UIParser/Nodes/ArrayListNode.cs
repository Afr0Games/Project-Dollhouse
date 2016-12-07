/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the UIParser library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System.Collections.Generic;
using Irony.Parsing;

namespace UIParser.Nodes
{
    public class ArrayListNode : UINode
    {
        public List<int> Numbers = new List<int>();

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            //InitChildrenAsList(nodes);
            foreach(ParseTreeNode Node in nodes)
            {
                if (Node.AstNode != null)
                    Numbers.Add((int)Node.Token.Value);
            }

            AsString = "ArrayList";
        }
    }
}
