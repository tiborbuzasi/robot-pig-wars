using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RobotPigWars.Logic;

namespace RobotPigWars.GUI
{
    /// <summary>
    /// Interaction logic for OsdActionInfoBox.xaml
    /// </summary>
    public partial class OsdActionInfoBox : UserControl
    {
        public Actions Action
        {
            get { return (Actions) GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register(
            "Action",
            typeof(Actions),
            typeof(OsdActionInfoBox),
            new PropertyMetadata(default(Actions), new PropertyChangedCallback(OnActionChanged))
        );

        public byte StepIndex
        {
            get { return (byte) GetValue(StepIndexProperty); }
            set { SetValue(StepIndexProperty, value); }
        }

        public static readonly DependencyProperty StepIndexProperty = DependencyProperty.Register(
            "StepIndex",
            typeof(byte),
            typeof(OsdActionInfoBox),
            new PropertyMetadata(default(byte), new PropertyChangedCallback(OnStepIndexChanged))
        );

        public OsdActionInfoBox()
        {
            InitializeComponent();

            // Initializing as a not defined step
            ContentImage.Children = new DrawingCollection();
            BorderRectangle.Style = Application.Current.Resources["OsdActionInfoBox"] as Style;
        }

        private static void OnActionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OsdActionInfoBox InfoBox = (OsdActionInfoBox) obj;
            InfoBox.Action = (Actions) args.NewValue;

            switch (InfoBox.Action)
            {
                case Actions.Forward:
                    InfoBox.ContentImage.Children = Application.Current.Resources["OsdUpArrowImage"] as DrawingCollection;
                    break;
                case Actions.Backward:
                    InfoBox.ContentImage.Children = Application.Current.Resources["OsdDownArrowImage"] as DrawingCollection;
                    break;
                case Actions.MoveLeft:
                    InfoBox.ContentImage.Children = Application.Current.Resources["OsdLeftArrowImage"] as DrawingCollection;
                    break;
                case Actions.MoveRight:
                    InfoBox.ContentImage.Children = Application.Current.Resources["OsdRightArrowImage"] as DrawingCollection;
                    break;
                case Actions.TurnLeft:
                    InfoBox.ContentImage.Children = Application.Current.Resources["OsdLeftTurnImage"] as DrawingCollection;
                    break;
                case Actions.TurnRight:
                    InfoBox.ContentImage.Children = Application.Current.Resources["OsdRightTurnImage"] as DrawingCollection;
                    break;
                case Actions.Fist:
                    InfoBox.ContentImage.Children = Application.Current.Resources["OsdFistActionImage"] as DrawingCollection;
                    break;
                case Actions.Gun:
                    InfoBox.ContentImage.Children = Application.Current.Resources["OsdGunActionImage"] as DrawingCollection;
                    break;
                default:
                    InfoBox.ContentImage.Children = new DrawingCollection();
                    break;
            }
        }

        private static void OnStepIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            OsdActionInfoBox InfoBox = (OsdActionInfoBox) obj;
            InfoBox.StepIndex = (byte) args.NewValue;

            if (InfoBox.StepIndex < 0)
            {
                InfoBox.StepIndex = 0;
            }
            else if (InfoBox.StepIndex > Logic.Game.numberOfSteps - 1)
            {
                InfoBox.StepIndex = Logic.Game.numberOfSteps - 1;
            }
        }
    }
}
