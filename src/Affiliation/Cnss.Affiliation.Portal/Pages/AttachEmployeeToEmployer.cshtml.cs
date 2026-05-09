using System.ComponentModel.DataAnnotations;
using Cnss.Affiliation.Application.AttachEmployeeToEmployer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MDiator;

namespace Cnss.Affiliation.Portal.Pages;

public sealed class AttachEmployeeToEmployerModel : PageModel
{
    private readonly IMediator _mediator;

    public AttachEmployeeToEmployerModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public AttachEmployeeInputModel Input { get; set; } = new();

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
                new AttachEmployeeToEmployerRequest(
                    Input.EmployerIdentifier,
                    Input.EmployeeRegistrationNumber,
                    Input.FirstName,
                    Input.LastName),
                cancellationToken);

            SuccessMessage = $"Employé rattaché avec succès. Identifiant: {response.EmployeeIdentifier}.";
            Input = new AttachEmployeeInputModel();
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

    public sealed class AttachEmployeeInputModel
    {
        [Required]
        [Display(Name = "Identifiant employeur")]
        public string EmployerIdentifier { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Matricule employé")]
        public string EmployeeRegistrationNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Prénom")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nom")]
        public string LastName { get; set; } = string.Empty;
    }
}
