using Game_Client;
using Game_Client.GameCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Packet_Library;

namespace Game_Client
{
    internal class LocalPaddleMovement : Component
    {
        // Movement speed of the paddle and rigidbody reference 
        float m_Speed;
        Rigidbody m_Rigidbody;

        public LocalPaddleMovement(GameObject gameObject, float speed) : base(gameObject)
        {
            m_Speed = speed;
        }

        protected override void Start(float deltaTime)
        {
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
        }

        protected override void Update(float deltaTime)
        {
            Vector2 movement = Vector2.Zero;

            // Keyboard input
            if (Keyboard.GetState().IsKeyDown(Keys.W)) 
            {
                movement.Y = -1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) 
            {
                movement.Y = 1;
            }
               
            // Scale movement by speed factor
            movement *= m_Speed;

            // Apply movement to the paddle rigidbody
            m_Rigidbody.m_Body.LinearVelocity = movement;

            // Determine which paddle each client controls
            ReplicatedObject paddleType = ClientNetworking.m_Instance.ClientID == 1? ReplicatedObject.LeftPaddle : ReplicatedObject.RightPaddle;

            // Send the updated paddle position by UDP
            ClientNetworking.m_Instance.UdpSendPosition(paddleType, m_Transform.Position.X, m_Transform.Position.Y, 0f);


        }
    }
}

