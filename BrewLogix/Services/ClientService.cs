using BrewLogix.Services;
using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;

namespace BrewLogix.Services;

public class ClientService
{
    private readonly AppDbContext _context;

    public ClientService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Client> GetAllClients()
    {
        return _context.Clients.AsNoTracking().ToList();
    }

    public void AddClient(Client client)
    {
        _context.Clients.Add(client);
        _context.SaveChanges();
    }

    public void DeleteClient(Client client)
    {
        var clientToRemove = _context.Clients.Find(client.Id);
        if (clientToRemove != null)
        {
            _context.Clients.Remove(clientToRemove);
            _context.SaveChanges();
        }
    }

    public void UpdateClient(Client client)
    {
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