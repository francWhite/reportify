namespace Reportify.Configuration.Validation;

internal interface IConfigurationValidator
{
  Task<IReadOnlyList<ValidationError>> ValidateAsync();
}