namespace FluentAAS.Templates;

/// <summary>
/// Contract for root builders that can collect submodels.
/// Adjust to match your actual root builder abstraction.
/// </summary>
public interface ISubmodelCollector
{
    void AddSubmodel(Submodel submodel);
}