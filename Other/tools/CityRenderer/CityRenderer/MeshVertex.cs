using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cityrenderer
{
    public struct CityVertex : IVertexType
    {
        public Vector3 Position;
        public Vector4 Color;
        public Vector2 TextureCoord;
        public Vector3 Normal;

        //public static int SizeInBytes = sizeof(float) * 10;
        public static int SizeInBytes = sizeof(float) * 12;

        public static readonly VertexDeclaration VertexElements = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3,
                VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector4,
                VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * /*7*/9, VertexElementFormat.Vector3,
                VertexElementUsage.Normal, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexElements; }
        }
    }
}