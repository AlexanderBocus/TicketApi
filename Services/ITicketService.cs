using TicketApi.Dtos;

namespace TicketApi.Services;

public interface ITicketService
{
    IEnumerable<TicketDto> GetAll();
    TicketDto? GetById(int id);
    TicketDto Create(CreateTicketDto dto);
    TicketDto? Update(int id, UpdateTicketDto dto);
    bool Delete(int id);
}