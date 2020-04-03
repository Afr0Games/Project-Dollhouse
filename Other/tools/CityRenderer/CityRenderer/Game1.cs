using System;
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

        private InputHelper m_Input = new InputHelper();

        private Texture2D[] m_TransA = new Texture2D[30], m_TransB = new Texture2D[30];
        private Texture2D[] m_Roads = new Texture2D[16], m_RoadCorners = new Texture2D[16];
        public Texture2D Atlas, RoadAtlas, RoadCAtlas, TransAtlas;
        private int m_CityNumber = 15;

        private CityRenderer Renderer;

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
        }

        /// <summary>
        /// Rotates a texture by 44.7 degrees, in order to make it easier to render.
        /// </summary>
        /// <returns>A texture rotated by 44.7 degrees.</returns>
        private Texture2D RotateTexture(Texture2D TextureToRotate, int TextureWidth = 512)
        {
            SpriteBatch SBatch = new SpriteBatch(graphics.GraphicsDevice);
            RenderTarget2D RTarget = new RenderTarget2D(graphics.GraphicsDevice, TextureWidth, 512);

            graphics.GraphicsDevice.Clear(Color.Black);
            SBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            graphics.GraphicsDevice.SetRenderTarget(RTarget);
            SBatch.Draw(TextureToRotate, new Vector2(-275, 250), null, Color.White, (float)-44.77, new Vector2(0, 0), 1.26f, SpriteEffects.None, 0);
            SBatch.End();

            graphics.GraphicsDevice.SetRenderTarget(null);
            SBatch = null;

            return RTarget;
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
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone; //don't cull
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Renderer.Draw();
            //Renderer.DrawGrid();

            base.Draw(gameTime);
        }
    }
}
