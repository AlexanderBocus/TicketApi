using TicketApi.Models;

namespace TicketApi.Repositories;

public interface ITicketRepository
{
    IEnumerable<Ticket> GetAll();
    Ticket? GetById(int id);
    Ticket Add(Ticket ticket);
    bool Update(Ticket ticket);
    bool Delete(int id);
}