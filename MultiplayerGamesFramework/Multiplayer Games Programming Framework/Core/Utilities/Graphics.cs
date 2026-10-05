using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace Game_Client.Core.Utilities
{
	internal static class Graphics
	{
		public static GraphicsDevice GraphicsDevice;

        public static Viewport Viewport { get { return GraphicsDevice.Viewport; } }

        public static readonly int InitScreenWidth = 640;
        public static readonly int InitScreenHeight = 360;

        //total amount of pixels of horizontal pixels
        public static float PixelsPerWorldUnit = 1000.0f;

        public const string DefaultWindowName = "MGP Client Window";
    }
}