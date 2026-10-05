using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Client.GameCode.Prefabs
{
    // Paddle Prefab
    internal class PingPongPaddleGO
    {
        public static GameObject Create(Scene scene, Vector2 position, bool isLocal = false)
        {
            GameObject paddle = GameObject.Instantiate<GameObject>(scene, new Transform(position, new Vector2(2f, 8f), 0f));

            paddle.AddComponent(new SpriteRenderer(paddle, "Square(10x10)"));

            Rigidbody rb = paddle.AddComponent(new Rigidbody(paddle, BodyType.Kinematic, 10, Vector2.Zero));
            rb.CreateRectangle(2f, 8f, 0f, 0f, false, Vector2.Zero);

            return paddle;
        }
    }
}