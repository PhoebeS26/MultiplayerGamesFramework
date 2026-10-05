using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;       
using Game_Client.Core.Utilities;    

namespace Game_Client.GameCode.Components
{
    internal class BallMovement : Component
    {
        Rigidbody m_Rigidbody;
        Vector2 m_Velocity;

        public BallMovement(GameObject gameObject, float speed) : base(gameObject)
        {
            m_Velocity = new Vector2(speed, speed);
        }

        protected override void Start(float deltaTime)
        {
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
        }

        protected override void Update(float deltaTime)
        {
            // Set the velocity every frame
            m_Rigidbody.m_Body.LinearVelocity = m_Velocity;
        }

        public void BounceX()
        {
            m_Velocity.X = -m_Velocity.X;
        }

        public void BounceY()
        {
            m_Velocity.Y = -m_Velocity.Y;
        }
    }
}
