using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using nkast.Aether.Physics2D.Dynamics;
using System.Diagnostics;
using Game_Client.GameCode.Components;
using Game_Client.Core.Utilities;
using Game_Client.GameCode.Prefabs;
using System.Reflection;
using nkast.Aether.Physics2D.Collision;

namespace Game_Client
{
    internal class PhysicsGameScene : Scene
    {
        Random m_Random = new Random();

        float m_GameTimer;
        float m_CameraMovSpeed = 50;
        float m_CameraRotSpeed = 1;
        float m_CameraZoomSpeed = 1;

        bool m_LeftClickReleased = false;
        private bool m_RightClickReleased;

        public PhysicsGameScene(SceneManager manager) : base(manager)
        {
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Random rnd = new Random();


            //Example of how to spawn objects on mass
            for (int i = 0; i < 50; i++)
            {
                GameObject go = GameObject.Instantiate<PhysicsGO>(this, new Transform(
                																		new Vector2(m_Random.Next(-800, 800), 0),
                																		new Vector2(m_Random.Next(1, 5), m_Random.Next(1, 5)),
                																		0));
                go.GetComponent<Rigidbody>().UpdatePosition(Vector2.Zero);
            }


            GameObject trigger = GameObject.Instantiate<CollisionBoxTrigger>(this, new Transform(new Vector2(0, -150), new Vector2(900, 50), 00));
            trigger.AddComponent(new PrintDebugOnCollision(trigger, "triggered"));
            trigger.AddComponent(new ChangeColourOnCollision(trigger, Color.Red));

            CreateBorder();
        }

        protected override string SceneName()
        {
            return "GameScene";
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

            //Update and move camera
            Vector2 camPos = m_Camera.m_Position;
            float camRot = m_Camera.m_Rotation;
            float camZoom = m_Camera.m_ZoomFactor;


            if (Keyboard.GetState().IsKeyDown(Keys.Left)) { camPos.X -= m_CameraMovSpeed * deltaTime; }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) { camPos.X += m_CameraMovSpeed * deltaTime; }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) { camPos.Y -= m_CameraMovSpeed * deltaTime; }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) { camPos.Y += m_CameraMovSpeed * deltaTime; }

            if (Keyboard.GetState().IsKeyDown(Keys.E)) { camRot -= m_CameraRotSpeed * deltaTime; }
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) { camRot += m_CameraRotSpeed * deltaTime; }
            if (Keyboard.GetState().IsKeyDown(Keys.T)) { camZoom += m_CameraZoomSpeed * deltaTime; }
            if (Keyboard.GetState().IsKeyDown(Keys.G)) { camZoom -= m_CameraZoomSpeed * deltaTime; }


            m_Camera.SetPosition(camPos);
            m_Camera.SetRotation(camRot);
            m_Camera.SetZoom(camZoom);

            MouseState mouseState = Mouse.GetState();
            Vector2 mouseWorldPoint = Camera.ScreenToWorldPoint(new Vector2(mouseState.X, mouseState.Y));



            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (m_LeftClickReleased)
                {
                    m_LeftClickReleased = false;   
                    m_World.QueryAABB(AddForceToPhysicsObjectOnClick, new AABB(Physics.ScreenToPhysics((new Vector2(mouseWorldPoint.X, mouseWorldPoint.Y))), 0.1f, 0.1f));

                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (m_RightClickReleased)
                {
                    m_RightClickReleased = false;
                    m_World.QueryAABB(DeletePhysicsObjectOnClick, new AABB(Physics.ScreenToPhysics((new Vector2(mouseWorldPoint.X, mouseWorldPoint.Y))), 0.1f, 0.1f));

                }
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                m_LeftClickReleased = true;
            }

            if (mouseState.RightButton == ButtonState.Released)
            {
                m_RightClickReleased = true;
            }
        }

        bool AddForceToPhysicsObjectOnClick(Fixture fixture)
        {
            if (fixture == null) return true;

            GameObject go = (fixture.Body.Tag as GameObject);

            if (go == null) return true;

            if (go.m_Name == "Physics Object")
            {
                Rigidbody rb = go.GetComponent<Rigidbody>();
                rb.m_Body.ApplyForce(new Vector2(0, -300));
                return false;
            }

            return true;
        }

        bool DeletePhysicsObjectOnClick(Fixture fixture)
        {
            if (fixture == null) return true;

            GameObject go = (fixture.Body.Tag as GameObject);

            if (go == null) return true;

            if (go.m_Name == "Physics Object")
            {
                go.Destroy();
                return false;
            }

            return true;
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
                new Vector2(100, 4), //top
				new Vector2(4, 56.25f), //right
				new Vector2(100, 4), //bottom
				new Vector2(4, 56.25f) //left
			};

            for (int i = 0; i < 4; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(this, new Transform(wallPos[i], wallScales[i], 0));
                SpriteRenderer sr = go.AddComponent(new SpriteRenderer(go, "Square(10x10)"));
                Rigidbody rb = go.AddComponent(new Rigidbody(go, BodyType.Static, 10, sr.m_TextureSize / 2));
                rb.CreateRectangle(sr.m_TextureSize.X, sr.m_TextureSize.Y, 0.0f, 1.0f, false, Vector2.Zero, Physics.GetCategoryByName("Wall"), Physics.GetCategoryByName("All"));
                sr.m_Color = new Color(0, 0.2f * i, 1);
                go.AddComponent(new ChangeColourOnCollision(go, Color.Red));
                m_GameObjects.Add(go);
            }
        }
    }
}