using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using RobotPigWars.Logic;

namespace RobotPigWars.GUI
{
    class Game : UserControl
    {
        // Object for accessing MainWindow
        private static MainWindow app = Application.Current.Windows[0] as MainWindow;
        // Object for accessing game field
        private static readonly Canvas field = app.Field;
        
        // Game logic object
        public Logic.Game GameLogic { get; private set; }

        // GUI Player objects
        private Player[] players;
        // Size of the drawn fields (x and y coordinates)
        private uint[] fieldElementSize;

        // Timer for processing steps
        private DispatcherTimer processTimer = new DispatcherTimer();
        // Step counter for processing steps
        private byte currentStepPart;

        public Game(byte fieldSize = 0, Logic.Game game = null)
        {
            // Set field size
            if (fieldSize == 0 || !Logic.Game.fieldSizes.Contains(fieldSize))
            {
                fieldSize = Logic.Game.fieldSizes[1];
            }

            // Set game logic
            if (game == null)
            { 
                GameLogic = new Logic.Game(fieldSize);
            }
            else
            {
                GameLogic = game;
            }

            // Calculate field size to draw
            fieldElementSize = new uint[2] { (uint) field.ActualWidth / fieldSize, (uint) field.ActualHeight / fieldSize };
            DisplayFieldBackground();

            // Create GUI players
            players = new Player[2] {
                new Player() { ColorIndex = 0, Direction = GameLogic.Players[0].Face },
                new Player() { ColorIndex = 1, Direction = GameLogic.Players[1].Face }
            };

            // Update and display players
            UpdatePlayers();
            DisplayPlayers();
        }

        // Add field elements to canvas
        private void DisplayFieldBackground()
        {
            field.Children.Clear();

            for (byte x = 0; x < GameLogic.FieldSize; x++)
            {
                for (byte y = 0; y < GameLogic.FieldSize; y++)
                {
                    Rectangle Back = new Rectangle();
                    Back.Width = fieldElementSize[0];
                    Back.Height = fieldElementSize[1];
                    Canvas.SetLeft(Back, x * Back.Width);
                    Canvas.SetTop(Back, y * Back.Height);
                    Back.Style = (x + y) % 2 == 0 ? (Style) Application.Current.FindResource("FieldBackGray") : (Style) Application.Current.FindResource("FieldBackPink");
                    field.Children.Add(Back);
                }
            }
        }

        // Add players to canvas
        private void DisplayPlayers()
        {
            for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
            {
                field.Children.Add(players[p]);
            }
        }

        // Initilaize processing of the steps
        public void ProcessStepInitialize()
        {
            currentStepPart = 0;

            processTimer.Interval = TimeSpan.FromSeconds(1);
            processTimer.Tick += OnProcessStep;
            processTimer.Start();
        }

        // Processing steps
        private void OnProcessStep(Object source, EventArgs e)
        {
            // Check current step
            if (currentStepPart < Logic.Game.numberOfSteps * 2)
            {
                currentStepPart++;
            }
            // End processing
            else
            {
                processTimer.Stop();
                processTimer.Tick -= OnProcessStep;
                app.ProcessEnd();
                return;
            }

            // Update step marker
            if (currentStepPart % 2 == 1)
            {
                GameLogic.ProcessStep();
                UpdatePlayers();
                app.ActionBox.NextStep();
                app.ActionBox.UpdateStepMarker();
            }
        }

        // Update players' position, facing direction and lives
        private void UpdatePlayers()
        {
            for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
            {
                Canvas.SetLeft(players[p], GameLogic.Players[p].Position[0] * fieldElementSize[0]);
                Canvas.SetTop(players[p], GameLogic.Players[p].Position[1] * fieldElementSize[1] - fieldElementSize[1] / 4);

                players[p].Direction = GameLogic.Players[p].Face;
                app.LifeBox.SetLife(p, (byte) GameLogic.Players[p].Life);
            }
        }
    }
}
