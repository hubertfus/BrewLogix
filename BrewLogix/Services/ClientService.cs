using BrewLogix.dbhelpers;
using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BrewLogix.Services
{
    public class ClientService
    {
        private readonly IDbContextProvider _dbContextProvider;

        public ClientService(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public IEnumerable<Client> GetAllClients()
        {
            using var _context = _dbContextProvider.GetDbContext();
            return _context.Clients.AsNoTracking().ToList();
        }

        public void AddClient(Client client)
        {
            using var _context = _dbContextProvider.GetDbContext();

            if (_context.Clients.Any(c => c.ContactEmail == client.ContactEmail))
            {
                throw new InvalidOperationException("Email is already taken by another client.");
            }

            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        public void DeleteClient(Client client)
        {
            using var _context = _dbContextProvider.GetDbContext();

            if (_context.Orders.Any(o => o.ClientId == client.Id))
            {
                throw new InvalidOperationException("This client cannot be deleted because they have associated orders.");
            }

            var clientToRemove = _context.Clients.Find(client.Id);
            if (clientToRemove != null)
            {
                _context.Clients.Remove(clientToRemove);
                _context.SaveChanges();
            }
        }


        public void UpdateClient(Client client)
        {
            using var _context = _dbContextProvider.GetDbContext();

            if (_context.Clients.Any(c => c.ContactEmail == client.ContactEmail && c.Id != client.Id))
            {
                throw new InvalidOperationException("Email is already taken by another client.");
            }

            var clientToUpdate = _context.Clients.Find(client.Id);
            if (clientToUpdate != null)
            {
                clientToUpdate.Name = client.Name;
                clientToUpdate.ContactEmail = client.ContactEmail;
                clientToUpdate.Address = client.Address;
                _context.SaveChanges();
            }
        }
    }
}
