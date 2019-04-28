using System;
using System.Collections.Generic;
using System.Linq;
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

        public MainWindow()
        {
            InitializeComponent();

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

        }

        private void WindowSettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WindowSaveGameButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WindowLoadGameButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WindowNewGameButton_Click(object sender, RoutedEventArgs e)
        {

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
            else
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
        }

        private void Field_Loaded(object sender, RoutedEventArgs e)
        {
            gameHandler = new GUI.Game();
        }
    }
}

