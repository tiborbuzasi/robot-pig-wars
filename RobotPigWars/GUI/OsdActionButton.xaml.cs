using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RobotPigWars.GUI
{
    /// <summary>
    /// Interaction logic for OsdActionButton.xaml
    /// </summary>
    public partial class OsdActionButton : UserControl
    {
        public enum Weapons { Fist, Gun };
        
        public Weapons Weapon
        {
            get { return (Weapons) GetValue(WeaponProperty); }
            set { SetValue(WeaponProperty, value); }
        }

        public static readonly DependencyProperty WeaponProperty = DependencyProperty.Register(
            "Weapon",
            typeof(Weapons),
            typeof(OsdActionButton),
            new PropertyMetadata(default(Weapons), new PropertyChangedCallback(OnWeaponChanged))
        );

        public OsdActionButton()
        {
            InitializeComponent();

            // Initializing as a Fist action button
            ContentImage.Children = Application.Current.Resources["OsdFistActionImage"] as DrawingCollection;
            BorderEllipse.Style = Application.Current.Resources["OsdActionButtonBorder"] as Style;
        }

        private static void OnWeaponChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OsdActionButton Button = (OsdActionButton) obj;
            Button.Weapon = (Weapons) args.NewValue;

            switch (Button.Weapon)
            {
                case Weapons.Gun:
                    Button.ContentImage.Children = Application.Current.Resources["OsdGunActionImage"] as DrawingCollection;
                    break;
                default:
                    Button.ContentImage.Children = Application.Current.Resources["OsdFistActionImage"] as DrawingCollection;
                    break;
            }
        }

        private void BorderEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ButtonAnimation();
            }
        }

        private void BorderEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ButtonAnimation(true);
            }
        }

        private void BorderEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonAnimation();
        }

        private void BorderEllipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ButtonAnimation(true);
        }

        private void ButtonAnimation(bool negative = false)
        {
            short direction = negative ? (short) -1 : (short) 1;

            Thickness Margin = new Thickness(Content.Margin.Left + direction, Content.Margin.Top + direction, Content.Margin.Right - direction, Content.Margin.Bottom - direction);
            Content.Margin = Margin;
        }
    }
}
