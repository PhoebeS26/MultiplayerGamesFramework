using Game_Client.Core.Utilities;
using Game_Client.GameCode.Components;
using Game_Client.GameCode.Prefabs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Dynamics;
using Packet_Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Game_Client.GameCode.Scenes
{
    internal class PingPongScene : Scene
    {

        // Local and remote paddles and the ball
        GameObject m_LeftPaddle;
        GameObject m_LeftRemotePaddle;
        GameObject m_RightPaddle;
        GameObject m_RightRemotePaddle;
        GameObject m_Ball;

        // Score tracking
        int m_LeftScore = 0;
        int m_RightScore = 0;
        PingPongUI m_UI;

        // Used to stop scoring multiple times in a short time
        bool m_HasScored = false;
        float m_ScoreCooldownTimer = 0f;
        float m_PaddleOffset = 20f;

        // Used to enqueue actions recieved from network callbacks
        Queue<Action> m_NetworkActions = new Queue<Action>();

        // Thread safe access lock
        private readonly object m_QueueLock = new object();

        GameModeState m_GameModeState;

        public PingPongScene(SceneManager manager) : base(manager)
        {
            m_GameModeState = GameModeState.AWAKE;
        }

        // Sets up, paddles, ball, UI 
        public override void LoadContent()
        {
            base.LoadContent();

            int clientId = ClientNetworking.m_Instance.ClientID;

            if (clientId == 1)
            {
                m_LeftPaddle = PingPongPaddleGO.Create(this, new Vector2(-400f, 0f), isLocal: true);
                m_LeftPaddle.AddComponent(new LocalPaddleMovement(m_LeftPaddle, 7));
            }
            else 
            {
                m_LeftRemotePaddle = GameObject.Instantiate<GameObject>(this, new Transform(new Vector2(-400f, 0f), new Vector2(2f, 8f), 0f));
                m_LeftRemotePaddle.AddComponent(new SpriteRenderer(m_LeftRemotePaddle, "Square(10x10)"));

                ClientNetworking.m_Instance.onRemoteObjectMovement += MoveRemoteObject;
            }

            if (clientId == 2)
            {
                m_RightPaddle = PingPongPaddleGO.Create(this, new Vector2(400f, 0f), isLocal: true);
                m_RightPaddle.AddComponent(new LocalPaddleMovement(m_RightPaddle, 7));
            }
            else
            {
                m_RightRemotePaddle = GameObject.Instantiate<GameObject>(this, new Transform(new Vector2(400f, 0f), new Vector2(2f, 8f), 0f));
                m_RightRemotePaddle.AddComponent(new SpriteRenderer(m_RightRemotePaddle, "Square(10x10)"));

                ClientNetworking.m_Instance.onRemoteObjectMovement += MoveRemoteObject;
            }

            if (clientId == 1)
            {
                m_Ball = PingPongBallGO.Create(this, Vector2.Zero, isLocal: true);
            }
            else
            {
                m_Ball = PingPongBallGO.Create(this, Vector2.Zero, isLocal: false);
            }

            GameObject canvas = GameObject.Instantiate<GameObject>(this, new Transform());
            m_UI = canvas.AddComponent(new PingPongUI(canvas));

            // Game state update from server
            ClientNetworking.m_Instance.onGameStateUpdate += HandleGameStateUpdate;

            CreateBorder();
        }

        protected override string SceneName()
        {
            return "PingPongScene";
        }

        protected override World CreateWorld()
        {
            Debug.WriteLine("Creating Scene");
            return new World(Vector2.Zero);
        }

        protected override Camera CreateCamera()
        {
            Debug.WriteLine("Creating Camera");
            return new Camera(Vector2.Zero);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Execute queued network actions
            lock (m_QueueLock)
            {
                while (m_NetworkActions.Count > 0)
                {
                    m_NetworkActions.Dequeue()();
                }
            }

            switch (m_GameModeState)
            {
                case GameModeState.AWAKE:
                    m_GameModeState = GameModeState.STARTING;
                    break;

                case GameModeState.STARTING:
                    m_GameModeState = GameModeState.PLAYING;
                    break;

                case GameModeState.PLAYING:
                    if (m_Ball != null && m_LeftPaddle != null && ClientNetworking.m_Instance.ClientID == 1)
                    {
                        float ballX = m_Ball.m_Transform.Position.X;
                        float leftPaddleX = m_LeftPaddle.m_Transform.Position.X;
                        float rightPaddleX = 400f;

                        if (m_ScoreCooldownTimer > 0f) 
                        {
                            m_ScoreCooldownTimer -= deltaTime;
                        }
                      
                        if (!m_HasScored && m_ScoreCooldownTimer <= 0f)
                        {
                            if (ballX > rightPaddleX + m_PaddleOffset)
                            {
                                m_LeftScore++;
                                m_HasScored = true;
                                m_ScoreCooldownTimer = 1f;
                                ResetBall(false);

                                // Send updated game state to server
                                var statePacket = new GameStatePacket(m_LeftScore, m_RightScore);
                                ClientNetworking.m_Instance.TCPSendGameState(statePacket);
                            }
                            else if (ballX < leftPaddleX - m_PaddleOffset)
                            {
                                m_RightScore++;
                                m_HasScored = true;
                                m_ScoreCooldownTimer = 1f;
                                ResetBall(true);

                                var statePacket = new GameStatePacket(m_LeftScore, m_RightScore);
                                ClientNetworking.m_Instance.TCPSendGameState(statePacket);
                            }

                            if (m_LeftScore >= 5 || m_RightScore >= 5)
                            {
                                string winner = m_LeftScore >= 5 ? "Left Player" : "Right Player";
                                var statePacket = new GameStatePacket(m_LeftScore, m_RightScore, winner);
                                ClientNetworking.m_Instance.TCPSendGameState(statePacket);
                            }
                        }

                        // Continuously send ball position by UDP 
                        ClientNetworking.m_Instance.UdpSendPosition(ReplicatedObject.Ball, m_Ball.m_Transform.Position.X,m_Ball.m_Transform.Position.Y,0f);
                    }
                    break;

                case GameModeState.ENDING:
                    Debug.WriteLine("Game Over");
                    break;
            }
        }

        // Setup four wall objects for the scene border
        void CreateBorder()
        {
            Vector2[] wallPositions = new Vector2[]
            {
               new Vector2(0, -276),   
               new Vector2(495, 0),    
               new Vector2(0, 276),    
               new Vector2(-495, 0)    
            };

            Vector2[] wallScales = new Vector2[]
            {
               new Vector2(100, 1),   
               new Vector2(1, 56.25f),
               new Vector2(100, 1),   
               new Vector2(1, 56.25f)  
            };

            for (int i = 0; i < 4; i++)
            {
                GameObject wall = GameObject.Instantiate<GameObject>(this, new Transform(wallPositions[i], wallScales[i], 0));

                SpriteRenderer sr = wall.AddComponent(new SpriteRenderer(wall, "Square(10x10)"));
                Rigidbody rb = wall.AddComponent(new Rigidbody(wall, BodyType.Static, 1, sr.m_TextureSize / 2));

                rb.CreateRectangle(sr.m_TextureSize.X, sr.m_TextureSize.Y, 0.0f, 1.0f, false, Vector2.Zero, Physics.GetCategoryByName("Wall"), Physics.GetCategoryByName("All"));
                sr.m_Color = new Color(0, 0.2f * i, 1); 
            }

            Debug.WriteLine("Created Border Walls");
        }

        void ResetBall(bool toRight)
        {
            if (m_Ball == null) return;

            Rigidbody ballRb = m_Ball.GetComponent<Rigidbody>();
            if (ballRb == null) return;

            ballRb.m_Body.Enabled = false;

            // Temporarily disable ball physics to reset position
            Vector2 resetPos = Vector2.Zero;
            m_Ball.m_Transform.Position = resetPos;
            ballRb.m_Body.Position = resetPos;
            ballRb.m_Body.LinearVelocity = Vector2.Zero;
            ballRb.m_Body.AngularVelocity = 0f;

            // Delay before serving to stop immediate scoring
            System.Threading.Tasks.Task.Delay(800).ContinueWith(_ =>
            {
                float speedX = toRight ? 8f : -8f;
                float speedY = new Random().Next(-4, 5);
                ballRb.m_Body.Enabled = true;
                ballRb.m_Body.LinearVelocity = new Vector2(speedX, speedY);
                m_HasScored = false; 
            });

            Debug.WriteLine($"Ball reset to center. Next serve: {(toRight ? "Right" : "Left")}");
        }

        private void HandleGameStateUpdate(int leftScore, int rightScore, string? winner)
        {
            // Enqueue updates to be executed
            lock (m_QueueLock)
            {
                m_NetworkActions.Enqueue(() =>
                {
                    m_LeftScore = leftScore;
                    m_RightScore = rightScore;
                    m_UI.SetScore(m_LeftScore, m_RightScore);

                    if (winner != null)
                    {
                        m_GameModeState = GameModeState.ENDING;
                        m_UI.ShowGameOver(winner);

                        // Hide all game objects to end the game
                        List<GameObject> objectsToHide = new List<GameObject>
                        {
                           m_Ball,
                           m_LeftPaddle,
                           m_RightPaddle,
                           m_LeftRemotePaddle,
                            m_RightRemotePaddle
                        };
                        
                     foreach (var obj in objectsToHide)
                        {
                            var sprite = obj?.GetComponent<SpriteRenderer>();
                            if (sprite != null) 
                            {
                                sprite.IsVisible = false;
                            }
                        }
                    }
                });
            }
        }

        void MoveRemoteObject(ReplicatedObject id, System.Numerics.Vector2 position, float rotation)
        {
            // Update remote objects positions from network events
            lock (m_QueueLock)
            {
                m_NetworkActions.Enqueue(() =>
                {
                    switch (id)
                    {
                        case ReplicatedObject.Ball:
                            if (m_Ball != null)
                                m_Ball.m_Transform.Position = position;
                            break;

                        case ReplicatedObject.LeftPaddle:
                            if (m_LeftRemotePaddle != null)
                                m_LeftRemotePaddle.m_Transform.Position = position;
                            break;

                        case ReplicatedObject.RightPaddle:
                            if (m_RightRemotePaddle != null)
                                m_RightRemotePaddle.m_Transform.Position = position;
                            break;
                    }
                });
            }
        }
    }
}
