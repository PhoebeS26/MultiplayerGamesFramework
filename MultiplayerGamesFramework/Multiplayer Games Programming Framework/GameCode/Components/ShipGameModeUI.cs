using System;
using System.Collections.Generic;
using Game_Client.Core.Utilities;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;

namespace Game_Client.GameCode.Components
{
    internal class ShipGameModeUI : Component
    {
        private Desktop m_Desktop;
        Label m_ScoreText;
        Label m_Time;
        public ShipGameModeUI(GameObject gameObject) : base(gameObject)
        {
            var grid = new Grid
            {
                ShowGridLines = false,
                RowSpacing = 8,
                ColumnSpacing = 8
            };

            int cols = 4;
            for (int i = 0; i < cols; ++i)
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

            m_ScoreText = new Label();
            m_ScoreText.Text = "Score : 0"; // sets value of the text
            Grid.SetColumn(m_ScoreText, 4);//sets in the top right corner based off of grid count
            m_ScoreText.HorizontalAlignment = HorizontalAlignment.Right; // alligns text to the right
            m_ScoreText.Left = -10; // gives a right padding of 10px;
            grid.Widgets.Add(m_ScoreText); // add the label to the grid

            m_Time = new Label();
            m_Time.Text = ""; // sets value of the text
            Grid.SetColumn(m_Time, 2);//sets in the top right corner based off of grid count
            m_Time.HorizontalAlignment = HorizontalAlignment.Center; // alligns text to the right
            grid.Widgets.Add(m_Time); // add the label to the grid
        }

        protected override void UIDraw(float deltaTime)
        {
            m_Desktop.Render();
        }

        public void SetScore(int score)
        {
            m_ScoreText.Text = "Score : " + score;
        }

        public void SetTime(float time)
        {
            time = MathF.Max(time, 0f);
            TimeSpan ts = TimeSpan.FromSeconds(time);
            m_Time.Text = ts.TotalSeconds.ToString("0.00");
        }
    }
}
