using InMemoryCRUDEmployeeOperationDhiki.Models;
using InMemoryCRUDEmployeeOperationDhiki.Services;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryCRUDEmployeeOperationDhiki.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    
    // GET: api/Employee dengan memberi parameter pageNumber dan pageSize untuk pagination
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees(int? pageNumber, int pageSize = 5)
    {
        try
        {
            pageNumber ??= 1;
            pageSize = (pageSize == 5 || pageSize == 10 || pageSize == 15) ? pageSize : 5; //jika input pageSize tidak sesuai, maka default pageSize = 5

            var employees = await employeeService.GetAllEmployeesAsync(pageNumber.Value, pageSize);
            if (!employees?.Any() ?? true)
            {
                return NotFound("No employees found.");
            }

            var employeeDtos = employees.Select(e => new EmployeeDto(e.EmployeeId, e.FullName, e.BirthDate));
            return Ok(employeeDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    
    // GET: api/Employee/{id} dengan memberi parameter id
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeById(string id)
    {
        try
        {
            var employee = await employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            var employeeDto = new EmployeeDto(employee.EmployeeId, employee.FullName, employee.BirthDate);
            return Ok(employeeDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    
    // POST: api/Employee dengan memberi parameter employeeDto
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> AddEmployee([FromBody] EmployeeDto employeeDto)
    {
        try
        {
            var employee = new Employee(employeeDto.EmployeeId, employeeDto.FullName, employeeDto.BirthDate);
            var createdEmployee = await employeeService.AddEmployeeAsync(employee);
            var createdEmployeeDto = new EmployeeDto(createdEmployee.EmployeeId, createdEmployee.FullName,
                createdEmployee.BirthDate);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployeeDto.EmployeeId },
                createdEmployeeDto);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (FormatException)
        {
            return BadRequest("Invalid date format. Please use 'dd-MMM-yyyy'.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    
    // PUT: api/Employee/{id} dengan memberi parameter id dan employeeDto
    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(string id, [FromBody] EmployeeDto employeeDto)
    {
        try
        {
            if (id != employeeDto.EmployeeId)
            {
                throw new ArgumentException("Employee ID mismatch.");
            }

            var employee = new Employee(employeeDto.EmployeeId, employeeDto.FullName, employeeDto.BirthDate);
            var updatedEmployee = await employeeService.UpdateEmployeeAsync(employee);
            var updatedEmployeeDto = new EmployeeDto(updatedEmployee.EmployeeId, updatedEmployee.FullName,
                updatedEmployee.BirthDate);
            return Ok(updatedEmployeeDto);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (FormatException)
        {
            return BadRequest("Invalid date format. Please use 'dd-MMM-yyyy'.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    
    // DELETE: api/Employee/{id} dengan memberi parameter id
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee(string id)
    {
        try
        {
            var result = await employeeService.DeleteEmployeeAsync(id);
            if (!result)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}