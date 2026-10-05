using Game_Client.Core.Utilities;
using Game_Client.GameCode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Packet_Library;

namespace Game_Client
{
    internal class LocalMovement : Component
    {
        float m_Speed;
		Rigidbody m_Rigidbody;
        public LocalMovement(GameObject gameObject, float speed) : base(gameObject)
        {
            m_Speed = speed;
        }

		protected override void Start(float deltaTime)
		{
			m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
		}

		protected override void Update(float deltaTime)
        {
			Vector2 input = Vector2.Zero;

            //Movement
            if (Keyboard.GetState().IsKeyDown(Keys.A))	{ input.X = -1; }
            if (Keyboard.GetState().IsKeyDown(Keys.D))	{ input.X = 1; }
            if (Keyboard.GetState().IsKeyDown(Keys.W))		{ input.Y = -1; }
            if (Keyboard.GetState().IsKeyDown(Keys.S))	{ input.Y = 1; }
            
            Vector2 Movement = (m_Transform.Right * input.X) + (m_Transform.Up * input.Y);
			m_Rigidbody.m_Body.LinearVelocity = (Movement * m_Speed * deltaTime);
            
            //Rotation to follow mouse
            MouseState mouseState = Mouse.GetState(); //Get the mouse
            Vector2 mouseWorldPoint = Camera.ScreenToWorldPoint(new Vector2(mouseState.X, mouseState.Y)); //Get the mouse position in world space

            Vector2 direction = -(Camera.ScreenToWorldPoint(new Vector2(mouseState.X, mouseState.Y)) - m_GameObject.m_Transform.Position); //Get the vector between the mouse and the GameObject

            m_Rigidbody.UpdateRotation(-MathF.Atan2(direction.X, direction.Y));

            //ClientNetworking.m_Instance.TCPSendMessage(new PositionPacket(m_Transform.Position.X, m_Transform.Position.Y, m_Transform.Rotation).ToJson());
		}
	}
}
