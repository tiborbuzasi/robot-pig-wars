/**
 * UI Window Title Bar Button
 * 
 * Copyright © 2019 Tibor Buzási, Mátyás Spitzner, Martin Szarvas. All rights reserved.
 * For licensing information see LICENSE in the project root folder.
 */

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RobotPigWars.GUI
{
    /// <summary>
    /// Interaction logic for WindowTitleBarButton.xaml
    /// </summary>
    public partial class WindowTitleBarButton : Button
    {
        public DrawingCollection Image
        {
            get { return (DrawingCollection) GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
            "Image",
            typeof(DrawingCollection),
            typeof(WindowTitleBarButton),
            new PropertyMetadata(default(DrawingCollection), new PropertyChangedCallback(OnImageChanged))
        );

        public WindowTitleBarButton()
        {
            InitializeComponent();

            // Initializing
            ButtonBase.Style = Application.Current.Resources["WindowTitleBarButton"] as Style;
        }

        private static void OnImageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            WindowTitleBarButton Button = (WindowTitleBarButton) obj;
            Button.ContentImage.Children = (DrawingCollection) args.NewValue;
        }
    }
}
