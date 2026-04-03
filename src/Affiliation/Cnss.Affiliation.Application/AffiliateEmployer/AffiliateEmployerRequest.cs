using MDiator;

namespace Cnss.Affiliation.Application.AffiliateEmployer;

public sealed record AffiliateEmployerRequest(
    string RegistrationNumber,
    string CompanyName) : IMDiatorRequest<AffiliateEmployerResponse>;
