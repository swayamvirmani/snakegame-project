using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace snakegame
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValtoImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }
        };

        private readonly int rows = 15;
        private readonly int cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
                gameRunning = true;
                Overlay.Visibility = Visibility.Hidden;
                await RunGame();
                gameRunning = false;
                gameState = new GameState(rows, cols);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameRunning || gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async Task RunGame()
        {
            Draw();
            await GameLoop();
            await ShowGameOver();
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(150);
                gameState.Move();
                Draw();
            }
        }

        private async Task ShowGameOver()
        {
            await Task.Delay(100);
            OverlayText.Text = "Game Over!\nPress any key to restart";
            Overlay.Visibility = Visibility.Visible;
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Image image = new Image { Source = Images.Empty };
                    images[i, j] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    GridValue value = gameState.Grid[i, j];
                    gridImages[i, j].Source = gridValtoImage[value];
                }
            }
        }
    }
}
