using TicketApi.Dtos;
using TicketApi.Models;
using TicketApi.Repositories;

namespace TicketApi.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _repository;

    public TicketService(ITicketRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<TicketDto> GetAll()
    {
        return _repository.GetAll().Select(ToDto);
    }

    public TicketDto? GetById(int id)
    {
        var ticket = _repository.GetById(id);
        return ticket is null ? null : ToDto(ticket);
    }

    public TicketDto Create(CreateTicketDto dto)
    {
        var ticket = new Ticket
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority
        };

        var created = _repository.Add(ticket);
        return ToDto(created);
    }

    public TicketDto? Update(int id, UpdateTicketDto dto)
    {
        var existing = _repository.GetById(id);
        if (existing is null)
        {
            return null;
        }

        var updated = new Ticket
        {
            Id = id,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            CreatedAt = existing.CreatedAt
        };

        if (!_repository.Update(updated))
        {
            return null;
        }

        return ToDto(updated);
    }

    public bool Delete(int id)
    {
        return _repository.Delete(id);
    }

    private static TicketDto ToDto(Ticket ticket)
    {
        return new TicketDto
        (
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Status,
            ticket.Priority,
            ticket.CreatedAt
        );
    }
}