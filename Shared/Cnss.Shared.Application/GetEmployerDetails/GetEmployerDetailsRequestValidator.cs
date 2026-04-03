using FluentValidation;

namespace Cnss.Shared.Application.GetEmployerDetails;

public sealed class GetEmployerDetailsRequestValidator : AbstractValidator<GetEmployerDetailsRequest>
{
    public GetEmployerDetailsRequestValidator()
    {
        RuleFor(x => x.EmployerIdentifier)
            .NotEmpty()
            .MaximumLength(50);
    }
}
