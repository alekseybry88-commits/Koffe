// TODO:
// 1. Добавить поля для характеристик кофе (обжарка, помол, страна происхождения)
// 2. Реализовать проверку корректности данных (цена, объем)
// 3. Реализовать метод расчета стоимости с учетом добавок

using System.Collections.Generic;

namespace CoffeeShop
{
    public class Coffee
    {
        public int Id { get; set; }                // Код напитков
        public string Name { get; set; }           // Название (эспрессо, латте, капучино)
        public string Description { get; set; }    // Описание
        public decimal BasePrice { get; set; }     // Базовая цена
        public int VolumeMl { get; set; }          // Объем в мл

         // TODO 1: Добавленные свойства
        public string RoastLevel { get; set; }     // Обжарка: светлая, средняя, темная
        public string GrindSize { get; set; }      // Помол: мелкий, средний, крупный
        public string Origin { get; set; }         // Страна происхождения зерен

        public Coffee(int id, string name, string description, decimal price, int volume,
                      string roast, string grind, string origin)
        {
            Id = id;
            Name = name;
            Description = description;

             // TODO 2: Проверка корректности данных
            BasePrice = price < 0 ? 100 : price;
            VolumeMl = volume <= 0 ? 200 : volume;

              // TODO 1: Сохраняем характеристики
            RoastLevel = roast;
            GrindSize = grind;
            Origin = origin;
        }

         // TODO 3: Расчет стоимости с добавками
        public decimal CalculatePriceWithAdditives(List<string> additives)
        {
            decimal finalPrice = BasePrice;
            if (additives == null) return finalPrice;

            foreach (var additive in additives)
            {
                switch (additive.ToLower())
                {
                    case "сироп":
                        finalPrice += 50;
                        break;
                    case "сливки":
                        finalPrice += 30;
                        break;
                    case "маршмеллоу":
                        finalPrice += 40;
                        break;
                    case "корица":
                        finalPrice += 20;
                        break;
                    case "двойная порция кофе":
                        finalPrice += 60;
                        break;
                }
            }
            return finalPrice;
        }

         // TODO 1: Определить крепость напитка
        public string GetStrength()
        {
            string nameLower = Name?.ToLower() ?? "";
            if (nameLower.Contains("эспрессо"))
                return "крепкий";
            if (nameLower.Contains("американо"))
                return "средний";
            if (nameLower.Contains("латте") || nameLower.Contains("капучино"))
                return "мягкий";
            return "не определено";
        }

         // TODO 2: Получить рекомендуемый объем
        public string GetRecommendedVolume()
        {
            string nameLower = Name?.ToLower() ?? "";
            if (nameLower.Contains("эспрессо"))
                return "30-60 мл";
            if (nameLower.Contains("американо"))
                return "150-200 мл";
            if (nameLower.Contains("капучино"))
                return "180-250 мл";
            if (nameLower.Contains("латте"))
                return "250-350 мл";
            return "не определено";
        }

        public override string ToString()
        {
            return $"{Name} - {VolumeMl} мл - {BasePrice} руб.";
        }
    }
}