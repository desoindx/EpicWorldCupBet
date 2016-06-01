using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

class Player
{
    private static List<int[]> _cols;
    private static bool _danger;
    private static int[,] _myGrid;
    private static int[,] _hisGrid;
    private static Tuple<int, int>[] _blockToCome;

    private static readonly Random Rng = new Random();
    static List<T[]> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1)
            return list.Select(t => new T[] { t }).ToList();

        return GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }).ToArray()).ToList();
    }

    class Combo
    {
        public int BlockCleaned { get; set; }

        public int Power { get; set; }

        public int Color { get; set; }

        public int Size { get; set; }

        public int BlockConnected { get; set; }

        public int Score
        {
            get
            {
                int p = 0;
                if (Power > 1)
                    p = (int)(8 * Math.Pow(2, Power - 1));
                int c = 0;
                if (Color > 1)
                    c = (int)Math.Pow(2, Color - 1);

                var bonus = (c + Size + p);

                if (bonus < 5)
                    bonus = 0;
                else
                    bonus *= 10;
                return BlockCleaned * bonus + BlockConnected;
            }
        }
    }

    class Board
    {
        private readonly int[,] _grid;

        public void Draw()
        {
            for (int i = 0; i < 12; i++)
            {
                Console.Error.WriteLine("{0} {1} {2} {3} {4} {5}", _grid[i, 0], _grid[i, 1], _grid[i, 2], _grid[i, 3], _grid[i, 4], _grid[i, 5]);
            }
        }

        public Board(int[,] grid)
        {
            _grid = grid;
        }

        public Board Copy()
        {
            return new Board((int[,])_grid.Clone());
        }

        public Combo MakeMove(Tuple<int, int> nextBlock, Tuple<int, int> move)
        {
            var col = move.Item1;
            switch (move.Item2)
            {
                case 0:
                    return DropBlocks(nextBlock, new Tuple<int, int>(col, col + 1));
                case 1:
                    return DropBlocks(nextBlock, new Tuple<int, int>(col, col));
                case 2:
                    return DropBlocks(nextBlock, new Tuple<int, int>(col, col - 1));
                case 3:
                    if (nextBlock.Item1 == nextBlock.Item2)
                    {
                        return null;
                    }
                    return DropBlocks(new Tuple<int, int>(nextBlock.Item2, nextBlock.Item1), new Tuple<int, int>(col, col));
            }
            return null;
        }

        private Combo DropBlocks(Tuple<int, int> nextBlock, Tuple<int, int> columns)
        {
            //            Console.Error.WriteLine("Drop: {0} {1}", nextBlock.Item1, columns.Item1);
            //            Console.Error.WriteLine("Drop: {0} {1}", nextBlock.Item2, columns.Item2);
            var dropped1 = DropBlock(nextBlock.Item1, columns.Item1);
            var dropped2 = DropBlock(nextBlock.Item2, columns.Item2);
            if (dropped1 == null || dropped2 == null)
                return null;
            var results = CleanBlock(dropped1, dropped2);
            var blockCleaned = results.Item1;
            var color = results.Item3;
            var combo = results.Item4;
            var size = results.Item5;

            while (results.Item1 > 0)
            {
                ApplyGravity();
                results = CleanBlock();
                blockCleaned += results.Item1;
                color += results.Item3;
                combo += results.Item4;
                size += results.Item5;
            }

            return new Combo { BlockCleaned = blockCleaned, BlockConnected = results.Item2, Color = color, Power = combo, Size = size };
        }

        private void ApplyGravity()
        {
            for (int j = 0; j < 6; j++)
            {
                int lowestFree = 11;
                for (int i = 11; i >= 0; i--)
                {
                    var currentColor = _grid[i, j];
                    if (currentColor != -1)
                    {
                        var tmp = _grid[lowestFree, j];
                        _grid[lowestFree, j] = currentColor;
                        _grid[i, j] = tmp;
                        lowestFree--;
                    }
                }
            }
        }

        private Tuple<int, int> DropBlock(int block, int column)
        {
            for (int i = 11; i >= 0; i--)
            {
                if (_grid[i, column] < 0)
                {
                    _grid[i, column] = block;
                    return new Tuple<int, int>(i, column);
                }
            }
            return null;
        }

        private Tuple<int, int, int, int, int> CleanBlock(Tuple<int, int> dropped1, Tuple<int, int> dropped2)
        {
            int blockCleaned = 0;
            int blockConnected = 0;
            var colors = new HashSet<int>();
            var combo = 0;
            var size = 0;
            var visitedCells = new HashSet<Tuple<int, int>>();
            blockConnected = BlockConnected(dropped1.Item1, dropped1.Item2, visitedCells, blockConnected, colors, ref combo, ref size, ref blockCleaned);
            blockConnected = BlockConnected(dropped2.Item1, dropped2.Item2, visitedCells, blockConnected, colors, ref combo, ref size, ref blockCleaned);
            return new Tuple<int, int, int, int, int>(blockCleaned, blockConnected, colors.Count, combo, size);
        }

        private Tuple<int, int, int, int, int> CleanBlock()
        {
            int blockCleaned = 0;
            int blockConnected = 0;
            var colors = new HashSet<int>();
            var combo = 0;
            var size = 0;
            var visitedCells = new HashSet<Tuple<int, int>>();
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    blockConnected = BlockConnected(i, j, visitedCells, blockConnected, colors, ref combo, ref size, ref blockCleaned);
                }
            }

            return new Tuple<int, int, int, int, int>(blockCleaned, blockConnected, colors.Count, combo, size);
        }

        private int BlockConnected(int i, int j, HashSet<Tuple<int, int>> visitedCells, int blockConnected, HashSet<int> colors, ref int combo,
            ref int size, ref int blockCleaned)
        {
            var cell = new Tuple<int, int>(i, j);
            if (visitedCells.Contains(cell))
            {
                return blockConnected;
            }

            var currentColor = _grid[i, j];
            if (currentColor > 0)
            {
                var connectedBlocks = new HashSet<Tuple<int, int>>();
                GetConnectedBlock(cell, currentColor, connectedBlocks);
                visitedCells.UnionWith(connectedBlocks);
                var count = connectedBlocks.Count;
                blockConnected += count - 1;
                if (count < 4)
                {
                    return blockConnected;
                }
                colors.Add(currentColor);
                combo++;
                size += count - 4;
                foreach (var connectedBlock in connectedBlocks)
                {
                    blockCleaned += CleanIfBlack(connectedBlock);
                    _grid[connectedBlock.Item1, connectedBlock.Item2] = -1;
                    blockCleaned++;
                }
            }
            return blockConnected;
        }

        private int CleanIfBlack(Tuple<int, int> cell)
        {
            int result = 0;
            if (cell.Item1 < 11 && _grid[cell.Item1 + 1, cell.Item2] == 0)
            {
                result++;
                _grid[cell.Item1 + 1, cell.Item2] = -1;
            }

            if (cell.Item1 > 0 && _grid[cell.Item1 - 1, cell.Item2] == 0)
            {
                result++;
                _grid[cell.Item1 - 1, cell.Item2] = -1;
            }

            if (cell.Item2 < 5 && _grid[cell.Item1, cell.Item2 + 1] == 0)
            {
                result++;
                _grid[cell.Item1, cell.Item2 + 1] = -1;
            }

            if (cell.Item2 > 0 && _grid[cell.Item1, cell.Item2 - 1] == 0)
            {
                result++;
                _grid[cell.Item1, cell.Item2 - 1] = -1;
            }
            return result;
        }

        private void GetConnectedBlock(Tuple<int, int> cell, int expectedColor, HashSet<Tuple<int, int>> connectedBlocks)
        {
            if (connectedBlocks.Contains(cell))
            {
                return;
            }

            var color = _grid[cell.Item1, cell.Item2];
            if (color != expectedColor)
            {
                return;
            }

            connectedBlocks.Add(cell);

            if (cell.Item1 < 11)
            {
                GetConnectedBlock(new Tuple<int, int>(cell.Item1 + 1, cell.Item2), expectedColor, connectedBlocks);
            }

            if (cell.Item1 > 0)
            {
                GetConnectedBlock(new Tuple<int, int>(cell.Item1 - 1, cell.Item2), expectedColor, connectedBlocks);
            }

            if (cell.Item2 < 5)
            {
                GetConnectedBlock(new Tuple<int, int>(cell.Item1, cell.Item2 + 1), expectedColor, connectedBlocks);
            }

            if (cell.Item2 > 0)
            {
                GetConnectedBlock(new Tuple<int, int>(cell.Item1, cell.Item2 - 1), expectedColor, connectedBlocks);
            }
        }

        public List<Tuple<int, int>> AvailableMoves(int color)
        {
            List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();
            bool nextCol = _grid[0, 0] == -1;
            for (int p = color; p < 6; p++)
            {
                if (p == 5)
                {
                    if (_grid[1, 5] == -1)
                    {
                        tuples.Add(new Tuple<int, int>(5, 1));
                        tuples.Add(new Tuple<int, int>(5, 3));
                    }
                    continue;
                }
                var currentCol = nextCol;
                nextCol = _grid[0, p + 1] == -1;
                if (!currentCol)
                {
                    continue;
                }

                if (nextCol)
                {
                    tuples.Add(new Tuple<int, int>(p, 0));
                    tuples.Add(new Tuple<int, int>(p + 1, 2));
                }

                if (_grid[1, p] == -1)
                {
                    tuples.Add(new Tuple<int, int>(p, 1));
                    tuples.Add(new Tuple<int, int>(p, 3));
                }
            }

            for (int p = 0; p < color; p++)
            {
                if (p == 5)
                {
                    if (_grid[1, 5] == -1)
                    {
                        tuples.Add(new Tuple<int, int>(5, 1));
                        tuples.Add(new Tuple<int, int>(5, 3));
                    }
                    continue;
                }
                var currentCol = nextCol;
                nextCol = _grid[0, p + 1] == -1;
                if (!currentCol)
                {
                    continue;
                }

                if (nextCol)
                {
                    tuples.Add(new Tuple<int, int>(p, 0));
                    tuples.Add(new Tuple<int, int>(p + 1, 2));
                }

                if (_grid[1, p] == -1)
                {
                    tuples.Add(new Tuple<int, int>(p, 1));
                    tuples.Add(new Tuple<int, int>(p, 3));
                }
            }

            return tuples;

            //            var next = Rng.Next(_cols.Count);
            //            var index = _cols[next];
            //            //            Console.Error.WriteLine("{6} : {0} {1} {2} {3} {4} {5}", index[0], index[1], index[2], index[3], index[4], index[5], next);
            //            return
            //                moves[index[0]].Concat(
            //                    moves[index[1]].Concat(
            //                        moves[index[2]].Concat(
            //                            moves[index[3]].Concat(
            //                                moves[index[4]].Concat(
            //                                    moves[index[5]]))))).ToList();
        }
    }

    class AI
    {
        public Tuple<int, int> GetBestMove(Board board, Tuple<int, int>[] blocksToCome, int timeOut, Stopwatch watch1)
        {
            //                    Console.Error.WriteLine("Drop {0} {1}", blocksToCome[0].Item1, blocksToCome[0].Item2);
            //                    Console.Error.WriteLine("On {0} {1}", move.Item1, move.Item2);
            //                    Console.Error.WriteLine("Score {0}", score1);
            //                    Console.Error.WriteLine("Then Drop {0} {1}", blocksToCome[1].Item1, blocksToCome[1].Item2);
            //                    Console.Error.WriteLine("On {0} {1}", nexMove.Item1, nexMove.Item2);
            //                    Console.Error.WriteLine("Score {0}", score);
            Tuple<int, int> bestMove = new Tuple<int, int>(2, 0);
            var bestScore = int.MinValue;

            var value = new List<Tuple<Board, int, Tuple<int, int>>>();
            var nextBlock = blocksToCome[0];
            foreach (var move in board.AvailableMoves(nextBlock.Item1))
            {
                var newBoard = board.Copy();
                var moveScore = GetMoveScore(newBoard, nextBlock, move);
                if (moveScore == null)
                    continue;
                var score = moveScore.Score;
                value.Add(new Tuple<Board, int, Tuple<int, int>>(newBoard, score, move));
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            if (bestScore > 1260)
            {
                return bestMove;
            }

            nextBlock = blocksToCome[1];
            var value2 = new List<Tuple<Board, int, Tuple<int, int>>>();
            foreach (var state in value)
            {
                foreach (var move in state.Item1.AvailableMoves(nextBlock.Item1))
                {
                    if (watch1.ElapsedMilliseconds > timeOut)
                    {
                        return bestMove;
                    }
                    var newBoard = state.Item1.Copy();
                    var moveScore = GetMoveScore(newBoard, nextBlock, move);
                    if (moveScore == null)
                        continue;
                    var score = moveScore.Score + state.Item2;
                    value2.Add(new Tuple<Board, int, Tuple<int, int>>(newBoard, score, state.Item3));
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = state.Item3;
                    }
                }
            }

            nextBlock = blocksToCome[2];
            value.Clear();
            var value3 = value;
            foreach (var state in value2)
            {
                foreach (var move in state.Item1.AvailableMoves(nextBlock.Item1))
                {
                    if (watch1.ElapsedMilliseconds > timeOut)
                    {
                        return bestMove;
                    }
                    var newBoard = state.Item1.Copy();
                    var moveScore = GetMoveScore(newBoard, nextBlock, move);
                    if (moveScore == null)
                        continue;
                    var score = moveScore.Score + state.Item2;
                    value3.Add(new Tuple<Board, int, Tuple<int, int>>(newBoard, score, state.Item3));

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = state.Item3;
                    }
                }
            }

            nextBlock = blocksToCome[3];
            value2.Clear();
            var value4 = value2;
            foreach (var state in value3)
            {
                if (watch1.ElapsedMilliseconds > timeOut)
                {
                    return bestMove;
                }
                foreach (var move in state.Item1.AvailableMoves(nextBlock.Item1))
                {
                    if (watch1.ElapsedMilliseconds > timeOut)
                    {
                        return bestMove;
                    }
                    var newBoard = state.Item1.Copy();
                    var moveScore = GetMoveScore(newBoard, nextBlock, move);
                    if (moveScore == null)
                        continue;
                    var score = moveScore.Score + state.Item2;
                    value4.Add(new Tuple<Board, int, Tuple<int, int>>(newBoard, score, state.Item3));

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = state.Item3;
                    }
                }
            }

            nextBlock = blocksToCome[4];
            //            var value4 = new List<Tuple<Board, int, Tuple<int, int>>>();
            foreach (var state in value4)
            {
                if (watch1.ElapsedMilliseconds > timeOut)
                {
                    return bestMove;
                }
                foreach (var move in state.Item1.AvailableMoves(nextBlock.Item1))
                {
                    if (watch1.ElapsedMilliseconds > timeOut)
                    {
                        return bestMove;
                    }
                    var newBoard = state.Item1.Copy();
                    var moveScore = GetMoveScore(newBoard, nextBlock, move);
                    if (moveScore == null)
                        continue;
                    var score = moveScore.Score + state.Item2;
                    //                    value4.Add(new Tuple<Board, int, Tuple<int, int>>(newBoard, score, state.Item3));
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = state.Item3;
                    }
                }
            }

            return bestMove;
        }

        private Combo GetMoveScore(Board board, Tuple<int, int> nextBlock, Tuple<int, int> move)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var moveScore = board.MakeMove(nextBlock, move);
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 5)
                Console.Error.WriteLine(stopwatch.ElapsedMilliseconds);
            return moveScore;
        }

        public int GetBestScore(Board board, Tuple<int, int> blocksToCome)
        {
            var bestScore = int.MinValue;

            foreach (var move in board.AvailableMoves(0))
            {
                var newBoard = board.Copy();
                var moveScore = GetMoveScore(newBoard, blocksToCome, move);
                if (moveScore == null)
                    continue;
                var score = moveScore.Score;
                if (score > bestScore)
                {
                    bestScore = score;
                }
            }
            return bestScore;
        }
    }

    static void Main(string[] args)
    {
        _cols = GetPermutations(Enumerable.Range(0, 6), 6);
        _myGrid = new int[12, 6];
        _hisGrid = new int[12, 6];
        _blockToCome = new Tuple<int, int>[8];
        var ai = new AI();
        int count = 0;
        while (true)
        {
            var watch1 = new Stopwatch();
            watch1.Start();
            for (int i = 0; i < 8; i++)
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int colorA = int.Parse(inputs[0]);
                int colorB = int.Parse(inputs[1]);
                _blockToCome[i] = new Tuple<int, int>(colorA, colorB);
            }
            for (int i = 0; i < 12; i++)
            {
                string row = Console.ReadLine();
                for (int j = 0; j < 6; j++)
                {
                    var c = row[j];
                    _myGrid[i, j] = c == '.' ? -1 : (int)char.GetNumericValue(c);
                }
            }
            for (int i = 0; i < 12; i++)
            {
                string row = Console.ReadLine();
                for (int j = 0; j < 6; j++)
                {
                    var c = row[j];
                    _hisGrid[i, j] = c == '.' ? -1 : (int)char.GetNumericValue(c);
                }
            }
            var hisBoard = new Board(_hisGrid);
            var hisScore = ai.GetBestScore(hisBoard, _blockToCome[0]);
            var board = new Board(_myGrid);
            _danger = hisScore > 1500;
            var bestMove = ai.GetBestMove(board, _blockToCome, 97, watch1);
            Console.WriteLine("{0} {1}", bestMove.Item1, bestMove.Item2);
        }
    }
}
