using FluentAAS.Builder;
using FluentAAS.Templates.DigitalNameplate;
using Shouldly;
using Environment = AasCore.Aas3_0.Environment;

namespace FluentAAS.Templates.Tests.DigitalNameplate;

/// <summary>
/// Unit tests for <see cref="DigitalNameplateShellBuilderExtensions"/>.
/// </summary>
public class DigitalNameplateShellBuilderExtensionsTests
{
    private static (ShellBuilder shellBuilder, Func<Environment> buildEnvironment) CreateShellBuilder()
    {
        var envBuilder   = AasBuilder.Create();
        var shellId      = "urn:aas:example:shell:" + Guid.NewGuid().ToString("N");
        var shellIdShort = "Shell-" + Guid.NewGuid().ToString("N")[..6];

        var shellBuilder = envBuilder.AddShell(shellId, shellIdShort);

        return (shellBuilder, () => envBuilder.Build());
    }

    [Fact]
    public void AddDigitalNameplate_ShouldThrowArgumentNullException_WhenShellBuilderIsNull()
    {
        // Arrange
        ShellBuilder? shellBuilder = null;

        // Act
        var ex = Record.Exception(() => shellBuilder!.AddDigitalNameplate("urn:aas:example:dnp"));

        // Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<ArgumentNullException>();
        ((ArgumentNullException) ex).ParamName.ShouldBe("shellBuilder");
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddDigitalNameplate_ShouldThrowArgumentException_WhenIdIsNullOrWhitespace(string? id)
    {
        // Arrange
        var (shellBuilder, _) = CreateShellBuilder();

        // Act
        var ex = Record.Exception(() => shellBuilder.AddDigitalNameplate(id!));

        // Assert
        ex.ShouldNotBeNull();
        ex.ShouldBeOfType<ArgumentException>();
        ex.Message.ShouldContain("Submodel id must not be empty.");
        ((ArgumentException) ex).ParamName.ShouldBe("id");
    }

    [Fact]
    public void AddDigitalNameplate_ShouldCreateBuilder_WithDefaultIdShort_AndAttachSubmodelOnBuild()
    {
        // Arrange
        var (shellBuilder, buildEnv) = CreateShellBuilder();
        const string submodelId = "urn:aas:example:dnp:1";

        // Act
        var dnpBuilder = shellBuilder.AddDigitalNameplate(submodelId);

        // Configure mandatory fields via the digital nameplate builder
        dnpBuilder.WithManufacturerName("en", "ACME Inc.")
                  .WithManufacturerProductDesignation("en", "Super Device")
                  .WithSerialNumber("SN-001")
                  .Build();

        var env = buildEnv();

        // Assert
        dnpBuilder.ShouldNotBeNull();

        env.Submodels.ShouldNotBeNull();
        env.Submodels!.Count.ShouldBe(1);

        var submodel = env.Submodels[0];
        submodel.Id.ShouldBe(submodelId);
        submodel.IdShort.ShouldBe("DigitalNameplate");

        // Ensure the shell actually references the submodel
        shellBuilder.Shell.Submodels.ShouldNotBeNull();
        shellBuilder.Shell.Submodels!.Count.ShouldBe(1);
        shellBuilder.Shell.Submodels[0].Keys.Single().Value.ShouldBe(submodelId);
    }

    [Fact]
    public void AddDigitalNameplate_ShouldUseCustomIdShort_WhenProvided()
    {
        // Arrange
        var (shellBuilder, buildEnv) = CreateShellBuilder();
        const string submodelId    = "urn:aas:example:dnp:2";
        const string customIdShort = "MyDigitalNameplate";

        // Act
        var dnpBuilder = shellBuilder.AddDigitalNameplate(submodelId, customIdShort);

        dnpBuilder.WithManufacturerName("en", "ACME Corp.")
                  .WithManufacturerProductDesignation("en", "Mega Device")
                  .WithSerialNumber("SN-002")
                  .Build();

        var env = buildEnv();

        // Assert
        env.Submodels.ShouldNotBeNull();
        env.Submodels!.Count.ShouldBe(1);

        var submodel = env.Submodels[0];
        submodel.Id.ShouldBe(submodelId);
        submodel.IdShort.ShouldBe(customIdShort);

        // Shell reference should point to the same submodel id
        shellBuilder.Shell.Submodels.ShouldNotBeNull();
        shellBuilder.Shell.Submodels!.Count.ShouldBe(1);
        shellBuilder.Shell.Submodels[0].Keys.Single().Value.ShouldBe(submodelId);
    }
}