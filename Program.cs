using System;

namespace CoffeeShop
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== КОФЕЙНЯ 'АРОМАТ' ===\n");

            CoffeeMenu menu = new CoffeeMenu();
            menu.ShowMainMenu();

            Console.WriteLine("\nСпасибо за посещение! Приходите еще!");
            Console.ReadKey();
        }
    }
}
