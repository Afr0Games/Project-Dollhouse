using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cityrenderer
{
    /// <summary>
    /// Represents a MeshVertex that makes up a face.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MeshVertex : IVertexType
    {
        public Vector3 Coord;
        /** UV Mapping **/
        public Vector2 TextureCoord;
        public Vector2 Texture2Coord;
        public Vector2 Texture3Coord;
        public Vector2 UVBCoord;
        public Vector2 RoadCoord;
        public Vector2 RoadCCoord;

        public static int SizeInBytes = sizeof(float) * 15;

        public static readonly VertexDeclaration VertexElements = new VertexDeclaration(

            new VertexElement(0, VertexElementFormat.Vector3,
                VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * (3 + 2), VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * (3 + 4), VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(sizeof(float) * (3 + 6), VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(sizeof(float) * (3 + 8), VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(sizeof(float) * (3 + 10), VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 5)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexElements; }
        }
    }

    public struct CityVertex : IVertexType
    {
        public Vector3 Position;
        public Vector4 Color;
        //public Vector2 TextureCoord;
        public Vector2 TerrainTypeCoord;
        public Vector2 GrassCoord;
        public Vector2 RockCoord;
        public Vector2 SandCoord;
        public Vector2 BlendCoord;
        public Vector2 SnowCoord;
        public Vector2 WaterCoord;
        public Vector3 Normal;

        //public static int SizeInBytes = sizeof(float) * 10;
        public static int SizeInBytes = sizeof(float) * 24;

        public static readonly VertexDeclaration VertexElements = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3,
                VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector4,
                VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 9, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(sizeof(float) * 13, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(sizeof(float) * 15, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 4),
            new VertexElement(sizeof(float) * 17, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 5),
            new VertexElement(sizeof(float) * 19, VertexElementFormat.Vector2,
                VertexElementUsage.TextureCoordinate, 6),
            new VertexElement(sizeof(float) * /*7*/21, VertexElementFormat.Vector3,
                VertexElementUsage.Normal, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexElements; }
        }
    }
}