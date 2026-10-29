# TicketApi

API REST per la gestione di ticket di assistenza, scritta in C# con ASP.NET Core.

È un progetto didattico che mostra, in piccolo, come si struttura un'API a livelli e come si usa la **Dependency Injection**. I dati sono salvati in un database tramite **Entity Framework Core**: SQLite per impostazione predefinita, sostituibile con altri database cambiando poche righe.

## Funzionalità

- CRUD completo sui ticket: creazione, lettura, modifica, cancellazione
- Validazione dei dati in ingresso con DTO e DataAnnotations
- Codici di stato HTTP corretti (200, 201, 204, 400, 404) ed errori nel formato standard *ProblemDetails*
- Database creato e aggiornato automaticamente all'avvio (migrazioni EF Core)
- Documentazione OpenAPI e interfaccia per provare gli endpoint dal browser (Scalar)

## Tecnologie

| Area | Scelta |
|---|---|
| Linguaggio e framework | C#, ASP.NET Core con controller, target `net11.0` |
| Accesso ai dati | Entity Framework Core 10 con SQLite |
| Documentazione API | OpenAPI integrato + Scalar |

## Requisiti

- [.NET SDK 11](https://dotnet.microsoft.com/download/dotnet/11.0): il progetto ha come target `net11.0`, che al momento della scrittura è in versione di anteprima
- Git

Non serve installare nessun database: con SQLite l'app crea da sola il file `tickets.db`.

## Avvio rapido

```bash
git clone https://github.com/AlexanderBocus/TicketApi.git
cd TicketApi
dotnet run
```

All'avvio l'app applica le migrazioni e crea il database. Nella console compare la riga `Now listening on: http://localhost:5010`.

- Interfaccia per provare l'API: <http://localhost:5010/scalar>
- Documento OpenAPI: <http://localhost:5010/openapi/v1.json>
- Elenco dei ticket: <http://localhost:5010/api/tickets>

L'interfaccia Scalar e il documento OpenAPI sono attivi solo in ambiente `Development`, che è quello usato da `dotnet run`.

## Endpoint

| Metodo | URL | Descrizione | Risposte |
|---|---|---|---|
| GET | `/api/tickets` | Elenco dei ticket | 200 |
| GET | `/api/tickets/{id}` | Dettaglio di un ticket | 200, 404 |
| POST | `/api/tickets` | Crea un ticket | 201, 400 |
| PUT | `/api/tickets/{id}` | Modifica un ticket (sostituzione completa) | 200, 400, 404 |
| DELETE | `/api/tickets/{id}` | Elimina un ticket | 204, 404 |

### Esempi

Creazione di un ticket (bash):

```bash
curl -X POST http://localhost:5010/api/tickets \
  -H "Content-Type: application/json" \
  -d '{"title": "Stampante guasta", "description": "Non stampa dal piano 2", "priority": "High"}'
```

Lo stesso in PowerShell:

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:5010/api/tickets `
  -ContentType "application/json" `
  -Body '{"title": "Stampante guasta", "description": "Non stampa dal piano 2", "priority": "High"}'
```

Risposta (`201 Created`, con l'indirizzo del nuovo ticket nell'header `Location`):

```json
{
  "id": 1,
  "title": "Stampante guasta",
  "description": "Non stampa dal piano 2",
  "status": "Open",
  "priority": "High",
  "createdAt": "2026-10-29T23:35:16.6783357Z"
}
```

Modifica di un ticket (`PUT /api/tickets/1`), con tutti i campi:

```json
{
  "title": "Stampante guasta",
  "description": "Sostituito il toner",
  "status": "InProgress",
  "priority": "Medium"
}
```

### Modello dati

| Campo | Tipo | Note |
|---|---|---|
| `id` | intero | Assegnato dal database |
| `title` | testo | Obbligatorio, da 3 a 100 caratteri |
| `description` | testo | Facoltativo, massimo 500 caratteri |
| `status` | testo | `Open` (predefinito), `InProgress`, `Closed`. Non modificabile in creazione |
| `priority` | testo | `Low`, `Medium` (predefinito), `High` |
| `createdAt` | data e ora UTC | Impostata alla creazione, non modificabile |

Se i dati non rispettano le regole, l'API risponde `400 Bad Request` con l'elenco degli errori. Se il ticket non esiste, risponde `404 Not Found`.

## Architettura

```
Controllers/   TicketsController          -> endpoint HTTP, nessuna logica
Services/      ITicketService, TicketService   -> logica applicativa, conversione DTO <-> Ticket
Repositories/  ITicketRepository          -> contratto di accesso ai dati
               EfTicketRepository         -> implementazione con EF Core (usata)
               InMemoryTicketRepository   -> implementazione con una lista (alternativa)
Data/          AppDbContext               -> mappatura delle tabelle
Migrations/    migrazioni EF Core
Dtos/          CreateTicketDto, UpdateTicketDto, TicketDto
Models/        Ticket, TicketStatus, TicketPriority
Program.cs     avvio dell'app e registrazione della Dependency Injection
```

Il flusso di una richiesta è: **Controller → Service → Repository → database**. Ogni livello conosce solo l'interfaccia del livello successivo e riceve le sue dipendenze dal costruttore. Il controller lavora solo con i DTO e non vede mai la classe `Ticket`.

### Dependency Injection

Le registrazioni sono in `Program.cs`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<ITicketRepository, EfTicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();
```

- `AddDbContext` registra il `DbContext` come *Scoped*: una istanza per richiesta HTTP.
- Il repository dipende dal `DbContext`, quindi è anch'esso *Scoped*. Registrarlo come Singleton sarebbe un errore (*captive dependency*).
- Per usare la versione in memoria basta registrare `InMemoryTicketRepository` come Singleton al posto di `EfTicketRepository`: service e controller non cambiano.

## Configurazione del database

La stringa di connessione è in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=tickets.db"
}
```

Può essere sovrascritta senza modificare il file, con una variabile d'ambiente (il doppio underscore sostituisce i due punti):

```bash
# bash
export ConnectionStrings__DefaultConnection="Data Source=altro.db"
```

```powershell
# PowerShell
$env:ConnectionStrings__DefaultConnection = "Data Source=altro.db"
```

Il file del database (`*.db`) è escluso da Git.

### Usare un altro database

Il progetto è provato con SQLite. Per un database diverso, in linea generale:

1. Installa il pacchetto del provider, con una versione compatibile con la versione di EF Core del progetto. Esempi: `Npgsql.EntityFrameworkCore.PostgreSQL` per PostgreSQL, `Microsoft.EntityFrameworkCore.SqlServer` per SQL Server.
2. In `Program.cs` sostituisci `UseSqlite(...)` con `UseNpgsql(...)` o `UseSqlServer(...)`.
3. Imposta la nuova stringa di connessione.
4. Le migrazioni esistenti sono generate per SQLite: elimina la cartella `Migrations` e creane una nuova con `dotnet ef migrations add InitialCreate`.

Non scrivere password in `appsettings.json` e non committarle. In sviluppo usa gli *user secrets*:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<stringa di connessione>"
```

In produzione usa le variabili d'ambiente.

### Migrazioni

All'avvio l'app applica da sola le migrazioni mancanti (`Database.Migrate()`). Per modificare la struttura del database, dopo aver cambiato i model:

```bash
dotnet tool install --global dotnet-ef   # una sola volta
dotnet ef migrations add NomeMigrazione
dotnet ef database update
```

## Possibili evoluzioni

- Metodi `async` in repository, service e controller
- Test unitari con xUnit, passando al service un repository finto
- Gestione globale degli errori
- Filtri e paginazione (per esempio `GET /api/tickets?status=Open`)
- Autenticazione e autorizzazione (JWT), necessaria prima di pubblicare l'API online
- `Dockerfile` e `docker-compose` con un database PostgreSQL
- Build e test automatici con GitHub Actions

## Note

Progetto realizzato a scopo didattico.
