using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Client.GameCode.Components
{
    internal class Rotator : Component
    {
        float m_Degrees;

        public Rotator(GameObject gameObject, float degrees) : base(gameObject)
        {
            this.m_Degrees = degrees;
        }

        protected override void Update(float deltaTime)
        {
            m_GameObject.m_Transform.Rotation += m_Degrees * deltaTime;
        }
    }
}
