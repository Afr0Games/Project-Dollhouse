/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
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
    public class DefineImageNode : UINode
    {
        public string Name = "";
        public string AssetID = "";

        public override void Accept(IUIVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override void InitChildren(ParseTreeNodeList nodes)
        {
            InitChildrenAsList(nodes);

            if (nodes[1].AstNode != null)
                Name = nodes[1].Token.Text;

            AssetID = nodes[2].ChildNodes[0].ChildNodes[0].ChildNodes[2].Token.Text.Replace("\"", "");
            AssetID = AssetID.Substring(2);

            AsString = "DefineImage";
        }
    }
}
