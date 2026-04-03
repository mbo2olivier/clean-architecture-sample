using Cnss.Affiliation.Domain.Repositories;
using FluentValidation;
using MDiator;

namespace Cnss.Shared.Application.GetEmployerDetails;

public sealed class GetEmployerDetailsHandler : IMDiatorHandler<GetEmployerDetailsRequest, GetEmployerDetailsResponse>
{
    private readonly IAffiliationRepository _affiliationRepository;
    private readonly IValidator<GetEmployerDetailsRequest> _validator;

    public GetEmployerDetailsHandler(
        IAffiliationRepository affiliationRepository,
        IValidator<GetEmployerDetailsRequest> validator)
    {
        _affiliationRepository = affiliationRepository;
        _validator = validator;
    }

    public async Task<GetEmployerDetailsResponse> Handle(
        GetEmployerDetailsRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var employer = await _affiliationRepository.GetEmployerAsync(request.EmployerIdentifier, cancellationToken);
        if (employer is null)
        {
            throw new KeyNotFoundException($"Aucun employeur trouvé avec l'identifiant '{request.EmployerIdentifier}'.");
        }

        return new GetEmployerDetailsResponse(
            employer.Identifier,
            employer.RegistrationNumber,
            employer.CompanyName,
            employer.EmployeeIdentifiers.ToArray());
    }
}
