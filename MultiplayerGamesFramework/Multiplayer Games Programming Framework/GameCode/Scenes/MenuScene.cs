using Myra;
using Myra.Graphics2D.UI;
using nkast.Aether.Physics2D.Dynamics;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Game_Client.Core.Utilities;
using Game_Client.GameCode;
using Game_Client.GameCode.Scenes;

namespace Game_Client
{
    internal class MenuScene : Scene
	{
		private Desktop m_Desktop;
		public MenuScene(SceneManager manager) : base(manager)
		{
			manager.m_Game.IsMouseVisible = true;
		}

		protected override World CreateWorld()
		{
			return null;
		}

        protected override Camera CreateCamera()
        {
            return new Camera(Vector2.Zero);
        }

        protected override string SceneName()
		{
			return "Main Menu";
		}

		public override void LoadContent()
		{
			MyraEnvironment.Game = m_Manager.m_Game;

			var grid = new Grid
			{
				ShowGridLines = false,
				RowSpacing = 8,
				ColumnSpacing = 8
			};

			int cols = 4;
			for(int i = 0; i < cols; ++i)
			{
				grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
			}

			int rows = 5;
			for (int i = 0; i < rows; ++i)
			{
				grid.RowsProportions.Add(new Proportion(ProportionType.Part));
			}

			m_Desktop = new Desktop();
			m_Desktop.Root = grid;


			var LoginButton = new TextButton();
			LoginButton.Text = "Login";
			LoginButton.GridRow = 2;
			LoginButton.GridColumn = 1;
			LoginButton.GridColumnSpan = 2;
			LoginButton.HorizontalAlignment = HorizontalAlignment.Center;
			LoginButton.VerticalAlignment = VerticalAlignment.Center;
			LoginButton.Width = (Graphics.Viewport.Width / cols) * LoginButton.GridColumnSpan;
			LoginButton.Height = (Graphics.Viewport.Height / rows) * LoginButton.GridRowSpan;
			grid.Widgets.Add(LoginButton);

			var PlayButton = new TextButton();
			PlayButton.Text = "Play";
			PlayButton.GridRow = 3;
			PlayButton.GridColumn = 1;
			PlayButton.GridColumnSpan = 2;
			PlayButton.HorizontalAlignment = HorizontalAlignment.Center;
			PlayButton.VerticalAlignment = VerticalAlignment.Center;
			PlayButton.Width = (Graphics.Viewport.Width / cols) * LoginButton.GridColumnSpan;
			PlayButton.Height = (Graphics.Viewport.Height / rows) * LoginButton.GridRowSpan;
			PlayButton.Enabled = false;
			grid.Widgets.Add(PlayButton);

            var DisconnectButton = new TextButton();
            DisconnectButton.Text = "Disconnect";
            DisconnectButton.GridRow = 4;
            DisconnectButton.GridColumn = 1;
            DisconnectButton.GridColumnSpan = 2;
            DisconnectButton.HorizontalAlignment = HorizontalAlignment.Center;
            DisconnectButton.VerticalAlignment = VerticalAlignment.Center;
            DisconnectButton.Width = (Graphics.Viewport.Width / cols) * DisconnectButton.GridColumnSpan;
            DisconnectButton.Height = (Graphics.Viewport.Height / rows);
            DisconnectButton.Enabled = false;
            grid.Widgets.Add(DisconnectButton);

            PlayButton.Click += (s, a) =>
			{
                Debug.WriteLine("Play button clicked — attempting to load PingPongScene");

               //m_Manager.LoadScene(new ShipGameScene(m_Manager));
                m_Manager.LoadScene(new PingPongScene(m_Manager));
			};

			var childPanel = new Panel();
			childPanel.GridColumn = 0;
			childPanel.GridRow = 0;
			
			grid.Widgets.Add(childPanel);

			LoginButton.Click += (s, a) =>
			{
				if (ClientNetworking.m_Instance.Connect("127.0.0.1", 4444))
				{
					PlayButton.Enabled = true;
                    DisconnectButton.Enabled = true;
                    ClientNetworking.m_Instance.Login();
				}
				else
				{
					Debug.WriteLine("Failed to connect");
				}
			};

            DisconnectButton.Click += (s, a) =>
            {
                Debug.WriteLine("Disconnect button clicked");
                ClientNetworking.m_Instance.Disconnect();
                PlayButton.Enabled = false;
                DisconnectButton.Enabled = false;
            };

        }

        public override void UIDraw(float deltaTime)
		{
			base.UIDraw(deltaTime);
			m_Desktop.Render();
		}
	}
}
