using Microsoft.EntityFrameworkCore;
using TicketApi.Data;
using TicketApi.Models;

namespace TicketApi.Repositories;

public class EfTicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public EfTicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Ticket> GetAll()
    {
        return _context.Tickets
            .AsNoTracking()
            .OrderBy(t => t.Id)
            .ToList();
    }

    public Ticket? GetById(int id)
    {
        return _context.Tickets
            .AsNoTracking()
            .FirstOrDefault(t => t.Id == id);
    }

    public Ticket Add(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        _context.SaveChanges();
        return ticket;
    }

    public bool Update(Ticket ticket)
    {
        var exists = _context.Tickets.Any(t => t.Id == ticket.Id);
        if (!exists)
        {
            return false;
        }

        _context.Tickets.Update(ticket);
        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var deleted = _context.Tickets
            .Where(t => t.Id == id)
            .ExecuteDelete();

        return deleted > 0;
    }
}