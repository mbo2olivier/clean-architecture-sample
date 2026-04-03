using Cnss.Affiliation.Domain.Aggregats;
using Cnss.Affiliation.Domain.Repositories;
using Cnss.Affiliation.Domain.Services;
using FluentValidation;
using MDiator;

namespace Cnss.Affiliation.Application.AttachEmployeeToEmployer;

public sealed class AttachEmployeeToEmployerHandler : IMDiatorHandler<AttachEmployeeToEmployerRequest, AttachEmployeeToEmployerResponse>
{
    private readonly IAffiliationRepository _affiliationRepository;
    private readonly IdentifierGenerator _identifierGenerator;
    private readonly IValidator<AttachEmployeeToEmployerRequest> _validator;

    public AttachEmployeeToEmployerHandler(
        IAffiliationRepository affiliationRepository,
        IdentifierGenerator identifierGenerator,
        IValidator<AttachEmployeeToEmployerRequest> validator)
    {
        _affiliationRepository = affiliationRepository;
        _identifierGenerator = identifierGenerator;
        _validator = validator;
    }

    public async Task<AttachEmployeeToEmployerResponse> Handle(
        AttachEmployeeToEmployerRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var employer = await _affiliationRepository.GetEmployerAsync(request.EmployerIdentifier, cancellationToken);
        if (employer is null)
        {
            throw new KeyNotFoundException($"Aucun employeur trouvé avec l'identifiant '{request.EmployerIdentifier}'.");
        }

        var employee = Employee.Create(
            _identifierGenerator.GenerateEmployeeIdentifier(),
            request.EmployeeRegistrationNumber,
            request.FirstName,
            request.LastName);

        employer.AttachEmployee(employee);

        await _affiliationRepository.AddEmployeeAsync(employee, cancellationToken);
        await _affiliationRepository.UpdateEmployerAsync(employer, cancellationToken);

        return new AttachEmployeeToEmployerResponse(
            employer.Identifier,
            employee.Identifier,
            employee.RegistrationNumber,
            employee.FirstName,
            employee.LastName);
    }
}
