namespace Cnss.Affiliation.Application.AffiliateEmployer;

public sealed record AffiliateEmployerResponse(
    string EmployerIdentifier,
    string RegistrationNumber,
    string CompanyName);
