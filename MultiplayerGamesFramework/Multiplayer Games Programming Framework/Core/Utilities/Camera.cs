using Microsoft.Xna.Framework;

namespace Game_Client.Core.Utilities
{
    internal class Camera
    {
        public static Matrix m_Matrix { get; protected set; }
        public Vector2 m_Position { get; protected set; }

        protected float m_Zoom { get; set; }
        public float m_ZoomFactor { get; protected set; }
        public float m_Rotation { get; protected set; }
        public bool m_isDirty { get; protected set; }


        public Camera(Vector2 position, float rotation = 0, float zoom = 1)
        {
            m_Position = position;
            m_Rotation = rotation;
            m_Zoom = Graphics.Viewport.Width / Graphics.PixelsPerWorldUnit;
            m_ZoomFactor = zoom;
            m_isDirty = true;

            UpdateViewMatrix();
        }

        public virtual void Update()
        {
            UpdateViewMatrix();
        }

        public void Move(Vector2 pos)
        {
            m_Position = pos;
            m_isDirty = true;
        }

        public void SetPosition(Vector2 position)
        {
            m_Position = position;
            m_isDirty = true;
        }

        public void Rotate(float angle)
        {
            m_Rotation += angle;
            m_isDirty = true;
        }

        public void SetRotation(float rotation)
        {
            m_Rotation = rotation;
            m_isDirty = true;
        }

        public void Zoom(float zoomAdjust)
        {
            m_ZoomFactor = MathHelper.Max(m_ZoomFactor + zoomAdjust, 0.1f);
            m_isDirty = true;
        }

        public void SetZoom(float zoomFactor)
        {
            m_ZoomFactor = MathHelper.Max(zoomFactor, 0.1f);
            m_isDirty = true;
        }

        protected virtual void UpdateViewMatrix()
        {
            if (m_isDirty)
            {
                Vector2 viewCenter = new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2);

                Matrix view =
                    Matrix.CreateTranslation(new Vector3(-m_Position.X, -m_Position.Y, 0)) *
                    Matrix.CreateRotationZ(m_Rotation) *
                    Matrix.CreateScale(m_Zoom * m_ZoomFactor, m_Zoom * m_ZoomFactor, 1) *
                    Matrix.CreateTranslation(new Vector3(viewCenter.X, viewCenter.Y, 0));

                m_Matrix = view;

                m_isDirty = false;
            }
        }

        public static Vector2 ScreenToWorldPoint(Vector2 screenPoint)
        {
            Vector2 worldPoint = new Vector2();
            Matrix inverse = Matrix.Invert(m_Matrix);
            return Vector2.Transform(screenPoint, inverse);          
        }
    }
}
