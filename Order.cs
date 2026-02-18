// TODO:
// 1. Добавить поля для информации о заказе (номер, дата, статус)
// 2. Реализовать расчет стоимости заказа
// 3. Реализовать учет скидок и акций

using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShop
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }        // принят, готовится, готов, выдан, отменен
        public string OrderType { get; set; }     // здесь, с собой, доставка
        public Customer Customer { get; set; }    // ссылка на зарегистрированного клиента (может быть null)

        private List<OrderItem> items = new List<OrderItem>();
        private decimal bonusDiscount = 0;        // скидка за бонусы (в рублях)

        public class OrderItem
        {
            public Coffee Coffee { get; set; }
            public int Quantity { get; set; }
            public List<string> Additives { get; set; } = new List<string>();
            public decimal ItemPrice { get; set; } // цена одной порции с добавками
        }

        public IReadOnlyList<OrderItem> Items => items.AsReadOnly();

        public Order(int id, string customerName, string orderType)
        {
            Id = id;
            OrderDate = DateTime.Now;
            CustomerName = customerName;
            Status = "принят";
            OrderType = orderType;
        }

        // Добавление напитка (количество по умолчанию 1)
        public void AddItem(Coffee coffee, List<string> additives = null, int quantity = 1)
        {
            var item = new OrderItem
            {
                Coffee = coffee,
                Quantity = quantity,
                Additives = additives ?? new List<string>()
            };
            item.ItemPrice = coffee.CalculatePriceWithAdditives(item.Additives);
            items.Add(item);
        }

        // Перегрузка для обратной совместимости
        public void AddCoffee(Coffee coffee, int quantity, List<string> additives = null)
        {
            AddItem(coffee, additives, quantity);
        }

        // Сумма до скидок (используется для расчётов и отображения)
        public decimal Subtotal => items.Sum(i => i.ItemPrice * i.Quantity);

        // Итоговая сумма с учётом всех скидок (автоскидки + бонусы)
        public decimal Total => CalculateTotal();

        private decimal CalculateTotal()
        {
            // Базовая сумма
            decimal sum = Subtotal;

            // Скидка за сумму заказа
            decimal autoDiscount = 0;
            if (sum >= 1000)
                autoDiscount = sum * 0.10m;
            else if (sum >= 500)
                autoDiscount = sum * 0.05m;

            decimal afterAuto = sum - autoDiscount;

            // Скидка для заказов "с собой"
            if (OrderType == "с собой")
                afterAuto *= 0.97m;

            // Применяем бонусную скидку (не более 30% от afterAuto)
            decimal maxBonusDiscount = afterAuto * 0.3m;
            decimal appliedBonus = Math.Min(bonusDiscount, maxBonusDiscount);

            return Math.Round(afterAuto - appliedBonus, 2);
        }

        // Применить скидку за бонусы (вызывается из Customer.UseBonusPoints)
        public void ApplyDiscount(decimal amount)
        {
            bonusDiscount += amount; // может накапливаться, но обычно одноразово
        }

        // Подтверждение заказа
        public void Confirm()
        {
            if (Status == "принят")
                Status = "готовится";
        }

        // Отмена заказа
        public bool Cancel()
        {
            if (Status == "принят")
            {
                Status = "отменен";
                return true;
            }
            return false;
        }

        // Время приготовления в минутах
        public int GetPreparationTimeMinutes()
        {
            double totalMinutes = 0;
            foreach (var item in items)
            {
                string name = item.Coffee.Name?.ToLower() ?? "";
                if (name.Contains("эспрессо"))
                    totalMinutes += 1.0 * item.Quantity;
                else if (name.Contains("американо"))
                    totalMinutes += 1.5 * item.Quantity;
                else if (name.Contains("капучино") || name.Contains("латте"))
                    totalMinutes += 2.0 * item.Quantity;
                else
                    totalMinutes += 1.5 * item.Quantity;

                if (item.Additives.Count > 0)
                    totalMinutes += 0.5 * item.Quantity;
            }

            if (OrderType == "с собой" || OrderType == "доставка")
                totalMinutes += 2;

            return (int)Math.Ceiling(totalMinutes);
        }

        // Добавление акционного товара
        public bool AddPromotionalItem(Coffee coffee, int requiredOrderAmount)
        {
            if (Subtotal >= requiredOrderAmount)
            {
                var promoItem = new OrderItem
                {
                    Coffee = coffee,
                    Quantity = 1,
                    Additives = new List<string>()
                };
                promoItem.ItemPrice = coffee.CalculatePriceWithAdditives(null) * 0.5m;
                items.Add(promoItem);
                return true;
            }
            return false;
        }

        // Краткая информация для списка
        public string GetShortInfo()
        {
            return $"Заказ №{Id} от {OrderDate:HH:mm} - {CustomerName} - {Status} - сумма {Total} руб.";
        }

        // Полная детализация
        public string GetFullDetails()
        {
            string details = $"=== ЗАКАЗ №{Id} ===\n" +
                             $"Клиент: {CustomerName}\n" +
                             $"Дата и время: {OrderDate:dd.MM.yyyy HH:mm}\n" +
                             $"Тип заказа: {OrderType}\n" +
                             $"Статус: {Status}\n\n" +
                             "Состав заказа:\n";

            foreach (var item in items)
            {
                details += $"  {item.Coffee.Name} x{item.Quantity} - {item.ItemPrice * item.Quantity} руб.";
                if (item.Additives.Count > 0)
                    details += $" (добавки: {string.Join(", ", item.Additives)})";
                details += "\n";
            }

            details += $"\nСумма до скидок: {Subtotal} руб.\n";
            details += $"Итоговая стоимость: {Total} руб.\n";
            details += $"Время приготовления: {GetPreparationTimeMinutes()} мин.\n";

            return details;
        }

        // Для обратной совместимости с CoffeeShopManager.CompleteOrder
        public void SetCustomer(Customer customer)
        {
            Customer = customer;
        }
    }
}