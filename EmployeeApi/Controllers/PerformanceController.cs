using EmployeeApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformanceController : ControllerBase
    {
        private readonly IDbContextFactory<AppDbContext> _factory;
        public PerformanceController(IDbContextFactory<AppDbContext> factory) => _factory = factory;

        private async Task<int> CountAsync(CancellationToken ct)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var count = await db.Employees.CountAsync(ct);
            await Task.Delay(300, ct);
            return count;
        }

        private async Task<decimal> AverageSalaryAsync(CancellationToken ct)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            var salaries = await db.Employees.Select(e => e.Salary).ToListAsync(ct);
            await Task.Delay(300, ct);
            return salaries.Count == 0 ? 0 : salaries.Average();
        }

        [HttpGet("compare")]
        public async Task<IActionResult> Compare(CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            var c1 = await CountAsync(ct);
            var a1 = await AverageSalaryAsync(ct);
            sw.Stop();
            var sequentialMs = sw.ElapsedMilliseconds;

            sw.Restart();
            var t1 = CountAsync(ct);
            var t2 = AverageSalaryAsync(ct);
            await Task.WhenAll(t1, t2);
            sw.Stop();
            var parallelMs = sw.ElapsedMilliseconds;

            return Ok(new { sequentialMs, parallelMs, savedMs = sequentialMs - parallelMs });
        }
    }
}
