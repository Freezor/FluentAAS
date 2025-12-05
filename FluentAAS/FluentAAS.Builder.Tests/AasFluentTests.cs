using Shouldly;

namespace FluentAAS.Builder.Tests;

public class AasFluentTests
{
    [Fact]
    public void CreateEnvironment_ShouldReturnNewAasBuilderInstance()
    {
        // Act
        var builder1 = AasFluent.CreateEnvironment();
        var builder2 = AasFluent.CreateEnvironment();

        // Assert
        builder1.ShouldNotBeNull();
        builder2.ShouldNotBeNull();
        builder1.ShouldBeOfType<AasBuilder>();
        builder2.ShouldBeOfType<AasBuilder>();

        // Ensure it is not returning a cached singleton instance
        builder1.ShouldNotBeSameAs(builder2);
    }
}