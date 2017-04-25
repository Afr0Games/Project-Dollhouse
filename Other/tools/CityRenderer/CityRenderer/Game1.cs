using System.IO;
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

        private Camera m_Camera;
        private CityCameraController m_CameraController;
        private InputHelper m_Input = new InputHelper();

        private Effect m_Effect;

        private Texture2D m_Heightmap, m_VertexColor;
        private Texture2D m_SandTex, m_GrassTex, m_RockTex, m_SnowTex;

        private Grid m_Grid;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            /*graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;*/

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_Camera = new Camera(graphics.GraphicsDevice);
            m_CameraController = new CityCameraController(m_Camera);
            m_Grid = new Grid(this);
            m_Grid.CellSize = 4;
            m_Grid.Dimension = 512;

            graphics.GraphicsDevice.Viewport = new Viewport(0, 0, 800, 600);

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

            m_Effect = Content.Load<Effect>("VTFDisplacement");

            m_Heightmap = Texture2D.FromStream(graphics.GraphicsDevice, 
                File.Open(GlobalSettings.Default.StartupPath + "cities\\city_0026\\elevation.bmp", 
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            Paloma.TargaImage TImg = new Paloma.TargaImage(File.Open(GlobalSettings.Default.StartupPath +
                "gamedata\\terrain\\newformat\\sd.tga", FileMode.Open, FileAccess.ReadWrite));
            MemoryStream ImgStream = new MemoryStream();
            TImg.Image.Save(ImgStream, ImageFormat.Bmp);

            m_SandTex = Texture2D.FromStream(graphics.GraphicsDevice, ImgStream);

            TImg = new Paloma.TargaImage(File.Open(GlobalSettings.Default.StartupPath + 
                "gamedata\\terrain\\newformat\\gr.tga", FileMode.Open, FileAccess.ReadWrite));
            ImgStream = new MemoryStream();
            TImg.Image.Save(ImgStream, ImageFormat.Bmp);

            m_GrassTex = Texture2D.FromStream(graphics.GraphicsDevice, ImgStream);

            TImg = new Paloma.TargaImage(File.Open(GlobalSettings.Default.StartupPath + 
                "gamedata\\terrain\\newformat\\rk.tga", FileMode.Open, FileAccess.ReadWrite));
            ImgStream = new MemoryStream();
            TImg.Image.Save(ImgStream, ImageFormat.Bmp);

            m_RockTex = Texture2D.FromStream(graphics.GraphicsDevice, ImgStream);

            TImg = new Paloma.TargaImage(File.Open(GlobalSettings.Default.StartupPath + 
                "gamedata\\terrain\\newformat\\sn.tga", FileMode.Open, FileAccess.ReadWrite));
            ImgStream = new MemoryStream();
            TImg.Image.Save(ImgStream, ImageFormat.Bmp);

            m_SnowTex = Texture2D.FromStream(graphics.GraphicsDevice, ImgStream);

            m_Grid.LoadGraphicsContent();
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

            // TODO: Add your update logic here
            m_Input.Update();
            m_CameraController.Update(m_Input);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //This sets DepthBufferEnable and DepthBufferWriteEnable.
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            m_Effect.Parameters["world"].SetValue(Matrix.Identity);
            m_Effect.Parameters["view"].SetValue(m_CameraController.View);
            m_Effect.Parameters["proj"].SetValue(m_CameraController.Projection);
            m_Effect.Parameters["maxHeight"].SetValue(128f);

            m_Effect.Parameters["DisplacementMap"].SetValue(m_Heightmap);
            m_Effect.Parameters["SandMap"].SetValue(m_SandTex);
            m_Effect.Parameters["GrassMap"].SetValue(m_GrassTex);
            m_Effect.Parameters["RockMap"].SetValue(m_RockTex);
            m_Effect.Parameters["SnowMap"].SetValue(m_SnowTex);

            foreach (EffectPass pass in m_Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_Grid.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
