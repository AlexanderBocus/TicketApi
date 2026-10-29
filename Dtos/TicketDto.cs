using TicketApi.Models;

namespace TicketApi.Dtos;

public record TicketDto
(
    int Id,
    string Title,
    string? Description,
    TicketStatus Status,
    TicketPriority Priority,
    DateTime CreatedAt
);