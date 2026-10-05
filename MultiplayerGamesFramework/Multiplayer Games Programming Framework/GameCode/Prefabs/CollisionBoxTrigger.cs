using Game_Client.Core.Utilities;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Client.GameCode.Prefabs
{
    internal class CollisionBoxTrigger : GameObject
    {
        public CollisionBoxTrigger(Scene scene, Transform transform) : base(scene, transform)
        {
            m_Name = "Trigger";
            SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "Square(1x1)"));
            sr.m_Color = Color.White;
            sr.m_Render = true;
            Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Dynamic, 0.0f, sr.m_TextureSize / 2));
            rb.m_Body.IgnoreGravity = true;
            rb.CreateRectangle(sr.m_TextureSize.X, sr.m_TextureSize.Y, 0.0f, 1.0f, true, Vector2.Zero, Physics.GetCategoryByName("Trigger"), Physics.GetCategoryByName("All"));
        }
    }
}
 