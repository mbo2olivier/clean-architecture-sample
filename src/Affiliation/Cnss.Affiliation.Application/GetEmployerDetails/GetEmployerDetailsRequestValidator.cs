using Cnss.Shared.Application.GetEmployerDetails;
using FluentValidation;

namespace Cnss.Affiliation.Application.GetEmployerDetails;

public sealed class GetEmployerDetailsRequestValidator : AbstractValidator<GetEmployerDetailsRequest>
{
    public GetEmployerDetailsRequestValidator()
    {
        RuleFor(x => x.EmployerIdentifier)
            .NotEmpty()
            .MaximumLength(50);
    }
}
