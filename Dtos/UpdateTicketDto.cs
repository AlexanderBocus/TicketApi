using System.ComponentModel.DataAnnotations;
using TicketApi.Models;

namespace TicketApi.Dtos;

public class UpdateTicketDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [EnumDataType(typeof(TicketStatus))]
    public TicketStatus Status { get; set; }

    [EnumDataType(typeof(TicketPriority))]
    public TicketPriority Priority { get; set; }
}