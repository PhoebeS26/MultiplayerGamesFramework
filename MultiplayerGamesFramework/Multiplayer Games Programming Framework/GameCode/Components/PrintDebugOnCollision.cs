using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Dynamics;
using System.Diagnostics;

namespace Game_Client.GameCode.Components
{
    internal class PrintDebugOnCollision : Component
    {
        string m_Debug;

        public PrintDebugOnCollision(GameObject gameObject, string debug) : base(gameObject)
        {
            m_Debug = debug;
        }

        protected override void OnCollisionEnter(Fixture sender, Fixture other, Contact contact)
        {
            GameObject s = sender.Body.Tag as GameObject;
            GameObject o = other.Body.Tag as GameObject;
            Debug.WriteLine("Collision. Sender = " + s.m_Name + " Other = " + o.m_Name + ". Debug Message = " + m_Debug);
        }
    }
}
