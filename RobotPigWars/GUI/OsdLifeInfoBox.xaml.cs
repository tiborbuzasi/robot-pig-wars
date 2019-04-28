using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using RobotPigWars.Logic;

namespace RobotPigWars.GUI
{
    /// <summary>
    /// Interaction logic for OsdLifeInfoBox.xaml
    /// </summary>
    public partial class OsdLifeInfoBox : UserControl
    {
        public byte Life
        {
            get { return (byte) GetValue(LifeProperty); }
            set { SetValue(LifeProperty, value); }
        }

        public static readonly DependencyProperty LifeProperty = DependencyProperty.Register(
            "Life",
            typeof(byte),
            typeof(OsdLifeInfoBox),
            new PropertyMetadata(default(byte), new PropertyChangedCallback(OnLifeChanged))
        );

        public byte ColorIndex
        {
            get { return (byte) GetValue(ColorIndexProperty); }
            set { SetValue(ColorIndexProperty, value); }
        }

        public static readonly DependencyProperty ColorIndexProperty = DependencyProperty.Register(
            "ColorIndex",
            typeof(byte),
            typeof(OsdLifeInfoBox),
            new PropertyMetadata((byte) 16, new PropertyChangedCallback(OnColorIndexChanged))
        );

        public OsdLifeInfoBox()
        {
            InitializeComponent();

            // Initializing as a not defined action
            Content.Content = Logic.Game.numberOfLives.ToString();
            Content.Style = Application.Current.Resources["OsdLifeInfoBoxContent"] as Style;
            BorderEllipse.Style = Application.Current.Resources["OsdLifeInfoBox"] as Style;
        }

        private static void OnLifeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OsdLifeInfoBox InfoBox = (OsdLifeInfoBox) obj;
            InfoBox.Life = (byte) args.NewValue;

            InfoBox.Content.Content = InfoBox.Life.ToString();
        }

        private static void OnColorIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OsdLifeInfoBox InfoBox = (OsdLifeInfoBox) obj;
            InfoBox.ColorIndex = (byte) args.NewValue;

            InfoBox.BorderEllipse.Fill = Player.playerColors[InfoBox.ColorIndex];
        }

}
}
