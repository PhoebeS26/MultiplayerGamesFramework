using Game_Client.Core.Utilities;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Game_Client.GameCode.Components;
using Game_Client.GameCode.Prefabs;

namespace Game_Client.GameCode.Scenes
{
    internal class EmptyGameSceneExample : Scene
    {
        GameModeState m_GameModeState;

        GameObject m_LocalPaddle;    // Your paddle that the local player controls
        GameObject m_RemotePaddle;   // The paddle controlled by the other player/network
        GameObject m_Ball;           // The ping pong ball


        public EmptyGameSceneExample(SceneManager manager) : base(manager)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            try
            {
                m_LocalPaddle = GameObject.Instantiate<GameObject>(this, new Transform(new Vector2(-400f, 0f), new Vector2(1f, 4f), 0f));
                m_LocalPaddle.AddComponent(new SpriteRenderer(m_LocalPaddle, "Ship"));
                Rigidbody rb = m_LocalPaddle.AddComponent(new Rigidbody(m_LocalPaddle, BodyType.Kinematic, 10, new Vector2(0.5f, 2f)));
               // m_LocalPaddle.AddComponent(new LocalPaddleMovement(m_LocalPaddle, 200f));
                Debug.WriteLine("Local Paddle created successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error creating paddle: " + ex);
            }


        }

        protected override string SceneName()
        {
            return "ExampleGameScene";
        }

        protected override World CreateWorld()
        {
            return new World(Physics.m_Gravity);
        }
        protected override Camera CreateCamera()
        {
            return new Camera(Vector2.Zero);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            switch (m_GameModeState)
            {
                case GameModeState.AWAKE:
                    m_GameModeState = GameModeState.STARTING;
                    break;

                case GameModeState.STARTING:

                    m_GameModeState = GameModeState.PLAYING;

                    break;

                case GameModeState.PLAYING:

                    if (true) //game end check
                    {
                        m_GameModeState = GameModeState.ENDING;
                    }
                    break;

                case GameModeState.ENDING:

                    Debug.WriteLine("Game Over");
                    break;
                default:
                    break;
            }
        }


    }
}
