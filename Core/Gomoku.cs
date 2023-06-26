namespace gomokuApp.Core;

public class Gomoku
{
    /// <summary>
    /// 表示用のボード
    /// </summary>
    private readonly Stone[,] board;

    /// <summary>
    /// 何も置かれていないマスのリスト
    /// </summary>
    private readonly List<Move> freeList;

    /// <summary>
    /// ボードサイズ
    /// </summary>
    private const int vMax = 8;
    private const int hMax = 8;

    /// <summary>
    /// playerの石の色
    /// </summary>
    private readonly Stone playerStone = CheckBoard.PlayerStone;

    /// <summary>
    /// comの石の色
    /// </summary>
    private readonly Stone comStone = CheckBoard.ComStone;

    /// <summary>
    /// 揃える判定のサイズ
    /// </summary>
    private const int matchLength = 5;

    /// <summary>
    /// 毎回画面をクリアにする
    /// </summary>
    private readonly bool isClear = false;

    /// <summary>
    /// 探索のデバッグ用
    /// </summary>
    private readonly bool isDebug = false;

    private Move? beforeCom;
    private Move? beforePlayer;

    public Gomoku()
    {
        board = new Stone[vMax, hMax];
        freeList = new List<Move>(vMax * hMax);

        for (int v = 0; v < board.GetLength(0); v++)
        {
            for (int h = 0; h < board.GetLength(1); h++)
            {
                board[v, h] = Stone.None;
                freeList.Add(new Move(v, h));
            }
        }
    }

    private void PrintBoard(Stone[,] printBoard)
    {
        int rows = printBoard.GetLength(0);
        int columns = printBoard.GetLength(1);

        // 列番号を表示
        Console.Write("   ");
        for (int h = 0; h < columns; h++)
        {
            // 0~7 -> 1~8
            Console.Write(" {0} ", h + 1);
        }

        Console.WriteLine();

        // マス目と石を表示
        for (int v = 0; v < rows; v++)
        {
            // 行番号を表示
            // 0~7 -> 1~8
            Console.Write("{0} |", v + 1);

            for (int h = 0; h < columns; h++)
            {
                Console.BackgroundColor = printBoard[v, h] == comStone ? ConsoleColor.White : ConsoleColor.Black;
                Console.ForegroundColor = printBoard[v, h] == comStone ? ConsoleColor.Black : ConsoleColor.White;
                // 直前の手は色を変える
                if (beforeCom != null && 
                    beforeCom.Value.X == v && beforeCom.Value.Y == h
                ) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("{0}", printBoard[v, h].GetPutStone());
                }
                else if (beforePlayer != null &&
                         beforePlayer.Value.X == v && beforePlayer.Value.Y == h
                ) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("{0}", printBoard[v, h].GetPutStone());
                }
                else
                {
                    Console.Write("{0}", printBoard[v, h].GetPutStone());
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("|");
            }

            Console.WriteLine();

            // 水平線を表示
            Console.Write("   ");
            for (int h = 0; h < columns; h++)
            {
                Console.Write("ー ");
            }

            Console.WriteLine();
        }
    }

    private bool OnFree(int v, int h)
    {

        var match = freeList.FindAll(item => item.Contains(v, h));
        return match.Count != 0;
    }

    // 未使用
    // private void RemoveFreeList(int index)
    // {
    //     freeList.RemoveAt(index);
    // }

    private void RemoveFreeList(Move val)
    {
        freeList.Remove(val);
    }
        

    /// <summary>
    /// ボードの指定のマスに石をセットする
    /// </summary>
    /// <param name="v">縦のマス</param>
    /// <param name="h">横のマス</param>
    /// <param name="stone">セットする石のタイプ</param>
    private void SetStone(int v, int h, Stone stone)
    {
        if (
            v is >= 0 and < vMax &&
            h is >= 0 and < hMax
        )
        {
            board[v, h] = stone;
            RemoveFreeList(new Move(v, h));
        }
    }

    /// <summary>
    /// ボードの指定のマスに石をセットする
    /// </summary>
    /// <param name="index">freeList内のインデックスでマスを指定する</param>
    /// <param name="stone">セットする石のタイプ</param>
    private void SetStone(int index, Stone stone)
    {
        Move moveIndex = freeList[index];
        board[moveIndex.X, moveIndex.Y] = stone;
    }
    
    private Move GetFreeMove(int index)
    {
        return freeList[index];
    }
        

    private int EvaluateBoard(Stone[,] searchBoard, Stone player)
    {
        int score = CheckBoard.ToAllPatternMatch(searchBoard, player);
        return score;
    }

    private int Minimax(Stone[,] searchBoard, int depth, bool isMaximizingPlayer, Stone player)
    {
            
        if (depth == 0 || IsMatchInBoard(player, board))
        {
            // 探索の深さの上限に達したか、勝敗がついた場合はボードを評価
            return EvaluateBoard(searchBoard, player);
        }

        if (isMaximizingPlayer)
        {
            int minEval = CheckBoard.MinScore;
            foreach (var move in GenerateAllMoves())
            {
                Stone[,] newBoard = ApplyMove(searchBoard, move, player);
                int eval = Minimax(newBoard, depth - 1, false, CheckBoard.SwitchPlayer(player));
                minEval = Math.Max(minEval, eval);
            }
            return minEval;
        }

        int maxEval = CheckBoard.MaxScore;
 
        foreach (var move in GenerateAllMoves())
        {
            Stone[,] newBoard = ApplyMove(searchBoard, move, player);
            int eval = Minimax(newBoard, depth - 1, true, CheckBoard.SwitchPlayer(player));
            maxEval = Math.Min(maxEval, eval);
        }
        return maxEval;
    }

    /// <summary>
    /// 与えられたボードに対して可能なすべての手を生成する
    /// </summary>
    /// <returns></returns>
    private IEnumerable<Move> GenerateAllMoves()
    {
        return freeList;
    }

    /// <summary>
    /// 与えられた手を適用し、新しいボードを生成する
    /// </summary>
    /// <param name="searchBoard"></param>
    /// <param name="move"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private Stone[,] ApplyMove(Stone[,] searchBoard, Move move, Stone player)
    {
        Stone[,] newBoard = (Stone[,])searchBoard.Clone();
        newBoard[move.X, move.Y] = player;
        return newBoard;
    }
        
    /// <summary>
    /// comの操作
    /// 並列処理で短縮
    /// </summary>
    private void OnCOM()
    {
        int maxEval = CheckBoard.MinScore;
        int bestMove = -1;
            
        // 並列処理で短縮　
        // 未使用　→ 0.5s
        // 使用   → 0.1s
        // var stopWatch = new Stopwatch();
        // stopWatch.Start();
        //for (int i = 0; i < freeList.Count; i++)
        Parallel.For(0, freeList.Count, i =>
        {
            Move moveIndex = freeList[i];
            Stone[,] tempBoard = (Stone[,])board.Clone();
            tempBoard[moveIndex.X, moveIndex.Y] = comStone;
            int eval = Minimax(tempBoard, CheckBoard.SearchDepth, false, comStone);
            if (eval > maxEval)
            {
                maxEval = eval;
                bestMove = i;
            }

            if (isDebug)
            {
                Console.WriteLine("Score {0} = {1}", i, eval);
                Console.ReadLine();
            }
            // }
        });
            
        // stopWatch.Stop();
        // Console.WriteLine("■処理Aにかかった時間");
        // TimeSpan ts = stopWatch.Elapsed;
        // Console.WriteLine($"　{ts.Minutes}分 {ts.Seconds}秒 {ts.Milliseconds}ミリ秒");

        if (bestMove != -1)
        {
            beforeCom = GetFreeMove(bestMove);
            SetStone(bestMove, comStone);
            freeList.RemoveAt(bestMove);
        }
        else
        {
            beforeCom = null;
        }
    }

    /// <summary>
    /// 縦 横 斜め　で揃ったマスを探索する
    /// </summary>
    /// <param name="stone">指定の色のタイプで探索する</param>
    /// <param name="searchBoard">探索する盤面</param>
    /// <param name="length">探索するサイズ　基本いじらない</param>
    /// <returns>揃ったかどうか</returns>
    private bool IsMatchInBoard(Stone stone, Stone[,] searchBoard, int length = matchLength)
    {

        // 縦方向の判定
        for (int row = 0; row <= vMax - length; row++)
        {
            for (int col = 0; col < hMax; col++)
            {
                bool isMatch = true;
                for (int i = 0; i < length; i++)
                {
                    if (searchBoard[row + i, col] != stone)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return true;
                }
            }
        }

        // 横方向の判定
        for (int row = 0; row < vMax; row++)
        {
            for (int col = 0; col <= hMax - length; col++)
            {
                bool isMatch = true;
                for (int i = 0; i < length; i++)
                {
                    if (searchBoard[row, col + i] != stone)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return true;
                }
            }
        }

        // 右斜め方向の判定
        for (int row = 0; row <= vMax - length; row++)
        {
            for (int col = 0; col <= hMax - length; col++)
            {
                bool isMatch = true;
                for (int i = 0; i < length; i++)
                {
                    if (searchBoard[row + i, col + i] != stone)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return true;
                }
            }
        }

        // 左斜め方向の判定
        for (int row = 0; row <= vMax - length; row++)
        {
            for (int col = length - 1; col < hMax; col++)
            {
                bool isMatch = true;
                for (int i = 0; i < length; i++)
                {
                    if (searchBoard[row + i, col - i] != stone)
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return true;
                }
            }
        }

        return false;
    }


    /// <summary>
    /// 五目並べを開始する
    /// </summary>
    public void Start()
    {
        Console.Clear();
        while (true)
        {
            PrintBoard(board);
            Console.WriteLine("endで終了します");

            Console.Write("縦：");
            var input = Console.ReadLine();
            if (input == "end") break;
            if (!int.TryParse(input, out var ver))
            {
                if (isClear) Console.Clear();
                Console.WriteLine("1~{0}で入力してください", vMax);
                continue;
            }

            if (ver is < 1 or > vMax)
            {
                if (isClear) Console.Clear();
                Console.WriteLine("1~{0}で入力してください", vMax);
                continue;
            }

            Console.Write("横：");
            input = Console.ReadLine();
            if (input == "end") break;
            if (!int.TryParse(input, out var hor))
            {
                if (isClear) Console.Clear();
                Console.WriteLine("1~{0}で入力してください", hMax);
                continue;
            }

            if (hor is < 1 or > hMax)
            {
                if (isClear) Console.Clear();
                Console.WriteLine("1~{0}で入力してください", hMax);
                continue;
            }

            ver--;
            hor--;

            if (OnFree(ver, hor))
            {
                SetStone(ver, hor, playerStone);
                beforePlayer = new Move(ver, hor);
                if (IsMatchInBoard(playerStone, board))
                {
                    if (isClear) Console.Clear();
                    PrintBoard(board);
                    Console.WriteLine("あなたの勝ちです");
                    break;
                }
            }
            else
            {
                if (isClear) Console.Clear();
                Console.WriteLine("空いているマスを入力してください");
                continue;
            }

            if (isClear) Console.Clear();
            // com
            OnCOM();
            if (IsMatchInBoard(comStone, board))
            {
                if (isClear) Console.Clear();
                PrintBoard(board);
                Console.WriteLine("あなたの負けです");
                break;
            }

            if (freeList.Count == 0)
            {
                if (isClear) Console.Clear();
                PrintBoard(board);
                Console.WriteLine("引き分けです");
                break;
            }
        }
    }
}