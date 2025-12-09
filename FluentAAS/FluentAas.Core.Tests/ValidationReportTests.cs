using AutoFixture;
using Shouldly;

namespace FluentAas.Core.Tests;

/// <summary>
/// Unit tests for <see cref="ValidationReport"/>.
/// </summary>
public sealed class ValidationReportTests
{
    private readonly Fixture _fixture = new();

    private ValidationResult CreateResult(ValidationLevel level)
    {
        var result = _fixture.Create<ValidationResult>();
        result = result! with {Level = level};
        return result;
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenResultsIsNull()
    {
        // Arrange
        IEnumerable<ValidationResult>? results = null;

        // Act
        var ex = Record.Exception(() => new ValidationReport(results!));

        // Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    public void HasErrors_ShouldBeFalse_WhenNoResults()
    {
        // Arrange
        var emptyResults = Enumerable.Empty<ValidationResult>();

        // Act
        var report = new ValidationReport(emptyResults);

        // Assert
        report.HasErrors.ShouldBeFalse();
    }

    [Fact]
    public void HasErrors_ShouldBeFalse_WhenNoErrorLevelResults()
    {
        // Arrange
        var results = new List<ValidationResult>
        {
            CreateResult(ValidationLevel.Warning),
            CreateResult(ValidationLevel.Info)
        };

        // Act
        var report = new ValidationReport(results);

        // Assert
        report.HasErrors.ShouldBeFalse();
    }

    [Fact]
    public void HasErrors_ShouldBeTrue_WhenAtLeastOneErrorResultExists()
    {
        // Arrange
        var results = new List<ValidationResult>
        {
            CreateResult(ValidationLevel.Warning),
            CreateResult(ValidationLevel.Error),
            CreateResult(ValidationLevel.Warning)
        };

        // Act
        var report = new ValidationReport(results);

        // Assert
        report.HasErrors.ShouldBeTrue();
    }

    [Fact]
    public void HasErrors_ShouldNotChange_WhenOriginalCollectionIsModifiedAfterConstruction()
    {
        // Arrange
        var results = new List<ValidationResult>
        {
            CreateResult(ValidationLevel.Warning)
        };

        var report = new ValidationReport(results);

        // Act
        results.Add(CreateResult(ValidationLevel.Error));

        // Assert
        report.HasErrors.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_ShouldMaterializeResultsOnce()
    {
        // Arrange
        // Enumerable, das nur einmal enumeriert werden sollte
        var enumerationCount = 0;

        // Act
        var report = new ValidationReport(Deferred());

        // Assert
        enumerationCount.ShouldBe(1);
        report.HasErrors.ShouldBeFalse();
        return;

        IEnumerable<ValidationResult> Deferred()
        {
            enumerationCount++;
            yield return CreateResult(ValidationLevel.Warning);
            yield return CreateResult(ValidationLevel.Warning);
        }
    }

    [Fact]
    public void ValidationReport_ShouldBeImmutableFromTheOutside()
    {
        // Arrange
        var results = new List<ValidationResult>
        {
            CreateResult(ValidationLevel.Error)
        };

        var report = new ValidationReport(results);

        // Act
        results.Clear();

        // Assert
        report.HasErrors.ShouldBeTrue();
    }
}
