using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BrewLogix.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Kegs)
                .ToList();
        }

        public Order? GetOrderById(int id)
        {
            return _context.Orders
                .Include(o => o.Kegs)
                .FirstOrDefault(o => o.Id == id);
        }

        public void AddOrder(Order order)
        {
            order.OrderedAt = order.OrderedAt.ToUniversalTime(); 
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            var existing = _context.Orders
                .Include(o => o.Kegs)
                .FirstOrDefault(o => o.Id == order.Id);

            if (existing != null)
            {
                existing.ClientId = order.ClientId;
                existing.OrderedAt = order.OrderedAt.ToUniversalTime();
                existing.Status = order.Status;

                existing.Kegs = order.Kegs;

                _context.SaveChanges();
            }
        }

        public void DeleteOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }
    }
}