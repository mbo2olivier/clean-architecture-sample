using MDiator;

namespace Cnss.Shared.Application.GetEmployerDetails;

public sealed record GetEmployerDetailsRequest(
    string EmployerIdentifier) : IMDiatorRequest<GetEmployerDetailsResponse>;
