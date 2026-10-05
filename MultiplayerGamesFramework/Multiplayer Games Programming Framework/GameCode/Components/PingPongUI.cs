using AssetManagementBase;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Client.GameCode.Components
{
    // UI for pingpong game
    internal class PingPongUI : Component
    {
        private Desktop m_Desktop;
        private Label m_ScoreLabel;

        public PingPongUI(GameObject gameObject) : base(gameObject)
        {
            var grid = new Grid
            {
                ShowGridLines = false,
                RowSpacing = 8,
                ColumnSpacing = 8
            };

            for (int i = 0; i < 3; i++) 
            {
                grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));

            }
            for (int i = 0; i < 3; i++) 
            {
                grid.RowsProportions.Add(new Proportion(ProportionType.Part));

            }
            
            m_Desktop = new Desktop();
            m_Desktop.Root = grid;

            m_ScoreLabel = new Label();
            m_ScoreLabel.Text = "0   -   0";
            m_ScoreLabel.HorizontalAlignment = HorizontalAlignment.Center;
            m_ScoreLabel.VerticalAlignment = VerticalAlignment.Top;
            m_ScoreLabel.Top = 10;

            Grid.SetColumn(m_ScoreLabel, 1);
            Grid.SetRow(m_ScoreLabel, 0);

            grid.Widgets.Add(m_ScoreLabel);
        }

        protected override void UIDraw(float deltaTime)
        {
            m_Desktop.Render();
        }

        public void SetScore(int left, int right)
        {
            m_ScoreLabel.Text = $"{left}   -   {right}";
        }

        public void ShowGameOver(string winner)
        {
            var overlay = new Panel
            {
                Background = new Myra.Graphics2D.Brushes.SolidBrush(new Microsoft.Xna.Framework.Color(0, 0, 0, 180)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var label = new Label
            {
                Text = $"GAME OVER \n{winner} Wins!",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextColor = Color.Yellow,
                Padding = new Myra.Graphics2D.Thickness(20),
                Id = "GameOverLabel"
            };

            overlay.Widgets.Add(label);

            m_Desktop.Root = overlay;
        }
    }
}
