using gomokuApp.Core;
namespace gomokuApp;

internal static class Program
{
    private static void Main()
    {
        do
        {
            var gomoku = new Gomoku();
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