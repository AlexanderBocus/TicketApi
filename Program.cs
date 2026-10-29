using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using TicketApi.Repositories;
using TicketApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(option => {
        option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();

builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
