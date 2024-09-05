using InMemoryCRUDEmployeeOperationDhiki.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InMemoryCRUDEmployeeOperationDhiki.Services;

//Interface Operation untuk memastikan bahwa EmployeeService akan mengimplementasikan method-method berikut
public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync(int pageNumber, int pageSize);
    Task<Employee> GetEmployeeByIdAsync(string employeeId);
    Task<Employee> AddEmployeeAsync(Employee employee);
    Task<Employee> UpdateEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(string employeeId);
}