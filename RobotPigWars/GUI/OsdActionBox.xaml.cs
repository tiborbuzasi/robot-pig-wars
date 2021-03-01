/**
 * OSD Action Box
 * 
 * Copyright © 2019 Tibor Buzási, Mátyás Spitzner, Martin Szarvas. All rights reserved.
 * For licensing information see LICENSE in the project root folder.
 */

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
    /// Interaction logic for OsdActionBox.xaml
    /// </summary>
    public partial class OsdActionBox : UserControl
    {
        const ushort positionBaseX = 21;
        const ushort positionBaseY = 42;
        const ushort horizontalDistance = 46;
        const ushort verticalDistance = 50;

        public enum DisplayTypes { OnePlayer, AllPlayers };

        private OsdActionInfoBox[,] actionInfo = new OsdActionInfoBox[2, 5];
        public byte CurrentStep { get; private set; }
        private static Rectangle[] StepMarker = new Rectangle[2];

        public DisplayTypes DisplayType
        {
            get { return (DisplayTypes)GetValue(DisplayTypeProperty); }
            set { SetValue(DisplayTypeProperty, value); }
        }

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
            InitializeActionInfoBoxes();
        }

        private void InitializeActionInfoBoxes()
        {
            for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
            {
                for (byte s = 0; s < Logic.Game.numberOfSteps; s++)
                {
                    actionInfo[p, s] = new OsdActionInfoBox() { Action = Actions.None ,StepIndex = s };
                    actionInfo[p, s].MouseLeftButtonDown += OsdActionInfoBox_MouseLeftButtonUp;
                    actionInfo[p, s].MouseLeftButtonDown += OsdActionBox_MouseLeftButtonUp;
                    actionInfo[p, s].MouseRightButtonUp += OsdActionInfoBox_MouseRightButtonUp;
                }
            }
            CurrentStep = 0;
        }

        private void OsdActionBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
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
                for (byte s = 0; s < Logic.Game.numberOfSteps; s++)
                {
                    AddActionBoxItem(ActionBox, (byte) (ActionBox.Player - 1), s);
                }
            }
            else if (ActionBox.DisplayType == DisplayTypes.AllPlayers)
            {
                for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
                {
                    Rectangle PlayerHighlight = new Rectangle();
                    PlayerHighlight.Width = ActionBox.Canvas.ActualWidth - ActionBox.Border.Margin.Left - ActionBox.Border.Margin.Right;
                    Canvas.SetLeft(PlayerHighlight, ActionBox.Border.Margin.Left);
                    Canvas.SetTop(PlayerHighlight, positionBaseY + p * verticalDistance - (verticalDistance - ActionBox.actionInfo[0,0].ActualHeight) / 2 + 1);
                    PlayerHighlight.Style = (Style) Application.Current.FindResource("OsdActionBoxPlayerHighlight");
                    Brush brush = new SolidColorBrush(GUI.Player.playerColors[p].Color);
                    brush.Opacity = 0.5;
                    PlayerHighlight.Fill = brush;
                    ActionBox.Canvas.Children.Add(PlayerHighlight);

                    StepMarker[p] = new Rectangle();
                    Canvas.SetLeft(StepMarker[p], positionBaseX + ActionBox.CurrentStep * horizontalDistance - (horizontalDistance - ActionBox.actionInfo[0, 0].ActualWidth) / 2 - 1);
                    Canvas.SetTop(StepMarker[p], positionBaseY + p * verticalDistance - (verticalDistance - ActionBox.actionInfo[0, 0].ActualHeight) / 2 + 1);
                    StepMarker[p].Style = (Style) Application.Current.FindResource("OsdActionBoxStepMarker");
                    ActionBox.Canvas.Children.Add(StepMarker[p]);


                    for (byte s = 0; s < Logic.Game.numberOfSteps; s++)
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

        public Actions[] GetActions()
        {
            Actions[] actions = new Actions[Logic.Game.numberOfSteps];

            for (byte s = 0; s < Logic.Game.numberOfSteps; s++)
            {
                actions[s] = actionInfo[Player - 1,s].Action;
            }

            return actions;
        }

        public void SetAction(Actions action)
        {
                actionInfo[Player - 1, CurrentStep].Action = action;
                NextStep();
        }

        public void NextStep()
        {
            if (CurrentStep < Logic.Game.numberOfSteps - 1)
            {
                CurrentStep++;
            }
        }

        public void UpdateStepMarker()
        {
            if (DisplayType == DisplayTypes.AllPlayers)
            {
                for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
                {
                    Canvas.SetLeft(StepMarker[p], positionBaseX + CurrentStep * horizontalDistance - (horizontalDistance - actionInfo[0, 0].ActualWidth) / 2 - 1);
                    Canvas.SetTop(StepMarker[p], positionBaseY + p * verticalDistance - (verticalDistance - actionInfo[0, 0].ActualHeight) / 2 + 1);
                }
            }
        }

        public void ResetStep()
        {
                CurrentStep = 0;
        }

        public void EndTurn()
        {
            InitializeActionInfoBoxes();
        }

        private void OsdActionInfoBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OsdActionInfoBox InfoBox = (OsdActionInfoBox) sender;
            CurrentStep = InfoBox.StepIndex;

            MessageBox.Show("1");
        }

        private void OsdActionInfoBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

            OsdActionInfoBox InfoBox = (OsdActionInfoBox) sender;
            CurrentStep = InfoBox.StepIndex;

            SetAction(Actions.None);
            MessageBox.Show("2");
        }
    }
}
