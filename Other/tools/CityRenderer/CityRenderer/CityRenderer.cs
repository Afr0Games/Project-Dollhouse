using System;
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

        private void SetUpVertices()
        {
            m_Vertices = new CityVertex[(m_TerrainWidth) * (m_TerrainHeight)];
            for (int x = 0; x < (m_TerrainWidth); x++)
            {
                for (int y = 0; y < (m_TerrainHeight); y++)
                {
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].Position = new Vector3(x, m_HeightData[x, y], -y);
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].Normal = Vector3.Up;
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].TerrainTypeCoord = new Vector2((float)x / (m_TerrainWidth), (float)y / (m_TerrainHeight));
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].BlendCoord = new Vector2((float)x / (m_TerrainWidth), (float)y / (m_TerrainHeight));
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].GrassCoord = new Vector2((float)x / (m_TerrainWidth), (float)y / (m_TerrainHeight));
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].RockCoord = new Vector2((float)x / (m_TerrainWidth), (float)y / (m_TerrainHeight));
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].SandCoord = new Vector2((float)x / (m_TerrainWidth), (float)y / (m_TerrainHeight));
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].SnowCoord = new Vector2((float)x / (m_TerrainWidth), (float)y / (m_TerrainHeight));
                    m_Vertices[x + y * (m_TerrainWidth /** 2*/)].WaterCoord = new Vector2((float)x / (m_TerrainWidth), (float)y / (m_TerrainHeight));
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
            }
        }
    }
}