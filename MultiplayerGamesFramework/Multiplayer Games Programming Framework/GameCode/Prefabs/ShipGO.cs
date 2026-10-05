using Microsoft.Xna.Framework;
using Game_Client.Core.Utilities;
using nkast.Aether.Physics2D.Dynamics;
using Game_Client.GameCode.Components;

namespace Game_Client
{
	internal class ShipGO : GameObject
	{
		public ShipGO(Scene scene, Transform transform) : base(scene, transform)
		{
			SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "Ship"));
			sr.m_DepthLayer = 1;

			Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Kinematic, 1, sr.m_TextureSize / 2));
			rb.CreateRectangle(sr.m_TextureSize.X, sr.m_TextureSize.Y, 0.0f, 1.0f, false, Vector2.Zero, Physics.GetCategoryByName("Player"), Physics.GetCategoryByName("All"));

			PlayerWeapon pw = AddComponent(new PlayerWeapon(this));
		}
	}
}