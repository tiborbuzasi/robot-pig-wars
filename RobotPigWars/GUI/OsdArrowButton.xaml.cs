/**
 * OSD Arrow Button
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
    /// Interaction logic for OsdArrowButton.xaml
    /// </summary>
    public partial class OsdArrowButton : UserControl
    {
        public enum Directions { Left, Right, Up, Down };

        public Directions Direction
        {
            get { return (Directions) GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction",
            typeof(Directions),
            typeof(OsdArrowButton),
            new PropertyMetadata(default(Directions), new PropertyChangedCallback(OnDirectionChanged))
        );

        public OsdArrowButton()
        {
            InitializeComponent();

            // Initializing as a Left arrow button
            BorderPolygon.Points = Application.Current.Resources["OsdLeftArrow"] as PointCollection;
            ContentImage.Children = Application.Current.Resources["OsdLeftArrowImage"] as DrawingCollection;
            BorderPolygon.Style = Application.Current.Resources["OsdArrowButtonBorder"] as Style;
            Content.Margin = new Thickness(0,0,20,0);
        }

        private static void OnDirectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OsdArrowButton Button = (OsdArrowButton) obj;
            Button.Direction = (Directions) args.NewValue;

            switch (Button.Direction)
            {
                case Directions.Right:
                    Button.BorderPolygon.Points = Application.Current.Resources["OsdRightArrow"] as PointCollection;
                    Button.ContentImage.Children = Application.Current.Resources["OsdRightArrowImage"] as DrawingCollection;
                    Button.Content.Margin = new Thickness(20, 0, 0, 0);
                    break;
                case Directions.Up:
                    Button.BorderPolygon.Points = Application.Current.Resources["OsdUpArrow"] as PointCollection;
                    Button.ContentImage.Children = Application.Current.Resources["OsdUpArrowImage"] as DrawingCollection;
                    Button.Content.Margin = new Thickness(0, 0, 0, 20);
                    break;
                case Directions.Down:
                    Button.BorderPolygon.Points = Application.Current.Resources["OsdDownArrow"] as PointCollection;
                    Button.ContentImage.Children = Application.Current.Resources["OsdDownArrowImage"] as DrawingCollection;
                    Button.Content.Margin = new Thickness(0, 20, 0, 0);
                    break;
                default:
                    Button.BorderPolygon.Points = Application.Current.Resources["OsdLeftArrow"] as PointCollection;
                    Button.ContentImage.Children = Application.Current.Resources["OsdLeftArrowImage"] as DrawingCollection;
                    Button.Content.Margin = new Thickness(0, 0, 20, 0);
                    break;
            }
        }

        private void BorderPolygon_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ButtonAnimation();
            }
        }

        private void BorderPolygon_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ButtonAnimation(true);
            }
        }

        private void BorderPolygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ButtonAnimation();
        }

        private void BorderPolygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
