using gomokuApp.Core;
namespace gomokuApp;

internal static class Program
{
    private static void Main(string[] args)
    {
        Gomoku gomoku;
        if (args.Length == 1)
        {
            gomoku = args[0] switch
            {
                "debug" => new Gomoku(false, true),
                "release" => new Gomoku(true),
                _ => new Gomoku()
            };
        }
        else
        {
            gomoku = new Gomoku();
        }
        
        do
        {
            gomoku.Start();

            Console.Write("もう一度遊びますか？ [y:n] ");
            var reInput = Console.ReadLine()?.ToLower();
            while (reInput != "y" && reInput != "n")
            {
                Console.Write("正しい値を入力してください [y:n] ");
                reInput = Console.ReadLine()?.ToLower();
            }

            if (reInput == "n")
                break;

        } while (true);

    }
}