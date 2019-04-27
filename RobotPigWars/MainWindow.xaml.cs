using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                ActionBox.SetAction(Actions.Gun);
            }
            else if (Button.Weapon == OsdActionButton.Weapons.Fist)
            {
                ActionBox.SetAction(Actions.Fist);
            }
        }

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
            ActionBox.DisplayType = OsdActionBox.DisplayTypes.AllPlayers;
        }
    }
}

