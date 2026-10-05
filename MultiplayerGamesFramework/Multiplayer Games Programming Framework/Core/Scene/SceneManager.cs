using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Game_Client.Core;
using System.Collections.Generic;
using Game_Client.GameCode.Scenes;

namespace Game_Client
{
    internal class SceneManager
    {
        public MainGame m_Game { get; private set; }
        public SpriteBatch m_SpriteBatch { get; private set; }
        public ContentManager m_ContentManager { get; private set; }
        List<Scene> m_Scenes;
        public Scene m_ActiveScene { get; private set; }

        public SceneManager(MainGame game)
        {
            m_Game = game;
            m_SpriteBatch = game.m_SpriteBatch;
            m_ContentManager = game.Content;

            m_Scenes = new List<Scene>();

           LoadScene(new MenuScene(this));
            //LoadScene(new ShipGameScene(this));
            //LoadScene(new PhysicsGameScene(this));
           // LoadScene(new EmptyGameSceneExample(this));
           // LoadScene(new PingPongScene(this));
        }

        public void Update(float deltaTime)
        {
            if (m_ActiveScene != null)
                m_ActiveScene.Update(deltaTime);
        }

        public void Draw(float deltaTime)
        {
            if (m_ActiveScene != null)
                m_ActiveScene.Draw(deltaTime);
        }

        public void UIDraw(float deltaTime)
        {
            if (m_ActiveScene != null)
                m_ActiveScene.UIDraw(deltaTime);
        }


        #region Scene Control
        public void LoadScene(Scene scene)
        {
            m_ActiveScene = scene;
            m_ActiveScene.LoadContent();
        }

        public void LoadSceneByName(string name)
        {
            for (int i = 0; i < m_Scenes.Count; ++i)
            {
                if (m_Scenes[i].m_Name == name)
                {
                    m_ActiveScene = m_Scenes[i];
                    break;
                }
            }

			m_ActiveScene.LoadContent();
		}

		public void AddScene(Scene scene)
        {
            m_Scenes.Add(scene);
        }

        public void AddAndLoadScene(Scene scene)
        {
            m_Scenes.Add(scene);
            m_ActiveScene = scene;
            m_ActiveScene.LoadContent();
        }

        public void RemoveScene(Scene scene)
        {
            m_Scenes.Remove(scene);
        }

        public void RemoveSceneByName(string name)
        {
            for (int i = 0; i < m_Scenes.Count; ++i)
            {
                if (m_Scenes[i].m_Name == name)
                {
                    m_Scenes.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion
    }
}
