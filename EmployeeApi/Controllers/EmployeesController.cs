using EmployeeApi.Dto;
using EmployeeApi.Model;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken ct)
        {
            return Ok(await _service.GetAllAsync(ct));
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDto>> Get(int id, CancellationToken ct)
        {
            var emp = await _service.GetByIdAsync(id, ct);
            return emp is null ? NotFound() : Ok(emp);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto employee, CancellationToken ct)
        {
            var created = await _service.CreateAsync(employee, ct);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, EmployeeDto employee, CancellationToken ct)
        {
            return await _service.UpdateAsync(id, employee, ct) ? NoContent() : NotFound();
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
           return  await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();

        }
    }
}
