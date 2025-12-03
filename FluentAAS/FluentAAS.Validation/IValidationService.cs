namespace FluentAas.Validation;

public interface IValidationService
{
    ValidationReport Validate(IAasEnvironmentAdapter environment);
}