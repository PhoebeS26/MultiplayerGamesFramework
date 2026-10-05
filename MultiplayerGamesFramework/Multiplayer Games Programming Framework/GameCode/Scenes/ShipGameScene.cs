using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using nkast.Aether.Physics2D.Dynamics;
using System.Diagnostics;
using Game_Client.GameCode.Components;
using Game_Client.Core.Utilities;
using Game_Client.GameCode.Prefabs;
using Game_Client.GameCode;

namespace Game_Client
{
    internal class ShipGameScene : Scene
	{
		ShipGO m_Player;
		ShipGameModeUI m_UI;
		List<Target> m_Targets; 
		Random m_Random = new Random();
        GameObject m_RemoteShip;

        GameModeState m_GameModeState;

		float m_GameTimer = 30.0f;
        int m_Score;

        public ShipGameScene(SceneManager manager) : base(manager)
		{
			m_GameModeState = GameModeState.AWAKE;

			m_Targets = new List<Target>();
		}

		public override void LoadContent()
		{
			base.LoadContent();

			Random rnd = new Random();

			for(int i = 0; i < 5; i++)
			{
				int scale = m_Random.Next(3, 6);
                TargetGO target = GameObject.Instantiate<TargetGO>(this, new Transform(new Vector2(m_Random.Next(-400, 400), m_Random.Next(-15, 150)),
																							new Vector2(scale,scale),
																							0));

				Target targetComponent = target.GetComponent<Target>();
				targetComponent.Init((i + 1) * 100, i); //set the point allocation and ID of the target object
				targetComponent.onDestroy += TargetDestroyed; //subscribe to the on Destroy Event. 

				m_Targets.Add(targetComponent); //add the target to the list of targets
            }

			if (ClientNetworking.m_Instance.ClientID == 1)
			{
				m_Player = GameObject.Instantiate<ShipGO>(this, new Transform(new Vector2(-450f, 0), new Vector2(0.5f, 0.5f), 0));
				m_Player.AddComponent(new LocalMovement(m_Player, 100));
			}
			else 
			{
				m_RemoteShip = GameObject.Instantiate<GameObject>(this, new Transform(new Vector2(-450f, 0), new Vector2(0.5f, 0.5f), 0));
				m_RemoteShip.AddComponent(new SpriteRenderer(m_RemoteShip, "Ship"));

				ClientNetworking.m_Instance.onRemoteShipMovement += MoveRemotePlayer;
			}



				GameObject canvas = GameObject.Instantiate<GameObject>(this, new Transform());
            m_UI = canvas.AddComponent(new ShipGameModeUI(canvas));

			CreateBorder();


		}

		protected override string SceneName()
		{
			return "GameScene";
		}

		protected override World CreateWorld()
		{
			return new World(Vector2.Zero); // no gravity
		}

        protected override Camera CreateCamera()
        {
			return new Camera(Vector2.Zero);
        }

        public override void Update(float deltaTime)
		{
			base.Update(deltaTime);


            m_GameTimer -= deltaTime;

			switch (m_GameModeState)
			{
				case GameModeState.AWAKE:
					m_GameModeState = GameModeState.STARTING;
					break;

				case GameModeState.STARTING:
					m_GameModeState = GameModeState.PLAYING;
					break;

				case GameModeState.PLAYING:

					m_UI.SetTime(m_GameTimer);

					if(m_Targets.Count == 0 || m_GameTimer <= 0.0f)
					{
						m_Player.Destroy();
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
		void TargetDestroyed(int id, int score)
		{
			Debug.WriteLine("Destroying " + id);
			m_Score += score;
			m_Targets.RemoveAll(x =>  x.m_ID == id);
			Debug.WriteLine("count = " + m_Targets.Count);

			m_UI.SetScore(m_Score);
		}

		/// <summary>
		/// this function creates a border around the edge of the screen. 
		/// </summary>
		void CreateBorder()
		{
            //Border
            Vector2[] wallPos = new Vector2[]
            {
                new Vector2(0, -276), //top
				new Vector2(495, 0), //right
				new Vector2(0, 276), //bottom
				new Vector2(-495, 0) //left
			};

            Vector2[] wallScales = new Vector2[]
            {
                new Vector2(100, 1), //top
				new Vector2(1, 56.25f), //right
				new Vector2(100, 1), //bottom
				new Vector2(1, 56.25f) //left
			};

            for (int i = 0; i < 4; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(this, new Transform(wallPos[i], wallScales[i], 0));
                SpriteRenderer sr = go.AddComponent(new SpriteRenderer(go, "Square(10x10)"));
                Rigidbody rb = go.AddComponent(new Rigidbody(go, BodyType.Static, 10, sr.m_TextureSize / 2));
                rb.CreateRectangle(sr.m_TextureSize.X, sr.m_TextureSize.Y, 0.0f, 1.0f, false, Vector2.Zero, Physics.GetCategoryByName("Wall"), Physics.GetCategoryByName("All"));
                sr.m_Color = new Color(0, 0.2f * i, 1);
                go.AddComponent(new ChangeColourOnCollision(go, Color.Red));
            }
        }

		void MoveRemotePlayer(System.Numerics.Vector2 position, float rotation) 
		{
			m_RemoteShip.m_Transform.Position = position;
			m_RemoteShip.m_Transform.Rotation = rotation;
		}
	}
}