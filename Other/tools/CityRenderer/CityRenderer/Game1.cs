using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing.Imaging;

namespace Cityrenderer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private InputHelper m_Input = new InputHelper();

        /*private Effect m_Effect;

        private Texture2D m_Heightmap, m_VertexColor;
        private Texture2D m_SandTex, m_GrassTex, m_RockTex, m_SnowTex;*/

        private Texture2D m_Elevation, m_VertexColor, m_TerrainType, m_ForestType, m_ForestDensity, m_RoadMap;
        private Texture2D m_Grass, m_Sand, m_Snow, m_Rock, m_Forest, m_DefaultHouse, m_Water;
        private Texture2D m_WhiteLine, m_StpWhiteLine;
        private Texture2D[] m_TransA = new Texture2D[30], m_TransB = new Texture2D[30];
        private Texture2D[] m_Roads = new Texture2D[16], m_RoadCorners = new Texture2D[16];
        public Texture2D Atlas, RoadAtlas, RoadCAtlas, TransAtlas;
        private int m_Width, m_Height;
        private int m_CityNumber = 15;
        private int m_MeshTris = 0;

        private Vector3 m_LightPosition = new Vector3();
        private float m_ViewOffX, m_ViewOffY, m_TargetVOffX, m_TargetVOffY;

        private bool m_Zoomed = false;
        private float m_ZoomProgress = 1.0f;

        private Vector2 m_MouseStart;

        private CityRenderer Renderer;

        private Grid m_Grid;
        private Effect m_PixelShader, m_VertexShader;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            /*graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;*/

            Content.RootDirectory = "Content";

            m_Grid = new Grid(this);
            m_Grid.CellSize = 4;
            m_Grid.Dimension = 512;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.GraphicsDevice.Viewport = new Viewport(0, 0, 800, 600);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            string CityStr = GlobalSettings.Default.StartupPath + "cities\\" + ((m_CityNumber >= 10) ? "city_00" + m_CityNumber.ToString() : "city_000" + m_CityNumber.ToString());

            Renderer = new CityRenderer(graphics.GraphicsDevice, CityStr, GlobalSettings.Default.StartupPath + "gamedata\\terrain\\newformat\\");
            Renderer.Initialize(Content.Load<Effect>("Effects"));

            m_Grid.LoadGraphicsContent();
            /*m_PixelShader = Content.Load<Effect>("PixelShader");
            m_VertexShader = Content.Load<Effect>("VTFDisplacement");*/

            /*m_Elevation = Content.Load<Texture2D>(CityStr + "\\elevation.bmp");
            m_Grass = LoadTexture(GlobalSettings.Default.StartupPath + "gamedata\\terrain\\newformat\\gr.tga");
            m_Rock = LoadTexture(GlobalSettings.Default.StartupPath + "gamedata\\terrain\\newformat\\rk.tga");
            m_Sand = LoadTexture(GlobalSettings.Default.StartupPath + "gamedata\\terrain\\newformat\\sd.tga");
            m_Snow = LoadTexture(GlobalSettings.Default.StartupPath + "gamedata\\terrain\\newformat\\sn.tga");*/
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
                return Texture2D.FromStream(graphics.GraphicsDevice, Png);
            }
            else
                return Texture2D.FromStream(graphics.GraphicsDevice, new FileStream(Path, FileMode.Open, FileAccess.Read));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Renderer.Update(m_Input);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Renderer.GenerateMesh();

            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone; //don't cull
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Renderer.Draw();

            /*m_VertexShader.Parameters["world"].SetValue(Matrix.Identity);
            m_VertexShader.Parameters["view"].SetValue(m_Camera.View);
            m_VertexShader.Parameters["proj"].SetValue(m_Camera.Projection);
            m_VertexShader.Parameters["maxHeight"].SetValue(128);
            m_VertexShader.Parameters["displacementMap"].SetValue(m_Elevation);

            m_PixelShader.Parameters["displacementMap"].SetValue(m_Elevation);
            m_PixelShader.Parameters["sandMap"].SetValue(m_Sand);
            m_PixelShader.Parameters["grassMap"].SetValue(m_Grass);
            m_PixelShader.Parameters["rockMap"].SetValue(m_Rock);
            m_PixelShader.Parameters["snowMap"].SetValue(m_Snow);

            foreach (EffectPass VertexPass in m_VertexShader.CurrentTechnique.Passes)
            {
                foreach (EffectPass PixelPass in m_PixelShader.CurrentTechnique.Passes)
                {
                    VertexPass.Apply();
                    PixelPass.Apply();
                    m_Grid.Draw();
                }
            }*/

            base.Draw(gameTime);
        }
    }
}
