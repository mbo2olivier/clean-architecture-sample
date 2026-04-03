using FluentValidation;

namespace Cnss.Affiliation.Application.AffiliateEmployer;

public sealed class AffiliateEmployerRequestValidator : AbstractValidator<AffiliateEmployerRequest>
{
    public AffiliateEmployerRequestValidator()
    {
        RuleFor(x => x.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(200);
    }
}
