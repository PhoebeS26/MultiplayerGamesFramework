using Game_Client.Core.Utilities;
using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Client.GameCode.Prefabs
{
    // Ball Prefab
    internal class PingPongBallGO
    {
        public static GameObject Create(Scene scene, Vector2 position, bool isLocal = false)
        {
            GameObject ball = GameObject.Instantiate<GameObject>(scene, new Transform(position, new Vector2(1f, 1f), 0f));
            ball.AddComponent(new SpriteRenderer(ball, "Square(10x10)"));

            if (isLocal) 
            {
                Rigidbody rb = ball.AddComponent(new Rigidbody(ball, BodyType.Dynamic, 1, Vector2.Zero));
                rb.CreateCircule(5f, 1f, 0f, false, Vector2.Zero, Physics.GetCategoryByName("All"), Physics.GetCategoryByName("All"));
                rb.m_Body.LinearVelocity = new Vector2(8f, 6f);
            }

            return ball;
        }
    }
}
