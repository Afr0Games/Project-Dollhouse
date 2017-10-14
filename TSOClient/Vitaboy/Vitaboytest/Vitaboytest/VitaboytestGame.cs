using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Files;
using Files.Manager;
using Vitaboy;

namespace Vitaboytest
{
    public class VitaboytestGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        private float m_RotationX = 0.0f, m_RotationY = 0.8f, m_RotationZ = 0.0f;
        private float m_Scale = 0.8f;
        private Matrix mViewMat, mWorldMat, mProjectionMat;
        private AdultAvatar m_Avatar;
        private Effect m_Shader;

        public VitaboytestGame() : base()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.DeviceCreated += M_Graphics_DeviceCreated;
            Content.RootDirectory = "Content";

            mViewMat = mWorldMat = mProjectionMat = Matrix.Identity;
        }

        private void M_Graphics_DeviceCreated(object sender, EventArgs e)
        {
            // Create camera and projection matrix
            mWorldMat = Matrix.Identity;
            mViewMat = Matrix.CreateLookAt(Vector3.Right * 6.0f, Vector3.Zero, Vector3.Forward);
            mProjectionMat = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 4.0f,
                    (float)m_Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth /
                    (float)m_Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight, 1.0f, 100.0f);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            FileManager.OnHundredPercentCompleted += FileManager_OnHundredPercentCompleted;
            FileManager.Initialize(this, GlobalSettings.Default.StartupPath);

            m_Shader = Content.Load<Effect>("Vitaboy");
        }

        /// <summary>
        /// Filemanager was initialized!
        /// </summary>
        private void FileManager_OnHundredPercentCompleted()
        {
            m_Avatar = new AdultAvatar(m_Graphics.GraphicsDevice, m_Shader);
            //m_Avatar.ChangeOutfit(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.fab001_sl__pjs4), SkinType.Medium);
            m_Avatar.ChangeOutfit(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.mab000_robin), SkinType.Medium);
            m_Avatar.SetHead(FileManager.GetOutfit((ulong)FileIDs.OutfitsFileIDs.mah020_willw), SkinType.Medium);
            m_Avatar.Animation = FileManager.GetAnimation(0x5f00000007);
        }

        protected override void Update(GameTime gameTime)
        {
            mWorldMat = Matrix.Identity * Matrix.CreateScale(m_Scale) * Matrix.CreateRotationX(m_RotationX) *
                Matrix.CreateRotationY(m_RotationY) * Matrix.CreateRotationZ(m_RotationZ);

            if (m_Avatar != null)
            {
                if(m_Avatar.Animation != null)
                    m_Avatar.AdvanceFrame(m_Avatar.Animation, 0.03f);

                m_Avatar.ComputeBonePositions(m_Avatar.Skel.RootBone, mWorldMat);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            m_Graphics.GraphicsDevice.Clear(Color.AliceBlue);

            if (m_Avatar != null)
                m_Avatar.Render(mViewMat, mWorldMat, mProjectionMat);

            base.Draw(gameTime);
        }
    }
}
