using MDiator;

namespace Cnss.Affiliation.Application.AttachEmployeeToEmployer;

public sealed record AttachEmployeeToEmployerRequest(
    string EmployerIdentifier,
    string EmployeeRegistrationNumber,
    string FirstName,
    string LastName) : IMDiatorRequest<AttachEmployeeToEmployerResponse>;
