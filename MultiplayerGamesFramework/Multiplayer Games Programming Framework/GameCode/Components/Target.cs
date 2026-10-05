using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using System;
using System.Diagnostics;

namespace Game_Client.GameCode.Components
{
    internal class Target : Component
    {
        int m_Points;
        public int m_ID { get; private set; }
        
        public Action<int, int> onDestroy;
        
        public Target(GameObject gameObject) : base(gameObject)
        {
        }

        public void Init(int points, int ID)
        {
            m_Points = points;
            m_ID = ID;
        }

        protected override void OnCollisionEnter(Fixture sender, Fixture other, Contact contact)
        {
            if ((other.Body.Tag as GameObject).m_Name == "Bullet")
            {
                onDestroy.Invoke(m_ID, m_Points);
                (sender.Body.Tag as GameObject).Destroy();
                (other.Body.Tag as GameObject).Destroy();
            }
        }
    }
}
