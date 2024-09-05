using System;
using System.ComponentModel.DataAnnotations;

namespace InMemoryCRUDEmployeeOperationDhiki.Models;

//Model Employee
public class Employee(string employeeId, string fullName, DateTime birthDate)
{
    [StringLength(10)]
    public string EmployeeId { get; set; } = employeeId;

    [StringLength(50)]
    public string FullName { get; set; } = fullName;

    public DateTime BirthDate { get; set; } = birthDate;
}