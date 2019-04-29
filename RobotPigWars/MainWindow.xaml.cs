using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using RobotPigWars.GUI;
using RobotPigWars.Logic;

namespace RobotPigWars
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GUI.Game gameHandler;
        private Viewbox[] views; 

        public MainWindow()
        {
            InitializeComponent();

            views = new Viewbox[] { GameField, GameOver, NewGame, Settings, Help };
            WindowMaximizeButton_SetImage(WindowMaximizeButton);
        }

    private void WindowTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

            if (e.ClickCount == 2)
            {
                WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            }
        }

        private void WindowCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WindowMaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            WindowMaximizeButton_SetImage((WindowTitleBarButton) sender);
        }

        private void WindowMinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Minimized) ? WindowState.Normal : WindowState.Minimized;
        }

        private void WindowHelpButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView("Help");
        }

        private void WindowSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView("Settings");
        }

        private void WindowSaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (views.Where(x => x.Name == "GameField").First().Visibility != Visibility.Visible)
            {
                return;
            }
            if (gameHandler.GameLogic.State != Logic.Game.GameState.Input)
            {
                MessageBox.Show("You can only save a game while you are in your turn.", "Warning - Robot Pig Wars", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "rpw";
            dialog.Filter = "Robot Pig Wars Saved Game file (*.rpw)|*.rpw|All files|*.*";
            if (dialog.ShowDialog() == false)
            {
                return;
            }

            FileInfo gameFileInfo;
            BinaryWriter gameFile= null;

            try
            {
                gameFileInfo = new FileInfo(dialog.FileName);
                gameFile = new BinaryWriter(
                    gameFileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));

                gameFile.Write(gameHandler.GameLogic.FieldSize);
                for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
                {
                    for (byte i = 0; i < 2; i++)
                    {
                        gameFile.Write((Int32) gameHandler.GameLogic.Players[p].Position[i]);
                    }
                    gameFile.Write((Int32) gameHandler.GameLogic.Players[p].Face);
                    gameFile.Write((Int32) gameHandler.GameLogic.Players[p].Life);
                }
                gameFile.Close();

                MessageBox.Show("Saving the game was successful.", "Save Game - Robot Pig Wars", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while saving the game!", "Save Game - Robot Pig Wars", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (gameFile != null)
                gameFile.Dispose();
        }

        private void WindowLoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameHandler.GameLogic.State != Logic.Game.GameState.GameOver)
            {
                MessageBoxResult result = MessageBox.Show("Loading an existing game will abandon the current game.\nAre you sure you want to proceed?", "Confirm Loading Existing Game - Robot Pig Wars", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "rpw";
            dialog.Filter = "Robot Pig Wars Saved Game file (*.rpw)|*.rpw|All files|*.*";
            if (dialog.ShowDialog() == false)
            {
                return;
            }

            FileInfo gameFileInfo;
            BinaryReader gameFile = null;

            try
            {
                gameFileInfo = new FileInfo(dialog.FileName);
                gameFile = new BinaryReader(gameFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read));

                byte fieldSize = gameFile.ReadByte();
                int[,] positions = new int[2, 2];
                int[,] facesAndLives = new int[2, 2];
                Logic.Player[] players = new Logic.Player[2];

                for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
                {
                    for (byte i = 0; i < 2; i++)
                    {
                        positions[p, i] = gameFile.ReadInt32();
                    }
                    for (byte i = 0; i < 2; i++)
                    {
                        facesAndLives[p, i] = gameFile.ReadInt32();
                    }
                    players[p] = new Logic.Player(facesAndLives[p, 1], positions[p, 0], positions[p, 1], (Directions) facesAndLives[p, 0], Enumerable.Repeat(Actions.None, Logic.Game.numberOfSteps).ToArray());
                }
                gameFile.Close();

                Logic.Game game = new Logic.Game(fieldSize, players);
                gameHandler = new GUI.Game(fieldSize, game);
                ShowView("GameField");
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured while loading the game!", "Load Game - Robot Pig Wars", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WindowNewGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameHandler.GameLogic.State != Logic.Game.GameState.GameOver)
            {
                MessageBoxResult result = MessageBox.Show("Creating a new game will abandon the current game.\nAre you sure you want to proceed?", "Confirm Creating New Game - Robot Pig Wars", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            ShowView("NewGame");
        }

        private void OsdAction_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OsdActionButton Button = (OsdActionButton) sender;
            if (Button.Weapon == OsdActionButton.Weapons.Gun)
            {
                gameHandler.GameLogic.SetAction(Actions.Gun);
                ActionBox.SetAction(Actions.Gun);
            }
            else if (Button.Weapon == OsdActionButton.Weapons.Fist)
            {
                gameHandler.GameLogic.SetAction(Actions.Fist);
                ActionBox.SetAction(Actions.Fist);
            }
        }

        private void OsdTurn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OsdTurnButton Button = (OsdTurnButton) sender;
            if (Button.Direction == OsdTurnButton.Directions.Left)
            {
                gameHandler.GameLogic.SetAction(Actions.TurnLeft);
                ActionBox.SetAction(Actions.TurnLeft);
            }
            else if (Button.Direction == OsdTurnButton.Directions.Right)
            {
                gameHandler.GameLogic.SetAction(Actions.TurnRight);
                ActionBox.SetAction(Actions.TurnRight);
            }
        }

        private void OsdArrow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OsdArrowButton Button = (OsdArrowButton) sender;
            if (Button.Direction == OsdArrowButton.Directions.Up)
            {
                gameHandler.GameLogic.SetAction(Actions.Forward);
                ActionBox.SetAction(Actions.Forward);
            }
            else if (Button.Direction == OsdArrowButton.Directions.Down)
            {
                gameHandler.GameLogic.SetAction(Actions.Backward);
                ActionBox.SetAction(Actions.Backward);
            }
            else if (Button.Direction == OsdArrowButton.Directions.Left)
            {
                gameHandler.GameLogic.SetAction(Actions.MoveLeft);
                ActionBox.SetAction(Actions.MoveLeft);
            }
            else if (Button.Direction == OsdArrowButton.Directions.Right)
            {
                gameHandler.GameLogic.SetAction(Actions.MoveRight);
                ActionBox.SetAction(Actions.MoveRight);
            }
        }

        private void WindowMaximizeButton_SetImage(WindowTitleBarButton Button)
        {
            if (WindowState == WindowState.Maximized)
            {
                Button.Image = (DrawingCollection) Application.Current.Resources["WindowRestoreSizeImage"];
            }
            else
            {
                Button.Image = (DrawingCollection) Application.Current.Resources["WindowMaximizeImage"];
            }
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            gameHandler.GameLogic.EndTurn();
            if (gameHandler.GameLogic.State == Logic.Game.GameState.Input)
            {
                ActionBox.Player = gameHandler.GameLogic.CurrentPlayer; ActionBox.Player++;
                ActionBox.ResetStep();
            }
            else if (gameHandler.GameLogic.State == Logic.Game.GameState.Process)
            {
                gameHandler.ProcessStepInitialize();
                ActionBox.DisplayType = OsdActionBox.DisplayTypes.AllPlayers;
                ActionBox.Player = gameHandler.GameLogic.CurrentPlayer; ActionBox.Player++;
                ActionBox.ResetStep();
                ActionBox.UpdateStepMarker();
                OsdControls.Visibility = Visibility.Collapsed;
                RowDefiniton2.Height = new GridLength(9.0, GridUnitType.Star);
                RowDefiniton3.Height = new GridLength(5.0, GridUnitType.Star);
                ProcessButton.IsEnabled = false;
            }
        }

        public void ProcessEnd()
        {
            gameHandler.GameLogic.EndTurn();
            ActionBox.EndTurn();
            ActionBox.Player = gameHandler.GameLogic.CurrentPlayer; ActionBox.Player++;
            ActionBox.DisplayType = OsdActionBox.DisplayTypes.OnePlayer;
            OsdControls.Visibility = Visibility.Visible;
            RowDefiniton2.Height = new GridLength(6.0, GridUnitType.Star);
            RowDefiniton3.Height = new GridLength(8.0, GridUnitType.Star);
            ProcessButton.IsEnabled = true;

            if (gameHandler.GameLogic.State == Logic.Game.GameState.GameOver)
            {
                StringBuilder content = new StringBuilder();
                if (gameHandler.GameLogic.Players[0].Life == 0 && gameHandler.GameLogic.Players[0].Life == 0)
                {
                    content.Append("The game ended in a draw!");
                }
                else
                {
                    if (gameHandler.GameLogic.Players[0].Life == 0)
                    {
                        content.Append("Player Red");
                    }
                    else
                    {
                        content.Append("Player Blue");
                    }
                    content.AppendLine(" has won the game!");
                }
                GameOverLabel.Content = content.ToString();
                ShowView("GameOver");
            }
        }

        private void Field_Loaded(object sender, RoutedEventArgs e)
        {
            gameHandler = new GUI.Game();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView("GameField");
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void Music_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox CheckBox = (CheckBox) sender;
            if (CheckBox.IsChecked == true)
            {
                BackgroundMusic.Play();
            }
            else
            {
                BackgroundMusic.Stop();
            }
        }

        private void BackgroundMusic_Opened(object sender, RoutedEventArgs e)
        {
            BackgroundMusic.Stop();
            BackgroundMusic.Play();
        }

        private void BackgroundMusic_Failed(object sender, RoutedEventArgs e)
        {
            MusicToggleButton.IsChecked = false;
            MusicToggleButton.IsEnabled = false;
        }

        private void FieldSizeButton_Click(object sender, RoutedEventArgs e)
        {
            Button Button = (Button) sender;

            switch (Button.Content)
            {
                case "4 × 4":
                    gameHandler = new GUI.Game(4);
                    break;
                case "6 × 6":
                    gameHandler = new GUI.Game(6);
                    break;
                case "8 × 8":
                    gameHandler = new GUI.Game(8);
                    break;
            }
            ShowView("GameField");
        }

        private void ShowView(string viewName)
        {
            foreach (Viewbox view in views)
            {
                view.Visibility = Visibility.Collapsed;
            }
            views.Where(x => x.Name == viewName).First().Visibility = Visibility.Visible;
        }
    }
}

