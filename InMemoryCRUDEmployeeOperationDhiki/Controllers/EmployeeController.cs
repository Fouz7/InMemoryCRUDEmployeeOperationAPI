using FluentValidation;
using FluentValidation.Results;
using InMemoryCRUDEmployeeOperationDhiki.Models;
using InMemoryCRUDEmployeeOperationDhiki.Services;
using Microsoft.AspNetCore.Mvc;

namespace InMemoryCRUDEmployeeOperationDhiki.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IValidator<EmployeeDto> _validator;

    public EmployeeController(IEmployeeService employeeService, IValidator<EmployeeDto> validator)
    {
        _employeeService = employeeService;
        _validator = validator;
    }

    // GET: api/Employee dengan memberi parameter pageNumber dan pageSize untuk pagination
    [HttpGet]
    public async Task<ActionResult> GetAllEmployees(int? pageNumber, int pageSize = 5)
    {
        try
        {
            pageNumber ??= 1;
            pageSize = (pageSize == 5 || pageSize == 10 || pageSize == 15)
                ? pageSize
                : 5; //jika input pageSize tidak sesuai, maka default pageSize = 5

            var employees = await _employeeService.GetAllEmployeesAsync(pageNumber.Value, pageSize);
            if (!employees?.Any() ?? true)
            {
                return NotFound(new { status = 404, message = "No employees found." });
            }

            var employeeDtos = employees.Select(e => new EmployeeDto(e.EmployeeId, e.FullName, e.BirthDate));

            var response = new
            {
                data = new
                {
                    employees = employeeDtos,
                },
                status = 200,
                pageNumber = pageNumber,
                pageSize = pageSize
            };

            return Ok(response);
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
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { status = 404, message = $"Employee with ID {id} not found." });
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
    public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto employeeDto)
    {
        ValidationResult results = _validator.Validate(employeeDto);

        if (!results.IsValid)
        {
            var errors = results.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return BadRequest(new { status = 400, errors });
        }

        try
        {
            // Cek jika employee dengan ID yang sama sudah ada
            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(employeeDto.EmployeeId);
            if (existingEmployee != null)
            {
                return Conflict(new { status = 409, message = $"Employee with {employeeDto.EmployeeId} ID already exists." });
            }

            var employee = new Employee(employeeDto.EmployeeId, employeeDto.FullName, employeeDto.BirthDate);
            var createdEmployee = await _employeeService.AddEmployeeAsync(employee);
            var createdEmployeeDto = new EmployeeDto(createdEmployee.EmployeeId, createdEmployee.FullName,
                createdEmployee.BirthDate);

            var response = new
            {
                data = new
                {
                    employeeid = createdEmployeeDto.EmployeeId,
                    fullname = createdEmployeeDto.FullName,
                    birthDate = createdEmployeeDto.BirthDate.ToString("dd-MMM-yyyy")
                },
                status = 200,
                message = "You have add employee successfully"
            };

            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployeeDto.EmployeeId }, response);
        }
        catch (Exception ex)
        {
            var response = new
            {
                status = 500,
                message = $"Internal server error: {ex.Message}"
            };
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    // PUT: api/Employee/{id} dengan memberi parameter id dan employeeDto
    [HttpPut("{id}")]
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(string id, [FromBody] EmployeeDto employeeDto)
    {
        ValidationResult results = _validator.Validate(employeeDto);

        if (!results.IsValid)
        {
            var errors = results.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return BadRequest(new { status = 400, errors });
        }

        try
        {
            if (id != employeeDto.EmployeeId)
            {
                throw new ArgumentException("Employee ID mismatch.");
            }

            var employee = new Employee(employeeDto.EmployeeId, employeeDto.FullName, employeeDto.BirthDate);
            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(employee);
            var updatedEmployeeDto = new EmployeeDto(updatedEmployee.EmployeeId, updatedEmployee.FullName,
                updatedEmployee.BirthDate);

            var response = new
            {
                data = new
                {
                    employeeid = updatedEmployeeDto.EmployeeId,
                    fullname = updatedEmployeeDto.FullName,
                    birthDate = updatedEmployeeDto.BirthDate.ToString("dd-MMM-yyyy")
                },
                status = 200,
                message = "You have edited employee successfully"
            };

            return Ok(response);
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
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result)
            {
                return NotFound(new { status = 404, message = $"Employee with ID {id} not found." });
            }

            return Ok(new { status = 200, message = "Employee deleted successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}