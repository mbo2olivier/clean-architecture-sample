using FluentValidation;

namespace Cnss.Affiliation.Application.AttachEmployeeToEmployer;

public sealed class AttachEmployeeToEmployerRequestValidator : AbstractValidator<AttachEmployeeToEmployerRequest>
{
    public AttachEmployeeToEmployerRequestValidator()
    {
        RuleFor(x => x.EmployerIdentifier)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.EmployeeRegistrationNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
