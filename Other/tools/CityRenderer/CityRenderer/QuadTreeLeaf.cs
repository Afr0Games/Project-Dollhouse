using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cityrenderer
{
    public class QuadtreeLeaf : QuadTreeNode
    {
        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;

        public QuadtreeLeaf(float CellSize, BoundingBox boundingBox, Color[] heightMapColors, 
            GraphicsDevice graphicsDevice) : base(boundingBox)
        {
            float maxHeight = 100.0f;
            //int heightMapWidth = 2048;
            int heightMapWidth = 255;
            int heightmapHeight = 512;

            int sx = (int)(boundingBox.Min.X / CellSize);
            int sy = (int)(boundingBox.Min.Z / CellSize);
            int width = (int)((boundingBox.Max.X - boundingBox.Min.X) / CellSize) + (sx < 2016 ? 1 : 0);
            int height = (int)((boundingBox.Max.Z - boundingBox.Min.Z) / CellSize) + (sy < 2016 ? 1 : 0);

            // vertex buffer generation
            CityVertex[] vertices = new CityVertex[width * height];

            this.boundingBox.Min.Y = maxHeight;
            this.boundingBox.Max.Y = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    float depth = heightMapColors[(sy + y) * heightMapWidth + sx + x].R / 255.0f;
                    vertices[y * width + x] = new CityVertex();
                    vertices[y * width + x].Position = new Vector3(sx + x, depth, sy + y);
                    vertices[y * width + x].Normal = Vector3.Up;
                    vertices[y * width + x].TextureCoord = new Vector2((float)x / (heightMapWidth), (float)y / (heightmapHeight));
                    depth *= maxHeight;
                    this.boundingBox.Min.Y = MathHelper.Min(this.boundingBox.Min.Y, depth);
                    this.boundingBox.Max.Y = MathHelper.Max(this.boundingBox.Max.Y, depth);
                }
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(CityVertex), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<CityVertex>(vertices);

            // index buffer generation
            int indicesPerRow = width * 2 + 2;
            short[] indices = new short[indicesPerRow * (height - 1)];

            for (int i = 0; i < height - 1; ++i)
            {
                int index;
                for (int ii = 0; ii < width; ++ii)
                {
                    index = i * indicesPerRow + ii * 2;
                    indices[index] = (short)((i) * width + ii);
                    indices[index + 1] = (short)((i + 1) * width + ii);
                }

                index = i * indicesPerRow + width * 2;
                indices[index] = indices[index - 1];
                indices[index + 1] = (short)((i + 1) * width);
            }

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }
    }
}
