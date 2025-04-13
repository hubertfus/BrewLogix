using BrewLogix.Models;
using System.Collections.Generic;
using System.Linq;

namespace BrewLogix.Services
{
    public class OrderService
    {
        private readonly List<Order> _orders;

        public OrderService()
        {
            _orders = new List<Order>
            {
                new Order 
                { 
                    Id = 1, 
                    ClientId = 1, 
                    OrderedAt = DateTime.Now.AddDays(-5), 
                    Status = "Pending",
                    Kegs = new List<Keg> { new Keg { Id = 1, Code = "KEG 001", Size = "5", IsDistributed = false } }
                },
                new Order 
                { 
                    Id = 2, 
                    ClientId = 2, 
                    OrderedAt = DateTime.Now.AddDays(-3), 
                    Status = "Shipped", 
                    Kegs = new List<Keg> { new Keg { Id = 2, Code = "KEG 002", Size = "10", IsDistributed = false } }
                }
            };
        }

        // Get all orders
        public IEnumerable<Order> GetAllOrders() => _orders;

        // Get order by id
        public Order GetOrderById(int id) => _orders.FirstOrDefault(o => o.Id == id);

        // Add a new order
        public void AddOrder(Order order)
        {
            order.Id = _orders.Max(o => o.Id) + 1;
            _orders.Add(order);
        }

        // Update an existing order
        public void UpdateOrder(Order order)
        {
            var index = _orders.FindIndex(o => o.Id == order.Id);
            if (index != -1)
            {
                _orders[index] = order;
            }
        }

        // Delete an order
        public void DeleteOrder(Order order)
        {
            _orders.Remove(order);
        }
    }
}