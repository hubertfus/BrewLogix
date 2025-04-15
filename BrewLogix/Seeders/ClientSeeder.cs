using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BrewLogix.Services;

namespace BrewLogix.Seeders
{
    public static class ClientSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Clients.Any()) return;

            var clients = new List<Client>
            {
                new Client
                {
                    Name = "Client A",
                    ContactEmail = "clientA@example.com",
                    Address = "1234 Main St, Cityville, USA"
                },
                new Client
                {
                    Name = "Client B",
                    ContactEmail = "clientB@example.com",
                    Address = "5678 Oak St, Townsville, USA"
                },
                new Client
                {
                    Name = "Client C",
                    ContactEmail = "clientC@example.com",
                    Address = "91011 Pine St, Villagetown, USA"
                }
            };

            context.Clients.AddRange(clients);
            context.SaveChanges();
        }
    }
}