namespace FluentAas.Validation;

public interface IValidationRule
{
    IEnumerable<ValidationResult> Evaluate(IAasEnvironmentAdapter env);
}