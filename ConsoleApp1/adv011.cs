using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class adv011
    {
        public void Run()
        {
            var grid = ParseInput(input2);

            var ret = RunSteps(grid, true);

            Debug.WriteLine($"Return {ret}");

        }

        int RunSteps(int[,] grid, bool checkForAllFlash = false)
        {
            int size = grid.GetLength(0);

            DebugGrid(grid, 0);

            int totalFlashes = 0;

            int maxStep = checkForAllFlash ? 10000 : 100;

            for (int step = 0; step < maxStep; step++)
            {               

                IncreaseStep(grid);

                StartCascade(grid);

                var flashes = ResetCountFlashes(grid);

                if(checkForAllFlash && IsAllFlash(grid))
                {
                    return step + 1;
                }

                totalFlashes += flashes;
            }

            return checkForAllFlash ? -1: totalFlashes;
        }

        bool IsAllFlash(int[,] grid)
        {
            int size = grid.GetLength(0);

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (grid[row, col] != 0)
                        return false;
                }
            }

            return true;
        }

        void StartCascade(int[,] grid)
        {
            int size = grid.GetLength(0);

            var copied = CopyGrid(grid);
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (copied[row, col] <= 9)
                        continue;

                    Cascade(row + 1, col, grid);
                    Cascade(row + 1, col + 1, grid);
                    Cascade(row, col + 1, grid);
                    Cascade(row - 1, col, grid);
                    Cascade(row - 1, col + 1, grid);
                    Cascade(row - 1, col - 1, grid);
                    Cascade(row, col - 1, grid);
                    Cascade(row + 1, col - 1, grid);
                }
            }
        }

        int[,] CopyGrid(int[,] grid)
        {
            int size = grid.GetLength(0);

            var newGrid = new int[size, size];
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    newGrid[row, col] = grid[row, col];
                }
            }

            return newGrid;
        }

        void Cascade(int row, int col, int[,] grid)
        {
            int size = grid.GetLength(0);
            if (row < 0 || col < 0 || row >= size || col >= size || grid[row, col] > 9)
                return;
            grid[row, col] = grid[row, col] + 1;
            if (grid[row, col] <= 9)
                return;

            Cascade(row+1, col, grid);
            Cascade(row+1, col+1, grid);
            Cascade(row, col+1, grid);
            Cascade(row-1, col, grid);
            Cascade(row-1, col+1, grid);
            Cascade(row-1, col-1, grid);
            Cascade(row, col-1, grid);
            Cascade(row+1, col-1, grid);
        }

        int ResetCountFlashes(int[,] grid)
        {
            int count = 0;

            int size = grid.GetLength(0);
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (grid[row, col] > 9)
                    {
                        count++;
                        grid[row, col] = 0;
                    }
                }
            }

            return count;
        }

        void IncreaseStep(int[,] grid)
        {
            int size = grid.GetLength(0);
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    grid[row, col] = grid[row, col] + 1;
                }
            }
        }

        void DebugGrid(int[,] grid, int step, int flashCount=0)
        {
            int size = grid.GetLength(0);

            Debug.WriteLine($"Step {step} ({flashCount} flashes):");

            for(int row=0; row<size; row++)
            {
                for(int col=0; col<size; col++)
                {
                    string val = grid[row, col] > 9 ? "*" : grid[row, col].ToString();
                    Debug.Write($"{val}");
                }
                Debug.Write("\n");
            }
        }

        int [,] ParseInput(string inp)
        {
            var lines = inp.Split('\n');

            int size = lines.Length;
            var grid = new int[size, size];

            for(int row=0; row<lines.Length; row++)
            {
                var line = lines[row];
                for(int col=0; col<size; col++)
                {
                    grid[row, col] = line[col]-'0';
                }
            }

            return grid;
        }

        public string input0 = @"11111
19991
19191
19991
11111";

        public string input = @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526";

        public string input2 = @"5723573158
3154748563
4783514878
3848142375
3637724151
8583172484
7747444184
1613367882
6228614227
4732225334";

    }
}
