using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Gonzo;
using Files.Manager;
using Sound;

namespace GonzoTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private HitVM SoundManager;
        private ScreenManager m_ScrManager;
        private SpriteFont[] m_Fonts = new SpriteFont[5];
        private InputHelper m_Input = new InputHelper();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GlobalSettings.Default.ScreenWidth;
            graphics.PreferredBackBufferHeight = GlobalSettings.Default.ScreenHeight;

            Resolution.Init(ref graphics);
            Resolution.SetResolution(GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight, GlobalSettings.Default.Fullscreen);
            Resolution.SetVirtualResolution(800, 600);

            this.Window.Title = "The Sims Online";
            this.Window.TextInput += Window_TextInput;

            Content.RootDirectory = "Content";
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            if (m_ScrManager != null)
                m_ScrManager.ReceivedTextInput(sender, e);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            StringManager.Initialize(GlobalSettings.Default.StartupPath, "gamedata\\uitext", "english");
            IsMouseVisible = true;

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

            FileManager.OnHundredPercentCompleted += FileManager_OnHundredPercentCompleted;
            FileManager.Initialize(this, GlobalSettings.Default.StartupPath);

            m_Fonts[0] = Content.Load<SpriteFont>("ProjectDollhouse_9px");
            m_Fonts[1] = Content.Load<SpriteFont>("ProjectDollhouse_10px");
            m_Fonts[1] = Content.Load<SpriteFont>("ProjectDollhouse_11px");
            m_Fonts[2] = Content.Load<SpriteFont>("ProjectDollhouse_12px");
            m_Fonts[3] = Content.Load<SpriteFont>("ProjectDollhouse_14px");
            m_Fonts[4] = Content.Load<SpriteFont>("ProjectDollhouse_16px");
            m_ScrManager = new ScreenManager(GraphicsDevice, m_Fonts, m_Input);
        }

        private void FileManager_OnHundredPercentCompleted()
        {
            SoundManager = new HitVM(GlobalSettings.Default.StartupPath);
            //m_ScrManager.AddScreen(new CreditsScreen(m_ScrManager, spriteBatch));
            //m_ScrManager.AddScreen(new SASScreen(m_ScrManager, spriteBatch));
            m_ScrManager.AddScreen(new CASScreen(m_ScrManager, spriteBatch));
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

            m_ScrManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Resolution.BeginDraw();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, 
                RasterizerState.CullCounterClockwise, null, Resolution.getTransformationMatrix());
            m_ScrManager.Draw();
            spriteBatch.End();

            //Reset device to defaults before rendering...
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            GraphicsDevice.Viewport = new Viewport(0, 0, GlobalSettings.Default.ScreenWidth, GlobalSettings.Default.ScreenHeight);
            //Note: Depth is the depth at which to render...
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 1);
            m_ScrManager.Draw3D();

            HitVM.Step();

            base.Draw(gameTime);
        }
    }
}
