namespace gomokuApp;

/// <summary>
/// ボード探索用
/// </summary>
public static class CheckBoard
{
    /// <summary>
    /// playerの石の色
    /// </summary>
    public static readonly Stone PlayerStone = Stone.White;

    /// <summary>
    /// comの石の色
    /// </summary>
    public static readonly Stone ComStone = Stone.Black;
    
    /// <summary>
    /// 探索する深さ
    /// 0以上
    /// </summary>
    public const int SearchDepth = 1
        ;
    /// <summary>
    /// 最大スコア
    /// </summary>
    public const int MaxScore = 100000;
    /// <summary>
    /// 最小スコア
    /// </summary>
    public const int MinScore = -100000;

    /// <summary>
    /// 2 : player
    /// 1 : enemy
    /// 0 : empty
    /// </summary>
    private static readonly Pattern[] BlockPatterns =
    {

        // 防ぐ手
        new(
            new[,]
            {
                { 2, 1, 1, 1, 1, 2 }
            },
            BlockType.Open, 9000),
        new(
            new[,]
            {
                { 1, 1, 1, 1, 2 }
            },
            BlockType.Open, 8000),
        new(
            new[,]
            {
                { 2, 1, 1, 1, 1 }
            },
            BlockType.Open, 8000),
         new(
            new[,]
            {
                { 1, 1, 2, 1, 1 }
            },
            BlockType.Open, 7500),

        new(
            new[,]
            {
                { 1, 1, 2, 1 }
            },
            BlockType.Open, 7000),
        new(
            new[,]
            {
                { 1, 2, 1, 1 }
            },
            BlockType.Open,7000),
        new(
            new[,]
            {
                {2, 1, 1, 1 }
            },
            BlockType.Open, 6000),
        new(
            new[,]
            {
                {1, 1, 1, 2 }
            },
            BlockType.Open, 6000),
        new(
            new[,]
            {
                {2, 1, 1, 1, 2 }
            },
            BlockType.Open, 1000),
      

        // 攻めの手
        new(
            new[,]
            {
                { 0, 2, 2, 2, 2, 0 }
            },
            BlockType.Open, 9000),
        new(
            new[,]
            {
                { 2, 2, 2, 2, 0 }
            },
            BlockType.Open, 5000),
        new(
            new[,]
            {
                { 0, 2, 2, 2, 2 }
            },
            BlockType.Open, 5000),
        new(
            new[,]
            {
                { 0, 2, 2, 2, 0 }
            },
            BlockType.Open, 3000),
        new(
            new[,]
            {
                { 0, 0, 2, 2, 2 }
            },
            BlockType.Open, 3000),
        new(
            new[,]
            {
                { 2, 2, 2, 0, 0 }
            },
            BlockType.Open, 2000),
        new(
            new[,]
            {
                {2, 2, 0, 2, 0 }
            },
            BlockType.Open, 1500),
        new(
            new[,]
            {
                { 0, 2, 2, 0, 2 }
            },
            BlockType.Open, 1200),
        new(
            new[,]
            {
                {2, 2, 0, 2 }
            },
            BlockType.Open, 400),
        new(
            new[,]
            {
                {2, 2, 0, 2, 2 }
            },
            BlockType.Open, 400),
        new(
            new[,]
            {
                {2, 2, 0, 0, 0 }
            },
            BlockType.Open, 225),
        new(
            new[,]
            {
                {0, 0, 0, 2, 2 }
            },
            BlockType.Open, 225),

        // 優先度低い守り
          new(
            new[,]
            {
                {2, 1, 1 }
            },
            BlockType.Open, 100),
         new(
            new[,]
            {
                {1, 1, 2 }
            },
            BlockType.Open, 100),




    };

    /// <summary>
    /// パターンとマッチするかをかえす
    /// </summary>
    /// <param name="board">探索用ボード</param>
    /// <param name="pattern">パターン</param>
    /// <param name="player">探索する石</param>
    /// <returns>マッチしたか</returns>
    private static bool PatternMatch(Stone[,] board, Pattern pattern, Stone player)
    {
        int boardHeight = board.GetLength(0);
        int boardWidth = board.GetLength(1);

        int patternLength = pattern.Value.GetLength(1);

        // 横方向のチェック
        for (int i = 0; i < boardHeight; i++)
        {
            for (int j = 0; j < boardWidth - patternLength + 1; j++)
            {
                if (IsMatchHorizontal(board, i, j, pattern, player))
                    return true;
            }
        }

        // 縦方向のチェック
        for (int i = 0; i < boardHeight - patternLength + 1; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                if (IsMatchVertical(board, i, j, pattern, player))
                    return true;
            }
        }

        // 斜め右上方向のチェック
        for (int i = 0; i < boardHeight - patternLength + 1; i++)
        {
            for (int j = 0; j < boardWidth - patternLength + 1; j++)
            {
                if (IsMatchDiagonalRight(board, i, j, pattern, player))
                    return true;
            }
        }

        // 斜め左上方向のチェック
        for (int i = 0; i < boardHeight - patternLength + 1; i++)
        {
            for (int j = patternLength - 1; j < boardWidth; j++)
            {
                if (IsMatchDiagonalLeft(board, i, j, pattern, player))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 横方向のパターンマッチング
    /// </summary>
    /// <param name="board"></param>
    /// <param name="row"></param>
    /// <param name="startCol"></param>
    /// <param name="pattern"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private static bool IsMatchHorizontal(Stone[,] board, int row, int startCol, Pattern pattern, Stone player)
    {
        for (int i = 0; i < pattern.Value.GetLength(1); i++)
        {
            if (board[row, startCol + i] != GetStone(pattern.Value[0, i], player))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 縦方向のパターンマッチング
    /// </summary>
    /// <param name="board"></param>
    /// <param name="startRow"></param>
    /// <param name="col"></param>
    /// <param name="pattern"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private static bool IsMatchVertical(Stone[,] board, int startRow, int col, Pattern pattern, Stone player)
    {
        for (int i = 0; i < pattern.Value.GetLength(1); i++)
        {
            if (board[startRow + i, col] != GetStone(pattern.Value[0, i], player))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 斜め右上方向のパターンマッチング
    /// </summary>
    /// <param name="board"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    /// <param name="pattern"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private static bool IsMatchDiagonalRight(Stone[,] board, int startRow, int startCol, Pattern pattern, Stone player)
    {
        for (int i = 0; i < pattern.Value.GetLength(1); i++)
        {
            if (board[startRow + i, startCol + i] != GetStone(pattern.Value[0, i], player))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 斜め左上方向のパターンマッチング
    /// </summary>
    /// <param name="board"></param>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    /// <param name="pattern"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private static bool IsMatchDiagonalLeft(Stone[,] board, int startRow, int startCol, Pattern pattern, Stone player)
    {
        for (int i = 0; i < pattern.Value.GetLength(1); i++)
        {
            if (board[startRow + i, startCol - i] != GetStone(pattern.Value[0, i], player))
                return false;
        }

        return true;
    }

    /// <summary>
    /// パターン内の数字をプレイヤーの石にマッピングする関数
    /// </summary>
    /// <param name="patternValue"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private static Stone GetStone(int patternValue, Stone player)
    {
        if (patternValue == 0)
            return Stone.None;
        if (patternValue == 1)
            return player;
        // patternValue == 2
        return SwitchPlayer(player);
    }


    /// <summary>
    /// boardのスコア取得用 (最長５マス)
    /// </summary>
    /// <param name="board">探索用ボード</param>
    /// <param name="player">Stoneのタイプ</param>
    /// <returns></returns>
    public static int ToAllPatternMatch(Stone[,] board, Stone player)
    {
        int score = 0;
        foreach (var pattern in BlockPatterns)
        {
            // 自身の石で探索
            if (PatternMatch(board, pattern, player))
            {
                score += pattern.Score;
            }

            // 相手の意志で探索
            var switchPattern = SwitchPattern(pattern);
            if (PatternMatch(board, switchPattern, player))
            {
                score += switchPattern.Score;
            }
        }
        

        return score;
    }

    // プレイヤーを切り替える
    public static Stone SwitchPlayer(Stone player)
    {
        return player == PlayerStone ? ComStone : PlayerStone;
    }

    /// <summary>
    /// パターンとスコアを逆にする
    /// </summary>
    /// <param name="a">反転するパターン</param>
    /// <returns>反転したパターン</returns>
    private static Pattern SwitchPattern(Pattern a)
    {
        int xLength = a.Value.GetLength(1);
        int yLength = a.Value.GetLength(0);
        int[,] pattern = new int[yLength, xLength];
        for(int y = 0; y < yLength; y++ ){
            for (int x = 0; x < xLength; x++)
            {
                int val = a.Value[y, x];
                if (val == 1)
                {
                    val = 2;
                }
                else if (val == 2)
                {
                    val = 1;
                }
                pattern[y, x] = val;
            }
        }

        return new Pattern(
            pattern,
            a.Type,
            -1 * a.Score
        );

    }
}


public enum BlockType
{
    Open,
    Block,
    None,
}

public struct Pattern
{
    public int Score { init; get; } = 0;
    public int[,] Value { get; }
    public BlockType Type { get; }

    public Pattern(int[,] value, BlockType type, int score)
    {
        Score = score;

        Type = type;

        Value = value;
    }
}