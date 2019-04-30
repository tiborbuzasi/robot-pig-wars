using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using RobotPigWars.Logic;

namespace RobotPigWars.GUI
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : UserControl
    {
        private static MainWindow app = Application.Current.Windows[0] as MainWindow;
        private static readonly Canvas field = app.Field;

        public static readonly SolidColorBrush[] playerColors = {
            new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF8B0000")),
            new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF00008B"))
        };

        public byte ColorIndex
        {
            get { return (byte) GetValue(ColorIndexProperty); }
            set { SetValue(ColorIndexProperty, value); }
        }

        public static readonly DependencyProperty ColorIndexProperty = DependencyProperty.Register(
            "ColorIndex",
            typeof(byte),
            typeof(Player),
            new PropertyMetadata(default(byte), new PropertyChangedCallback(OnColorIndexChanged))
        );

        public Directions Direction
        {
            get { return (Directions) GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction",
            typeof(Directions),
            typeof(Player),
            new PropertyMetadata(default(Directions), new PropertyChangedCallback(OnDirectionChanged))
        );

        public Player()
        {
            InitializeComponent();

            Rectangle Back = (Rectangle) field.Children[0];
            Width = Back.Width;
            Height = Back.Height;

            // Initializing as a pig with face up
            ContentImage.Children = Application.Current.Resources["PlayerFaceUp"] as DrawingCollection;
            PlayerRibbon.Brush = playerColors[ColorIndex];

        }

        private static void OnColorIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Player Player = (Player) obj;
            Player.ColorIndex = (byte) args.NewValue;

            Player.PlayerRibbon.Brush = playerColors[Player.ColorIndex];
        }

        private static void OnDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Player Player = (Player) obj;
            Player.Direction = (Directions) args.NewValue;

            switch (Player.Direction)
            {
                case Directions.Down:
                    Player.ContentImage.Children = Application.Current.Resources["PlayerFaceDown"] as DrawingCollection;
                    break;
                case Directions.Left:
                    Player.ContentImage.Children = Application.Current.Resources["PlayerFaceLeft"] as DrawingCollection;
                    break;
                case Directions.Right:
                    Player.ContentImage.Children = Application.Current.Resources["PlayerFaceRight"] as DrawingCollection;
                    break;
                default:
                    Player.ContentImage.Children = Application.Current.Resources["PlayerFaceUp"] as DrawingCollection;
                    break;
            }
        }
    }
}
