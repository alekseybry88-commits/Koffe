// TODO:
// 1. Добавить поля для информации о клиенте (любимый напиток, аллергии)
// 2. Реализовать учет бонусных баллов
// 3. Реализовать систему уровней лояльности

using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShop
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string FavoriteCoffee { get; set; }
        public string Allergies { get; set; }
        public DateTime? BirthDate { get; private set; }

        private List<Order> orderHistory = new List<Order>();
        private int bonusPoints = 0;
        private string loyaltyLevel = "Новичок";

        public Customer(int id, string name, string phone, string favoriteCoffee, string allergies)
        {
            Id = id;
            FullName = name;
            Phone = phone;
            RegistrationDate = DateTime.Now;
            FavoriteCoffee = favoriteCoffee;
            Allergies = allergies;
        }

        // Добавление заказа в историю с начислением бонусов
        public void AddOrder(Order order)
        {
            orderHistory.Add(order);
            int earned = (int)(order.Total * 0.1m);
            AddBonusPoints(earned);
            UpdateLoyaltyLevel();
        }

        public void UpdateLoyaltyLevel()
        {
            int ordersCount = orderHistory.Count;
            if (ordersCount >= 31)
                loyaltyLevel = "Мастер";
            else if (ordersCount >= 16)
                loyaltyLevel = "Знаток";
            else if (ordersCount >= 6)
                loyaltyLevel = "Любитель";
            else
                loyaltyLevel = "Новичок";
        }

        // Использование бонусов: проверка и применение скидки к заказу
        public bool UseBonusPoints(int points, Order order)
        {
            if (points <= 0 || points > bonusPoints)
                return false;

            decimal maxDiscount = order.Subtotal * 0.3m;
            if (points > maxDiscount)
                return false;

            bonusPoints -= points;
            order.ApplyDiscount(points);   // применяем скидку к заказу
            return true;
        }

        public int CalculateAge()
        {
            if (!BirthDate.HasValue) return 0;
            int age = DateTime.Now.Year - BirthDate.Value.Year;
            if (DateTime.Now < BirthDate.Value.AddYears(age)) age--;
            return age;
        }

        public (int totalOrders, decimal totalSpent, string favoriteCoffee, int currentBonus) GetPurchaseStats()
        {
            int total = orderHistory.Count;
            decimal spent = 0;
            Dictionary<string, int> coffeeFrequency = new Dictionary<string, int>();

            foreach (var order in orderHistory)
            {
                spent += order.Total;
                foreach (var item in order.Items)
                {
                    string coffeeName = item.Coffee.Name;
                    if (coffeeFrequency.ContainsKey(coffeeName))
                        coffeeFrequency[coffeeName]++;
                    else
                        coffeeFrequency[coffeeName] = 1;
                }
            }

            string favorite = coffeeFrequency.OrderByDescending(kv => kv.Value)
                                             .Select(kv => kv.Key)
                                             .FirstOrDefault() ?? FavoriteCoffee ?? "—";
            return (total, spent, favorite, bonusPoints);
        }

        public void SetBirthDate(DateTime birthDate) => BirthDate = birthDate;

        public bool CheckBirthdayDiscount()
        {
            if (!BirthDate.HasValue) return false;
            bool isBirthdayToday = BirthDate.Value.Day == DateTime.Now.Day &&
                                   BirthDate.Value.Month == DateTime.Now.Month;
            bool sufficientLevel = loyaltyLevel == "Знаток" || loyaltyLevel == "Мастер";
            return isBirthdayToday && sufficientLevel;
        }

        public void AddBonusPoints(int points)
        {
            if (points > 0) bonusPoints += points;
        }

        public string GetLoyaltyLevel() => loyaltyLevel;
        public int GetBonusPoints() => bonusPoints;

        public void ShowCustomerInfo()
        {
            Console.WriteLine($"Клиент: {FullName}");
            Console.WriteLine($"Телефон: {Phone}");
            Console.WriteLine($"Дата регистрации: {RegistrationDate:dd.MM.yyyy}");
            Console.WriteLine($"Любимый напиток: {FavoriteCoffee}");
            if (!string.IsNullOrEmpty(Allergies))
                Console.WriteLine($"Аллергии: {Allergies}");

            var stats = GetPurchaseStats();
            Console.WriteLine("\nСтатистика:");
            Console.WriteLine($"  Уровень: {loyaltyLevel}");
            Console.WriteLine($"  Всего заказов: {stats.totalOrders}");
            Console.WriteLine($"  Общая сумма: {stats.totalSpent:F2} руб.");
            Console.WriteLine($"  Бонусные баллы: {stats.currentBonus}");

            if (CheckBirthdayDiscount())
                Console.WriteLine("  ★ СЕГОДНЯ У ВАС ДЕНЬ РОЖДЕНИЯ! Скидка 20% ★");
        }
    }
}