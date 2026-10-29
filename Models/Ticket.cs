namespace TicketApi.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}