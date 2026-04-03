namespace Cnss.Affiliation.Application.AttachEmployeeToEmployer;

public sealed record AttachEmployeeToEmployerResponse(
    string EmployerIdentifier,
    string EmployeeIdentifier,
    string EmployeeRegistrationNumber,
    string FirstName,
    string LastName);
