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
    private readonly IValidator<EmployeeUpdateDto> _employeeUpdateDtoValidator;

    public EmployeeController(IEmployeeService employeeService, IValidator<EmployeeDto> validator, IValidator<EmployeeUpdateDto> employeeUpdateDtoValidator)
    {
        _employeeService = employeeService;
        _validator = validator;
        _employeeUpdateDtoValidator = employeeUpdateDtoValidator;
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
                throw new Exception("No employees found.");
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
            if (ex.Message.Contains("No employees found"))
            {
                return NotFound(new { status = 404, message = ex.Message });
            }

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
                throw new Exception($"Employee with ID {id} not found.");
            }

            var employeeDto = new EmployeeDto(employee.EmployeeId, employee.FullName, employee.BirthDate);
            return Ok(employeeDto);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound(new { status = 404, message = ex.Message });
            }

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // POST: api/Employee dengan memberi parameter employeeDto
    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] EmployeeDto employeeDto)
    {
        try
        {
            _validator.ValidateAndThrow(employeeDto);

            // Cek jika employee dengan ID yang sama sudah ada
            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(employeeDto.EmployeeId);
            if (existingEmployee != null)
            {
                throw new Exception($"Employee with {employeeDto.EmployeeId} ID already exists.");
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
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return BadRequest(new { status = 400, errors });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("already exists"))
            {
                return Conflict(new { status = 409, message = ex.Message });
            }

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
    public async Task<ActionResult<EmployeeDto>> UpdateEmployee(string id,
        [FromBody] EmployeeUpdateDto updateEmployeeDto)
    {
        try
        {
            ValidationResult results = _employeeUpdateDtoValidator.Validate(updateEmployeeDto);

            if (!results.IsValid)
            {
                var errors = results.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new { status = 400, errors });
            }

            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
            {
                throw new Exception($"Employee with ID {id} not found.");
            }

            existingEmployee.FullName = updateEmployeeDto.FullName;
            existingEmployee.BirthDate = updateEmployeeDto.BirthDate;

            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(existingEmployee);

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
        catch (ArgumentException ex)
        {
            return BadRequest(new { status = 400, message = ex.Message });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound(new { status = 404, message = ex.Message });
            }

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
                throw new Exception($"Employee with ID {id} not found.");
            }

            return Ok(new { status = 200, message = "Employee deleted successfully." });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
            {
                return NotFound(new { status = 404, message = ex.Message });
            }

            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}