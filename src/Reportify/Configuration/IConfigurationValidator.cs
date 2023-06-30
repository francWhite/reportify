namespace Reportify.Configuration;

internal interface IConfigurationValidator
{
  Task ValidateAsync();
}