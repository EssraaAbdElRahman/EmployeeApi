# Employee Management API

ASP.NET Core Web API for managing employees (CRUD) with EF Core, built to practice **async/await**, **CancellationToken** handling, and **performance measurement** (sequential vs parallel execution).

## Features

- Full async CRUD: `ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`
- Controller and Service layers use `Task` / `Task<T>` end to end (no `.Result` or `.Wait()`)
- `CancellationToken` passed from every endpoint through the service to every EF Core call
- Domain model with validation (`Employee.Create` / `Employee.Update`) and DTOs for input/output
- Performance endpoint comparing two independent operations sequentially and in parallel, measured with `Stopwatch`

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger (Swashbuckle)

## Project Structure

```
EmployeeApi/
├── Controllers/     # EmployeesController, PerformanceController
├── Services/        # IEmployeeService, EmployeeService
├── Model/           # Employee entity
├── Dtos/            # EmployeeDto
├── Data/            # AppDbContext
└── Program.cs
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full instance)

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/<your-username>/EmployeeApi.git
   cd EmployeeApi
   ```

2. Set the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Server=.;Database=EmployeeDb;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

3. Create the database:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

5. Open Swagger at `https://localhost:<port>/swagger`.

## API Endpoints

| Method | Endpoint                     | Description                              |
|--------|------------------------------|------------------------------------------|
| GET    | `/api/employees`             | Get all employees                        |
| GET    | `/api/employees/{id}`        | Get employee by id                       |
| POST   | `/api/employees`             | Create an employee                       |
| PUT    | `/api/employees/{id}`        | Update an employee                       |
| DELETE | `/api/employees/{id}`        | Delete an employee                       |
| GET    | `/api/performance/compare`   | Compare sequential vs parallel execution |

## CancellationToken

Every endpoint accepts a `CancellationToken` that ASP.NET Core links to the HTTP request. If the client closes the connection, the token is cancelled and the running EF Core query stops instead of wasting server resources.

```csharp
[HttpGet("{id:int}")]
public async Task<ActionResult<EmployeeDto>> Get(int id, CancellationToken ct)
{
    var emp = await _service.GetByIdAsync(id, ct);
    return emp is null ? NotFound() : Ok(emp);
}
```

## Performance Measurement

`GET /api/performance/compare` runs two independent operations (employee count and average salary, each with a simulated 300 ms external call):

1. **Sequentially:** one `await` after the other
2. **In parallel:** both started first, then `await Task.WhenAll(...)`

Each operation uses its own `DbContext` created from `IDbContextFactory<AppDbContext>`, because `DbContext` is not thread-safe.

### Results

> Replace these with your own measured numbers (ignore the first run, it includes warm-up).

| Run | Sequential (ms) | Parallel (ms) | Difference (ms) |
|-----|-----------------|---------------|-----------------|
| 1   |                 |               |                 |
| 2   |                 |               |                 |
| 3   |                 |               |                 |

### Conclusion

- Sequential time is roughly the **sum** of both operations; parallel time is roughly the time of the **slowest** one.
- Parallelism helped here because the operations are independent and I/O-bound (database and network waits).
- It would give little or no benefit for very fast operations, operations that depend on each other, or CPU-bound work, and it uses more database connections.

## License

This project was built for educational purposes.
