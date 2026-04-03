namespace Cnss.Cotisation.Application.SubmitDeclaration;

public sealed record SubmitDeclarationResponse(
    string DeclarationIdentifier,
    string EmployerIdentifier,
    int Year,
    int Month,
    int ItemsCount,
    decimal TotalAmount);
