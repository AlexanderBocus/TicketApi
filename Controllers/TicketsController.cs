using Microsoft.AspNetCore.Mvc;
using TicketApi.Dtos;
using TicketApi.Services;

namespace TicketApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketsController(ITicketService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TicketDto>> GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<TicketDto> GetById(int id)
    {
        var ticket = _service.GetById(id);
        if (ticket is null)
        {
            return NotFound();
        }

        return Ok(ticket);
    }

    [HttpPost]
    public ActionResult<TicketDto> Create(CreateTicketDto dto)
    {
        var created = _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TicketDto> Update(int id, UpdateTicketDto dto)
    {
        var updated = _service.Update(id, dto);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_service.Delete(id))
        {
            return NotFound();
        }

        return NoContent();
    }
}