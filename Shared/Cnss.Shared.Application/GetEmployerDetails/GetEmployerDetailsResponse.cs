namespace Cnss.Shared.Application.GetEmployerDetails;

public sealed record GetEmployerDetailsResponse(
    string EmployerIdentifier,
    string RegistrationNumber,
    string CompanyName,
    IReadOnlyCollection<string> EmployeeIdentifiers);
