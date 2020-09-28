using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Cityrenderer
{
    /// <summary>
    /// This class represents the root and branches of the terrain's quadtree.
    /// </summary>
    public class QuadTreeNode
    {
        public BoundingBox boundingBox;
        public QuadTreeNode topLeft, topRight, bottomLeft, bottomRight;

        public QuadTreeNode(BoundingBox boundingBox)
        {
            this.boundingBox = boundingBox;
        }
    }
}
