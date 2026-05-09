using System.ComponentModel.DataAnnotations;
using Cnss.Affiliation.Application.AffiliateEmployer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MDiator;

namespace Cnss.Affiliation.Portal.Pages;

public sealed class AffiliateEmployerModel : PageModel
{
    private readonly IMediator _mediator;

    public AffiliateEmployerModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public AffiliateEmployerInputModel Input { get; set; } = new();

    public string? SuccessMessage { get; private set; }

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var response = await _mediator.Send(
                new AffiliateEmployerRequest(Input.RegistrationNumber, Input.CompanyName),
                cancellationToken);

            SuccessMessage = $"Employeur affilié avec succès. Identifiant: {response.EmployerIdentifier}.";
            Input = new AffiliateEmployerInputModel();
            ModelState.Clear();
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

    public sealed class AffiliateEmployerInputModel
    {
        [Required]
        [Display(Name = "Numéro d'immatriculation")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Raison sociale")]
        public string CompanyName { get; set; } = string.Empty;
    }
}
