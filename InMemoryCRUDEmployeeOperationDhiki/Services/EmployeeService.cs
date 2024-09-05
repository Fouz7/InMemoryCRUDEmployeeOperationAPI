using InMemoryCRUDEmployeeOperationDhiki.Data;
using InMemoryCRUDEmployeeOperationDhiki.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InMemoryCRUDEmployeeOperationDhiki.Services;

public class EmployeeService(DataContext context) : IEmployeeService
{
    //GetAllEmployeesAsync dengan menerapkan pagination
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(int pageNumber, int pageSize)
    {
        return await context.Employees
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    //GetEmployeeByIdAsync mendapatkan data employee berdasarkan employeeId
    public async Task<Employee> GetEmployeeByIdAsync(string employeeId)
    {
        return await context.Employees.FindAsync(employeeId);
    }

    //AddEmployeeAsync menambahkan data employee
    public async Task<Employee> AddEmployeeAsync(Employee employee)
    {
        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    //UpdateEmployeeAsync mengupdate data employee
    public async Task<Employee> UpdateEmployeeAsync(Employee employee)
    {
        context.Employees.Update(employee);
        await context.SaveChangesAsync();
        return employee;
    }

    //DeleteEmployeeAsync menghapus data employee
    public async Task<bool> DeleteEmployeeAsync(string employeeId)
    {
        var employee = await context.Employees.FindAsync(employeeId);
        if (employee == null)
        {
            return false;
        }

        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
        return true;
    }
}