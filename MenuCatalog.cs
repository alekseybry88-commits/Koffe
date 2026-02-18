// TODO:
// 1. Реализовать хранение меню с категориями
// 2. Реализовать поиск напитков по параметрам
// 3. Реализовать учет популярности напитков

using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShop
{
    public class MenuCatalog
    {
        private List<Coffee> coffees = new List<Coffee>();
        private Dictionary<Coffee, int> popularityStats = new Dictionary<Coffee, int>();
        private Dictionary<string, List<Coffee>> categoryMap = new Dictionary<string, List<Coffee>>();
        private Dictionary<int, Coffee> coffeeById = new Dictionary<int, Coffee>(); // для быстрого поиска

        public MenuCatalog()
        {
            categoryMap["классика"] = new List<Coffee>();
            categoryMap["авторские"] = new List<Coffee>();
            categoryMap["без кофеина"] = new List<Coffee>();
            categoryMap["сезонные"] = new List<Coffee>();
        }

        public void AddCoffee(Coffee coffee)
        {
            if (coffee == null) return;
            coffees.Add(coffee);
            popularityStats[coffee] = 0;
            coffeeById[coffee.Id] = coffee;

            string category = DetermineCategory(coffee);
            if (!categoryMap.ContainsKey(category))
                categoryMap[category] = new List<Coffee>();
            categoryMap[category].Add(coffee);
        }

        private string DetermineCategory(Coffee coffee)
        {
            string nameLower = coffee.Name?.ToLower() ?? "";
            string descLower = coffee.Description?.ToLower() ?? "";
            if (nameLower.Contains("тыквенный") || nameLower.Contains("глинтвейн") ||
                nameLower.Contains("пряный") || descLower.Contains("сезонный"))
                return "сезонные";
            if (nameLower.Contains("декаф") || descLower.Contains("без кофеина"))
                return "без кофеина";
            if (nameLower.Contains("раф") || nameLower.Contains("карамель") ||
                nameLower.Contains("ваниль") || nameLower.Contains("лаванда") ||
                nameLower.Contains("орех"))
                return "авторские";
            return "классика";
        }

        public List<Coffee> GetCoffeesByCategory(string category)
        {
            if (string.IsNullOrEmpty(category)) return new List<Coffee>();
            string cat = category.ToLower();
            return categoryMap.ContainsKey(cat) ? categoryMap[cat] : new List<Coffee>();
        }

        public Coffee FindCoffeeByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return coffees.FirstOrDefault(c =>
                c.Name != null && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public List<Coffee> FindCoffeesByStrength(string strength)
        {
            if (string.IsNullOrEmpty(strength)) return new List<Coffee>();
            return coffees.Where(c =>
                c.GetStrength().Equals(strength, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Coffee> FindCoffeesByOrigin(string origin)
        {
            if (string.IsNullOrEmpty(origin)) return new List<Coffee>();
            return coffees.Where(c =>
                c.Origin != null && c.Origin.IndexOf(origin, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }

        // Фиксация продажи (принимает объект Coffee)
        public void RecordSale(Coffee coffee)
        {
            if (coffee == null) return;
            if (popularityStats.ContainsKey(coffee))
                popularityStats[coffee]++;
            else
                popularityStats[coffee] = 1;
        }

        // Получить самые популярные напитки
        public List<Coffee> GetMostPopularCoffees(int count = 5)
        {
            return popularityStats
                .OrderByDescending(kv => kv.Value)
                .Take(count)
                .Select(kv => kv.Key)
                .ToList();
        }

        // Для CoffeeShopManager.GetCoffeeShopStats
        public string GetMostPopularCoffee()
        {
            var top = popularityStats.OrderByDescending(kv => kv.Value).FirstOrDefault();
            return top.Key?.Name ?? "—";
        }

        // Поиск по ID
        public Coffee GetCoffeeById(int id)
        {
            coffeeById.TryGetValue(id, out Coffee coffee);
            return coffee;
        }

        // Сезонные рекомендации
        public List<Coffee> GetSeasonalRecommendations(int currentMonth)
        {
            if (categoryMap.ContainsKey("сезонные") && categoryMap["сезонные"].Count > 0)
                return categoryMap["сезонные"];

            var recommendations = new List<Coffee>();
            switch (currentMonth)
            {
                case 12:
                case 1:
                case 2:
                    recommendations.Add(new Coffee(99, "Глинтвейн", "Пряный горячий напиток", 280, 250, "средняя", "средний", "Германия"));
                    recommendations.Add(new Coffee(98, "Коричный латте", "Латте с корицей", 250, 300, "светлая", "крупный", "Шри-Ланка"));
                    break;
                case 3:
                case 4:
                case 5:
                    recommendations.Add(new Coffee(97, "Лавандовый раф", "Раф с лавандой", 270, 250, "светлая", "средний", "Франция"));
                    recommendations.Add(new Coffee(96, "Цитрусовый американо", "Американо с апельсином", 200, 200, "средняя", "средний", "Мексика"));
                    break;
                case 6:
                case 7:
                case 8:
                    recommendations.Add(new Coffee(95, "Фраппе", "Холодный кофе со льдом", 230, 300, "средняя", "крупный", "Греция"));
                    recommendations.Add(new Coffee(94, "Кофейный лимонад", "Лимонад с эспрессо", 210, 350, "светлая", "крупный", "Италия"));
                    break;
                case 9:
                case 10:
                case 11:
                    recommendations.Add(new Coffee(93, "Тыквенный латте", "Латте с тыквенным сиропом", 260, 300, "средняя", "крупный", "США"));
                    recommendations.Add(new Coffee(92, "Карамельное яблоко", "Капучино с карамелью и яблоком", 270, 250, "темная", "средний", "Польша"));
                    break;
            }
            return recommendations;
        }

        // Полное меню по категориям
        public void ShowMenu()
        {
            Console.WriteLine("=== МЕНЮ КОФЕЙНИ ===");
            Console.WriteLine("\n--- КЛАССИКА ---");
            foreach (var coffee in GetCoffeesByCategory("классика"))
                Console.WriteLine($"  {coffee.Id}. {coffee.Name} - {coffee.VolumeMl} мл - {coffee.BasePrice} руб.");

            Console.WriteLine("\n--- АВТОРСКИЕ ---");
            foreach (var coffee in GetCoffeesByCategory("авторские"))
                Console.WriteLine($"  {coffee.Id}. {coffee.Name} - {coffee.VolumeMl} мл - {coffee.BasePrice} руб.");

            Console.WriteLine("\n--- БЕЗ КОФЕИНА ---");
            foreach (var coffee in GetCoffeesByCategory("без кофеина"))
                Console.WriteLine($"  {coffee.Id}. {coffee.Name} - {coffee.VolumeMl} мл - {coffee.BasePrice} руб.");

            Console.WriteLine("\n--- СЕЗОННЫЕ ---");
            foreach (var coffee in GetCoffeesByCategory("сезонные"))
                Console.WriteLine($"  {coffee.Id}. {coffee.Name} - {coffee.VolumeMl} мл - {coffee.BasePrice} руб.");
        }

        // Краткое меню (ID, название, цена)
        public void ShowMenuCompact()
        {
            foreach (var coffee in coffees)
            {
                Console.WriteLine($"{coffee.Id}. {coffee.Name} - {coffee.BasePrice} руб.");
            }
        }
    }
}