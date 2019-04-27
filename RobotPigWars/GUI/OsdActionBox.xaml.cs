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
    /// Interaction logic for OsdActionButton.xaml
    /// </summary>
    public partial class OsdActionBox : UserControl
    {
        const byte numberOfPlayers = 2;
        const byte numberOfSteps = 5;
        const ushort positionBaseX = 21;
        const ushort positionBaseY = 42;
        const ushort horizontalDistance = 46;
        const ushort verticalDistance = 50;
        public static readonly Brush[] playerColors = {
            new SolidColorBrush((Color) ColorConverter.ConvertFromString("#7F8B0000")),
            new SolidColorBrush((Color) ColorConverter.ConvertFromString("#7F00008B"))
        };

        public enum DisplayTypes { OnePlayer, AllPlayers };

        private OsdActionInfoBox[,] actionInfo = new OsdActionInfoBox[2,5];

        public DisplayTypes DisplayType
        {
            get { return (DisplayTypes) GetValue(DisplayTypeProperty); }
            set { SetValue(DisplayTypeProperty, value); }
        }

        public byte CurrentStep { get; private set; }

        public static readonly DependencyProperty DisplayTypeProperty = DependencyProperty.Register(
            "DisplayType",
            typeof(DisplayTypes),
            typeof(OsdActionBox),
            new PropertyMetadata(default(DisplayTypes), new PropertyChangedCallback(OnPropertyChanged))
        );

        public byte Player
        {
            get { return (byte)GetValue(PlayerProperty); }
            set { SetValue(PlayerProperty, value); }
        }

        public static readonly DependencyProperty PlayerProperty = DependencyProperty.Register(
            "Player",
            typeof(byte),
            typeof(OsdActionBox),
            new PropertyMetadata(default(byte), new PropertyChangedCallback(OnPropertyChanged))
        );

        public OsdActionBox()
        {
            InitializeComponent();

            // Initializing action info boxes
            for (byte p = 0; p < 2; p++)
            {
                for (byte s = 0; s < 5; s++)
                {
                    actionInfo[p, s] = new OsdActionInfoBox() { Action = Actions.None };
                }
            }
            CurrentStep = 0;
        }

        private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OsdActionBox ActionBox = (OsdActionBox) obj;
            
            if (args.Property.Name == "DisplayType")
            {
                ActionBox.DisplayType = (DisplayTypes) args.NewValue;
            }
            else if (args.Property.Name == "Player")
            {
                ActionBox.Player = (byte) args.NewValue;
            }

            DisplayActionBox(ActionBox);
        }

        private static void DisplayActionBox(OsdActionBox ActionBox)
        {
            ClearActionBox(ActionBox);
            if (ActionBox.DisplayType == DisplayTypes.OnePlayer && ActionBox.Player != 0)
            {
                for (byte s = 0; s < numberOfSteps; s++)
                {
                    AddActionBoxItem(ActionBox, (byte) (ActionBox.Player - 1), s);
                }
            }
            else if (ActionBox.DisplayType == DisplayTypes.AllPlayers)
            {
                for (byte p = 0; p < numberOfPlayers; p++)
                {
                    Rectangle PlayerHighlight = new Rectangle();
                    PlayerHighlight.Width = ActionBox.Canvas.ActualWidth - ActionBox.Border.Margin.Left - ActionBox.Border.Margin.Right;
                    Canvas.SetLeft(PlayerHighlight, ActionBox.Border.Margin.Left);
                    Canvas.SetTop(PlayerHighlight, positionBaseY + p * verticalDistance - (verticalDistance - ActionBox.actionInfo[0,0].ActualHeight) / 2 + 1);
                    PlayerHighlight.Style = (Style) Application.Current.FindResource("OsdActionBoxPlayerHighlight");
                    PlayerHighlight.Fill = playerColors[p];
                    ActionBox.Canvas.Children.Add(PlayerHighlight);

                    Rectangle StepMarker = new Rectangle();
                    Canvas.SetLeft(StepMarker, positionBaseX + ActionBox.CurrentStep * horizontalDistance - (horizontalDistance - ActionBox.actionInfo[0, 0].ActualWidth) / 2 - 1);
                    Canvas.SetTop(StepMarker, positionBaseY + p * verticalDistance - (verticalDistance - ActionBox.actionInfo[0, 0].ActualHeight) / 2 + 1);
                    StepMarker.Style = (Style) Application.Current.FindResource("OsdActionBoxStepMarker");
                    ActionBox.Canvas.Children.Add(StepMarker);


                    for (byte s = 0; s < numberOfSteps; s++)
                    {
                        AddActionBoxItem(ActionBox, p, s, p);
                    }
                }
            }
        }

        private static void AddActionBoxItem(OsdActionBox ActionBox, byte player, byte step, byte row = 0)
        {
            Canvas.SetLeft(ActionBox.actionInfo[player, step], positionBaseX + step * horizontalDistance);
            Canvas.SetTop(ActionBox.actionInfo[player, step], positionBaseY + row * verticalDistance);
            ActionBox.actionInfo[player, step].BorderRectangle.Style = (Style) Application.Current.FindResource("OsdActionInfoBox");
            ActionBox.Canvas.Children.Add(ActionBox.actionInfo[player, step]);
        }

        private static void ClearActionBox(OsdActionBox ActionBox)
        {
            ActionBox.Canvas.Children.Clear();
        }

        public Actions GetAction()
        {
            return actionInfo[Player - 1, CurrentStep].Action;
        }

        public void SetAction(Actions action)
        {
                actionInfo[Player - 1, CurrentStep].Action = action;
                NextStep();
        }

        public void NextStep()
        {
            if (CurrentStep < numberOfSteps - 1)
            {
                CurrentStep++;
            }
        }
    }
}
