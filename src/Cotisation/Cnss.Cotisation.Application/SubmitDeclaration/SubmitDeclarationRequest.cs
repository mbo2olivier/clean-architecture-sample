using MDiator;

namespace Cnss.Cotisation.Application.SubmitDeclaration;

public sealed record SubmitDeclarationRequest(
    string EmployerIdentifier,
    int Year,
    int Month,
    IReadOnlyCollection<SubmitDeclarationItemRequest> Items) : IMDiatorRequest<SubmitDeclarationResponse>;

public sealed record SubmitDeclarationItemRequest(
    string EmployeeIdentifier,
    decimal GrossSalary);
