using Cnss.Affiliation.Domain.Aggregats;
using Cnss.Affiliation.Domain.Repositories;
using Cnss.Affiliation.Domain.Services;
using FluentValidation;
using MDiator;

namespace Cnss.Affiliation.Application.AffiliateEmployer;

public sealed class AffiliateEmployerHandler : IMDiatorHandler<AffiliateEmployerRequest, AffiliateEmployerResponse>
{
    private readonly IAffiliationRepository _affiliationRepository;
    private readonly IdentifierGenerator _identifierGenerator;
    private readonly IValidator<AffiliateEmployerRequest> _validator;

    public AffiliateEmployerHandler(
        IAffiliationRepository affiliationRepository,
        IdentifierGenerator identifierGenerator,
        IValidator<AffiliateEmployerRequest> validator)
    {
        _affiliationRepository = affiliationRepository;
        _identifierGenerator = identifierGenerator;
        _validator = validator;
    }

    public async Task<AffiliateEmployerResponse> Handle(
        AffiliateEmployerRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var employer = Employer.Affiliate(
            _identifierGenerator.GenerateEmployerIdentifier(),
            request.RegistrationNumber,
            request.CompanyName);

        await _affiliationRepository.AddEmployerAsync(employer, cancellationToken);

        return new AffiliateEmployerResponse(
            employer.Identifier,
            employer.RegistrationNumber,
            employer.CompanyName);
    }
}
