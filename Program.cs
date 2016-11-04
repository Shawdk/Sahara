using System;

namespace Sahara
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = "Loading Sahara...";
            Console.CursorVisible = false;

            Sahara.Initialize();

            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}
