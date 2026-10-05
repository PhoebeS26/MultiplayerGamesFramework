using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Game_Client.Core.Utilities
{
    internal static class Physics
    {
        public static Vector2 m_Gravity = new Vector2(0, 9.81f);

        public static float m_PhysicsWidth = ScreenToPhysics(Graphics.GraphicsDevice.Viewport.Width);
        public static float m_PhysicsHeight = ScreenToPhysics(Graphics.GraphicsDevice.Viewport.Height);

        public static float ScreenToPhysics(float value)
        {
            return value * 0.02f;
        }

        public static Vector2 ScreenToPhysics(Vector2 value)
        {
            return new Vector2(ScreenToPhysics(value.X), ScreenToPhysics(value.Y));
        }

        public static float PhysicstoScreen(float value)
        {
            return value * 50.0f;
        }

        public static Vector2 PhysicstoScreen(Vector2 value)
        {
            return new Vector2(PhysicstoScreen(value.X), PhysicstoScreen(value.Y));
        }

        public static Category GetCategoryByName(string name)
        {
            switch (name)
            {
                case "All":
                    return Category.All;
                case "None":
                    return Category.None;
                case "Player":
                    return Category.Cat1;
                case "WorldObject":
                    return Category.Cat2;
                case "Wall":
                    return Category.Cat3;
                case "Trigger":
                    return Category.Cat4;
                case "Projectile":
                    return Category.Cat5;
                case "Target":
                    return Category.Cat6;
            }

            Debug.WriteLine("Category not found: " + name + ". Returning Category.None");
            return Category.None;
        }

        public class RayCastClosestCallback
        {
            public Fixture HitFixture { get; private set; }
            public Vector2 Point { get; private set; }
            public Vector2 Normal { get; private set; }
            public float Fraction { get; private set; } = 1f;

            public float ReportFixture(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
            {
                HitFixture = fixture;
                Point = point;
                Normal = normal;
                Fraction = fraction;

                return fraction;
            }
        }
    }
}
