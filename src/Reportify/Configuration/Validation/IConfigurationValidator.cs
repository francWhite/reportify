namespace Reportify.Configuration.Validation;

internal interface IConfigurationValidator
{
  Task ValidateAsync();
}