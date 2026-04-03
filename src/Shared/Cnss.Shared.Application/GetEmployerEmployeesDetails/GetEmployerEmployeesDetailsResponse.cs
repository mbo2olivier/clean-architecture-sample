namespace Cnss.Shared.Application.GetEmployerEmployeesDetails;

public sealed record GetEmployerEmployeesDetailsResponse(
    string EmployerIdentifier,
    IReadOnlyCollection<EmployerEmployeeDetailsResponse> Employees);

public sealed record EmployerEmployeeDetailsResponse(
    string EmployeeIdentifier,
    string RegistrationNumber,
    string FirstName,
    string LastName);
