using FluentValidation;

namespace Cnss.Shared.Application.GetEmployerEmployeesDetails;

public sealed class GetEmployerEmployeesDetailsRequestValidator : AbstractValidator<GetEmployerEmployeesDetailsRequest>
{
    public GetEmployerEmployeesDetailsRequestValidator()
    {
        RuleFor(x => x.EmployerIdentifier)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.EmployeeIdentifiers)
            .NotNull()
            .Must(x => x.Count > 0)
            .WithMessage("Au moins un identifiant employé est requis.");

        RuleForEach(x => x.EmployeeIdentifiers)
            .NotEmpty()
            .MaximumLength(50);
    }
}
