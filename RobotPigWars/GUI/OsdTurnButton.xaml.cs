/**
 * OSD Turn Button
 * 
 * Copyright © 2019 Tibor Buzási, Mátyás Spitzner, Martin Szarvas. All rights reserved.
 * For licensing information see LICENSE in the project root folder.
 */

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RobotPigWars.GUI
{
    /// <summary>
    /// Interaction logic for OsdTurnButton.xaml
    /// </summary>
    public partial class OsdTurnButton : UserControl
    {
        public enum Directions { Left, Right };
        
        public Directions Direction
        {
            get { return (Directions) GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction",
            typeof(Directions),
            typeof(OsdTurnButton),
            new PropertyMetadata(default(Directions), new PropertyChangedCallback(OnDirectionChanged))
        );

        public OsdTurnButton()
        {
            InitializeComponent();

            // Initializing as a Left turn button
            ContentImage.Children = Application.Current.Resources["OsdLeftTurnImage"] as DrawingCollection;
            BorderRectangle.Style = Application.Current.Resources["OsdTurnButtonBorder"] as Style;
        }

        private static void OnDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OsdTurnButton Button = (OsdTurnButton) obj;
            Button.Direction = (Directions) args.NewValue;

            switch (Button.Direction)
            {
                case Directions.Right:
                    Button.ContentImage.Children = Application.Current.Resources["OsdRightTurnImage"] as DrawingCollection;
                    break;
                default:
                    Button.ContentImage.Children = Application.Current.Resources["OsdLeftTurnImage"] as DrawingCollection;
                    break;
            }
        }

        private void BorderRectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ButtonAnimation();
            }
        }

        private void BorderRectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ButtonAnimation(true);
            }
        }

        private void BorderRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonAnimation();
        }

        private void BorderRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ButtonAnimation(true);
        }

        private void ButtonAnimation(bool negative = false)
        {
            short direction = negative ? (short)-1 : (short)1;

            Thickness Margin = new Thickness(Content.Margin.Left + direction, Content.Margin.Top + direction, Content.Margin.Right - direction, Content.Margin.Bottom - direction);
            Content.Margin = Margin;
        }
    }
}
