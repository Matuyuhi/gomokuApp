namespace gomokuApp.Core;

public class Stone
{
    private readonly string display;

    private Stone(string display)
    {
        this.display = display;
    }

    public string GetPutStone()
    {
        return display;
    }

    public static readonly Stone None = new ("　");
    public static readonly Stone White = new ("⚫︎");
    public static readonly Stone Black = new ("〇");
}

public struct Move
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    public Move(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public static class MoveEdit
{
        
    public static bool Contains(this Move a, int x, int y)
    {
        return a.X == x && a.Y == y;
    }

    public static bool Contains(this Move a, Move b)
    {
        return b.X == a.X && b.Y == a.Y;
    }
}