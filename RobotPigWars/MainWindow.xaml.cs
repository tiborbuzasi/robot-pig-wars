/**
 * Main Window
 * 
 * Copyright © 2019 Tibor Buzási, Mátyás Spitzner, Martin Szarvas. All rights reserved.
 * For licensing information see LICENSE in the project root folder.
 */

using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
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
        // GUI Game handler
        private GUI.Game gameHandler;
        // Viewbox names
        private Viewbox[] views; 

        public MainWindow()
        {
            InitializeComponent();

            views = new Viewbox[] { GameField, GameOver, NewGame, Settings, Help };
            WindowMaximizeButton_SetImage(WindowMaximizeButton);
        }

        // Handle clicking and dragging of the window title bar
        private void WindowTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

            // Change window size on double click
            if (e.ClickCount == 2)
            {
                WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            }
        }

        // Handle clicking of the close button
        private void WindowCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Handle clicking of the maximize button
        private void WindowMaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            WindowMaximizeButton_SetImage((WindowTitleBarButton) sender);
        }

        // Handle clicking of the minimize button
        private void WindowMinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Minimized) ? WindowState.Normal : WindowState.Minimized;
        }

        // Handle clicking of the help button
        private void WindowHelpButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView("Help");
        }

        // Handle clicking of the settings button
        private void WindowSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView("Settings");
        }

        // Handle clicking of the save game button
        private void WindowSaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if game field is visible
            if (views.Where(x => x.Name == "GameField").First().Visibility != Visibility.Visible)
            {
                return;
            }
            // Check if game is in input state
            if (gameHandler.GameLogic.State != Logic.Game.GameState.Input)
            {
                MessageBox.Show("You can only save a game while you are in your turn.", "Warning - Robot Pig Wars", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get filename via dialog
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "rpw";
            dialog.Filter = "Robot Pig Wars Saved Game file (*.rpw)|*.rpw|All files|*.*";
            if (dialog.ShowDialog() == false)
            {
                return;
            }

            // Try to save the game
            if (gameHandler.GameLogic.SaveGame(dialog.FileName))
            {
                MessageBox.Show("Saving the game was successful.", "Save Game - Robot Pig Wars", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("An error occured while saving the game!", "Save Game - Robot Pig Wars", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handle clicking of the load game button
        private void WindowLoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if game has to be aborted
            if (gameHandler.GameLogic.State != Logic.Game.GameState.GameOver)
            {
                MessageBoxResult result = MessageBox.Show("Loading an existing game will abandon the current game.\nAre you sure you want to proceed?", "Confirm Loading Existing Game - Robot Pig Wars", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            // Get filename via dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "rpw";
            dialog.Filter = "Robot Pig Wars Saved Game file (*.rpw)|*.rpw|All files|*.*";
            if (dialog.ShowDialog() == false)
            {
                return;
            }

            // Try to load the game
            Logic.Game game = gameHandler.GameLogic.LoadGame(dialog.FileName);
            if (game != null)
            { 
                gameHandler = new GUI.Game(game.FieldSize, game);
                ShowView("GameField");
            }
            else
            {
                MessageBox.Show("An error occured while loading the game!", "Load Game - Robot Pig Wars", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handle clicking of the new game button
        private void WindowNewGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if game has to be aborted
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

        // Handle clicking of the OSD action buttons
        private void OsdAction_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OsdActionButton Button = (OsdActionButton) sender;
            if (Button.Weapon == OsdActionButton.Weapons.Gun)
            {
                ActionBox.SetAction(Actions.Gun);
            }
            else if (Button.Weapon == OsdActionButton.Weapons.Fist)
            {
                ActionBox.SetAction(Actions.Fist);
            }
        }

        // Handle clicking of the OSD turn buttons
        private void OsdTurn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OsdTurnButton Button = (OsdTurnButton) sender;
            if (Button.Direction == OsdTurnButton.Directions.Left)
            {
                ActionBox.SetAction(Actions.TurnLeft);
            }
            else if (Button.Direction == OsdTurnButton.Directions.Right)
            {
                ActionBox.SetAction(Actions.TurnRight);
            }
        }

        // Handle clicking of the OSD arrow buttons
        private void OsdArrow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OsdArrowButton Button = (OsdArrowButton) sender;
            if (Button.Direction == OsdArrowButton.Directions.Up)
            {
                ActionBox.SetAction(Actions.Forward);
            }
            else if (Button.Direction == OsdArrowButton.Directions.Down)
            {
                ActionBox.SetAction(Actions.Backward);
            }
            else if (Button.Direction == OsdArrowButton.Directions.Left)
            {
                ActionBox.SetAction(Actions.MoveLeft);
            }
            else if (Button.Direction == OsdArrowButton.Directions.Right)
            {
                ActionBox.SetAction(Actions.MoveRight);
            }
        }

        // Set window maximize button image
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

        // Handle clicking of the end turn button
        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            // Pass steps to the game logic and check turn end state
            gameHandler.GameLogic.SetActions(ActionBox.GetActions());
            gameHandler.GameLogic.EndTurn();

            if (gameHandler.GameLogic.State == Logic.Game.GameState.Input)
            {
                // Show the steps of the next player
                ActionBox.Player = gameHandler.GameLogic.CurrentPlayer; ActionBox.Player++;
                ActionBox.ResetStep();
            }
            else if (gameHandler.GameLogic.State == Logic.Game.GameState.Process)
            {
                // Initialize processing the steps
                gameHandler.ProcessStepInitialize();
                // Change action box display mode
                ActionBox.DisplayType = OsdActionBox.DisplayTypes.AllPlayers;
                ActionBox.Player = gameHandler.GameLogic.CurrentPlayer; ActionBox.Player++;
                ActionBox.ResetStep();
                ActionBox.UpdateStepMarker();
                // Resize grid for action box
                OsdControls.Visibility = Visibility.Collapsed;
                RowDefiniton2.Height = new GridLength(9.0, GridUnitType.Star);
                RowDefiniton3.Height = new GridLength(5.0, GridUnitType.Star);
                // Disable the end turn button
                ProcessButton.IsEnabled = false;
            }
        }

        // Handle end of step processing
        public void ProcessEnd()
        {
            // Check turn end state
            gameHandler.GameLogic.EndTurn();
            // Change action box display mode
            ActionBox.EndTurn();
            ActionBox.Player = gameHandler.GameLogic.CurrentPlayer; ActionBox.Player++;
            ActionBox.DisplayType = OsdActionBox.DisplayTypes.OnePlayer;
            // Resize grid for action box
            OsdControls.Visibility = Visibility.Visible;
            RowDefiniton2.Height = new GridLength(6.0, GridUnitType.Star);
            RowDefiniton3.Height = new GridLength(8.0, GridUnitType.Star);
            // Enable the end turn button
            ProcessButton.IsEnabled = true;

            // Show the game over view
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
                        content.Append("Player Blue");
                    }
                    else
                    {
                        content.Append("Player Red");
                    }
                    content.AppendLine(" has won the game!");
                }
                GameOverLabel.Content = content.ToString();
                ShowView("GameOver");
            }
        }

        // Initialize GUI game handler
        private void Field_Loaded(object sender, RoutedEventArgs e)
        {
            gameHandler = new GUI.Game();
        }

        // Handle clicking of the close button
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ShowView("GameField");
        }

        // Handle clicking of the hyperlink
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        // Handle changing the music settings
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

        // Handle successful loading of the background music
        private void BackgroundMusic_Opened(object sender, RoutedEventArgs e)
        {
            BackgroundMusic.Stop();
            BackgroundMusic.Play();
        }

        // Handle unsuccessful loading of the background music
        private void BackgroundMusic_Failed(object sender, RoutedEventArgs e)
        {
            MusicToggleButton.IsChecked = false;
            MusicToggleButton.IsEnabled = false;
        }

        // Handle clicking of the field size buttons
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

        // Show the view with the given name
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

