using Cnss.Cotisation.Application.SubmitDeclaration;
using Microsoft.AspNetCore.Mvc;
using MDiator;

namespace Cnss.Cotisation.Api.Controllers;

[ApiController]
[Route("api/declarations")]
public sealed class DeclarationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeclarationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubmitDeclarationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromBody] SubmitDeclarationHttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _mediator.Send(
                new SubmitDeclarationRequest(
                    request.EmployerIdentifier,
                    request.Year,
                    request.Month,
                    request.Items
                        .Select(item => new SubmitDeclarationItemRequest(item.EmployeeIdentifier, item.GrossSalary))
                        .ToArray()),
                cancellationToken);

            return Ok(response);
        }
        catch (Exception exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}

public sealed record SubmitDeclarationHttpRequest(
    string EmployerIdentifier,
    int Year,
    int Month,
    IReadOnlyCollection<SubmitDeclarationHttpItemRequest> Items);

public sealed record SubmitDeclarationHttpItemRequest(
    string EmployeeIdentifier,
    decimal GrossSalary);
