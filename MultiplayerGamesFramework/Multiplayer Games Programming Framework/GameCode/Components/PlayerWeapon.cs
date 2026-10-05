using Game_Client.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;

namespace Game_Client.GameCode.Components
{
    internal class PlayerWeapon : Component
    {
        const float m_FireTime = 0.5f;
        float m_FireTimer = 0f;
        int m_BulletSpeed = 20;

        SpriteRenderer m_SpriteRenderer;

        public PlayerWeapon(GameObject gameObject) : base(gameObject)
        {
            m_SpriteRenderer = m_GameObject.GetComponent<SpriteRenderer>();
        }

        protected override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            m_FireTimer -= deltaTime;
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (m_FireTimer <= 0f)
                {
                    Fire();
                    m_FireTimer = m_FireTime;
                }
            }
        }

        void Fire()
        {
            Vector2 firePoint = m_GameObject.m_Transform.Position + (-m_GameObject.m_Transform.Up * m_SpriteRenderer.m_ScaledSize);
            GameObject go = GameObject.Instantiate<GameObject>(m_GameObject.m_Scene, new Transform(firePoint, Vector2.One * 0.5f, m_GameObject.m_Transform.Rotation));
            go.m_Name = "Bullet";

            SpriteRenderer sr = go.AddComponent(new SpriteRenderer(go, "Laser"));
            sr.m_DepthLayer = 1;
            sr.m_Color = Color.Red;

            Rigidbody rb = go.AddComponent(new Rigidbody(go, BodyType.Dynamic, 1, sr.m_TextureSize / 2));
            rb.m_Body.IgnoreGravity = true;
            rb.CreateRectangle(sr.m_TextureSize.X, sr.m_TextureSize.Y, 0.0f, 1.0f, false, Vector2.Zero, Physics.GetCategoryByName("Projectile"), Physics.GetCategoryByName("All"));
            rb.m_Body.LinearVelocity = -m_Transform.Up * m_BulletSpeed;
            
            DestoryInTime DiT = go.AddComponent(new DestoryInTime(go, 1.0f));
        }
    }
}
