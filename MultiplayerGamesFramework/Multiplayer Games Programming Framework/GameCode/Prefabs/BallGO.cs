using System;
using Microsoft.Xna.Framework;
using Game_Client.Core.Utilities;
using nkast.Aether.Physics2D.Dynamics;

namespace Game_Client;

// Ball Prefab
internal class BallGO : GameObject
{
	public BallGO(Scene scene, Transform transform) : base(scene, transform)
	{
		SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "ball"));
		sr.m_DepthLayer = 0;

		Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Dynamic, 0.1f, sr.m_TextureSize / 2));
		rb.m_Body.IgnoreGravity = true;
		rb.m_Body.FixedRotation = true;
		rb.CreateCircule(Math.Max(sr.m_TextureSize.X, sr.m_TextureSize.Y) / 2, 0.0f, 0.0f, false, Vector2.Zero, Physics.GetCategoryByName("WorldObject"), Physics.GetCategoryByName("WorldObject") | Physics.GetCategoryByName("Player") | Physics.GetCategoryByName("Wall"));
		
		AddComponent(new BallControllerComponent(this));
	}
}