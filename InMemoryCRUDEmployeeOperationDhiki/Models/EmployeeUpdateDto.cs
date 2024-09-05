using System.Text.Json.Serialization;
using InMemoryCRUDEmployeeOperationDhiki.Utilities;
using FluentValidation;

namespace InMemoryCRUDEmployeeOperationDhiki.Models;

// EmployeeUpdateDto yang memastikan bahwa data yang dikirimkan sesuai dengan aturan yang diterapkan
public record EmployeeUpdateDto(
    string FullName,
    [property: JsonConverter(typeof(DateFormatConverter))] DateTime BirthDate
);

// Validasi data yang dikirimkan
public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
{
    public EmployeeUpdateDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required.")
            .Length(1, 50).WithMessage("FullName must be between 1 and 50 characters.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("BirthDate is required.")
            .Must(date => IsValidDateFormat(date.ToString("dd-MMM-yyyy"))).WithMessage("Invalid date format. Please use 'dd-MMM-yyyy'.");
    }

    private bool IsValidDateFormat(string date)
    {
        return DateTime.TryParseExact(date, "dd-MMM-yyyy", null, System.Globalization.DateTimeStyles.None, out _);
    }
}