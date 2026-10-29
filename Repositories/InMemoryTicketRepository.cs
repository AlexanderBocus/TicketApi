using TicketApi.Models;

namespace TicketApi.Repositories;

public class InMemoryTicketRepository : ITicketRepository
{
    private readonly List<Ticket> _tickets = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public IEnumerable<Ticket> GetAll()
    {
        lock (_lock)
        {
            return _tickets.ToList();
        }
    }

    public Ticket? GetById(int id)
    {
        lock (_lock)
        {
            return _tickets.FirstOrDefault(t => t.Id == id);
        }
    }

    public Ticket Add(Ticket ticket)
    {
        lock (_lock)
        {
            ticket.Id = _nextId++;
            _tickets.Add(ticket);
            return ticket;
        }
    }

    public bool Update(Ticket ticket)
    {
        lock (_lock)
        {
            var index = _tickets.FindIndex(t => t.Id == ticket.Id);
            if (index == -1)
            {
                return false;
            }

            _tickets[index] = ticket;
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            return _tickets.RemoveAll(t => t.Id == id) > 0;
        }
    }
}