using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace damajatek_ZRSTHS
{
    class Checker
    {
        public string side;
        public int x, y;
        public bool isDame = false;

        public Checker(string side, int x, int y)
        {
            this.isDame = false;
            this.side = side;
            this.x = x;
            this.y = y;
        }
        public Checker(string side, bool isDame, int x, int y)
        {
            this.isDame = isDame;
            this.side = side;
            this.x = x;
            this.y = y;
        }
    }
    class Space
    {
        public Checker checker;
        public char symbol;
        public Space()
        {
            checker = null;
            symbol = '■'; // ■
        }

        public Space(Checker checker)
        {
            this.checker = checker;
            if (checker.side == "player" || checker.side == "computer")
            {
                symbol = '☻'; // o, ■
            }
        }
    }
    class Game
    {
        public string playerName;
        private ConsoleColor playerColor = ConsoleColor.Blue;
        private ConsoleColor computerColor = ConsoleColor.Red;
        private int playerScore, computerScore;
        public string[] log;
        private Space[,] spaces = new Space[8, 8];

        public Game(string playerName)
        {
            this.playerName = playerName;
            log = new string[6];

            CreateBoard(true);
            RefreshScreen();
        }
        public Game()
        {
            log = new string[6];
        }

        public string StartGame()
        {
            while (CountChecker("player") > 0 && CountChecker("computer") > 0)
            {

                if (MoveChecker(InputConverter(Console.ReadLine())))
                {
                    if (CountChecker("computer") != 0)
                    {
                        bool computerMove = ComputerCheckerController();
                        if (!computerMove)
                        {
                            AddToLog("A computer nem tud többet lépni.");
                            return "player";
                        }
                    }
                }
            }

            if (CountChecker("player") == 0)
            {
                return "computer";
            }
            else if (CountChecker("computer") == 0)
            {
                return "player";
            }
            return null;
        }
        public void LoadGame(string saveName)
        {
            StreamReader sr = new StreamReader(saveName + ".txt");
            playerName = sr.ReadLine();

            string[] lines = new string[8];
            CreateBoard(false);

            for (int x = 0; x < lines.Length; x++)
            {
                lines[x] = sr.ReadLine();

                string[] checkers = lines[x].Split(';').ToArray();

                for (int y = 0; y < checkers.Length - 1; y++)
                {
                    if (checkers[y] == "null")
                    {
                        spaces[x, y].checker = null;
                    }
                    else
                    {
                        string[] details = checkers[y].Split('-');
                        bool isDame = false;
                        if (details[1] == "True")
                        {
                            isDame = true;
                        }
                        spaces[x, y] = new Space(new Checker(details[0], isDame, int.Parse(details[2]), int.Parse(details[3])));
                    }
                }
            }
            sr.Close();
            AddToLog("Sikeres betöltés!");

            RefreshScreen();
        }
        private void SaveGame()
        {
            Console.WriteLine("Mentés neve: ");
            StreamWriter sw = new StreamWriter(Console.ReadLine() + ".txt");

            sw.WriteLine(playerName);
            for (int x = 0; x < spaces.GetLength(0); x++)
            {
                for (int y = 0; y < spaces.GetLength(1); y++)
                {
                    if (spaces[x, y].checker != null)
                    {
                        sw.Write(spaces[x, y].checker.side + "-" + spaces[x, y].checker.isDame + "-" + spaces[x, y].checker.x + "-" + spaces[x, y].checker.y + ";");
                    }
                    else { sw.Write("null;"); }
                }
                sw.WriteLine();
            }
            sw.Close();

            AddToLog("Sikeres mentés!");
            RefreshScreen();
        }
        public void CreateBoard(bool setBoard)
        {
            for (int x = 0; x < spaces.GetLength(0); x++)
            {
                for (int y = 0; y < spaces.GetLength(1); y++)
                {
                    spaces[x, y] = new Space();
                }
            }
            if (setBoard)
            {
                SetBoard();
            }
        }
        private void SetBoard()
        {
            //computer
            spaces[0, 0] = new Space(new Checker("computer", 0, 0));
            spaces[0, 2] = new Space(new Checker("computer", 0, 2));
            spaces[0, 4] = new Space(new Checker("computer", 0, 4));
            spaces[0, 6] = new Space(new Checker("computer", 0, 6));

            spaces[1, 1] = new Space(new Checker("computer", 1, 1));
            spaces[1, 3] = new Space(new Checker("computer", 1, 3));
            spaces[1, 5] = new Space(new Checker("computer", 1, 5));
            spaces[1, 7] = new Space(new Checker("computer", 1, 7));

            spaces[2, 0] = new Space(new Checker("computer", 2, 0));
            spaces[2, 2] = new Space(new Checker("computer", 2, 2));
            spaces[2, 4] = new Space(new Checker("computer", 2, 4));
            spaces[2, 6] = new Space(new Checker("computer", 2, 6));

            //player
            spaces[5, 1] = new Space(new Checker("player", 5, 1));
            spaces[5, 3] = new Space(new Checker("player", 5, 3));
            spaces[5, 5] = new Space(new Checker("player", 5, 5));
            spaces[5, 7] = new Space(new Checker("player", 5, 7));

            spaces[6, 0] = new Space(new Checker("player", 6, 0));
            spaces[6, 2] = new Space(new Checker("player", 6, 2));
            spaces[6, 4] = new Space(new Checker("player", 6, 4));
            spaces[6, 6] = new Space(new Checker("player", 6, 6));

            spaces[7, 1] = new Space(new Checker("player", 7, 1));
            spaces[7, 3] = new Space(new Checker("player", 7, 3));
            spaces[7, 5] = new Space(new Checker("player", 7, 5));
            spaces[7, 7] = new Space(new Checker("player", 7, 7));
        }
        private void DrawBoard()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   a b c d e f g h");
            for (int i = 0; i < spaces.GetLength(0); i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write((i + 1) + ".│");

                for (int j = 0; j < spaces.GetLength(1); j++)
                {
                    if (spaces[i, j].checker != null)
                    {
                        if (spaces[i, j].checker.side == "computer")
                        {
                            Console.ForegroundColor = computerColor;
                        }
                        else if (spaces[i, j].checker.side == "player")
                        {
                            Console.ForegroundColor = playerColor;
                        }
                        Console.Write(string.Format("{0} ", spaces[i, j].symbol));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(string.Format("{0} ", spaces[i, j].symbol));
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
        }
        private Checker FindChecker(int x, int y)
        {
            if (x < 8 && x >= 0 && y < 8 && y >= 0 && spaces[x, y].checker != null)
            {
                return spaces[x, y].checker;
            }
            return null;
        }
        private bool ComputerCheckerController()
        {
            bool isMoved = false;
            int tries = 0;

            //Find checker
            List<Checker> checkers = new List<Checker>();

            for (int x = 0; x < spaces.GetLength(0); x++)
            {
                for (int y = 0; y < spaces.GetLength(1); y++)
                {
                    if (spaces[x, y].checker != null && spaces[x, y].checker.side == "computer")
                    {
                        checkers.Add(spaces[x, y].checker);
                    }
                }
            }

            Random rnd = new Random();
            int randomIndex = rnd.Next(0, (checkers.Count - 1));

            while (!isMoved && tries <= checkers.Count)
            {
                Checker checker = checkers[randomIndex];
                int x1 = checker.x;
                int y1 = checker.y;

                Space checkSpaceDownLeft = null;
                Space checkSpaceDownRight = null;
                Space checkSpaceUpLeft = null;
                Space checkSpaceUpRight = null;

                Space checkSpaceDownLeftToJump = null;
                Space checkSpaceDownRightToJump = null;
                Space checkSpaceUpLeftToJump = null;
                Space checkSpaceUpRightToJump = null;

                if ((x1 + 1) < 8 && (y1 - 1) > -1) // checkSpaceDownLeft
                {
                    checkSpaceDownLeft = spaces[x1 + 1, y1 - 1];
                    if ((x1 + 2) < 8 && (y1 - 2) > -1)
                    {
                        checkSpaceDownLeftToJump = spaces[x1 + 2, y1 - 2];
                    }
                }
                if ((x1 + 1) < 8 && (y1 + 1) < 8) //checkSpaceDownRight
                {
                    checkSpaceDownRight = spaces[x1 + 1, y1 + 1];
                    if ((x1 + 2) < 8 && (y1 + 2) < 8)
                    {
                        checkSpaceDownRightToJump = spaces[x1 + 2, y1 + 2];
                    }
                }
                if (checker.isDame && (x1 - 1) > -1 && (y1 - 1) > -1) // checkSpaceUpLeft
                {
                    checkSpaceUpLeft = spaces[x1 - 1, y1 - 1];
                    if ((x1 - 2) > -1 && (y1 - 2) > -1)
                    {
                        checkSpaceUpLeftToJump = spaces[x1 - 2, y1 - 2];
                    }
                }
                if (checker.isDame && (x1 - 1) > -1 && (y1 + 1) < 8) // checkSpaceUpRight
                {
                    checkSpaceUpRight = spaces[x1 - 1, y1 + 1];
                    if ((x1 - 2) > -1 && (y1 + 2) < 8)
                    {
                        checkSpaceUpRightToJump = spaces[x1 - 2, y1 + 2];
                    }
                }

                //Check for player checker in neighbor
                if (checkSpaceDownLeft != null && checkSpaceDownLeft.checker != null && checkSpaceDownLeft.checker.side == "player" && checkSpaceDownLeftToJump != null && checkSpaceDownLeftToJump.checker == null)
                {
                    MoveComputerChecker(4, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceDownRight != null && checkSpaceDownRight.checker != null && checkSpaceDownRight.checker.side == "player" && checkSpaceDownRightToJump != null && checkSpaceDownRightToJump.checker == null)
                {
                    MoveComputerChecker(5, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceUpLeft != null && checkSpaceUpLeft.checker != null && checkSpaceUpLeft.checker.side == "player" && checkSpaceUpLeftToJump != null && checkSpaceUpLeftToJump.checker == null)
                {
                    MoveComputerChecker(6, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceUpRight != null && checkSpaceUpRight.checker != null && checkSpaceUpRight.checker.side == "player" && checkSpaceUpRightToJump != null && checkSpaceUpRightToJump.checker == null)
                {
                    MoveComputerChecker(7, x1, y1);
                    isMoved = true;
                }
                //Check for player checker in range
                else if (checkSpaceDownLeft != null && checkSpaceDownLeft.checker == null && checkSpaceDownLeftToJump != null && checkSpaceDownLeftToJump.checker != null && checkSpaceDownLeftToJump.checker.side == "player")
                {
                    MoveComputerChecker(0, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceDownRight != null && checkSpaceDownRight.checker == null && checkSpaceDownRightToJump != null && checkSpaceDownRightToJump.checker != null && checkSpaceDownRightToJump.checker.side == "player")
                {
                    MoveComputerChecker(1, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceUpLeft != null && checkSpaceUpLeft.checker == null && checkSpaceUpLeftToJump != null && checkSpaceUpLeftToJump.checker != null && checkSpaceUpLeftToJump.checker.side == "player")
                {
                    MoveComputerChecker(2, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceUpRight != null && checkSpaceUpRight.checker == null && checkSpaceUpRightToJump != null && checkSpaceUpRightToJump.checker != null && checkSpaceUpRightToJump.checker.side == "player")
                {
                    MoveComputerChecker(3, x1, y1);
                    isMoved = true;
                }

                //random moves
                else if (checkSpaceDownLeft != null && checkSpaceDownLeft.checker == null &&
                        checkSpaceDownRight != null && checkSpaceDownRight.checker == null &&
                        checkSpaceUpLeft != null && checkSpaceUpLeft.checker == null &&
                        checkSpaceUpRight != null && checkSpaceUpRight.checker == null)
                {
                    MoveComputerChecker(rnd.Next(0, 4), x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceDownLeft != null && checkSpaceDownLeft.checker == null &&
                        checkSpaceDownRight != null && checkSpaceDownRight.checker == null)
                {
                    MoveComputerChecker(rnd.Next(0, 2), x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceUpLeft != null && checkSpaceUpLeft.checker == null &&
                        checkSpaceUpRight != null && checkSpaceUpRight.checker == null)
                {
                    MoveComputerChecker(rnd.Next(2, 4), x1, y1);
                    isMoved = true;
                }

                //direct moves
                else if (checkSpaceDownLeft != null && checkSpaceDownLeft.checker == null)
                {
                    MoveComputerChecker(0, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceDownRight != null && checkSpaceDownRight.checker == null)
                {
                    MoveComputerChecker(1, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceUpLeft != null && checkSpaceUpLeft.checker == null)
                {
                    MoveComputerChecker(2, x1, y1);
                    isMoved = true;
                }
                else if (checkSpaceUpRight != null && checkSpaceUpRight.checker == null)
                {
                    MoveComputerChecker(3, x1, y1);
                    isMoved = true;
                }

                //If unable to move
                else
                {
                    tries++;
                    checkers.RemoveAt(randomIndex);
                    isMoved = false;
                }
            }
            return isMoved;
        }
        private void MoveComputerChecker(int movement, int x1, int y1)
        {
            int new_x = -1;
            int new_y = -1;

            switch (movement)
            {
                case 0:
                    new_x = x1 + 1;
                    new_y = y1 - 1;
                    break;
                case 1:
                    new_x = x1 + 1;
                    new_y = y1 + 1;
                    break;
                case 2:
                    new_x = x1 - 1;
                    new_y = y1 - 1;
                    break;
                case 3:
                    new_x = x1 - 1;
                    new_y = y1 + 1;
                    break;
                case 4:
                    new_x = x1 + 2;
                    new_y = y1 - 2;
                    spaces[x1 + 1, y1 - 1] = new Space();
                    AddToLog("Computer levett egy bábut!");
                    break;
                case 5:
                    new_x = x1 + 2;
                    new_y = y1 + 2;
                    spaces[x1 + 1, y1 + 1] = new Space();
                    AddToLog("Computer levett egy bábut!");
                    break;
                case 6:
                    new_x = x1 - 2;
                    new_y = y1 - 2;
                    spaces[x1 - 1, y1 - 1] = new Space();
                    AddToLog("Computer levett egy bábut!");
                    break;
                case 7:
                    new_x = x1 - 2;
                    new_y = y1 + 2;
                    spaces[x1 - 1, y1 + 1] = new Space();
                    AddToLog("Computer levett egy bábut!");
                    break;
            }

            spaces[new_x, new_y] = spaces[x1, y1];
            spaces[new_x, new_y].checker.x = new_x;
            spaces[new_x, new_y].checker.y = new_y;
            spaces[x1, y1] = new Space();

            if (new_x == 7 && !spaces[new_x, new_y].checker.isDame)
            {
                spaces[new_x, new_y].checker.isDame = true;
                AddToLog("A computer bábuja dáma lett!");
            }

            AddToLog("Computer lépett a(z) " + (new_x + 1) + CoordinateConverter(new_y) + " pozícióra.");

            System.Threading.Thread.Sleep(150);
            RefreshScreen();
        }
        private string CoordinateConverter(int x)
        {
            switch (x)
            {
                case 0:
                    return "a";
                case 1:
                    return "b";
                case 2:
                    return "c";
                case 3:
                    return "d";
                case 4:
                    return "e";
                case 5:
                    return "f";
                case 6:
                    return "g";
                case 7:
                    return "h";
            }
            return "error";
        }
        private int[] InputConverter(string input)
        {
            char[] chars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            if (input == "mentés")
            {
                SaveGame();
            }
            else if (input == "betöltés")
            {

                Console.WriteLine("Mentés neve: ");
                string saveName = Console.ReadLine();
                LoadGame(saveName);
            }
            else if (input.Length == 5 && Char.IsNumber(input[0]) && chars.Contains(input[1]) && input[2] == ' ' && Char.IsNumber(input[3]) && chars.Contains(input[4]))
            {
                string[] coordinates = input.Split(' ');

                char[] currentCoordinates = coordinates[0].ToCharArray();
                char[] moveCoordinates = coordinates[1].ToCharArray();

                int x1 = int.Parse(currentCoordinates[0].ToString()) - 1;
                int y1 = -1;
                char charx2 = currentCoordinates[1];


                int x2 = int.Parse(moveCoordinates[0].ToString()) - 1;
                int y2 = -1;
                char char_y2 = moveCoordinates[1];


                switch (charx2)
                {
                    case 'a':
                        y1 = 0;
                        break;
                    case 'b':
                        y1 = 1;
                        break;
                    case 'c':
                        y1 = 2;
                        break;
                    case 'd':
                        y1 = 3;
                        break;
                    case 'e':
                        y1 = 4;
                        break;
                    case 'f':
                        y1 = 5;
                        break;
                    case 'g':
                        y1 = 6;
                        break;
                    case 'h':
                        y1 = 7;
                        break;
                }

                switch (char_y2)
                {
                    case 'a':
                        y2 = 0;
                        break;
                    case 'b':
                        y2 = 1;
                        break;
                    case 'c':
                        y2 = 2;
                        break;
                    case 'd':
                        y2 = 3;
                        break;
                    case 'e':
                        y2 = 4;
                        break;
                    case 'f':
                        y2 = 5;
                        break;
                    case 'g':
                        y2 = 6;
                        break;
                    case 'h':
                        y2 = 7;
                        break;
                }

                return new int[] { x1, y1, x2, y2 };
            }
            else { AddToLog("Nincs ilyen utasítás!"); RefreshScreen(); }
            return null;
        }
        private bool MoveChecker(int[] coordinates)
        {
            bool isMoved = false;

            if (coordinates != null)
            {
                int x1 = coordinates[0];
                int y1 = coordinates[1];
                int x2 = coordinates[2];
                int y2 = coordinates[3];

                Checker target = FindChecker(x2, y2);
                Checker checker = FindChecker(x1, y1);

                if (checker != null && target == null)
                {
                    if (checker.side == "player")
                    {
                        if (checker.isDame)
                        {
                            if (y2 == y1 + 1 || y2 == y1 - 1)
                            {
                                spaces[x2, y2] = spaces[x1, y1];
                                spaces[x1, y1] = new Space();

                                AddToLog(playerName + " lépett a(z) " + (x2 + 1) + CoordinateConverter(y2) + " pozícióra.");
                                isMoved = true;
                            }
                        }
                        else
                        {
                            if (x2 < x1 && (y2 == y1 + 1 || y2 == y1 - 1))
                            {
                                spaces[x2, y2] = spaces[x1, y1];
                                spaces[x1, y1] = new Space();

                                AddToLog(playerName + " lépett a(z) " + (x2 + 1) + CoordinateConverter(y2) + " pozícióra.");

                                if (x2 == 0 && !spaces[x2, y2].checker.isDame)
                                {
                                    spaces[x2, y2].checker.isDame = true;
                                    AddToLog("A bábud dáma lett!");
                                }
                                isMoved = true;
                            }
                            else
                            {
                                AddToLog("Nem lehettséges lépés!");
                                isMoved = false;
                            }
                        }
                    }
                    else
                    {
                        isMoved = false;
                        AddToLog("Nem található a bábu!");
                    }
                }
                else if (checker != null && target != null && target.side == "computer")
                {
                    if (checker.side == "player")
                    {
                        if (checker.isDame)
                        {
                            if (x2 < x1)
                            {
                                if (y2 == y1 + 1 && FindChecker(x1 - 2, y1 + 2) == null)
                                {
                                    spaces[x2 - 1, y2 + 1] = spaces[x1, y1];
                                    spaces[x2, y2] = new Space();
                                    spaces[x1, y1] = new Space();
                                    isMoved = true;

                                    AddToLog(playerName + " lépett a(z) " + (x2 - 2) + CoordinateConverter(y2 + 1) + " pozícióra.");

                                }
                                else if (y2 == y1 - 1 && FindChecker(x1 - 2, y1 - 2) == null)
                                {
                                    spaces[x2 - 1, y2 - 1] = spaces[x1, y1];
                                    spaces[x2, y2] = new Space();
                                    spaces[x1, y1] = new Space();
                                    isMoved = true;

                                    AddToLog(playerName + " lépett a(z) " + (x2 - 2) + CoordinateConverter(y2 - 1) + " pozícióra.");
                                }
                                else
                                {
                                    AddToLog("Nem lehettséges a lépés!");
                                }
                            }
                            else if (x2 > x1)
                            {
                                if (y2 == y1 + 1 && FindChecker(x1 + 2, y1 + 2) == null)
                                {
                                    spaces[x2 + 1, y2 + 1] = spaces[x1, y1];
                                    spaces[x2, y2] = new Space();
                                    spaces[x1, y1] = new Space();
                                    isMoved = true;

                                    AddToLog(playerName + " lépett a(z) " + (x2 + 2) + CoordinateConverter(y2 + 1) + " pozícióra.");
                                }
                                else if (y2 == y1 - 1 && FindChecker(x1 + 2, y1 - 2) == null)
                                {
                                    spaces[x2 + 1, y2 - 1] = spaces[x1, y1];
                                    spaces[x2, y2] = new Space();
                                    spaces[x1, y1] = new Space();
                                    isMoved = true;

                                    AddToLog(playerName + " lépett a(z) " + (x2 + 2) + CoordinateConverter(y2 - 1) + " pozícióra.");
                                }
                            }
                        }
                        else
                        {
                            if (x2 < x1)
                            {
                                if (y2 == y1 + 1 && FindChecker(x1 - 2, y1 + 2) == null)
                                {
                                    spaces[x2 - 1, y2 + 1] = spaces[x1, y1];
                                    spaces[x2, y2] = new Space();
                                    spaces[x1, y1] = new Space();

                                    if (x2 - 1 == 0 && !spaces[x2 - 1, y2 + 1].checker.isDame)
                                    {
                                        spaces[x2 - 1, y2 + 1].checker.isDame = true;
                                        AddToLog("A bábud dáma lett!");
                                    }

                                    isMoved = true;
                                    AddToLog(playerName + " lépett a(z) " + (x2) + CoordinateConverter(y2 + 1) + " pozícióra.");
                                }
                                else if (y2 == y1 - 1 && FindChecker(x1 - 2, y1 - 2) == null)
                                {
                                    spaces[x2 - 1, y2 - 1] = spaces[x1, y1];
                                    spaces[x2, y2] = new Space();
                                    spaces[x1, y1] = new Space();

                                    if (x2 - 1 == 0 && !spaces[x2 - 1, y2 - 1].checker.isDame)
                                    {
                                        spaces[x2 - 1, y2 - 1].checker.isDame = true;
                                        AddToLog("A bábud dáma lett!");
                                    }

                                    isMoved = true;
                                    AddToLog(playerName + " lépett a(z) " + (x2) + CoordinateConverter(y2 - 1) + " pozícióra.");
                                }
                                else
                                {
                                    isMoved = false;
                                }
                            }
                            else
                            {
                                AddToLog("Nem lehettséges a lépés!");
                                isMoved = false;
                            }
                        }
                    }
                }
                else if (checker == null)
                {
                    AddToLog("Nem található ilyen bábu!");
                }

                RefreshScreen();
                return isMoved;
            }
            return false;
        }
        private int CountChecker(string side)
        {
            int counter = 0;
            for (int x = 0; x < spaces.GetLength(0); x++)
            {
                for (int y = 0; y < spaces.GetLength(1); y++)
                {
                    if (spaces[x, y].checker != null && spaces[x, y].checker.side == side)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }
        public void AddToLog(string text)
        {
            string[] shiftedLog = new string[6];
            Array.Copy(log, 1, shiftedLog, 0, log.Length - 1);

            log = shiftedLog;
            log[5] = text;
        }
        public void RefreshScreen()
        {
            Console.Clear();
            Console.WriteLine("---------------<DÁMA>---------------");

            playerScore = CountChecker("player");
            computerScore = CountChecker("computer");

            Console.ForegroundColor = playerColor;
            Console.WriteLine(@"{0}: {1}", playerName, playerScore);
            Console.ForegroundColor = computerColor;
            Console.WriteLine(@"Computer: {0}", computerScore);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("------------------------------------");

            Console.WriteLine();
            DrawBoard();
            Console.WriteLine();

            Console.WriteLine("---------------<LOG>----------------");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log[5]);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log[4]);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(log[3]);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(log[2]);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(log[1]);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(log[0]);


            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("------------<Utasítások>------------");
            Console.CursorVisible = true;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Menu();
            Console.ReadKey();
        }

        static void Menu()
        {
            int selectedOption = 0;
            ConsoleKey key;

            Console.CursorVisible = false;

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine(@"  _____     _                   ");
                Console.WriteLine(@" |  __ \   |_|                  ");
                Console.WriteLine(@" | |  | | __ _  _ __ ___   __ _ ");
                Console.WriteLine(@" | |  | |/ _` || '_ ` _ \ / _` |");
                Console.WriteLine(@" | |__| | (_| || | | | | | (_| |");
                Console.WriteLine(@" |_____/ \__,_||_| |_| |_|\__,_|");
                Console.WriteLine("\n");

                if (selectedOption == 0)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("-> " + "Új játék");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Új játék");
                }

                if (selectedOption == 1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("-> " + "Betöltés");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Betöltés");
                }

                if (selectedOption == 2)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("-> " + "Kilépés");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Kilépés");
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (selectedOption > 0)
                            {
                                selectedOption--;
                            }
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            if (selectedOption < 2)
                            {
                                selectedOption++;
                            }
                            break;
                        }
                }
            } while (key != ConsoleKey.Enter);

            if (selectedOption == 0)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("-------------<Új játék>-------------\n");

                Console.WriteLine("Játék közben használható utasítások: \n\t-Lépés: A bábu kordinátája és a hely ahová lépne. pl. (6b 5c)\n\t-Mentés: mentés\n\t-Betöltés: betöltés\n");
                Console.WriteLine("Hogy hívnak?: ");

                Game game = new Game(Console.ReadLine());

                if (game.StartGame() == "computer")
                {
                    game.AddToLog("Vesztettél!");
                    game.RefreshScreen();
                }
                else
                {
                    game.AddToLog("Nyertél!");
                    game.RefreshScreen();
                }

            }
            else if (selectedOption == 1)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("-------------<Betöltés>-------------\n");
                Console.WriteLine("Mentés neve:");

                Game game = new Game();
                game.LoadGame(Console.ReadLine());
                game.StartGame();
            }
            else if (selectedOption == 2) { Environment.Exit(0); }
        }
    }
}
