using EmployeeApi.Data;
using EmployeeApi.Dto;
using EmployeeApi.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;
        public EmployeeService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto emp, CancellationToken ct)
        {
            var entry = Employee.Create(emp.Name, emp.Salary);

            _db.Employees.Add(entry);
            await _db.SaveChangesAsync(ct);
            return new EmployeeDto(entry.Id, entry.Name, entry.Salary);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var existing = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (existing is null) return false;

            _db.Employees.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public Task<List<EmployeeDto>> GetAllAsync(CancellationToken ct)
        {
            var empList = _db.Employees.AsNoTracking()
                .Select(e => new EmployeeDto(e.Id, e.Name, e.Salary))
                .ToListAsync(ct);

            return empList;
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var emp = await _db.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, ct);

            return emp is null
                ? null
                : new EmployeeDto(emp.Id, emp.Name, emp.Salary);
        }

        public async Task<bool> UpdateAsync(int id, EmployeeDto employee, CancellationToken ct)
        {
            var existing = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
            if (existing is null) return false;

            existing.Update(employee.Name, employee.Salary);
            await _db.SaveChangesAsync(ct);
            return true;
        }
        
    }
}
