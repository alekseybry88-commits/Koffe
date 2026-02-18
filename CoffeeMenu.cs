// TODO:
// 1. Реализовать просмотр меню и добавление напитков
// 2. Реализовать оформление заказа с выбором добавок
// 3. Реализовать программу лояльности и бонусы

using System;
using System.Collections.Generic;

namespace CoffeeShop
{
    public class CoffeeMenu
    {
        private CoffeeShopManager manager;

        public CoffeeMenu()
        {
            manager = new CoffeeShopManager();
            manager.InitializeMenu();
        }

        public void ShowMainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== КОФЕЙНЯ 'АРОМАТ' ===");
                Console.WriteLine("1. Посмотреть меню");
                Console.WriteLine("2. Сделать заказ");
                Console.WriteLine("3. Мои заказы");
                Console.WriteLine("4. Регистрация клиента");
                Console.WriteLine("5. Моя карта лояльности");
                Console.WriteLine("6. Статистика кофейни");
                Console.WriteLine("7. Выход");
                Console.WriteLine("8. Завершить заказ (для персонала)");
                Console.Write("Выберите: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowMenu();
                        break;
                    case "2":
                        CreateOrder();
                        break;
                    case "3":
                        ShowOrders();
                        break;
                    case "4":
                        RegisterCustomer();
                        break;
                    case "5":
                        ShowLoyaltyCard();
                        break;
                    case "6":
                        ShowStats();
                        break;
                    case "7":
                        exit = true;
                        Console.WriteLine("До свидания!");
                        break;
                    case "8":
                        CompleteOrderForStaff();
                        break;
                    default:
                        Console.WriteLine("Неверный выбор! Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public void ShowMenu()
        {
            Console.Clear();
            var catalog = manager.GetMenuCatalog();
            catalog.ShowMenu();
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        public void CreateOrder()
        {
            Console.Clear();
            Console.WriteLine("=== ОФОРМЛЕНИЕ ЗАКАЗА ===");

            Console.Write("Ваше имя (или телефон для бонусов): ");
            string customerInput = Console.ReadLine();

            Customer customer = null;
            if (!string.IsNullOrWhiteSpace(customerInput))
                customer = manager.FindCustomerByPhone(customerInput);

            string customerName;
            if (customer != null)
            {
                customerName = customer.FullName;
                Console.WriteLine($"Добро пожаловать, {customerName}! У вас {customer.GetBonusPoints()} бонусов.");
            }
            else
            {
                customerName = customerInput;
                Console.WriteLine("Вы не зарегистрированы. Заказ будет оформлен как гость.");
            }

            Console.WriteLine("\nТип заказа:");
            Console.WriteLine("1. Здесь");
            Console.WriteLine("2. С собой");
            Console.WriteLine("3. Доставка");
            Console.Write("Выберите: ");
            string typeChoice = Console.ReadLine();
            string orderType = typeChoice switch
            {
                "1" => "здесь",
                "2" => "с собой",
                "3" => "доставка",
                _ => "здесь"
            };

            var order = manager.CreateOrder(customerName, orderType);

            bool addingItems = true;
            while (addingItems)
            {
                Console.Clear();
                Console.WriteLine($"=== ЗАКАЗ #{order.Id} ===");
                Console.WriteLine($"Клиент: {customerName}");
                Console.WriteLine($"Тип: {orderType}\n");

                var catalog = manager.GetMenuCatalog();
                catalog.ShowMenuCompact();

                Console.Write("\nВведите ID напитка (0 - завершить выбор): ");
                if (int.TryParse(Console.ReadLine(), out int coffeeId) && coffeeId != 0)
                {
                    var coffee = catalog.GetCoffeeById(coffeeId);
                    if (coffee != null)
                    {
                        List<string> additives = new List<string>();
                        Console.WriteLine("\nДобавки (вводите номер, пустая строка - закончить):");
                        Console.WriteLine("1. Сироп (+50 руб.)");
                        Console.WriteLine("2. Сливки (+30 руб.)");
                        Console.WriteLine("3. Маршмеллоу (+40 руб.)");
                        Console.WriteLine("4. Корица (+20 руб.)");
                        Console.WriteLine("5. Двойная порция кофе (+60 руб.)");

                        bool addMore = true;
                        while (addMore)
                        {
                            Console.Write("Добавка №: ");
                            string addChoice = Console.ReadLine();
                            switch (addChoice)
                            {
                                case "1": additives.Add("сироп"); break;
                                case "2": additives.Add("сливки"); break;
                                case "3": additives.Add("маршмеллоу"); break;
                                case "4": additives.Add("корица"); break;
                                case "5": additives.Add("двойная порция кофе"); break;
                                case "":
                                case null:
                                    addMore = false;
                                    break;
                                default:
                                    Console.WriteLine("Неверный ввод, попробуйте ещё раз.");
                                    break;
                            }
                        }

                        order.AddItem(coffee, additives);
                        Console.WriteLine($"\nДобавлено: {coffee.Name} с добавками. Промежуточная сумма: {order.Subtotal} руб.");
                    }
                    else
                    {
                        Console.WriteLine("Напиток с таким ID не найден.");
                    }
                }
                else
                {
                    addingItems = false;
                }

                if (addingItems)
                {
                    Console.Write("\nНажмите любую клавишу для продолжения добавления...");
                    Console.ReadKey();
                }
            }

            // Применение бонусов (если клиент зарегистрирован)
            if (customer != null && customer.GetBonusPoints() > 0)
            {
                Console.WriteLine($"\nУ вас {customer.GetBonusPoints()} бонусов. Желаете списать бонусы? (1 - да, 2 - нет)");
                string bonusChoice = Console.ReadLine();
                if (bonusChoice == "1")
                {
                    Console.Write("Сколько бонусов списать? (1 бонус = 1 рубль): ");
                    if (int.TryParse(Console.ReadLine(), out int pointsToUse))
                    {
                        if (customer.UseBonusPoints(pointsToUse, order))
                            Console.WriteLine($"Бонусы применены. Скидка {pointsToUse} руб.");
                        else
                            Console.WriteLine("Не удалось применить бонусы (недостаточно или превышен лимит 30%).");
                    }
                }
            }

            Console.WriteLine("\n=== ИТОГ ЗАКАЗА ===");
            Console.WriteLine(order.GetFullDetails());
            Console.Write("\nПодтвердить заказ? (1 - да, 2 - нет): ");
            string confirm = Console.ReadLine();

            if (confirm == "1")
            {
                order.Confirm();
                Console.WriteLine("Заказ подтверждён! Спасибо!");
                // Заказ уже добавлен в список при создании
            }
            else
            {
                Console.WriteLine("Заказ отменён.");
                // Здесь можно удалить заказ из списка, но для простоты оставим как есть
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        public void ShowOrders()
        {
            Console.Clear();
            Console.WriteLine("=== АКТИВНЫЕ ЗАКАЗЫ ===");

            var activeOrders = manager.GetActiveOrders();
            if (activeOrders == null || activeOrders.Count == 0)
            {
                Console.WriteLine("Нет активных заказов.");
            }
            else
            {
                foreach (var order in activeOrders)
                {
                    Console.WriteLine(order.GetShortInfo());
                    Console.WriteLine("----------------------");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        public void RegisterCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== РЕГИСТРАЦИЯ НОВОГО КЛИЕНТА ===");

            Console.Write("ФИО: ");
            string name = Console.ReadLine();

            Console.Write("Телефон: ");
            string phone = Console.ReadLine();

            Console.Write("Любимый напиток: ");
            string favorite = Console.ReadLine();

            Console.Write("Аллергии (если есть, через запятую): ");
            string allergies = Console.ReadLine();

            Customer customer = new Customer(0, name, phone, favorite, allergies);
            manager.AddCustomer(customer);

            Console.WriteLine($"Клиент зарегистрирован! Ваш уровень: {customer.GetLoyaltyLevel()}");
            Console.WriteLine("При следующем заказе начислятся бонусные баллы!");
            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        public void ShowLoyaltyCard()
        {
            Console.Clear();
            Console.WriteLine("=== КАРТА ЛОЯЛЬНОСТИ ===");

            Console.Write("Введите телефон: ");
            string phone = Console.ReadLine();

            Customer customer = manager.FindCustomerByPhone(phone);
            if (customer != null)
            {
                customer.ShowCustomerInfo();

                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. История заказов");
                Console.WriteLine("2. Назад");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowOrderHistory(customer);
                        break;
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Клиент не найден.");
                Console.ReadKey();
            }
        }

        private void ShowOrderHistory(Customer customer)
        {
            Console.WriteLine($"\n=== ИСТОРИЯ ЗАКАЗОВ {customer.FullName} ===");
            var orders = manager.GetOrdersByCustomer(customer.Phone);
            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine("У вас ещё нет заказов.");
            }
            else
            {
                foreach (var order in orders)
                {
                    Console.WriteLine(order.GetFullDetails());
                    Console.WriteLine("----------------------");
                }
            }
            Console.ReadKey();
        }

        public void ShowStats()
        {
            Console.Clear();
            Console.WriteLine("=== СТАТИСТИКА КОФЕЙНИ ===");

            var stats = manager.GetCoffeeShopStats(); // кортеж (revenue, ordersCount, avgCheck, bestCoffee)

            Console.WriteLine($"Выручка за день: {stats.revenue} руб.");
            Console.WriteLine($"Количество заказов: {stats.ordersCount}");
            Console.WriteLine($"Средний чек: {stats.avgCheck:F2} руб.");
            Console.WriteLine($"Самый популярный напиток: {stats.bestCoffee}");

            // Дополнительно покажем популярные напитки
            var catalog = manager.GetMenuCatalog();
            var popular = catalog.GetMostPopularCoffees(3);
            if (popular.Count > 0)
            {
                Console.WriteLine("\n--- Популярные напитки ---");
                foreach (var coffee in popular)
                {
                    Console.WriteLine($"{coffee.Name}");
                }
            }

            Console.WriteLine("\n--- Сезонные рекомендации ---");
            var seasonal = catalog.GetSeasonalRecommendations(DateTime.Now.Month);
            foreach (var coffee in seasonal)
            {
                Console.WriteLine($"{coffee.Name} - {coffee.BasePrice} руб.");
            }

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }

        // Новый метод для завершения заказа (для персонала)
        public void CompleteOrderForStaff()
        {
            Console.Clear();
            Console.WriteLine("=== ЗАВЕРШЕНИЕ ЗАКАЗА (ДЛЯ ПЕРСОНАЛА) ===");
            Console.Write("Введите номер заказа: ");
            if (int.TryParse(Console.ReadLine(), out int orderId))
            {
                if (manager.CompleteOrder(orderId))
                    Console.WriteLine($"Заказ {orderId} завершён. Выручка и бонусы обновлены.");
                else
                    Console.WriteLine("Не удалось завершить заказ (неверный ID или статус).");
            }
            else
            {
                Console.WriteLine("Некорректный номер.");
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
    }
}