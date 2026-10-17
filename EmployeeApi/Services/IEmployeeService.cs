using EmployeeApi.Dto;
using EmployeeApi.Model;

namespace EmployeeApi.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllAsync(CancellationToken ct);
        Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto employee, CancellationToken ct);
        Task<bool> UpdateAsync(int id, EmployeeDto employee, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);

    }
}
