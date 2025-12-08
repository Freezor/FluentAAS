using AasCore.Aas3_0;
using Shouldly;

namespace FluentAAS.Builder.Tests;

public class ShellBuilderTests
{
    // Helper to create a minimal valid shell and parent builder.
    // Adjusts to the full AssetAdministrationShell constructor.
    private static (AasBuilder Parent, AssetAdministrationShell Shell, ShellBuilder Builder)
        CreateBuilder()
    {
        var parent = new AasBuilder();

        // Use the full constructor signature; only Id is really important for these tests.
        var shell = new AssetAdministrationShell(
                                                 id: Guid.NewGuid().ToString(),
                                                 idShort: null,
                                                 category: null,
                                                 description: null,
                                                 administration: null,
                                                 assetInformation: new AssetInformation(AssetKind.Instance),
                                                 submodels: null,
                                                 derivedFrom: null,
                                                 extensions: null,
                                                 embeddedDataSpecifications: null
                                                );

        var builder = new ShellBuilder(parent, shell);
        return (parent, shell, builder);
    }

    [Fact]
    public void Constructor_WithNullParent_ShouldThrowArgumentNullException()
    {
        // Arrange
        AasBuilder? parent = null;
        var shell = new AssetAdministrationShell(
                                                 id: Guid.NewGuid().ToString(),
                                                 idShort: null,
                                                 category: null,
                                                 description: null,
                                                 administration: null,
                                                 assetInformation: null!,
                                                 submodels: null,
                                                 derivedFrom: null,
                                                 extensions: null,
                                                 embeddedDataSpecifications: null
                                                );

        // Act
        var act = () => new ShellBuilder(parent!, shell);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullShell_ShouldThrowArgumentNullException()
    {
        // Arrange
        var                       parent = new AasBuilder();
        AssetAdministrationShell? shell  = null;

        // Act
        var act = () => new ShellBuilder(parent, shell!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void WithGlobalAssetId_WithNullOrWhitespace_ShouldThrowArgumentException()
    {
        // Arrange
        var (_, _, builder) = CreateBuilder();

        // Act
        var actNull       = () => builder.WithGlobalAssetId(null!);
        var actEmpty      = () => builder.WithGlobalAssetId(string.Empty);
        var actWhitespace = () => builder.WithGlobalAssetId("   ");

        // Assert
        actNull.ShouldThrow<ArgumentException>();
        actEmpty.ShouldThrow<ArgumentException>();
        actWhitespace.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void WithGlobalAssetId_WithValidId_ShouldSetGlobalAssetIdAndReturnBuilder()
    {
        // Arrange
        var (_, _, builder) = CreateBuilder();
        var globalId = $"global-{Guid.NewGuid():N}";

        // Act
        var result = AasBuilder.Create()
                               .AddShell("urn:aas:example:my-shell", "MyShell").WithGlobalAssetId(globalId);

        // Assert
        result.ShouldNotBeSameAs(builder);
    }

    [Fact]
    public void WithSpecificAssetId_WithInvalidArguments_ShouldThrowArgumentException()
    {
        // Arrange
        var (_, _, builder) = CreateBuilder();

        // Act
        var actNullKey       = () => builder.WithSpecificAssetId(null!, "value", "ns");
        var actEmptyKey      = () => builder.WithSpecificAssetId(string.Empty, "value", "ns");
        var actWhitespaceKey = () => builder.WithSpecificAssetId("   ", "value", "ns");

        var actNullValue       = () => builder.WithSpecificAssetId("key", null!, "ns");
        var actEmptyValue      = () => builder.WithSpecificAssetId("key", string.Empty, "ns");
        var actWhitespaceValue = () => builder.WithSpecificAssetId("key", "   ", "ns");

        var actNullNamespace  = () => builder.WithSpecificAssetId("key", "value", null!);
        var actEmptyNamespace = () => builder.WithSpecificAssetId("key", "value", string.Empty);
        var actWhitespaceNs   = () => builder.WithSpecificAssetId("key", "value", "   ");

        // Assert
        actNullKey.ShouldThrow<ArgumentException>();
        actEmptyKey.ShouldThrow<ArgumentException>();
        actWhitespaceKey.ShouldThrow<ArgumentException>();

        actNullValue.ShouldThrow<ArgumentException>();
        actEmptyValue.ShouldThrow<ArgumentException>();
        actWhitespaceValue.ShouldThrow<ArgumentException>();

        actNullNamespace.ShouldThrow<ArgumentException>();
        actEmptyNamespace.ShouldThrow<ArgumentException>();
        actWhitespaceNs.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void WithSpecificAssetId_WithValidArguments_ShouldAddSpecificAssetIdAndReturnBuilder()
    {
        // Arrange
        var (_, shell, builder) = CreateBuilder();
        var key       = "key-" + Guid.NewGuid().ToString("N")[..8];
        var value     = "value-" + Guid.NewGuid().ToString("N")[..8];
        var nameSpace = "ns-" + Guid.NewGuid().ToString("N")[..8];

        // Act
        builder.WithSpecificAssetId(key, value, nameSpace).Done();

        // Assert
        //result.ShouldBeEquivalentTo(builder);
        shell.AssetInformation.ShouldNotBeNull();
        shell.AssetInformation.SpecificAssetIds.ShouldNotBeNull();
        shell.AssetInformation.SpecificAssetIds.ShouldContain(id =>
                                                                  id.Name == key &&
                                                                  id.Value == value &&
                                                                  id.ExternalSubjectId!.Keys.Count > 0 &&
                                                                  id.ExternalSubjectId.Keys[0].Value == nameSpace);
    }

    [Fact]
    public void AddSubmodelReference_WithNullSubmodel_ShouldThrowArgumentNullException()
    {
        // Arrange
        var (_, _, builder) = CreateBuilder();

        // Act
        var act = () => builder.AddSubmodelReference(null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void AddSubmodelReference_WithValidSubmodel_ShouldAddReference()
    {
        // Arrange
        var (_, shell, builder) = CreateBuilder();
        var submodelId = "submodel-" + Guid.NewGuid().ToString("N")[..8];
        var submodel   = new Submodel(id: submodelId);

        // Act
        builder.AddSubmodelReference(submodel);

        // Assert
        shell.Submodels.ShouldNotBeNull();
        shell.Submodels.ShouldContain(r =>
                                          r.Keys.Count == 1 &&
                                          r.Keys[0].Type == KeyTypes.Submodel &&
                                          r.Keys[0].Value == submodelId);
    }

    [Fact]
    public void AddSubmodel_ShouldCreateSubmodelAndAddReference()
    {
        // Arrange
        var (_, shell, builder) = CreateBuilder();
        var submodelId      = "sub-" + Guid.NewGuid().ToString("N")[..8];
        var submodelIdShort = "subShort-" + Guid.NewGuid().ToString("N")[..4];

        // Act
        var submodelBuilder = builder.AddSubmodel(submodelId, submodelIdShort).Done();

        // Assert
        submodelBuilder.ShouldNotBeNull();
        shell.Submodels.ShouldNotBeNull();
        shell.Submodels.Count.ShouldBe(1);
        shell.Submodels[0].Keys.Count.ShouldBe(1);
        shell.Submodels[0].Keys[0].Value.ShouldBe(submodelId);
    }

    [Fact]
    public void Done_ShouldReturnParentBuilder()
    {
        // Arrange
        var (parent, _, builder) = CreateBuilder();

        // Act
        var result = builder.Done();

        // Assert
        result.ShouldBeSameAs(parent);
    }
}