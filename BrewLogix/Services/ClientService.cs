using BrewLogix.Models;

namespace BrewLogix.Services;

public class ClientService
{
    private readonly List<Client> _clients;

    public ClientService()
    {

        _clients = new List<Client>
        {
            new Client { Id = 1, Name = "Klient A", ContactEmail = "a@example.com", Address = "st. test 1" },
            new Client { Id = 2, Name = "Klient B", ContactEmail = "b@example.com", Address = "st. test 2" },
            new Client { Id = 3, Name = "Klient C", ContactEmail = "c@example.com", Address = "st. test 3" },
            new Client { Id = 4, Name = "Klient D", ContactEmail = "d@example.com", Address = "st. test 4" },
            new Client { Id = 5, Name = "Klient E", ContactEmail = "e@example.com", Address = "st. test 5" },
            new Client { Id = 6, Name = "Klient F", ContactEmail = "f@example.com", Address = "st. test 6" },
            new Client { Id = 7, Name = "Klient G", ContactEmail = "g@example.com", Address = "st. test 7" },
            new Client { Id = 8, Name = "Klient H", ContactEmail = "h@example.com", Address = "st. test 8" },
            new Client { Id = 9, Name = "Klient I", ContactEmail = "i@example.com", Address = "st. test 9" },
            new Client { Id = 10, Name = "Klient J", ContactEmail = "j@example.com", Address = "st. test 10" },
            new Client { Id = 11, Name = "Klient K", ContactEmail = "k@example.com", Address = "st. test 11" }
        };
    }

    public IEnumerable<Client> GetAllClients() => _clients;

    public void AddClient(Client client)
    {
        client.Id = _clients.Max(c => c.Id) + 1;
        _clients.Add(client);
    }

    public void DeleteClient(Client client)
    {
        _clients.Remove(client);
    }

    public void UpdateClient(Client client)
    {
        var index = _clients.FindIndex(c => c.Id == client.Id);
        _clients[index] = client;
    }
}