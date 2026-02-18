// TODO:
// 1. Реализовать хранение заказов, клиентов и меню
// 2. Реализовать создание и обработку заказов
// 3. Реализовать учет выручки и статистику

using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShop
{
    public class CoffeeShopManager
    {
        private MenuCatalog menuCatalog = new MenuCatalog();
        private List<Order> orders = new List<Order>();
        private List<Customer> customers = new List<Customer>();

        private int nextOrderId = 1000;
        private int nextCustomerId = 1;
        private decimal dailyRevenue = 0;
        private int dailyOrderCount = 0;

        public void InitializeMenu()
        {
            menuCatalog.AddCoffee(new Coffee(
                id: 1,
                name: "Эспрессо",
                description: "Классический итальянский эспрессо",
                price: 120,
                volume: 50,
                roast: "темная",
                grind: "мелкий",
                origin: "Италия"
            ));
            menuCatalog.AddCoffee(new Coffee(
                id: 2,
                name: "Американо",
                description: "Эспрессо с горячей водой",
                price: 150,
                volume: 200,
                roast: "средняя",
                grind: "средний",
                origin: "Колумбия"
            ));
            menuCatalog.AddCoffee(new Coffee(
                id: 3,
                name: "Капучино",
                description: "Кофе с молочной пеной",
                price: 200,
                volume: 200,
                roast: "средняя",
                grind: "средний",
                origin: "Бразилия"
            ));
            menuCatalog.AddCoffee(new Coffee(
                id: 4,
                name: "Латте",
                description: "Кофе с большим количеством молока",
                price: 220,
                volume: 300,
                roast: "светлая",
                grind: "крупный",
                origin: "Эфиопия"
            ));
            menuCatalog.AddCoffee(new Coffee(
                id: 5,
                name: "Раф",
                description: "Кофе со сливками и ванильным сахаром",
                price: 250,
                volume: 250,
                roast: "светлая",
                grind: "средний",
                origin: "Коста-Рика"
            ));
        }

        public void AddCustomer(Customer customer)
        {
            customer.Id = nextCustomerId++;
            customers.Add(customer);
        }

        public Order CreateOrder(string customerName, string orderType)
        {
            Customer registered = FindCustomerByName(customerName);
            Order order = new Order(nextOrderId, customerName, orderType);
            nextOrderId++;

            if (registered != null)
            {
                order.SetCustomer(registered);
            }

            orders.Add(order);
            return order;
        }

        public bool CompleteOrder(int orderId)
        {
            Order order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
                return false;

            if (order.Status == "готовится" || order.Status == "принят")
            {
                order.Status = "готов";
                dailyRevenue += order.Total;
                dailyOrderCount++;

                foreach (var item in order.Items)
                {
                    menuCatalog.RecordSale(item.Coffee); // передаём объект Coffee
                }

                if (order.Customer != null)
                {
                    order.Customer.AddOrder(order); // используем AddOrder
                }

                return true;
            }

            return false;
        }

        public Customer FindCustomerByPhone(string phone)
        {
            foreach (var customer in customers)
            {
                if (customer.Phone == phone)
                    return customer;
            }
            return null;
        }

        public (decimal revenue, int ordersCount, decimal avgCheck, string bestCoffee) GetCoffeeShopStats()
        {
            decimal avg = dailyOrderCount > 0 ? dailyRevenue / dailyOrderCount : 0;
            string best = menuCatalog.GetMostPopularCoffee();
            return (dailyRevenue, dailyOrderCount, avg, best);
        }

        public List<Order> GetActiveOrders()
        {
            List<Order> active = new List<Order>();
            foreach (var order in orders)
            {
                if (order.Status == "принят" || order.Status == "готовится")
                    active.Add(order);
            }
            return active;
        }

        public Customer FindCustomerByName(string name)
        {
            foreach (var customer in customers)
            {
                if (customer.FullName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return customer;
            }
            return null;
        }

        // Новый метод для получения заказов клиента по телефону
        public List<Order> GetOrdersByCustomer(string phone)
        {
            var customer = FindCustomerByPhone(phone);
            if (customer == null) return new List<Order>();
            // Заказы, где Customer совпадает (зарегистрированный клиент)
            return orders.Where(o => o.Customer == customer).ToList();
        }

        public MenuCatalog GetMenuCatalog()
        {
            return menuCatalog;
        }

        public int GetNextOrderId()
        {
            return nextOrderId++;
        }
    }
}