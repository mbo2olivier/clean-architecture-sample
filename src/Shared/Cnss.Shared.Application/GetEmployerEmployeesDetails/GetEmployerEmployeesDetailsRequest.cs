using MDiator;

namespace Cnss.Shared.Application.GetEmployerEmployeesDetails;

public sealed record GetEmployerEmployeesDetailsRequest(
    string EmployerIdentifier,
    IReadOnlyCollection<string> EmployeeIdentifiers) : IMDiatorRequest<GetEmployerEmployeesDetailsResponse>;
