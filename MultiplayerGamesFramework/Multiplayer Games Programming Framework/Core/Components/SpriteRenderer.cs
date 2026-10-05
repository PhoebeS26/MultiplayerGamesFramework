using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Game_Client
{
    internal class SpriteRenderer : Component
    {
        SpriteBatch m_SpriteBatch;
		public Texture2D m_Texture { get; private set; }
		public Vector2 m_TextureSize { get; private set; }
        public Vector2 m_ScaledSize { get { return m_TextureSize * m_GameObject.m_Transform.Scale; } }

        public float m_DepthLayer = 0;

        public Color m_Color = Color.White;

        public bool m_Render = true;

        public bool IsVisible = true;

        public SpriteRenderer(GameObject gameObject, string texture) : base(gameObject)
        {
            m_SpriteBatch = gameObject.m_Scene.GetSpriteBatch();
            m_Texture = gameObject.m_Scene.GetContentManager().Load<Texture2D>(texture);

            if (m_Texture == null)
            {
                Console.WriteLine("Texture not found");
                return;
            }

            m_TextureSize = new Vector2(m_Texture.Width, m_Texture.Height);
        }

		protected override void Draw(float deltaTime)
        {
            if (!IsVisible || !m_Render)
                return;

            if (m_Render)
                m_SpriteBatch.Draw(m_Texture, m_Transform.Position, null, m_Color, MathHelper.ToRadians(m_Transform.Rotation), m_TextureSize / 2, m_Transform.Scale, new SpriteEffects(), m_DepthLayer);

        }
    }
}
