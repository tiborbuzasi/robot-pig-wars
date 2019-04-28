using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using RobotPigWars.Logic;

namespace RobotPigWars.GUI
{
    /// <summary>
    /// Interaction logic for OsdLifeBox.xaml
    /// </summary>
    public partial class OsdLifeBox : UserControl
    {
        const ushort positionBaseX = 105;
        const ushort positionBaseY = 13;
        const ushort horizontalDistance = 70;

        private OsdLifeInfoBox[] lifeInfo = new OsdLifeInfoBox[2];

        public OsdLifeBox()
        {
            InitializeComponent();

            // Initializing life info boxes
            InitializeLifeInfoBoxes();
        }

        private void InitializeLifeInfoBoxes()
        {
            for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
            {
                lifeInfo[p] = new OsdLifeInfoBox() { ColorIndex = p, Life = Logic.Game.numberOfLives };
            }
            DisplayLifeBox(this);
        }

        private static void DisplayLifeBox(OsdLifeBox LifeBox)
        {
            ClearLifeBox(LifeBox);
            for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
            {
                AddLifeBoxItem(LifeBox, p);
            }
        }

        private static void AddLifeBoxItem(OsdLifeBox LifeBox, byte player)
        {
            Canvas.SetLeft(LifeBox.lifeInfo[player], positionBaseX + player * horizontalDistance);
            Canvas.SetTop(LifeBox.lifeInfo[player], positionBaseY);
            LifeBox.Canvas.Children.Add(LifeBox.lifeInfo[player]);
        }

        private static void ClearLifeBox(OsdLifeBox LifeBox)
        {
            LifeBox.Canvas.Children.Clear();
        }

        public void SetLife(byte player, byte life)
        {
            lifeInfo[player].Life = life;
        }
    }
}
