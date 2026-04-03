using FluentValidation;

namespace Cnss.Cotisation.Application.SubmitDeclaration;

public sealed class SubmitDeclarationRequestValidator : AbstractValidator<SubmitDeclarationRequest>
{
    public SubmitDeclarationRequestValidator()
    {
        RuleFor(x => x.EmployerIdentifier)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(2000);

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);

        RuleFor(x => x.Items)
            .NotNull()
            .Must(items => items.Count > 0)
            .WithMessage("Une déclaration doit contenir au moins un élément.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.EmployeeIdentifier)
                    .NotEmpty()
                    .MaximumLength(50);

                item.RuleFor(x => x.GrossSalary)
                    .GreaterThan(0);
            });
    }
}
