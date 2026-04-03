using Cnss.Affiliation.Domain.Repositories;
using Cnss.Shared.Application.GetEmployerEmployeesDetails;
using FluentValidation;
using MDiator;

namespace Cnss.Affiliation.Application.GetEmployerEmployeesDetails;

public sealed class GetEmployerEmployeesDetailsHandler : IMDiatorHandler<GetEmployerEmployeesDetailsRequest, GetEmployerEmployeesDetailsResponse>
{
    private readonly IAffiliationRepository _affiliationRepository;
    private readonly IValidator<GetEmployerEmployeesDetailsRequest> _validator;

    public GetEmployerEmployeesDetailsHandler(
        IAffiliationRepository affiliationRepository,
        IValidator<GetEmployerEmployeesDetailsRequest> validator)
    {
        _affiliationRepository = affiliationRepository;
        _validator = validator;
    }

    public async Task<GetEmployerEmployeesDetailsResponse> Handle(
        GetEmployerEmployeesDetailsRequest request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var employer = await _affiliationRepository.GetEmployerAsync(request.EmployerIdentifier, cancellationToken);
        if (employer is null)
        {
            throw new KeyNotFoundException($"Aucun employeur trouvé avec l'identifiant '{request.EmployerIdentifier}'.");
        }

        var requestedIdentifiers = request.EmployeeIdentifiers
            .Select(x => x.Trim())
            .ToArray();

        var unknownEmployeeIdentifiers = requestedIdentifiers
            .Except(employer.EmployeeIdentifiers, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (unknownEmployeeIdentifiers.Length > 0)
        {
            throw new InvalidOperationException(
                $"Les employés suivants ne sont pas rattachés à l'employeur '{request.EmployerIdentifier}' : {string.Join(", ", unknownEmployeeIdentifiers)}.");
        }

        var employees = new List<EmployerEmployeeDetailsResponse>(requestedIdentifiers.Length);

        foreach (var employeeIdentifier in requestedIdentifiers)
        {
            var employee = await _affiliationRepository.GetEmployeeAsync(employeeIdentifier, cancellationToken);
            if (employee is null)
            {
                throw new KeyNotFoundException($"Aucun employé trouvé avec l'identifiant '{employeeIdentifier}'.");
            }

            employees.Add(new EmployerEmployeeDetailsResponse(
                employee.Identifier,
                employee.RegistrationNumber,
                employee.FirstName,
                employee.LastName));
        }

        return new GetEmployerEmployeesDetailsResponse(employer.Identifier, employees);
    }
}
