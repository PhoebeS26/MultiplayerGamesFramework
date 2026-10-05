using Microsoft.Xna.Framework;
using Game_Client.Core.Utilities;
using nkast.Aether.Physics2D.Dynamics;
using System;

namespace Game_Client.GameCode.Prefabs
{
    internal class PhysicsGO : GameObject
    {
        public PhysicsGO(Scene scene, Transform transform) : base(scene, transform)
        {
            Random rng = new Random();
            m_Name = "Physics Object";
            SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "Square(10x10)"));
            sr.m_DepthLayer = 0;
            sr.m_Color = new Color((float)rng.NextDouble(), (float)rng.NextDouble(), (float)rng.NextDouble());
            
            Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Dynamic, 1, sr.m_TextureSize / 2));
            rb.CreateRectangle(sr.m_TextureSize.X, sr.m_TextureSize.Y, 0.0f, 1.0f, false, Vector2.Zero, Physics.GetCategoryByName("WorldObject"), Physics.GetCategoryByName("All"));

            AddComponent(new BallControllerComponent(this));
        }
    }
}
