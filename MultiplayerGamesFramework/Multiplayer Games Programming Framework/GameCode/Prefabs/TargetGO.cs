using Microsoft.Xna.Framework;
using Game_Client.Core.Utilities;
using nkast.Aether.Physics2D.Dynamics;
using System;
using Game_Client.GameCode.Components;

namespace Game_Client.GameCode.Prefabs
{
    internal class TargetGO : GameObject
    {
        public TargetGO(Scene scene, Transform transform) : base(scene, transform)
        {
            m_Name = "Target";
            SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "Square(10x10)"));
            sr.m_DepthLayer = 0;
            sr.m_Color = Color.Green;

            Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Static, 1, sr.m_TextureSize / 2));
            rb.CreateRectangle(sr.m_TextureSize.X, sr.m_TextureSize.Y, 0.0f, 1.0f, true, Vector2.Zero, Physics.GetCategoryByName("Target"), Physics.GetCategoryByName("All"));

            AddComponent(new Target(this));
        }
    }
}
