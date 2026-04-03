using Cnss.Cotisation.Domain.Factories;
using Cnss.Cotisation.Domain.Repositories;
using Cnss.Cotisation.Domain.ValuesObject;
using FluentValidation;
using MDiator;

namespace Cnss.Cotisation.Application.SubmitDeclaration;

public sealed class SubmitDeclarationHandler : IMDiatorHandler<SubmitDeclarationRequest, SubmitDeclarationResponse>
{
    private readonly IDeclarationRepository _declarationRepository;
    private readonly DeclarationFactory _declarationFactory;
    private readonly IValidator<SubmitDeclarationRequest> _validator;

    public SubmitDeclarationHandler(
        IDeclarationRepository declarationRepository,
        DeclarationFactory declarationFactory,
        IValidator<SubmitDeclarationRequest> validator)
    {
        _declarationRepository = declarationRepository;
        _declarationFactory = declarationFactory;
        _validator = validator;
    }

    public async Task<SubmitDeclarationResponse> Handle(
        SubmitDeclarationRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var data = new DeclarationData(
            request.EmployerIdentifier,
            request.Year,
            request.Month,
            request.Items
                .Select(item => new DeclarationItemData(item.EmployeeIdentifier, item.GrossSalary))
                .ToArray());

        var declaration = _declarationFactory.Create(data);
        declaration.Publish();

        await _declarationRepository.AddAsync(declaration, cancellationToken);

        return new SubmitDeclarationResponse(
            declaration.Identifier,
            declaration.EmployerIdentifier.Value,
            declaration.Period.Year,
            declaration.Period.Month,
            declaration.Items.Count,
            declaration.TotalAmount);
    }
}
