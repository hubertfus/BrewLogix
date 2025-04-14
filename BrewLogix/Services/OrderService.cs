using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using BrewLogix.dbhelpers;

namespace BrewLogix.Services
{
    public class OrderService
    {
        private readonly IDbContextProvider _dbContextProvider;

        public OrderService(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            using var _context = _dbContextProvider.GetDbContext();
            return _context.Orders
                .Include(o => o.Kegs)
                .ToList();
        }

        public Order? GetOrderById(int id)
        {
            using var _context = _dbContextProvider.GetDbContext();
            return _context.Orders
                .Include(o => o.Kegs)
                .FirstOrDefault(o => o.Id == id);
        }

        public void AddOrder(Order order)
        {
            using var _context = _dbContextProvider.GetDbContext();
            order.OrderedAt = order.OrderedAt.ToUniversalTime();
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            using var _context = _dbContextProvider.GetDbContext();
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
            using var _context = _dbContextProvider.GetDbContext();
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }
    }
}
