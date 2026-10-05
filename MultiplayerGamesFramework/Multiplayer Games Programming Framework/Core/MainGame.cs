using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game_Client.Core.Utilities;
using Myra;

namespace Game_Client.Core
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        public SpriteBatch m_SpriteBatch { get; private set; }

        SceneManager m_SceneManager;

        public MainGame()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.PreferredBackBufferWidth = Graphics.InitScreenWidth;
            m_Graphics.PreferredBackBufferHeight = Graphics.InitScreenHeight;
            IsFixedTimeStep = true;
            MyraEnvironment.Game = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            GameCode.ClientNetworking.m_Instance.OnClientIDUpdate += UpdateWindowNameWithClientID;
        }

        protected override void Initialize()
        {
            Graphics.GraphicsDevice = m_Graphics.GraphicsDevice;
            base.Initialize();
            Window.Title = Graphics.DefaultWindowName + " | No client ID";
        }

        protected override void LoadContent()
        {
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            m_SceneManager = new SceneManager(this);
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            m_SceneManager.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            m_SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: Camera.m_Matrix);
            m_SceneManager.Draw(deltaTime);
            m_SpriteBatch.End();

            m_SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: Camera.m_Matrix);
            m_SceneManager.UIDraw(deltaTime);
            m_SpriteBatch.End();

            base.Draw(gameTime);
        }

        void UpdateWindowNameWithClientID(int id)
        {
           Window.Title = Graphics.DefaultWindowName + " | client ID " + id;
        }
    }
}




