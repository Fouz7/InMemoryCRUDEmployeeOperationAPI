using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InMemoryCRUDEmployeeOperationDhiki.Utilities;

namespace InMemoryCRUDEmployeeOperationDhiki.Models;

//EmployeeDto yang memastikan bahwa data yang dikirimkan sesuai dengan aturan yang diterapkan
public record EmployeeDto(
    [Required] [StringLength(10)] string EmployeeId,
    [Required] [StringLength(50)] string FullName,
    [Required] [property: JsonConverter(typeof(DateFormatConverter))] DateTime BirthDate
);