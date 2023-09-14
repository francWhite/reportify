namespace Reportify.Configuration.Validation;

internal interface IValidationErrorWriter
{
  void Write(IReadOnlyList<ValidationError> validationErrors);
}