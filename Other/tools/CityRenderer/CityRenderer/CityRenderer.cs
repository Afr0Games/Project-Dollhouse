﻿using System;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cityrenderer
{
    public enum TerrainType : int
    {
        Grass = 0,
        Rock = 1,
        Sand = 2,
        Snow = 3,
        Water = 4,
        Unused = 5
    }

    public class CityRenderer
    {
        private string m_CityPath = "", m_TerrainPath = "";
        private GraphicsDevice m_Device;

        private Texture2D m_Elevation, m_TerrainType, m_VertexColor;
        private Texture2D m_Grass, m_Rock, m_Sand, m_Snow, m_Water;

        private Camera m_Camera;
        private CityCameraController m_CController;

        private Vector2 m_MouseStart;

        private Effect m_Effect;

        private int m_TerrainWidth, m_TerrainHeight;
        private float[,] m_HeightData;
        private int[] m_Indices;
        private CityVertex[] m_Vertices;
        private Matrix m_ViewMatrix, m_ProjectionMatrix;

        private QuadTreeNode m_QuadTreeRoot; 

        public CityRenderer(GraphicsDevice Device, string CityPath, string TerrainPath)
        {
            m_CityPath = CityPath;
            m_TerrainPath = TerrainPath;
            m_Device = Device;
        }

        public void Initialize(Effect FX)
        {
            m_Effect = FX;

            m_Elevation = LoadTexture(m_CityPath + "\\elevation.bmp");
            m_Elevation = RotateTexture(m_Elevation, 256);
            m_TerrainType = LoadTexture(m_CityPath + "\\terraintype.bmp");
            m_TerrainType = RotateTexture(m_TerrainType, 256);
            m_VertexColor = LoadTexture(m_CityPath + "\\vertexcolor.bmp");
            m_VertexColor = RotateTexture(m_VertexColor, 256);

            m_Grass = LoadTexture(m_TerrainPath + "gr.tga");
            m_Rock = LoadTexture(m_TerrainPath + "rk.tga");
            m_Water = LoadTexture(m_TerrainPath + "wt.tga");
            m_Sand = LoadTexture(m_TerrainPath + "sd.tga");
            m_Snow = LoadTexture(m_TerrainPath + "sn.tga");

            m_Camera = new Camera(m_Device);
            m_CController = new CityCameraController(m_Camera);

            LoadHeightData(m_Elevation);
            SetUpVertices();
            SetUpIndices();
            InitializeNormals();

            m_QuadTreeRoot = BuildQuadtree(m_Elevation);

            m_Elevation.Dispose();
        }

        private void LoadHeightData(Texture2D HeightMap)
        {
            m_TerrainWidth = m_Elevation.Width;
            m_TerrainHeight = m_Elevation.Height;

            Color[] InitialHeightMapColors = new Color[m_TerrainWidth * m_TerrainHeight];
            HeightMap.GetData(InitialHeightMapColors);

            //Color[] HeightMapColors = new Color[(m_TerrainWidth * 2) * (m_TerrainHeight * 2)];
            Color[] HeightMapColors = new Color[(m_TerrainWidth) * (m_TerrainHeight)];

            for (int i = 0; i < InitialHeightMapColors.Length; i++)
            {
                HeightMapColors[i] = InitialHeightMapColors[i];
                //HeightMapColors[i + 1] = InitialHeightMapColors[i];
            }

            m_HeightData = new float[m_TerrainWidth, m_TerrainHeight];
            for (int x = 0; x < ((m_TerrainWidth) - 1); x++)
            {
                for (int y = 0; y < ((m_TerrainHeight) - 1); y++)
                {
                    m_HeightData[x, y] = HeightMapColors[x + y * m_TerrainWidth].R / 12.0f;
                    m_HeightData[x + 1, y + 1] = HeightMapColors[(x + 1) + (y + 1) * m_TerrainWidth].R / 12.0f;
                }
            }
        }

        /// <summary>
        /// Recursively subdivides the terrain quadtree.
        /// </summary>
        /// <param name="depth">The current zero-based depth of the algorithm.</param>
        /// <param name="heightMapColors">The array of heightmap colors.</param>
        /// <param name="node">The node of the quadtree that will be subdivided.</param>
        protected void BuildQuadtree(Color[] heightMapColors, QuadTreeNode node, int depth = 0)
        {
            Vector3 min = node.boundingBox.Min;
            Vector3 max = node.boundingBox.Max;
            Vector3 middle = (min + max) / 2;
            BoundingBox topLeft = new BoundingBox(min, middle);
            BoundingBox topRight = new BoundingBox(new Vector3(middle.X, 0, min.Z), new Vector3(max.X, 0, middle.Z));
            BoundingBox bottomLeft = new BoundingBox(new Vector3(min.X, 0, middle.Z), new Vector3(middle.X, 0, max.Z));
            BoundingBox bottomRight = new BoundingBox(middle, max);

            if (depth < 5)
            {
                BuildQuadtree(heightMapColors, node.topLeft = new QuadTreeNode(topLeft), depth + 1);
                BuildQuadtree(heightMapColors, node.topRight = new QuadTreeNode(topRight), depth + 1);
                BuildQuadtree(heightMapColors, node.bottomLeft = new QuadTreeNode(bottomLeft), depth + 1);
                BuildQuadtree(heightMapColors, node.bottomRight = new QuadTreeNode(bottomRight), depth + 1);
            }
            else
            {
                node.topLeft = new QuadtreeLeaf(m_CellSize, topLeft, heightMapColors, m_Device);
                node.topRight = new QuadtreeLeaf(m_CellSize, topRight, heightMapColors, m_Device);
                node.bottomLeft = new QuadtreeLeaf(m_CellSize, bottomLeft, heightMapColors, m_Device);
                node.bottomRight = new QuadtreeLeaf(m_CellSize, bottomRight, heightMapColors, m_Device);
            }

            float[] mins = new float[4] { node.topLeft.boundingBox.Min.Y,
                node.topRight.boundingBox.Min.Y,
                node.bottomLeft.boundingBox.Min.Y,
                node.bottomRight.boundingBox.Min.Y };
            Array.Sort(mins);
            float[] maxes = new float[4] { node.topLeft.boundingBox.Max.Y,
                node.topRight.boundingBox.Max.Y,
                node.bottomLeft.boundingBox.Max.Y,
                node.bottomRight.boundingBox.Max.Y };
            Array.Sort(maxes);

            node.boundingBox.Min.Y = MathHelper.Min(100.0f, mins[0]);
            node.boundingBox.Max.Y = MathHelper.Max(node.boundingBox.Max.Y, maxes[3]);
        }

        /// <summary>
        /// Builds a terrain quadtree from the heightmap.
        /// </summary>
        /// <param name="heightMap">The heightmap of the terrain.</param>
        protected QuadTreeNode BuildQuadtree(Texture2D heightMap)
        {
            QuadTreeNode root = new QuadTreeNode(new BoundingBox(Vector3.Zero, new Vector3(heightMap.Width * m_CellSize, 0.0f, heightMap.Height * m_CellSize)));

            Color[] heightMapColors = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData(heightMapColors);

            BuildQuadtree(heightMapColors, root);

            return root;
        }

        private float m_CellSize = 4.5f;

        private void SetUpVertices()
        {
            m_Vertices = new CityVertex[(m_TerrainWidth) * (m_TerrainHeight)];
            for (int x = 0; x < (m_TerrainWidth); x++)
            {
                for (int y = 0; y < (m_TerrainHeight); y++)
                {
                    //m_Vertices[x + y * (m_TerrainWidth /** 2*/)].Position = new Vector3(x, m_HeightData[x, y], -y);
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].Position = new Vector3((x - 256 / 2.0f) * m_CellSize, (m_HeightData[x, y]) * m_CellSize, (y - 512 / 2.0f) * m_CellSize);
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].TextureCoord = new Vector2((float)x / (m_TerrainWidth), (float)y / (m_TerrainHeight));
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].Normal = Vector3.Up;
                }
            }
        }

        private void SetUpIndices()
        {
            m_Indices = new int[((m_TerrainWidth) - 1) * ((m_TerrainHeight) - 1) * 6];
            int counter = 0;
            for (int y = 0; y < (m_TerrainHeight) - 1; y++)
            {
                for (int x = 0; x < (m_TerrainWidth) - 1; x++)
                {
                    int lowerLeft = x + y * (m_TerrainWidth);
                    int lowerRight = (x + 1) + y * (m_TerrainWidth);
                    int topLeft = x + (y + 1) * (m_TerrainWidth);
                    int topRight = (x + 1) + (y + 1) * (m_TerrainWidth);

                    m_Indices[counter++] = topLeft;
                    m_Indices[counter++] = lowerRight;
                    m_Indices[counter++] = lowerLeft;

                    m_Indices[counter++] = topLeft;
                    m_Indices[counter++] = topRight;
                    m_Indices[counter++] = lowerRight;
                }
            }
        }

        private void InitializeNormals()
        {
            for (int i = 0; i < m_Vertices.Length - 1; i++)
                m_Vertices[i].Normal = Vector3.Zero;

            for (int i = 0; i < m_Indices.Length - 1; i += 6)
            {
                int index0 = m_Indices[i];
                int index1 = m_Indices[i + 1];
                int index2 = m_Indices[i + 2];

                int index3 = m_Indices[i + 3];
                int index4 = m_Indices[i + 4];
                int index5 = m_Indices[i + 5];

                Vector3 side0 = m_Vertices[index0].Position - m_Vertices[index2].Position;
                Vector3 side1 = m_Vertices[index0].Position - m_Vertices[index1].Position;
                Vector3 normal = Vector3.Cross(side0, side1);

                Vector3 side2 = m_Vertices[index3].Position - m_Vertices[index5].Position;
                Vector3 side3 = m_Vertices[index3].Position - m_Vertices[index4].Position;
                Vector3 normal2 = Vector3.Cross(side2, side3);

                m_Vertices[index0].Normal += normal;
                m_Vertices[index1].Normal += normal;
                m_Vertices[index2].Normal += normal;

                m_Vertices[index3].Normal += normal2;
                m_Vertices[index4].Normal += normal2;
                m_Vertices[index5].Normal += normal2;
            }
        }

        /// <summary>
        /// Rotates a texture by 44.7 degrees, in order to make it easier to render.
        /// </summary>
        /// <returns>A texture rotated by 44.7 degrees.</returns>
        private Texture2D RotateTexture(Texture2D TextureToRotate, int TextureWidth = 512)
        {
            SpriteBatch SBatch = new SpriteBatch(m_Device);
            RenderTarget2D RTarget = new RenderTarget2D(m_Device, TextureWidth, 512);

            m_Device.Clear(Color.Black);
            SBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            m_Device.SetRenderTarget(RTarget);
            SBatch.Draw(TextureToRotate, new Vector2(-275, 250), null, Color.White, (float)MathHelper.ToRadians(-44.77f), new Vector2(0, 0), 1.26f, SpriteEffects.None, 0);
            SBatch.End();

            m_Device.SetRenderTarget(null);
            SBatch = null;

            return RTarget;
        }

        /// <summary>
        /// Loads a texture from the specified path.
        /// </summary>
        /// <param name="Path">The path from which to load a texture.</param>
        /// <returns>A Texture2D instance.</returns>
        private Texture2D LoadTexture(string Path)
        {
            if (Path.Contains(".tga"))
            {
                MemoryStream Png = new MemoryStream();
                Paloma.TargaImage TGA = new Paloma.TargaImage(new FileStream(Path, FileMode.Open, FileAccess.Read));
                TGA.Image.Save(Png, ImageFormat.Png);
                Png.Position = 0;
                return Texture2D.FromStream(m_Device, Png);
            }
            else
                return Texture2D.FromStream(m_Device, new FileStream(Path, FileMode.Open, FileAccess.Read));
        }

        public void Update(InputHelper Input)
        {
            Input.Update();
            m_CController.Update(Input);
        }

        /// <summary>
        /// Recursively draws the quadtree terrain nodes that are in the viewing frustum.
        /// </summary>
        /// <param name="node">The node or leaf to draw.</param>
        protected void DrawQuadtree(QuadTreeNode node)
        {
            if (!(new BoundingFrustum(Matrix.Identity * m_CController.View * m_CController.Projection)).Intersects(node.boundingBox))
                return;

            if (node.GetType() == typeof(QuadtreeLeaf))
            {
                QuadtreeLeaf leaf = (QuadtreeLeaf)node;

                m_Device.SetVertexBuffer(leaf.vertexBuffer);
                m_Device.Indices = leaf.indexBuffer;

                m_Device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, leaf.indexBuffer.IndexCount - 4);
            }
            else
            {
                DrawQuadtree(node.topLeft);
                DrawQuadtree(node.topRight);
                DrawQuadtree(node.bottomLeft);
                DrawQuadtree(node.bottomRight);
            }
        }

        public void Draw()
        {
            Matrix WorldMatrix = Matrix.CreateTranslation((-m_TerrainWidth) / 2.0f, 0, (m_TerrainHeight) / 2.0f);

            m_Effect.CurrentTechnique = m_Effect.Techniques["Textured"];
            m_Effect.Parameters["Blend"].SetValue(m_VertexColor);
            m_Effect.Parameters["TerrainType"].SetValue(m_TerrainType);
            m_Effect.Parameters["Grass"].SetValue(m_Grass);
            m_Effect.Parameters["Rock"].SetValue(m_Rock);
            m_Effect.Parameters["Sand"].SetValue(m_Sand);
            m_Effect.Parameters["Snow"].SetValue(m_Snow);
            m_Effect.Parameters["Water"].SetValue(m_Water);
            m_Effect.Parameters["xView"].SetValue(m_CController.View);
            m_Effect.Parameters["xProjection"].SetValue(m_CController.Projection);
            m_Effect.Parameters["xWorld"].SetValue(WorldMatrix);

            RasterizerState RS = new RasterizerState();
            RS.CullMode = CullMode.None;
            //RS.FillMode = FillMode.WireFrame;
            m_Device.RasterizerState = RS;

            m_Device.Clear(Color.CornflowerBlue);

            foreach (EffectPass Pass in m_Effect.CurrentTechnique.Passes)
            {
                Pass.Apply();

                m_Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, m_Vertices, 0, m_Vertices.Length,
                    m_Indices, 0, m_Indices.Length / 3, CityVertex.VertexElements);
                //DrawQuadtree(m_QuadTreeRoot);
            }
        }
    }
}