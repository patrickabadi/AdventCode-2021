using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    public class adv023
    {
        public void Run()
        {
            var board = Parse(input3);

            var result = SolveBoard(board);

            Debug.WriteLine($"Result - {result}");
        }

        int minEnergy = int.MaxValue;

        int SolveBoard(Board board)
        {
            board.DebugBoard();

            HashSet<string> visited = new HashSet<string>();

            var moves = board.PossibleMoves();
            foreach (var m in moves)
            {
                Solve(new Board(board), m.Item1, m.Item2, m.Item3, 0, visited);
            }

            return minEnergy;
        }

        void Solve(Board board, int from, int to, int energy, int totalEnergy, HashSet<string> visited )
        {
            if (totalEnergy + energy > minEnergy)
                return;// don't waste your energy

            

            //Debug.WriteLine("******");
            //board.DebugBoard();
            //board.DebugMove(from, to, energy, totalEnergy);
            
             board.Move(from, to);

            

            
            //board.DebugBoard();
            //Debug.WriteLine("******");

            totalEnergy += energy;

            if(board.IsComplete())
            {
                Debug.WriteLine($"Solved with energy {totalEnergy}");

                //board.Move(to, from);
                //board.DebugBoard();
                minEnergy = Math.Min(totalEnergy, minEnergy);

                return;
            }

            var hash = board.Hash(totalEnergy);
            if (visited.Contains(hash))
            {
                //Debug.WriteLine("Already visited");
                return;
            }
            visited.Add(hash);

            var moves = board.PossibleMoves();
            foreach(var m in moves)
            {
                Solve(new Board(board), m.Item1, m.Item2, m.Item3, totalEnergy, visited);
            }
        }

        private Board Parse(string input)
        {
            var lines = input.Split('\n');

            var board = new Board(lines.Length-2);

            for(int i=0; i<lines.Length-3; i++)
            {
                // room a
                board.board[i+1, 2] = lines[i+2][3];

                // room b
                board.board[i+1, 4] = lines[i+2][5];

                // room c
                board.board[i+1, 6] = lines[i+2][7];

                // room d
                board.board[i+1, 8] = lines[i+2][9];
            }

            

            return board;
        }

        /*
         *  
         *  Board 11x3
            ...........
            ##A#B#C#D##
             #A#B#C#D#

            Hallway = 0,0.... 10,0
            RoomA = 2,1 - 2,2
            RoomB = 4,1 - 4,2
            RoomC = 6,1 - 6,2
            RoomD = 8,1 - 8,2

         */
        public class Board
        {
            public char[,] board;
            static int[] hallway;
            static int[] rooms;
             public int height;

            static Board()
            {
                hallway = new int[] { 0, 1, 3, 5, 7, 9, 10 };
                rooms = new int[] { 2, 4, 6, 8 };
            }

            public Board(Board copy)
            {
                board = copy.board.Clone() as char[,];
                height = copy.height;
            }
            public Board(int height)
            {
                this.height = height;
                board = new char[height, 11];
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < 11; col++)
                    {
                        if (row == 0)
                        {
                            board[row, col] = '.';
                        }
                        else if (col != 2 && col != 4 && col != 6 && col != 8)
                        {
                            board[row, col] = '#';
                        }
                        else
                        {
                            board[row, col] = '.';
                        }
                    }
                }
            }

            public string Hash(int energy)
            {
                StringBuilder str = new StringBuilder();
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < 11; col++)
                    {
                        str.Append(board[row, col]);
                    }
                }
                str.Append(energy.ToString());
                return str.ToString();
            }

            public void DebugBoard()
            {
                for(int row=0; row<height; row++)
                {
                    for(int col=0; col<11; col++)
                    {
                        Debug.Write($"{board[row, col]}");
                    }
                    Debug.Write("\n");
                }

                //Debug.Write("\n");
            }

            public void DebugMove(int from, int to, int energy, int totalEnergy)
            {
                var f = RowCol(from);
                var t = RowCol(to);

                var letter = board[f.Item1, f.Item2];
                Debug.WriteLine($"Move {letter}, ({f.Item1},{f.Item2})-({t.Item1},{t.Item2}), energy {energy}, total {totalEnergy}");
            }

            public void Move(int from, int to)
            {
                var f = RowCol(from);
                var t = RowCol(to);

                if (IsEmpty(f.Item1, f.Item2) || !IsEmpty(t.Item1, t.Item2))
                    throw new Exception("Red alert, you did something wrong");

                var letter = board[f.Item1, f.Item2];
                board[f.Item1, f.Item2] = '.';
                board[t.Item1, t.Item2] = letter;
            }

            public bool IsComplete()
            {
                for(int row=1; row<height; row++)
                {
                    if (board[row, 2] != 'A' || board[row, 4] != 'B' || board[row, 6] != 'C' || board[row, 8] != 'D')
                        return false;
                }
                return true;
            }

            bool IsEmpty(int row, int col)
            {
                return board[row, col] == '.';
            }

            int DesiredRoomCol(char c)
            {
                return rooms[c - 'A'];
            }

            int EnergyFactor(char c)
            {
                switch(c)
                {
                    case 'A': return 1;
                    case 'B': return 10;
                    case 'C': return 100;
                    case 'D': return 1000;
                    default: return 0;
                }
            }

            int TraverseHallway(int startCol, int desiredCol)
            {
                int steps = 0;
                bool blocked = false;
                if (desiredCol > startCol)
                {
                    for (int j = startCol + 1; j <= desiredCol; j++)
                    {
                        steps++;
                        if (!IsEmpty(0, j))
                        {
                            blocked = true;
                            break;
                        }
                    }
                }
                else
                {
                    for (int j = startCol - 1; j >= desiredCol; j--)
                    {
                        steps++;
                        if (!IsEmpty(0, j))
                        {
                            blocked = true;
                            break;
                        }
                    }
                }

                if (blocked)
                    return -1;

                return steps;
            }

            int IndexOf(int row, int col)
            {
                return row * 11 + col;
            }

            Tuple<int,int> RowCol(int index)
            {
                return new Tuple<int, int>(index / 11, index % 11);
            }

            public List<Tuple<int, int, int>> PossibleMoves()
            {
                var moves = new List<Tuple<int, int, int>>();

                

                // check hallway letters moving into their respective room
                for (int i = 0; i < hallway.Length; i++)
                {
                    var halCol = hallway[i];
                    if (IsEmpty(0, halCol))
                        continue;

                    var letter = board[0, halCol];

                    var desiredCol = DesiredRoomCol(letter);
                    if (!IsEmpty(1, desiredCol))
                        continue;

                    int lowestRow = -1;
                    // can't put the desired letter if a letter below is not desired
                    for (int row = height - 1; row > 0; row--)
                    {
                        if (!IsEmpty(row, desiredCol))
                        {
                            if (letter != board[row, desiredCol])
                                break;
                            else
                                continue;
                        }
                        lowestRow = row;
                        break;
                    }

                    if (lowestRow == -1)
                        continue;

                    var steps = TraverseHallway(halCol, desiredCol);
                    if (steps == -1) // if it's blocked
                        continue;

                    steps += lowestRow;
                    moves.Add(new Tuple<int, int, int>(IndexOf(0, halCol), IndexOf(lowestRow, desiredCol), steps * EnergyFactor(letter)));
                }

                // rooms letter
                for (int row = height - 1; row > 0; row--)
                {
                    for (int i = 0; i < rooms.Length; i++)
                    {
                        var roomCol = rooms[i];
                        if (IsEmpty(row, roomCol) || !IsEmpty(row - 1, roomCol))
                            continue;

                        var letter = board[row, roomCol];

                        int desiredCol = DesiredRoomCol(letter);
                        // if the letter is in the desired room and the letter below is also desired
                        if (roomCol == desiredCol)
                        {
                            bool lettersBelowSame = true;
                            for (int k = row + 1; k < height; k++)
                            {
                                if (letter != board[k, desiredCol])
                                {
                                    lettersBelowSame = false;
                                    break;
                                }
                            }

                            if (lettersBelowSame)
                                continue;
                        }

                        var steps = row;
                        // go left
                        for (int j = roomCol - 1; j >= 0; j--)
                        {
                            steps++;
                            if (!IsEmpty(0, j))
                                break;
                            // can't land on a room hallway square
                            if (rooms.Contains(j))
                                continue;

                            moves.Add(new Tuple<int, int, int>(IndexOf(row, roomCol), IndexOf(0, j), steps * EnergyFactor(letter)));
                        }

                        steps = row;
                        // go right
                        for (int j = roomCol + 1; j < 11; j++)
                        {
                            steps++;
                            if (!IsEmpty(0, j))
                                break;

                            // can't land on a room hallway square
                            if (rooms.Contains(j))
                                continue;

                            moves.Add(new Tuple<int, int, int>(IndexOf(row, roomCol), IndexOf(0, j), steps * EnergyFactor(letter)));
                        }
                    }
                }

                return moves;

            }

        }

        string input = @"#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########";

        string input0 = @"#############
#...........#
###B#C#B#D###
  #D#C#B#A#
  #D#B#A#C#
  #A#D#C#A#
  #########";

        string input2 = @"#############
#...........#
###D#B#C#C###
  #D#A#B#A#
  #########";

        string input3 = @"#############
#...........#
###D#B#C#C###
  #D#C#B#A#
  #D#B#A#C#
  #D#A#B#A#
  #########";
    }
}
