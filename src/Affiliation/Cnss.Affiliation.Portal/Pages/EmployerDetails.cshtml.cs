using System.ComponentModel.DataAnnotations;
using Cnss.Shared.Application.GetEmployerDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MDiator;

namespace Cnss.Affiliation.Portal.Pages;

public sealed class EmployerDetailsModel : PageModel
{
    private readonly IMediator _mediator;

    public EmployerDetailsModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty(SupportsGet = true)]
    public EmployerDetailsInputModel Input { get; set; } = new();

    public GetEmployerDetailsResponse? Employer { get; private set; }

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!Request.Query.ContainsKey($"{nameof(Input)}.{nameof(Input.EmployerIdentifier)}"))
        {
            return Page();
        }

        try
        {
            Employer = await _mediator.Send(
                new GetEmployerDetailsRequest(Input.EmployerIdentifier),
                cancellationToken);
        }
        catch (FluentValidation.ValidationException exception)
        {
            ModelState.AddFluentValidationErrors(exception, nameof(Input));
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }

        return Page();
    }

    public sealed class EmployerDetailsInputModel
    {
        [Required]
        [Display(Name = "Identifiant employeur")]
        public string EmployerIdentifier { get; set; } = string.Empty;
    }
}
