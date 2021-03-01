/**
 * Game Logic - Game
 * 
 * Copyright © 2019 Tibor Buzási, Mátyás Spitzner, Martin Szarvas. All rights reserved.
 * For licensing information see LICENSE in the project root folder.
 */

using System;
using System.IO;
using System.Linq;

namespace RobotPigWars.Logic
{
    class Game
    {
        // States of the game
        public enum GameState { Input, Process, GameOver };
        // Sizes of the game field
        public static readonly byte[] fieldSizes = { 4, 6, 8 };

        // Number of lives
        public const int numberOfLives = 3;
        // Number of players
        public const byte numberOfPlayers = 2;
        // Number of steps
        public const byte numberOfSteps = 5;
        // Movement steps 
        public readonly Actions[] movementActions = {
            Actions.Forward,
            Actions.Backward,
            Actions.MoveLeft,
            Actions.MoveRight
        };

        // State of the game
        public GameState State { get; private set; }
        // Size of the game field
        public byte FieldSize { get; private set; }
        // Current step
        public byte CurrentStep { get; private set; }
        // Current player
        public byte CurrentPlayer { get; private set; }
        // Players
        public Player[] Players { get; private set; }

        // Constructor for creating new game
        public Game(byte fieldSize)
        {
            // Set field size and game state
            FieldSize = fieldSize;
            State = GameState.Input;

            // Create players
            Players = new Player[2];
            Players[0] = new Player(
                numberOfLives,
                FieldSize / 2 - 1,
                FieldSize / 2 - 1,
                Directions.Down,
                Enumerable.Repeat(Actions.None, numberOfSteps).ToArray()
            );
            Players[1] = new Player(
                numberOfLives,
                FieldSize / 2,
                FieldSize / 2,
                Directions.Up,
                Enumerable.Repeat(Actions.None, numberOfSteps).ToArray()
            );

            // Set current step and player
            CurrentStep = 0;
            CurrentPlayer = 0;
        }

        // Constructor for loading a game
        public Game(byte fieldSize, Player[] players)
        {
            // Set field size and game state
            FieldSize = fieldSize;
            State = GameState.Input;

            // Create players
            Players = players;

            // Set current step and player
            CurrentStep = 0;
            CurrentPlayer = 0;
        }

        // Set the step list of the current player
        public void SetActions(Actions[] action)
        {
            for (byte s = 0; s < numberOfSteps; s++)
            {
                Players[CurrentPlayer].Steps[s] = action[s];
            }
        }

        // Clear step list of the players
        private void ClearActions()
        {
            for (byte p = 0; p < numberOfPlayers; p++)
            {
                Players[p].Steps = Enumerable.Repeat(Actions.None, numberOfSteps).ToArray();
            }
        }

        // Processing the steps
        public void ProcessStep()
        {
            // Check if the current step is valid
            if (CurrentStep >= numberOfSteps)
            {
                return;
            }

            // Process movement steps if no collision is forecasted
            if (!ForecastCollision())
            {
                for (byte p = 0; p < numberOfPlayers; p++)
                {
                    Players[p].Position = CalculatePosition(Players[p]);
                }
            }

            // Process turning steps
            for (byte p = 0; p < numberOfPlayers; p++)
            {
                switch (Players[p].Steps[CurrentStep])
                {
                    case Actions.TurnLeft:
                        if (Players[p].Face == Directions.Up)
                        {
                            Players[p].Face = Directions.Left;
                        }
                        else
                        {
                            Players[p].Face = Players[p].Face - 1;
                        }
                        break;
                    case Actions.TurnRight:
                        if (Players[p].Face == Directions.Left)
                        {
                            Players[p].Face = Directions.Up;
                        }
                        else
                        {
                            Players[p].Face = Players[p].Face + 1;
                        }
                        break;
                }
            }

            // Process action steps
            for (byte p = 0; p < numberOfPlayers; p++)
            {
                switch (Players[p].Steps[CurrentStep])
                {
                    case Actions.Fist:
                        ProcessAction(Players[p], CalculateFist(Players[p]));
                        break;
                    case Actions.Gun:
                        ProcessAction(Players[p], CalculateGun(Players[p]));
                        break;
                }
            }

            // Step on to the next step
            CurrentStep++;
        }

        // Forecast player collision
        public bool ForecastCollision()
        {
            // Check for movement steps and calculate new player positions
            if ((movementActions.Contains(Players[0].Steps[CurrentStep]) ||
                movementActions.Contains(Players[1].Steps[CurrentStep])) &&
                Enumerable.SequenceEqual(CalculatePosition(Players[0]), CalculatePosition(Players[1])))
            {
                return true;
            }

            // No collision detected
            return false;
        }

        // Calculate new position of the given player
        private int[] CalculatePosition(Player player)
        {
            int[] movement = new int[2] { 0, 0 };
            // Set matrix for movement (forward/backward = 1/-1, right/left = 1/-1)
            switch (player.Steps[CurrentStep])
            {
                case Actions.Forward:
                    movement = new int[2] { 1, 0 };
                    break;
                case Actions.Backward:
                    movement = new int[2] { -1, 0 };
                    break;
                case Actions.MoveLeft:
                    movement = new int[2] { 0, -1 };
                    break;
                case Actions.MoveRight:
                    movement = new int[2] { 0, 1 };
                    break;
            }

            // Transform matrix for facing direction
            switch (player.Face)
            {
                case Directions.Left:
                    movement = movement.Select(i => -i).ToArray();
                    break;
                case Directions.Down:
                    movement = movement.Reverse().ToArray();
                    break;
                case Directions.Up:
                    movement = movement.Select(i => -i).Reverse().ToArray();
                    break;
            }

            // Fix matrix tranformation
            if ((player.Face == Directions.Up || player.Face == Directions.Down) &&
                (player.Steps[CurrentStep] == Actions.MoveLeft || player.Steps[CurrentStep] == Actions.MoveRight))
            {
                movement = movement.Select(i => -i).ToArray();
            }

            // Fix position
            return FixPosition(new int[2] { player.Position[0] + movement[0], player.Position[1] + movement[1] });
        }

        // Fix position (if it is over the bounds)
        private int[] FixPosition(int[] position)
        {
            position[0] = (position[0] < 0) ? 0 : position[0];
            position[0] = (position[0] >= FieldSize - 1) ? FieldSize - 1 : position[0];
            position[1] = (position[1] < 0) ? 0 : position[1];
            position[1] = (position[1] >= FieldSize - 1) ? FieldSize - 1 : position[1];

            return position;
        }

        // Check turn end state
        public void EndTurn()
        {
            switch (State)
            {
                // Step on to the next player or to the processing state
                case GameState.Input:
                    CurrentStep = 0;
                    CurrentPlayer++;
                    if (CurrentPlayer == numberOfPlayers)
                    {
                        State = GameState.Process;
                    }
                    break;
                // Check if game is over or step on to the input state
                case GameState.Process:
                    if (Players.Any(p => p.Life == 0))
                    {
                        State = GameState.GameOver;
                    }
                    else
                    {
                        State = GameState.Input;
                    }
                    // Reset current player, current step and stored steps
                    CurrentPlayer = 0;
                    CurrentStep = 0;
                    ClearActions();
                    break;
            }
        }

        // Calculate effected are of the fist action
        public int[][] CalculateFist(Player player)
        {
            // Effected area (upper left corner/bottom right corner)(position x/y coordinates)
            int[][] area = new int[2][];

            area[0] = FixPosition(new int[2] { player.Position[0] - 1, player.Position[1] - 1 });
            area[1] = FixPosition(new int[2] { player.Position[0] + 1, player.Position[1] + 1 });

            return area;
        }

        // Calculate effected are of the gun action
        public int[][] CalculateGun(Player player)
        {
            // Effected area (upper left corner/bottom right corner)(position x/y coordinates)
            int[][] area = new int[2][];

            switch (player.Face)
            {
                case Directions.Right:
                    area[0] = FixPosition(new int[2] { player.Position[0] + 1, player.Position[1] });
                    area[1] = FixPosition(new int[2] { FieldSize - 1, player.Position[1] });
                    break;
                case Directions.Left:
                    area[0] = FixPosition(new int[2] { 0, player.Position[1] });
                    area[1] = FixPosition(new int[2] { player.Position[0] - 1, player.Position[1] });
                    break;
                case Directions.Down:
                    area[0] = FixPosition(new int[2] { player.Position[0], player.Position[1] + 1 });
                    area[1] = FixPosition(new int[2] { player.Position[0], FieldSize - 1 });
                    break;
                case Directions.Up:
                    area[0] = FixPosition(new int[2] { player.Position[0], 0 });
                    area[1] = FixPosition(new int[2] { player.Position[0], player.Position[1] - 1 });
                    break;
            }

            return area;
        }

        // Process action
        private void ProcessAction(Player player, int[][] area)
        {
            // Check for players in the effected area
            Player[] effectedPlayers = Players.Where(p => p != player)
                .Where(p => p.Position[0] >= area[0][0] && p.Position[1] >= area[0][1])
                .Where(p => p.Position[0] <= area[1][0] && p.Position[1] <= area[1][1]).ToArray();
            foreach (Player effectedPlayer in effectedPlayers)
            {
                // Decrement players lives
                if (effectedPlayer.Life > 0)
                {
                    effectedPlayer.Life--;
                }
            }
        }

        // Save game data into given file
        public bool SaveGame(string filename)
        {
            FileInfo gameFileInfo;
            BinaryWriter gameFile = null;

            // Try to save game
            try
            {
                // Open file
                gameFileInfo = new FileInfo(filename);
                gameFile = new BinaryWriter(
                    gameFileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));

                // Write game data into file
                gameFile.Write(FieldSize);
                for (byte p = 0; p < Logic.Game.numberOfPlayers; p++)
                {
                    for (byte i = 0; i < 2; i++)
                    {
                        gameFile.Write((Int32) Players[p].Position[i]);
                    }
                    gameFile.Write((Int32) Players[p].Face);
                    gameFile.Write((Int32) Players[p].Life);
                }

                // Close file
                gameFile.Close();

                if (gameFile != null)
                    gameFile.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Load game data from given file
        public Game LoadGame(string filename)
        {
            FileInfo gameFileInfo;
            BinaryReader gameFile = null;

            // Try to load game
            try
            {
                // Open file
                gameFileInfo = new FileInfo(filename);
                gameFile = new BinaryReader(gameFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read));

                byte fieldSize = gameFile.ReadByte();
                int[,] positions = new int[2, 2];
                int[,] facesAndLives = new int[2, 2];
                Player[] players = new Player[2];

                // Read game data from file
                for (byte p = 0; p < numberOfPlayers; p++)
                {
                    for (byte i = 0; i < 2; i++)
                    {
                        positions[p, i] = gameFile.ReadInt32();
                    }
                    for (byte i = 0; i < 2; i++)
                    {
                        facesAndLives[p, i] = gameFile.ReadInt32();
                    }
                    players[p] = new Player(facesAndLives[p, 1], positions[p, 0], positions[p, 1], (Directions)facesAndLives[p, 0], Enumerable.Repeat(Actions.None, numberOfSteps).ToArray());
                }

                // Close file
                gameFile.Close();

                if (gameFile != null)
                    gameFile.Dispose();

                // Create game logic from read data
                Game game = new Game(fieldSize, players);
                return game;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
