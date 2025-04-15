using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BrewLogix.Services;

namespace BrewLogix.Seeders
{
    public static class OrderSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Orders.Any()) return;

            var orders = new List<Order>
            {
                new Order
                {
                    ClientId = 1,
                    OrderedAt = DateTime.UtcNow.AddDays(-5),
                    Status = "Completed"
                },
                new Order
                {
                    ClientId = 2, 
                    OrderedAt = DateTime.UtcNow.AddDays(-2),
                    Status = "In Progress"
                },
                new Order
                {
                    ClientId = 3, 
                    OrderedAt = DateTime.UtcNow.AddDays(-1),
                    Status = "Pending"
                }
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}