using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cityrenderer
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Grid
    {
        public Grid(Game game)
        {
            this.game = game;
        }
        Game game;

        GraphicsDevice device;
        VertexBuffer vb;
        IndexBuffer ib;
        VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[4];
        int[] m_Indices = new int[6];

        private VertexDeclaration m_VertexDecl;

        private float m_CellSize = 4;

        public float CellSize
        {
            get { return m_CellSize; }
            set { m_CellSize = value; }
        }

        private short m_Dimension = 512;

        public short Dimension
        {
            get { return m_Dimension; }
            set { m_Dimension = value; }
        }

        public void GenerateStructures()
        {
            vertices = new VertexPositionNormalTexture[(m_Dimension) * (m_Dimension)];
            m_Indices = new int[m_Dimension * m_Dimension * 6];
            for (int i = 0; i < m_Dimension; i++)
            {
                for (int j = 0; j < m_Dimension; j++)
                {
                    VertexPositionNormalTexture vert = new VertexPositionNormalTexture();
                    vert.Position = new Vector3((i - m_Dimension / 2.0f) * m_CellSize, 0, (j - m_Dimension / 2.0f) * m_CellSize);
                    vert.Normal = Vector3.Up;
                    vert.TextureCoordinate = new Vector2((float)i / m_Dimension, (float)j / m_Dimension);
                    vertices[i * (m_Dimension) + j] = vert;
                }
            }

            for (int i = 0; i < m_Dimension; i++)
            {
                for (int j = 0; j < m_Dimension; j++)
                {
                    m_Indices[6 * (i * m_Dimension + j)] = (i * (m_Dimension) + j);
                    m_Indices[6 * (i * m_Dimension + j) + 1] = (i * (m_Dimension) + j + 1);
                    m_Indices[6 * (i * m_Dimension + j) + 2] = ((i + 1) * (m_Dimension) + j + 1);

                    m_Indices[6 * (i * m_Dimension + j) + 3] = (i * (m_Dimension) + j);
                    m_Indices[6 * (i * m_Dimension + j) + 4] = ((i + 1) * (m_Dimension) + j + 1);
                    m_Indices[6 * (i * m_Dimension + j) + 5] = ((i + 1) * (m_Dimension) + j);
                }
            }

        }

        public void Draw()
        {
            IGraphicsDeviceService igs = (IGraphicsDeviceService)game.Services.GetService(typeof(IGraphicsDeviceService));
            device = igs.GraphicsDevice;

            device.SetVertexBuffer(vb, 0);
            device.Indices = ib;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (m_Dimension + 1) * (m_Dimension + 1), 0, 2 * m_Dimension * m_Dimension);

        }

        public void LoadGraphicsContent()
        {
            GenerateStructures();

            IGraphicsDeviceService igs = (IGraphicsDeviceService)game.Services.GetService(typeof(IGraphicsDeviceService));
            device = igs.GraphicsDevice;

            vb = new VertexBuffer(device, typeof(VertexPositionNormalTexture), (m_Dimension) * (m_Dimension), BufferUsage.WriteOnly);
            ib = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, 6 * m_Dimension * m_Dimension * sizeof(int), BufferUsage.WriteOnly);
            vb.SetData(vertices);
            ib.SetData(m_Indices);
        }
    }
}