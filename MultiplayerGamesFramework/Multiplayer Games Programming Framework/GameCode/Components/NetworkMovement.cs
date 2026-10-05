using Microsoft.Xna.Framework;
using Game_Client.Core;

namespace Game_Client.GameCode.Components
{
	internal class NetworkMovement : Component
	{
		int m_Index;
		Rigidbody m_Rigidbody;

		public NetworkMovement(GameObject gameObject, int index) : base(gameObject)
		{
			m_Index = index;
		}
		
		protected override void Start(float deltaTime)
		{
			m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
		}

		public void UpdatePosition(Vector2 pos)
		{
			m_Rigidbody.UpdatePosition(pos);
		}
	}
}
