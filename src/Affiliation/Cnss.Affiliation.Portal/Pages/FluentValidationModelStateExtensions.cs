using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cnss.Affiliation.Portal.Pages;

internal static class FluentValidationModelStateExtensions
{
    public static void AddFluentValidationErrors(
        this ModelStateDictionary modelState,
        ValidationException exception,
        string modelPrefix)
    {
        foreach (var error in exception.Errors)
        {
            var key = string.IsNullOrWhiteSpace(error.PropertyName)
                ? string.Empty
                : $"{modelPrefix}.{error.PropertyName}";

            modelState.AddModelError(key, error.ErrorMessage);
        }
    }
}
